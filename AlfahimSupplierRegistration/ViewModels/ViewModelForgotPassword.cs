using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AlfahimSupplierRegistration.ViewModels
{
    public class ViewModelForgotPassword
    {

        [Required(ErrorMessage = " Required Email Address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Not Valid")]

        public String EmailAddress { get; set; }
    }

    public class ViewModelAdminLogin
    {
        public String UserName { get; set; }
        public String Password { get; set; }

    }
}