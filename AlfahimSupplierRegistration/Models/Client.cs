using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace AlfahimSupplierRegistration.Models
{
    public class Client
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public DateTime DOB { get; set; }
        public string Email { get; set; }

        public static List<Client> GenerateDumpClientList()
        { 
            List<Client> client = new List<Client>

        {
            new Client { FirstName = "Salman", LastName = "sss",  Email = "Salman11" },
            new Client { FirstName = "Salman", LastName = "sss",  Email = "Salman11" },
            new Client { FirstName = "Salman", LastName = "sss",  Email = "Salman11" }
        };

            return client;
        }
    }


    public static class EmailValidation
    {
        /// <summary>
        /// Regular expression, which is used to validate an E-Mail address.
        /// </summary>
        public const string MatchEmailPattern =
                  @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
           + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
           + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
           + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

        /// <summary>
        /// Checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and 
        /// contains a valid E-Mail address;
        /// otherwise false.</returns>
        /// 

        public static bool IsEmail(string email)
        {
            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }

    }
}