using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using System.Web.Security;
using AlfahimSupplierRegistration.Models;
using AlfahimSupplierRegistration.ViewModels;
using System.Text.RegularExpressions;

namespace AlfahimSupplierRegistration.Controllers
{
    public class AccountController : Controller
    {
        private IDbConnection db;

        // GET: Account
        public ActionResult Registration(string email, string Password)
        {
            string ErrorMsg = "";
            //Check email Validation 
            bool valid = EmailValidation.IsEmail(email);

            if (valid==false)
            {
                ErrorMsg = "Invalid Email Address";
                return new JsonResult
                {
                    Data = ErrorMsg,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            ErrorMsg = "Success";
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);



            string sql = " INSERT INTO tblUsers(UserName,Password) " +
                             " VALUES (@email,@Password)";

            try
            { 
            db.Execute(sql, new
            {
                email,
                Password
               
            });
            }
            catch (Exception e)
            {
                if (e.Message.IndexOf("Email-Already-Exist") > 0)
                {

                    ErrorMsg = "Email Address Already Exist....";
                }
                else
                { 
                ErrorMsg= e.Message;
                }
                db.Close();
            }
 
            return new JsonResult
            {
                Data = ErrorMsg,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        public JsonResult LoginIn(string email, string Password)
        {

            

            string ErrorMsg = "false";
            int UserId = 0;
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);

            string sql = " Select ID from tblUsers Where UserName ='" + System.Web.HttpUtility.HtmlEncode(email) + "' And Password='" + System.Web.HttpUtility.HtmlEncode(Password) + "' ";

            try
            {
                var obj = db.Query(sql).ToArray();
                if (obj.Length > 0)
                {
                    ErrorMsg ="true";
                    UserId = obj[0].ID;
                    FormsAuthentication.SetAuthCookie(email,false);

                    if (Request.IsAuthenticated)
                    {

                    }
                    else
                    {


                    }
                }
         
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message;
                db.Close();
            }




            var Data = new { Message = ErrorMsg, UserID = UserId };
            return Json(Data, JsonRequestBehavior.AllowGet);
        
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login","Admin");
        }

       


        public ActionResult Forgot()
        {
            return View();

        }
        [HttpPost]
        public ActionResult Forgot(ViewModels.ViewModelForgotPassword Model)
        {
            string ErrorMsg = "false";
            string Password = "";
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);

            string sql = " Select Password from tblUsers Where UserName ='" + System.Web.HttpUtility.HtmlEncode(Model.EmailAddress) + "' ";

            try
            {
                var obj = db.Query(sql).ToArray();
                if (obj.Length > 0)
                {
                    ErrorMsg = "true";

                    //Send an
                    Password = obj[0].Password;
                    SendEmail(Password,Model.EmailAddress);
                    ViewBag.Message = "Password has been sent.";
                    ViewBag.OK = "true";

                }
                else
                {
                    ViewBag.Message = "Email Address not exist";
                    ViewBag.OK = "false";
                }

            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
                ViewBag.OK = "false";
                db.Close();
            }

            return View();

        }


        private void SendEmail(string password ,string UserName)
        {

            //Company Represntative
            EmailManager EmailManager = new EmailManager();
        
            EmailManager.Subject = ConfigurationManager.AppSettings["ForgetPasswordSubject"].ToString();
            EmailManager.Body = "Your Password is:" + password;
            
            EmailManager.SendEmail(UserName, "Alfahim-Administrator");

        }




    }
}