using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace TransactionDebitService
{
    public class Service1 : IService1
    {
        readonly string CONNECTION_STRING = ConfigurationManager.ConnectionStrings["SampleDbConnectionString1"].ConnectionString;

        [OperationBehavior(TransactionScopeRequired = true)]
        public bool PerformDebitTransaction(string debitAccountID, double amount)
        {
            bool debitResult = false;
            
            try
            {
                using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
                {
                    con.Open();

                    // Let us do a debit
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = string.Format(
                            "update Account set Amount = Amount - {0} where ID = {1}",
                            amount, debitAccountID);

                        debitResult = cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
            catch
            {
                throw new FaultException("Something went wring during debit");
            }
            return debitResult;
        }
    }
}
