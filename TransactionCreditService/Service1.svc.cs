using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace TransactionCreditService
{
    public class Service1 : IService1
    {
        readonly string CONNECTION_STRING = ConfigurationManager.ConnectionStrings["SampleDbConnectionString1"].ConnectionString;

        [OperationBehavior(TransactionScopeRequired = true)]
        public bool PerformCreditTransaction(string creditAccountID, double amount)
        {
            bool creditResult = false;
            
            try
            {
                using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
                {
                    con.Open();

                    // And now do a credit
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = string.Format(
                            "update Account set Amount = Amount + {0} where ID = {1}",
                            amount, creditAccountID);
                        
                        creditResult = cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
            catch
            {
                throw new FaultException("Something went wring during credit");
            }
            return creditResult;
        }
        
        public decimal GetAccountDetails(int id)
        {
            decimal? result = null;

            using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("select Amount from Account where ID = {0}", id);

                    try
                    {
                        con.Open();
                        result = cmd.ExecuteScalar() as decimal?;
                    }
                    catch (Exception ex)
                    {
                        throw new FaultException(ex.Message);
                    }
                }
            }

            if (result.HasValue)
            {
                return result.Value;
            }
            else
            {
                throw new FaultException("Unable to retrieve the amount");
            }
        }
    }
}
