using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Security;

namespace EmployeeDetailMsys.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult LogIn()
        {
            return View();
        }
       

        [HttpGet]
        public ActionResult LogIn(string returnUrl)
        {
            var model = new LogInM
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult LogIn(LogInM model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            if (model.Email == "barik@admin.com" && model.Password == "password")
            {
                var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, "Barik"),
                new Claim(ClaimTypes.Email, "a@b.com"),
                new Claim(ClaimTypes.Country, "Nepal")
            },
                    "ApplicationCookie");

                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;

                authManager.SignIn(identity);
                //FormsAuthentication.SetAuthCookie(.Username);

                return Redirect(GetRedirectUrl(model.ReturnUrl));
            }

            // user authN failed
            //ModelState.AddModelError("", "Invalid email or password");
            ViewBag.Message = "Invalid Email or password";
            return View();
            //return RedirectToAction("LogIn");
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("FetchImportData", "Employee");
            }

            return returnUrl;
        }

        
        public ActionResult Logout()
        {
            //FormsAuthentication.SignOut();
            //return RedirectToAction("LogIn");
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("LogIn", "Auth");
        }

    }
}