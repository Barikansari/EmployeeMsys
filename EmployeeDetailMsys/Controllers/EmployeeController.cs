using EmployeeDetailMsys.DB;
using EmployeeDetailMsys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using OfficeOpenXml;
using Newtonsoft.Json;
using System.Net;
using PagedList;
using Rotativa;
using System.Web.Security;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using GridMvc;

namespace EmployeeDetailMsys.Controllers
{

    public class EmployeeController : Controller
    {
        DBConn conn = new DBConn();
        // GET: Employee
        

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(EmployeeM emp, HttpPostedFileBase file)
        {

            if (file != null && file.ContentLength > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string imgpath = Path.Combine(Server.MapPath("~/UserImages/"), filename);
                file.SaveAs(imgpath);
            }
            string message;
            conn.SaveData(emp, file, out message);
            ViewBag.Message = "Employee Record is Added Successfully";
            return View("Index");
            //return RedirectToAction("EmployeeRecord");
        }

        [Authorize]
        public ActionResult ImportData()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public ActionResult ImportData(HttpPostedFileBase postedfile)
        {
            string path = Server.MapPath("~/Excelfiles/");
            bool isValidationSuccess = true;
            string validationMessage = string.Empty;


            if (postedfile != null)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileName(postedfile.FileName);
                string serverPath = Path.Combine(path, fileName);
                postedfile.SaveAs(serverPath);

                List<ImportM> datas = new List<ImportM>();
                FileInfo fileInfo = new FileInfo(serverPath);

                ExcelPackage package = new ExcelPackage(fileInfo);
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

                // get number of rows and columns in the sheet
                int rows = worksheet.Dimension.Rows; // 20
                int columns = worksheet.Dimension.Columns; // 7

                // loop through the worksheet rows and columns
                for (int i = 1; i <= rows; i++)
                {
                    datas.Add(new ImportM
                    {
                        Index = i,
                        FullName = worksheet.Cells[i, 1]?.Value?.ToString(),
                        DateOfBirth = worksheet.Cells[i, 2].Value == null ? null : Convert.ToDateTime(worksheet.Cells[i, 2].Value).ToString("yyyy/MM/dd"),
                        Gender = worksheet.Cells[i, 3]?.Value?.ToString(),
                        Salary = worksheet.Cells[i, 4]?.Value == null ? 0.00M : Convert.ToDecimal(worksheet.Cells[i, 4]?.Value),
                        Designation = worksheet.Cells[i, 5]?.Value?.ToString(),
                        importdate = DateTime.Now.ToString("yyyy/MM/dd")
                    });
                }

                int[] arr = Array.Empty<int>();
                arr = datas.Where(x => string.IsNullOrEmpty(x.FullName)
                                        && string.IsNullOrEmpty(x.DateOfBirth)
                                        && string.IsNullOrEmpty(x.Gender)
                                        && string.IsNullOrEmpty(x.Designation))
                    .Select(x => x.Index).ToArray();

                if (arr.Any())
                {
                    validationMessage = $"X rows [{String.Join(",", arr)}] were skipped since they did not have data";
                    isValidationSuccess = false;
                }

                if (!arr.Any() && datas.Any(x => string.IsNullOrEmpty(x.FullName)))
                {
                    validationMessage = "Invalid excel. Full name can not be empty.";
                    isValidationSuccess = false;
                }
                if (!arr.Any() && datas.Any(x => string.IsNullOrEmpty(x.DateOfBirth)))
                {
                    validationMessage = "Invalid excel. Date Of Birth can not be empty.";
                    isValidationSuccess = false;
                }
                if (!arr.Any() && datas.Any(x => string.IsNullOrEmpty(x.Gender)))
                {
                    validationMessage = "Invalid excel. Gender can not be empty.";
                    isValidationSuccess = false;
                }
                if (!arr.Any() && datas.Any(x => x.Salary < 0))
                {
                    validationMessage = "Invalid excel. Salary can not be less than zero.";
                    isValidationSuccess = false;
                }
                if (!arr.Any() && datas.Any(x => string.IsNullOrEmpty(x.Designation)))
                {
                    validationMessage = "Invalid excel. Designation can not be empty.";
                    isValidationSuccess = false;
                }

                if (isValidationSuccess)
                {
                    string jsondata = JsonConvert.SerializeObject(datas);
                    conn.SaveImportData(jsondata, out string message);
                }

            }

            ViewBag.Validation = new ResponseModel
            {
                IsSuccess = isValidationSuccess,
                Message = validationMessage
            };
            ViewBag.Message = "File Imported Successfully";
            return View();
            //return RedirectToAction("FetchImportData");
        }

        [HttpGet]
        [Authorize]
        public ActionResult FetchImportData()
        {

            List<ImportM> importList = conn.GetImportData();
            List<ImportM> finalData = new List<ImportM>();

            int counter = 1;
            foreach (var item in importList)
            {
                item.SN = counter;
                finalData.Add(item);
                counter++;
            }

            return View(finalData);

        }

        public ActionResult Detail()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Detail(int? id)
        {
            ImportM Details = conn.ImportDetail(id);
            return View(Details);

        }

        [HttpGet]
        [Authorize]
        public ActionResult EmployeeRecord()
        {
            List<EmployeeM> resultList = conn.GetEmployeeData();

            return View(resultList);
        }

        //public void Controls(FormCollection form)
        //{
        //    var checkBox = form.GetValues("selectedIds");
        //    if (checkBox != null)
        //    {
        //        foreach (var id in checkBox)
        //        {
        //        }
        //    }
        //}
        public ActionResult ExportToExcel()
        {
            DataTable dt = conn.GetImportDatatable();
            
            string sheetName = new Guid().ToString();
            string dateFormat = "yyyy-MM-dd HH:mm:ss";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add(sheetName);
                ws.Cells["a1"].LoadFromDataTable(dt, PrintHeaders: true);
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (dt.Columns[c].DataType == typeof(DateTime))
                    {
                        ws.Column(c + 1).Style.Numberformat.Format = dateFormat;
                    }
                }
                //string fileName = Guid.NewGuid().ToString();
                //return File(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + "emp.xlsx");
                return File(pck.GetAsByteArray(), "application/xlsx", "emp.xlsx");
            }



        }
        public ActionResult ExportToPdf(string arrr)
        {
            int[] arr = Array.ConvertAll(arrr.Split(','), s => int.Parse(s));
            
            DataTable dt = conn.GetImportDatatable();
            Document document = new Document();
            MemoryStream workStream = new MemoryStream();
            
            PdfWriter.GetInstance(document, workStream).CloseStream = false;


            document.Open();
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);
            PdfPTable table = new PdfPTable(dt.Columns.Count);
            table.SetWidths(new float[] { 4F, 4F, 4F, 4F, 4F, 4F, 4F, 4F });
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));

            cell.Colspan = dt.Columns.Count;

            foreach (DataColumn c in dt.Columns)
            {

                table.AddCell(new Phrase(c.ColumnName, font5));
            }

            foreach (DataRow r in dt.Rows)
            {
                if (dt.Rows.Count > 0)
                {
                    table.AddCell(new Phrase(r[0].ToString(), font5));
                    table.AddCell(new Phrase(r[1].ToString(), font5));
                    table.AddCell(new Phrase(r[2].ToString(), font5));
                    table.AddCell(new Phrase(r[3].ToString(), font5));
                    table.AddCell(new Phrase(r[4].ToString(), font5));
                    table.AddCell(new Phrase(r[5].ToString(), font5));
                    table.AddCell(new Phrase(r[6].ToString(), font5));
                    table.AddCell(new Phrase(r[7].ToString(), font5));
                }
            }
            document.Add(table);
            document.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return File(workStream, "application/pdf", "empRecord.pdf");

        }

      
        public ActionResult ExportToCsv()
        {
            
            DataTable dt = conn.GetImportDatatable();
            StringBuilder sb = new StringBuilder();
            string[] columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName).
                                              ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                ToArray();
                sb.AppendLine(string.Join(",", fields));
            }

            MemoryStream workStream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return File(workStream, "application/txt", "emp.txt");

        }


        public ActionResult Edit()
        {

            return View();

        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            ImportM EditList = conn.EditData(id);
            return View(EditList);

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(ImportM emp, HttpPostedFileBase file)
        {

            if (file != null && file.ContentLength > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string imgpath = Path.Combine(Server.MapPath("~/UserImages/"), filename);
                file.SaveAs(imgpath);
            }

            string message;
            conn.UpdateData(emp, file, out message);
            //return Json(new JsonResult { Data = message });
            return RedirectToAction("FetchImportData");

        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImportM deleteinfo = conn.Deleteinfo(id);
            return View(deleteinfo);

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            string message;
            conn.DeleteEmpData(id, out message);
            ViewBag.Message = "Employee Record Delete Successfully";
            return RedirectToAction("FetchImportData");
            
        }
        [HttpPost]
        public ActionResult search( string startdate, string enddate,string searchString, string designationString, string sSalary, string eSalary, string genderstring, ImportM EmpM)
        {

            List<ImportM> searchList = conn.GetSearchData(startdate, enddate, searchString, designationString, sSalary, eSalary, genderstring, EmpM);
            
            return View("FetchImportData", searchList);


        }
       
    }
}