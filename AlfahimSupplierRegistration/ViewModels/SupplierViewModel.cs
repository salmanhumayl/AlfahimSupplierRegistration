using AlfahimSupplierRegistration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlfahimSupplierRegistration.ViewModels
{
    public class SupplierViewModel
    {
        public Supplier Supplier { get; set; }
        public List<CheckModel> DealWith { get; set; }

        public SupplierViewModel()
        {
            var list = new List<CheckModel>
            {
                 new CheckModel{Id = 1, Name = "ALFAHIM (Head Office)", Checked = false},
                 new CheckModel{Id = 2, Name = " Automotive (EMC, WM, CME, and EM", Checked = false},
                 new CheckModel{Id = 3, Name = "Auto Leasing (TAJEER)", Checked = false},
                 new CheckModel{Id = 4, Name = "Real Estate (EPICO)", Checked = false},
                 new CheckModel{Id = 5, Name = " Industrial (MID)", Checked = false},


            };
            Supplier=new Supplier();
           // DealWith = list;
        }
    }


    public class RegisteredSupplierViewModel
    {
        public List<Supplier> Supplier { get; set; }


    }



    public class PDFSupplierViewModel
    {
        public Supplier Supplier { get; set; }
        public string DealWith { get; set; }



    }


    public class SupplierDirecotry
    {
        public string SupplierCode { get; set; }
        public string NameCompany { get; set; }

        public string TradeLicenseURL { get; set; }
        public string OtherURL { get; set; }

        public string BankLetterURL { get; set; }
        public string VATCertificateURL { get; set; }

        public string VATClassification { get; set; }


        


    }

}