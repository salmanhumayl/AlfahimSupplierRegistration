using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AlfahimSupplierRegistration
{
    public class ValidateFileAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string[] sAllowExt = new string[] { ".pdf" };
            var file = value as HttpPostedFileBase;
            if (file == null)
            
                return false;
            else if  (!sAllowExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                {
                ErrorMessage = "Please Upload type of ";
                
                    return false;

                }
            return true;
        }
    }
}