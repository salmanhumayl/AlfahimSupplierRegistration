using AlfahimSupplierRegistration.Models;
using AlfahimSupplierRegistration.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace AlfahimSupplierRegistration.Controllers
{
    public class SupplierController : Controller
    {
        private IDbConnection db;
        Boolean lSendEmail = false;
        string ErrorMsg = "OK";
        // GET: Supplier
        private void FillEmirates()
        {
            Dictionary<string, string> lstType = new Dictionary<string, string>();
            lstType.Add("Abu Dhabi", "Abu Dhabi");
            lstType.Add("Dubai", "Dubai");
            lstType.Add("Sharjah", "Sharjah");
            lstType.Add("Fujairah", "Fujairah");
            lstType.Add("Ajman", "Ajman");
            lstType.Add("Ras Al Khaimah", "Ras Al Khaimah");
            lstType.Add("Umm Al Quwain", "Umm Al Quwain");
            ViewBag.Emirates = new SelectList(lstType, "Key", "Value");
        }
        private void FillIssuingAuthority()
        {
            Dictionary<string, string> lstIssuingAuthority = new Dictionary<string, string>();
            lstIssuingAuthority.Add("Abu Dhabi", "Abu Dhabi");
            lstIssuingAuthority.Add("Dubai", "Dubai");
            lstIssuingAuthority.Add("Sharjah", "Sharjah");
            lstIssuingAuthority.Add("Fujairah", "Fujairah");
            lstIssuingAuthority.Add("Ajman", "Ajman");
            lstIssuingAuthority.Add("Ras Al Khaimah", "Ras Al Khaimah");
            lstIssuingAuthority.Add("Umm Al Quwain", "Umm Al Quwain");
            ViewBag.IssuingAuthority = new SelectList(lstIssuingAuthority, "Key", "Value");

        }

        private void FillVatClassification()
        {

            Dictionary<string, string> lstVATClassification = new Dictionary<string, string>();
            lstVATClassification.Add("R", "Local Vendor (Registered)");
            lstVATClassification.Add("N", "Local Vendor (Non-Registered)");
            lstVATClassification.Add("F", "Freezone (Registered)");


            ViewBag.VATClassification = new SelectList(lstVATClassification, "Key", "Value");
        }

       

        public ActionResult CreateGO(int ID)
        {
            TempData["ID"]= ID;
            return RedirectToAction("Create");
        }

      

        [Authorize]
       
        public ActionResult Create()//int ID
        {
            Int16 ID = Convert.ToInt16(TempData["ID"]);
            FillEmirates();
            FillIssuingAuthority();

            FillVatClassification();
            var viewModel = new SupplierViewModel();
            viewModel.Supplier = GetSupplier(ID);//User_id 
            viewModel.Supplier.User_id = ID;//override in case of Edit...

            if (viewModel.Supplier.ID > 0) //edit 
            {
                viewModel.DealWith = GetSupplierDealWith(viewModel.Supplier.SupplierCode);
                ViewBag.VATClassificationSelection = viewModel.Supplier.VATClassification;
                return View("Edit", viewModel);
            }
            else
            {
                //get Email User Email Address and assing into model
               
                viewModel.Supplier.EmailAddress=GetSignInEmailAddress(ID);
                viewModel.DealWith=GetDealWith();
            }
            return View("Supplier",viewModel);
        }

        public ActionResult Confirmation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(SupplierViewModel Model)
        {
            Boolean DealWith = false;
            //Server Side Validation
            for (var i = 0; i < Model.DealWith.Count(); i++)// if none is selected then return error 
            {
                if (Model.DealWith[i].Checked == true)
                {
                    DealWith = true;
                }
            }

            if (DealWith == false)
            {
                FillEmirates();
                FillIssuingAuthority();
                FillVatClassification();
                ModelState.AddModelError("ConfimationMsg", "Please Select Deal With..");
                return View("Supplier", Model);
            }
            //Deal Validation Ends 

            if (Insert(Model) == "OK")
            {
                UpdateURL(Model);

                //Send Email to Supplier and company Administrator ,once addedd /
                if(lSendEmail==true)
                { 
                    SendEmail(Model);
                }
                return RedirectToAction("Confirmation");
            }
            else
            {
                FillEmirates();
                FillIssuingAuthority();
                FillVatClassification();
                ModelState.AddModelError("ConfimationMsg", ErrorMsg);
                return View("Supplier",Model);
            }
        }


        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }


       

        public Supplier GetSupplier(int UserId)
        {
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
            var obj =db.Query<Supplier>("Select * from tblSupplier Where User_id=" + UserId).SingleOrDefault();

            if (obj == null)
            {
                obj = new Supplier(); //Add New
                obj.TradelicenseExpiryDate = DateTime.Now.Date;
            }
            db.Close();
            return obj;
        }
        private string GetSignInEmailAddress(int UserId)
        {
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
            var EmailAddress = db.Query<string>("Select UserName from tblUsers Where id=" + UserId).ToArray();

            //  var obj1 = db.Query("Select UserName from tblUsers Where id=" + UserId).ToArray();
            db.Close();
            if (EmailAddress.Length > 0 )
            {
                return EmailAddress[0];
            }
            return "";
        }


        public List<CheckModel> GetDealWith()
        {
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
            var obj = db.Query<CheckModel>("Select *,0 as Checked from tblDealWith").ToList();

            
            return obj;
        }

        public List<CheckModel> GetSupplierDealWith(string SupplierCode)
        {
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
            var obj = db.Query<CheckModel>(" Select a.*,1 as checked from tblDealWith a "+
                                           " Left  outer join tblSupplierDealWith b on a.id = b.DealWithId" +
                                           " Where b.SupplierCode= '" +SupplierCode +"'").ToList();


            return obj;
        }

        private void UpdateURL(SupplierViewModel Model)
        {
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);

            string sql = " Update tblSupplier Set TradeLicenseURL='" + Model.Supplier.TradeLicenseURL + "'," +
                    "OtherURL='" + Model.Supplier.TradeLicenseURL + "'," +
                    "BankLetterURL='" + Model.Supplier.BankLetterURL + "'," +
                    "VATCertificateURL='" + Model.Supplier.VATCertificateURL + "' Where id =" + Model.Supplier.ID;

            db.Execute(sql);
            db.Close();
        }


        public string Insert(SupplierViewModel Model)
        {
            try
            {

                string sql = "";
                db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);

                if (Model.Supplier.ID == 0)
                {
                    sql = " INSERT INTO tblSupplier(SupplierCode,User_id,NameCompany,Emirates,Address,POBox,Owner, " +
                               " EmailAddress, Phone1, Phone2, Mobile, Fax, TradelicenseNo,IssuingAuthority,TradelicenseExpiryDate," +
                               "BankName,BankAddress,BeneficiaryName,IBANNumber,SwiftCode,VATClassification,VATRegistrationNo) " +
                               " VALUES (@SupplierCode,@User_id,@NameCompany,@Emirates,@Address,@POBox,@Owner," +
                                         "@EmailAddress, @Phone1, @Phone2, @Mobile, @Fax,@TradelicenseNo, @IssuingAuthority,@TradelicenseExpiryDate," +
                                         "@BankName,@BankAddress,@BeneficiaryName,@IBANNumber,@SwiftCode,@VATClassification,@VATRegistrationNo)" +
                                         "Select Cast(SCOPE_IDENTITY() AS int)";

                    try
                    {
                        int ID = db.Query<int>(sql, new
                        {
                            Model.Supplier.SupplierCode,
                            Model.Supplier.User_id,
                            Model.Supplier.NameCompany,
                            Model.Supplier.Emirates,
                            Model.Supplier.Address,
                            Model.Supplier.POBox,
                            Model.Supplier.Owner,
                            Model.Supplier.EmailAddress,
                            Model.Supplier.Phone1,
                            Model.Supplier.Phone2,
                            Model.Supplier.Mobile,
                            Model.Supplier.Fax,
                            Model.Supplier.TradelicenseNo,
                            Model.Supplier.IssuingAuthority,
                            Model.Supplier.TradelicenseExpiryDate,
                            Model.Supplier.BankName,
                            Model.Supplier.BankAddress,
                            Model.Supplier.BeneficiaryName,
                            Model.Supplier.IBANNumber,
                            Model.Supplier.SwiftCode,
                            Model.Supplier.VATClassification,
                            Model.Supplier.VATRegistrationNo
                        }).SingleOrDefault();

                        if (ID > 0)
                        {
                            Model.Supplier.ID = ID;
                            lSendEmail = true;
                            for (var i = 0; i < Model.DealWith.Count(); i++)// if none is selected then return error 
                            {
                                if (Model.DealWith[i].Checked == true)
                                {
                                    sql = " INSERT INTO tblSupplierDealWith(SupplierCode,DealWithId)" +
                                          " VALUES (@SupplierCode,@Id)";

                                    db.Execute(sql, new
                                    {
                                        Model.Supplier.SupplierCode,
                                        Model.DealWith[i].Id

                                    });
                                }
                            }
                        }

                        db.Close();

                    }//try catch
                    catch (Exception e)
                    {
                        if (e.Message.IndexOf("IX_tblSupplier-Code") > 0)
                        {
                            ErrorMsg = "Supplier Code Already Exist....";

                        }
                        else
                        {
                            ErrorMsg = e.Message;
                        }
                        db.Close();
                    }
                }
                else //UPDATE 
                {
                    sql = " Update tblSupplier Set NameCompany=@NameCompany," +
                          " Emirates=@Emirates,Address=@Address,POBox=@POBox,EmailAddress=@EmailAddress," +
                          " Phone1=@Phone1,Phone2=@Phone2,Mobile=@Mobile,Fax=@Fax,TradelicenseNo=@TradelicenseNo," +
                          " IssuingAuthority=@IssuingAuthority,TradelicenseExpiryDate=@TradelicenseExpiryDate," +
                          " BankName=@BankName,BankAddress=@BankAddress,BeneficiaryName=@BeneficiaryName," +
                          " IBANNumber=@IBANNumber,SwiftCode=@SwiftCode,VATClassification=@VATClassification,VATRegistrationNo=@VATRegistrationNo" +
                          " Where ID=@ID";
                    try
                    {
                        db.Execute(sql, new
                        {

                            Model.Supplier.NameCompany,
                            Model.Supplier.Emirates,
                            Model.Supplier.Address,
                            Model.Supplier.POBox,
                            Model.Supplier.Owner,
                            Model.Supplier.EmailAddress,
                            Model.Supplier.Phone1,
                            Model.Supplier.Phone2,
                            Model.Supplier.Mobile,
                            Model.Supplier.Fax,
                            Model.Supplier.TradelicenseNo,
                            Model.Supplier.IssuingAuthority,
                            Model.Supplier.TradelicenseExpiryDate,
                            Model.Supplier.BankName,
                            Model.Supplier.BankAddress,
                            Model.Supplier.BeneficiaryName,
                            Model.Supplier.IBANNumber,
                            Model.Supplier.SwiftCode,
                            Model.Supplier.VATClassification,
                            Model.Supplier.VATRegistrationNo,
                            Model.Supplier.ID
                        });
                        db.Close();
                    }
                    catch (Exception e)
                    {
                        ErrorMsg = e.Message;
                        db.Close();
                    }
                }
                UploadDocuments(Model);
                
            }//Main
            catch (Exception e)
            {
                ErrorMsg = e.Message;
                db.Close();
            }

            return ErrorMsg;

}

        private void UploadDocuments(SupplierViewModel Model)
        {
            string DirectoryPath = @"\UploadFiles\Supplier_";
            string FolderPath;
            //Upload Documents 
            FolderPath = DirectoryPath + Model.Supplier.SupplierCode;
            if (!Directory.Exists(Server.MapPath(FolderPath)))
            {
                Directory.CreateDirectory(Server.MapPath(FolderPath));
            }

            var fileName = "";

            // in case of edit it could be empty 

            if (Model.Supplier.PostedFileTradeLicense !=null)
            {
                fileName = "ValidTradeLicense" + Path.GetExtension(Model.Supplier.PostedFileTradeLicense.FileName);
                Model.Supplier.TradeLicenseURL = Path.Combine(FolderPath, fileName);
                Model.Supplier.PostedFileTradeLicense.SaveAs(Path.Combine(Server.MapPath(FolderPath), fileName));
            }
            if (Model.Supplier.PostedFileOther!=null )
            { 
                 fileName = "Other" + Path.GetExtension(Model.Supplier.PostedFileOther.FileName);
                 Model.Supplier.OtherURL = Path.Combine(FolderPath, fileName);
                 Model.Supplier.PostedFileOther.SaveAs(Path.Combine(Server.MapPath(FolderPath), fileName));
            }

            if (Model.Supplier.PostedFileBankLetter!=null)
            {
                fileName = "BankLetter" + Path.GetExtension(Model.Supplier.PostedFileBankLetter.FileName);
                Model.Supplier.BankLetterURL = Path.Combine(FolderPath, fileName);
                Model.Supplier.PostedFileBankLetter.SaveAs(Path.Combine(Server.MapPath(FolderPath), fileName));
            }

            if (Model.Supplier.VATClassification == "R" || Model.Supplier.VATClassification == "F" )
            {
                if (Model.Supplier.PostedFileVATCertificate!=null)
                {
                    fileName = "VATCertificate" + Path.GetExtension(Model.Supplier.PostedFileVATCertificate.FileName);
                    Model.Supplier.VATCertificateURL = Path.Combine(FolderPath, fileName);
                    Model.Supplier.PostedFileVATCertificate.SaveAs(Path.Combine(Server.MapPath(FolderPath), fileName));
                }
            }


        }
        private void SendEmail (SupplierViewModel Model)
        {

            //Company Represntative
            EmailManager EmailManager = new EmailManager();
            EmailManager.Subject = "Supplier Registration";
            EmailManager.Body = GenerateEmailBody(Model);
            EmailManager.SendEmail(ConfigurationManager.AppSettings["CmpyREPEmailAddress"].ToString(), ConfigurationManager.AppSettings["CmpyREPName"].ToString());

            //Send Another Email to Supplier

            EmailManager.Subject = "Thanks for Registration";
            EmailManager.Body = "Thanks for Registration";
            EmailManager.SendEmail(Model.Supplier.EmailAddress, Model.Supplier.NameCompany); //Company Represntative

        }

        private string GenerateEmailBody(SupplierViewModel Model)
        {
            string SelectedDeal="";
            string strMailBody = "";
            string msg = "<table border='1' cellpadding='2' cellspacing='5'  style='text-align:left;'>";
            msg += "<tr><td>Supplier Code </td> <td><b>" + Model.Supplier.SupplierCode + "</b></td>";
            msg += "<td>Name of Company </td> <td><b>" + Model.Supplier.NameCompany + "</b></td></tr>";


            for (var i = 0; i < Model.DealWith.Count(); i++)
            {
                if (Model.DealWith[i].Checked == true)
                {
                    SelectedDeal+=Model.DealWith[i].Name;
                }
            }

            msg += "<tr><td colspan='4'>Deal With</td>";
            msg += "<tr><td colspan='4'>" + SelectedDeal + "</td></tr>";

            msg += "<tr><td>Emirates </td> <td>" + Model.Supplier.Emirates + "</td>";
            msg += "<td>Address </td> <td>" + Model.Supplier.Address + "</td></tr>";


            msg += "<tr><td>P.O.BOX </td> <td>" + Model.Supplier.POBox + "</td>";
            msg += "<td>Owner </td> <td>" + Model.Supplier.Owner + "</td></tr>";

            msg += "<tr><td>Email Address </td> <td>" + Model.Supplier.EmailAddress + "</td>";
            msg += "<td>Phone1 </td> <td>" + Model.Supplier.Phone1 + "</td></tr>";

            msg += "<tr><td>Phone2 </td> <td>" + Model.Supplier.Phone2 + "</td>";
            msg += "<td>Mobile </td> <td>" + Model.Supplier.Mobile + "</td></tr>";

            msg += "<tr><td>Trade License No </td> <td>" + Model.Supplier.TradelicenseNo + "</td>";
            msg += "<td>Issuing Authority </td> <td>" + Model.Supplier.IssuingAuthority + "</td></tr>";

            msg += "<tr><td>Bank Name </td> <td>" + Model.Supplier.BankName + "</td>";
            msg += "<td>Bank Address </td> <td>" + Model.Supplier.BankAddress + "</td></tr>";

         

            string Classification = "";
            if (Model.Supplier.VATClassification == "R")
            {
                Classification = "Local Vendor (Registered)";
            }
            else if(Model.Supplier.VATClassification == "N")
            {
                Classification = "Local Vendor (Non-Registered)";
            }

            else if (Model.Supplier.VATClassification == "F")
            {
                Classification = "Freezone (Registered)";
            }

            msg += "<tr><td>VAT Clssification </td> <td>" + Classification + "</td>";
            msg += "<td>VAT Registration No </td> <td>" + Model.Supplier.VATRegistrationNo + "</td></tr>";

            msg += "</table>";

            strMailBody = msg;

            return strMailBody;
        }


    }
}