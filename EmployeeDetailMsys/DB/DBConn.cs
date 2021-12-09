using EmployeeDetailMsys.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using PagedList;
using System.Data;

namespace EmployeeDetailMsys.DB
{
    public class DBConn
    {
        private string connectionString = string.Empty;
        private SqlConnection sqlcon;


        public DBConn()
        {
            connectionString = ConfigurationManager.ConnectionStrings["myConnection"].ToString();
        }

        public void createconnection()
        {
            sqlcon = new SqlConnection(connectionString);
        }

        public void SaveData(EmployeeM emp, HttpPostedFileBase file, out string message)
        {
            try
            {
                createconnection();
                SqlCommand cmd = new SqlCommand("[addRecord]", sqlcon);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FullName", emp.FullName);
                cmd.Parameters.AddWithValue("@DateOfBirth", emp.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", emp.Gender);
                cmd.Parameters.AddWithValue("@Salary", emp.Salary);
                cmd.Parameters.AddWithValue("@Designation", emp.Designation);
                cmd.Parameters.AddWithValue("@importdate", DateTime.Now.ToString("yyyy/MM/dd"));
                cmd.Parameters.AddWithValue("@Uimg",file.FileName);
                sqlcon.Open();
                cmd.ExecuteNonQuery();
                sqlcon.Close();

                message = "User Record" + emp.FullName + "is save successfully !";

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        public void SaveImportData(string jsonData, out string message)
        {


            try
            {
                createconnection();
                SqlCommand cmd = new SqlCommand("[saveimportrecord]", sqlcon);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@jsonData", jsonData);
                sqlcon.Open();
                cmd.ExecuteNonQuery();
                sqlcon.Close();

                message = "Import Record is save successfully !";

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }





        }

       

        public List<EmployeeM> GetEmployeeData()
        {

            createconnection();
            List<EmployeeM> resultList = new List<EmployeeM>();
            SqlCommand cmd = new SqlCommand("select * from EmployeeData", sqlcon);
            cmd.CommandType = System.Data.CommandType.Text;
            sqlcon.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var Employeedata = new EmployeeM();
                Employeedata.ID = int.Parse(rdr["EmpID"].ToString());
                Employeedata.FullName = rdr["FullName"].ToString();
                Employeedata.DateOfBirth = rdr["DateOfBirth"].ToString();
                Employeedata.Gender = rdr["Gender"].ToString();
                Employeedata.Salary = int.Parse(rdr["Salary"].ToString());
                Employeedata.Designation = rdr["Designation"].ToString();
                Employeedata.Uimg = rdr["Uimg"].ToString();


                resultList.Add(Employeedata);
            }

            sqlcon.Close();
            return resultList;
        }

        public List<ImportM> GetImportData()
        {

            
            createconnection();
            List<ImportM> importList = new List<ImportM>();
            SqlCommand cmd = new SqlCommand("select * from ImportRecord", sqlcon);
            cmd.CommandType = System.Data.CommandType.Text;
            sqlcon.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
           
           

                    while (rdr.Read())
            {
                var importdata = new ImportM();
                importdata.Index = int.Parse(rdr["Id"].ToString());
                importdata.FullName = rdr["FullName"].ToString();
                importdata.DateOfBirth = rdr["DateOfBirth"].ToString();
                importdata.Gender = rdr["Gender"].ToString();
                importdata.Salary = int.Parse(rdr["Salary"].ToString());
                importdata.Designation = rdr["Designation"].ToString();
                importdata.importdate = rdr["importdate"].ToString();
                importdata.Uimg = rdr["Uimg"].ToString();


                importList.Add(importdata);
            }



            sqlcon.Close();
            return importList;
        }

        public DataTable GetImportDatatable()
        {
            

            createconnection();
            DataTable dt = new DataTable();
            using (var con = sqlcon)
            using (var dataadapter = new SqlDataAdapter("select * from ImportRecord", con))
            {

                dataadapter.Fill(dt);
            }

            sqlcon.Close();
            return dt;
        }


        
        public ImportM ImportDetail(int? id)
        {
            createconnection();
            ImportM Details = new ImportM();
            SqlCommand cmd = new SqlCommand("[ImportDetail]", sqlcon);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);
            sqlcon.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Details.Index = int.Parse(rdr["Id"].ToString());
                Details.FullName = rdr["FullName"].ToString();
                Details.DateOfBirth = rdr["DateOfBirth"].ToString();
                Details.Gender = rdr["Gender"].ToString();
                Details.Salary = int.Parse(rdr["Salary"].ToString());
                Details.Designation = rdr["Designation"].ToString();
                Details.importdate = rdr["importdate"].ToString();
                Details.Uimg = rdr["Uimg"].ToString();

            }

            sqlcon.Close();
            return Details;

        }
        public ImportM EditData(int? id)
        {
            createconnection();
            ImportM editdetail = new ImportM();
            SqlCommand cmd = new SqlCommand("[EditAll]", sqlcon);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);
            sqlcon.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                editdetail.Index = int.Parse(rdr["Id"].ToString());
                editdetail.importdate = rdr["importdate"].ToString();
                editdetail.FullName = rdr["FullName"].ToString();
                editdetail.DateOfBirth = rdr["DateOfBirth"].ToString();
                editdetail.Gender = rdr["Gender"].ToString();
                editdetail.Salary = int.Parse(rdr["Salary"].ToString());
                editdetail.Designation = rdr["Designation"].ToString();
                editdetail.Uimg = rdr["Uimg"].ToString();

            }
           
            sqlcon.Close();
            return editdetail;

        }

        public ImportM Deleteinfo(int? id)
        {
            createconnection();
            ImportM deleteinfo = new ImportM();
            SqlCommand cmd = new SqlCommand("[EditAll]", sqlcon);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);
            sqlcon.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                deleteinfo.Index = int.Parse(rdr["Id"].ToString());
                deleteinfo.importdate = rdr["importdate"].ToString();
                deleteinfo.FullName = rdr["FullName"].ToString();
                deleteinfo.DateOfBirth = rdr["DateOfBirth"].ToString();
                deleteinfo.Gender = rdr["Gender"].ToString();
                deleteinfo.Salary = int.Parse(rdr["Salary"].ToString());
                deleteinfo.Designation = rdr["Designation"].ToString();
                deleteinfo.Uimg = rdr["Uimg"].ToString();

            }

            sqlcon.Close();
            return deleteinfo;

        }

        public void UpdateData(ImportM emp, HttpPostedFileBase file, out string message)
        {
            try
            {
                createconnection();
                SqlCommand cmd = new SqlCommand("[UpdateRecord]", sqlcon);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", emp.Index);
                cmd.Parameters.AddWithValue("@FullName", emp.FullName);
                cmd.Parameters.AddWithValue("@DateOfBirth", emp.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", emp.Gender);
                cmd.Parameters.AddWithValue("@Salary", emp.Salary);
                cmd.Parameters.AddWithValue("@Designation", emp.Designation);
                cmd.Parameters.AddWithValue("@importdate", emp.importdate);
                cmd.Parameters.AddWithValue("@Uimg", file.FileName);
                sqlcon.Open();
                cmd.ExecuteNonQuery();
                sqlcon.Close();

                message = "User Record" + emp.FullName + "is successfully Updated!";

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        public void DeleteEmpData(int? id, out string message)
        {
            try
            {
                createconnection();
                SqlCommand cmd = new SqlCommand("[DeleteEmpData]", sqlcon);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);
                sqlcon.Open();
                cmd.ExecuteNonQuery();
                sqlcon.Close();

                message = "Success";

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

        }

        public List<ImportM> GetSearchData(string startdate, string enddate,string searchString, string designationString,string sSalary, string eSalary,string genderstring, ImportM EmpM)
        {
            
                createconnection();
                List<ImportM> searchList = new List<ImportM>();
                SqlCommand cmd = new SqlCommand("[search]", sqlcon);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@flag", "s");
                cmd.Parameters.AddWithValue("@startdate", startdate);
                cmd.Parameters.AddWithValue("@enddate", enddate);
                cmd.Parameters.AddWithValue("@searchString", searchString);
                cmd.Parameters.AddWithValue("@designationString", designationString);
                cmd.Parameters.AddWithValue("@sSalary", sSalary);
                cmd.Parameters.AddWithValue("@eSalary", eSalary);
                cmd.Parameters.AddWithValue("@genderstring", genderstring);
            sqlcon.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var Employeedata = new ImportM();
                    Employeedata.Index = int.Parse(rdr["Id"].ToString());
                    Employeedata.importdate = rdr["importdate"].ToString();
                    Employeedata.FullName = rdr["FullName"].ToString();
                    Employeedata.DateOfBirth = rdr["DateOfBirth"].ToString();
                    Employeedata.Gender = rdr["Gender"].ToString();
                    Employeedata.Salary = int.Parse(rdr["Salary"].ToString());
                    Employeedata.Designation = rdr["Designation"].ToString();
                    Employeedata.Uimg = rdr["Uimg"].ToString();


                    searchList.Add(Employeedata);
                }
                
            
            
            sqlcon.Close();
            return searchList;
        }
    }
}