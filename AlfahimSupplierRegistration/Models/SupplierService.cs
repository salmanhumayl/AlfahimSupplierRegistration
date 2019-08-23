using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace AlfahimSupplierRegistration.Models
{
    public class SupplierService
    {
       
        public static List<Supplier> GetAllSupplier()
        {
            IDbConnection db;
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
            var obj = db.Query<Supplier>("Select * from tblSupplier").ToList();
           
            db.Close();
            return obj;
        }


        public static Supplier GetSupplierByCode(string SCode)
        {
            IDbConnection db;
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
            var obj = db.Query<Supplier>("Select * from tblSupplier Where  SupplierCode='" + SCode + "'").FirstOrDefault();

            db.Close();
            return obj;
        }


        public static List<CheckModel> GetSupplierDealWith(string SupplierCode)
        {
            IDbConnection db;

            db = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
            var obj = db.Query<CheckModel>(" Select a.*,1 as checked from tblDealWith a " +
                                           " Left  outer join tblSupplierDealWith b on a.id = b.DealWithId" +
                                           " Where b.SupplierCode= '" + SupplierCode + "'").ToList();

            db.Close();
            return obj;
        }

    }
}