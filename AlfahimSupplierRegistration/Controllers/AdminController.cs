using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.Configuration;
using System.Data;
using AlfahimSupplierRegistration.ViewModels;
using System.Data.SqlClient;
//using System.Text;
using System.IO;
using AlfahimSupplierRegistration.Models;
using System.Web.UI.WebControls;
using Rotativa;
using System.Web.Security;

namespace AlfahimSupplierRegistration.Controllers
{
    public class AdminController : Controller
    {
        private IDbConnection db;
        // GET: Admin
        [Authorize(Users = "superuser")]
        public ActionResult Index()
        {

            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
            var obj = db.Query<Models.Supplier>("Select * from tblSupplier").ToList();
            db.Close();

            return View(obj);
        }

        [Authorize(Users = "superuser")]
        public ActionResult ShowDocument(String SupplierCode)
        {

            var obj = GetSupplierDocuments(SupplierCode);
            return PartialView("_ViewDocument", obj);
        }


        public ActionResult Login()
        {
            return View("AdminLogin");
        }

        [HttpPost]
        public ActionResult Login(ViewModelAdminLogin Model)
        {
            string ErrorMsg = "false";

            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);

            string sql = " Select * from tblSuperUser Where UserName ='" + System.Web.HttpUtility.HtmlEncode(Model.UserName) + "' And Password='" + System.Web.HttpUtility.HtmlEncode(Model.Password) + "' ";

            try
            {
                var obj = db.Query(sql).ToArray();
                if (obj.Length > 0)
                {
                    db.Close();
                    ErrorMsg = "true";
                    FormsAuthentication.SetAuthCookie(Model.UserName, false);
                    return RedirectToAction("Index");

                }
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message;
                db.Close();
            }
            return View("AdminLogin");
        }

        public FileStreamResult GetPDF()
        {
            FileStream fs = new FileStream(Server.MapPath(@"\UploadFiles\Supplier_GP123456\ValidTradeLicense.PDF"), FileMode.Open, FileAccess.Read);
            return File(fs, "application/pdf");
        }

        public ActionResult GetSupplierByCode(string SCode)
        {
            //PDFSupplierViewModel model = new PDFSupplierViewModel();

            var viewModel = new SupplierViewModel();

            var supplier = SupplierService.GetSupplierByCode(SCode);
            var SupplierDealWith = SupplierService.GetSupplierDealWith(SCode);

            viewModel.Supplier = supplier;
            viewModel.DealWith = SupplierDealWith;

            return View("SupplierDetail", viewModel);
        }
        
        public ActionResult PrintSupplierReport(string SCode)
        {
            var report = new ActionAsPdf("GetSupplierByCode" ,new { SCode= SCode });
            return report;
        }





        public SupplierDirecotry GetSupplierDocuments(String SupplierCode)
        {
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);

            
            var obj = db.Query<SupplierDirecotry>("Select SupplierCode,NameCompany,TradeLicenseURL,OtherURL,BankLetterURL,VATCertificateURL,VATClassification from tblSupplier Where SupplierCode='" + SupplierCode+"'").FirstOrDefault();

            db.Close();
            return obj;


        }




        public void ExportToCsv()
        {
            StringWriter sw = new StringWriter();
            //sw.WriteLine("\"First Name\",\"LastName\",\"Email\"");

            sw.WriteLine("\"Supplier Code\",\"Company Name\",\"Emirates\",\"Address\",\"POBox\",\"Owner\",\"EmailAddress\",\"Phone1\",\"Phone2\",\"Mobile\",\"Fax\",\"TradelicenseNo\",\"IssuingAuthority\",\"TradelicenseExpiryDate\",\"BankName\",\"BankAddress\",\"BeneficiaryName\",\"IBANNumber\",\"SwiftCode\",\"VATClassification\",\"VATRegistrationNo\",\"DealWith\"");


            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=ExportedSupplierlist.csv");
            Response.ContentType = "text/csv";

            var suppliers = SupplierService.GetAllSupplier();

            //  var clients = Client.GenerateDumpClientList();

            //   foreach (var client in clients)
            //   {
            //     sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\"",
            //   client.FirstName,
            //    client.LastName,
            //   client.Email));

            // }
            string strDealWith="";
            string strVatClassification = "";
            foreach (var supplier in suppliers)
            {

                var SupplierDealWith = SupplierService.GetSupplierDealWith(supplier.SupplierCode);
                 foreach (var item in SupplierDealWith)  //Get Supplier DealWith
                {
                        strDealWith += item.Name + "-";
                }

                if (supplier.VATClassification == "R")
                {
                    strVatClassification = "Local Vendor(Registered)";
                }
                else if (supplier.VATClassification == "N")
                {
                    strVatClassification = "Local Vendor (Non-Registered)";
               }
                else if (supplier.VATClassification == "F")
                {
                    strVatClassification = "Freezone (Registered)";
                }


                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\"",
                supplier.SupplierCode,
                supplier.NameCompany,
                supplier.Emirates,
                supplier.Address,
                supplier.POBox,
                supplier.Owner,
                supplier.EmailAddress,
                supplier.Phone1,
                supplier.Phone2,
                supplier.Mobile,
                supplier.Fax,
                supplier.TradelicenseNo,
                supplier.IssuingAuthority,
                supplier.TradelicenseExpiryDate,
                supplier.BankName,
                supplier.BankAddress,
                supplier.BeneficiaryName,
                supplier.IBANNumber,
                supplier.SwiftCode,
                strVatClassification,
                supplier.VATRegistrationNo,
                strDealWith
                ));
                strDealWith = "";
                strVatClassification = "";
            }
            Response.Write(sw.ToString());
            Response.End(); 
        }

        public void ExportToExcel()
        {
            string SitePath=ConfigurationManager.AppSettings["URL"].ToString();
            var suppliers = SupplierService.GetAllSupplier();

            var grid = new GridView();
            grid.DataSource = from data in suppliers
                              select new {
                                  Code = data.SupplierCode,
                                  Company = data.NameCompany,
                                  Emirates = data.Emirates,
                                  Address = data.Address,
                                  POBox = data.POBox,
                                  Owner = data.Owner,
                                  EmailAddress = data.EmailAddress,
                                  Phone1 = data.Phone1,
                                  Phone2 = data.Phone2,
                                  Mobile = data.Mobile,
                                  fax = data.Fax,
                                  TradelicenseNo = data.TradelicenseNo,
                                  IssuingAuthority = data.IssuingAuthority,
                                  TradelicenseExpiryDate = data.TradelicenseExpiryDate,
                                  BankName = data.BankName,
                                  BankAddress = data.BankAddress,
                                  BeneficiaryName = data.BeneficiaryName,
                                  IBANNumber = data.IBANNumber,
                                  SwiftCode = data.SwiftCode,
                                  VATClassification = GetVatClassification(data.VATClassification),
                                  VATRegistrationNo=data.VATRegistrationNo,
                                  DealWith= GetDealWith(data.SupplierCode),
                                  TradeLicenseURL= SitePath + data.TradeLicenseURL,
                                  OtherURL= SitePath + data.OtherURL,
                                  BankLetter= SitePath + data.BankLetterURL,
                                  VatCertificationUrl= SitePath + data.VATCertificateURL 
                              };

            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=ExportedSupplierClist.xls");
            Response.ContentType = "application/excel";

            StringWriter sw = new StringWriter();
            System.Web.UI.HtmlTextWriter htmlTextWrite = new System.Web.UI.HtmlTextWriter(sw);
            grid.RenderControl(htmlTextWrite);
            Response.Write(sw.ToString());
            Response.End();

            

        }

        private string GetVatClassification(string VatType)
        {
            string strVatClassification = "";
            if (VatType == "R")
            {
                strVatClassification = "Local Vendor(Registered)";
            }
            else if (VatType == "N")
            {
                strVatClassification = "Local Vendor (Non-Registered)";
            }
            else if (VatType == "F")
            {
                strVatClassification = "Freezone (Registered)";
            }
            return strVatClassification;
        }

        private String GetDealWith(string SupplierCode)
        {
            string strDealWith = "";
            var SupplierDealWith = SupplierService.GetSupplierDealWith(SupplierCode);
            foreach (var item in SupplierDealWith)  //Get Supplier DealWith
            {
                strDealWith += item.Name + "-";
            }
            return strDealWith;
        }
        private string GetAbsolutPath(string URLPath)
        {
            HyperLink URLLink = new HyperLink();
            string a=Request.Url.AbsoluteUri;
            URLLink.Text="TradeLicence";
            URLLink.NavigateUrl ="http://localhost:3363" + URLPath;
            return URLLink.ToString();
            //return <a hrefUrl.ToString();



        }

    }
}