using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AlfahimSupplierRegistration.Models
{
    public class Supplier
    {

        public int ID { get; set; }

        public int User_id { get; set; }

        [Required(ErrorMessage = " Required Supplier Code ")]
        //[StringLength(15, ErrorMessage = "Not allowed more than 15 characters")]
        //[StringLength(15, MinimumLength = 15, ErrorMessage = "should be 15 characters")]
        // [RegularExpression(@"^([G-P]{2}|\d{2})\d{6}$", ErrorMessage ="GP")]

        [RegularExpression(@"^([G-P]{2}\d{6})", ErrorMessage = "GPXXXXXX")]

        public string SupplierCode { get; set; }
        public List<CheckModel> DealWith  { get; set; }

        [Required(ErrorMessage = "Required Name of Company")]
        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        public string NameCompany { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        public string Emirates { get; set; }

        [Required(ErrorMessage = "Required Address")]
        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Required POBox")]
        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        public string POBox { get; set; }

        [Required(ErrorMessage = "Required Owner")]
        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        public string Owner { get; set; }

        [Required(ErrorMessage = "Required EmailAddress")]
        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        [DataType(DataType.EmailAddress, ErrorMessage = "not valid")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Required Phone1")]
        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\+971(\d{8}|\s\(\d{3}\)\s\(\d{3}\s\d{4}\))$", ErrorMessage = "Invalid Phone1 Number")]
        public string Phone1 { get; set; }


        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\+971(\d{8}|\s\(\d{3}\)\s\(\d{3}\s\d{4}\))$", ErrorMessage = "Invalid Phone2 Number")]
        public string Phone2 { get; set; }


        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\+971(\d{9}|\s\(\d{3}\)\s\(\d{3}\s\d{4}\))$", ErrorMessage = "Invalid Mobile Number")]
        [Required(ErrorMessage = "Required Mobile")]
        public string Mobile { get; set; }

        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\+971(\d{8}|\s\(\d{3}\)\s\(\d{3}\s\d{4}\))$", ErrorMessage = "Invalid Fax No")]

        public string Fax { get; set; }

        [Required(ErrorMessage = "Required Trade LicenseNo")]
        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        public string TradelicenseNo { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        public string IssuingAuthority { get; set; }

      
        [DataType(DataType.Date)]
        public System.DateTime TradelicenseExpiryDate { set; get; }

        [Required(ErrorMessage = "Required BankName")]
        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Required Bank Address")]
        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        public string BankAddress { get; set; }

        [Required(ErrorMessage = "Required Beneficiary Name")]
        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        public string BeneficiaryName { get; set; }

        [Required(ErrorMessage = "Required IBAN Number")]
        [StringLength(23, MinimumLength = 23, ErrorMessage = "should be 23 characters")]
        [RegularExpression(@"^([A-E]{2}\d{21})", ErrorMessage = "AEXXXXXXXXXXXXXXXXXXXXX")]

        public string IBANNumber  { get; set; }   





        [Required(ErrorMessage = "Required Swift Code")]
        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]
        public string SwiftCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]

        public string VATClassification { get; set; }

        public string TradeLicenseURL { get; set; }
        public string OtherURL { get; set; }

        public string BankLetterURL { get; set; }
        public string VATCertificateURL { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(35, ErrorMessage = "Not allowed more than 35 characters")]

        public string  VATRegistrationNo { get; set; }


        [Required (ErrorMessage ="Please upload Trade License")]
        [RegularExpression (@"([a-zA-Z0-9\s_\\.\-:])+(.pdf)$",ErrorMessage ="only pdf")]
        public HttpPostedFileBase PostedFileTradeLicense { set; get; }

        [Required(ErrorMessage = "Please upload Other Documents")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.pdf)$", ErrorMessage = "only pdf")]
        public HttpPostedFileBase PostedFileOther { set; get; }

        [Required(ErrorMessage = "Please upload Bank Letter")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.pdf)$", ErrorMessage = "only pdf")]

        public HttpPostedFileBase PostedFileBankLetter { set; get; }

        [Required(ErrorMessage = "Please upload VAT Certification")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.pdf)$", ErrorMessage = "only pdf")]
        public HttpPostedFileBase PostedFileVATCertificate { set; get; }

        public DateTime RegisteredOn { get; set; }

        public Supplier()
        {
           // var list = new List<CheckModel>
           // {
             //    new CheckModel{Id = 1, Name = "ALFAHIM (Head Office)", Checked = false},
               //  new CheckModel{Id = 2, Name = "Automotive (EMC, WM, CME, and EM", Checked = false},
               //  new CheckModel{Id = 3, Name = "Auto Leasing (TAJEER)", Checked = false},
                // new CheckModel{Id = 4, Name = "Real Estate (EPICO)", Checked = false},
                // new CheckModel{Id = 5, Name = "Industrial (MID)", Checked = false},
           

           // };
           // DealWith = list;
        }
    }



    public class CheckModel
    {
        public int Id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public bool Checked
        {
            get;
            set;
        }
    }
}