using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace TransactionClient
{
    class Program
    {
        static void Main(string[] args)
        {
            bool userWantsToExit = false;

            while (!userWantsToExit)
            {
                PresentTotalAmount();

                Console.WriteLine("How much you want to transfer ?");
                var amountChoice = Console.ReadLine();
                Console.WriteLine("");
                Console.WriteLine("a - From Account 1 to Account 2");
                Console.WriteLine("b - From Account 2 to Account 1");
                Console.WriteLine("");
                var accountChoice = Console.ReadLine();

                double amount = 0.0;

                switch (accountChoice)
                {
                    case "a":
                     
                        double.TryParse(amountChoice, out amount);
                        PerformTransaction("2", "1", amount);
                        break;
                    case "b":
                        
                        double.TryParse(amountChoice, out amount);
                        PerformTransaction("1", "2", amount);
                        break;

                }

                PresentTotalAmount();

                Console.WriteLine("Choose to exit?");
                var isExit = Console.ReadLine();

                if (isExit == "y")
                    userWantsToExit = true;
            }


        }

        private static void PerformTransaction(string creditAccountID, string debitAccountID, double amount)
        {
            bool debitResult = false;
            bool creditResult = false;

            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    #region Service Call

                    #region Without Inner TransactionScope
                    using (TransactionDebitService.Service1Client client = new TransactionDebitService.Service1Client())
                    {
                        debitResult = client.PerformDebitTransaction(debitAccountID, amount);
                    }

                    throw new Exception();

                    using (TransactionCreditService.Service1Client client = new TransactionCreditService.Service1Client())
                    {
                        creditResult = client.PerformCreditTransaction(creditAccountID, amount);
                    } 
                    #endregion

                    #region With Inner TransactionScope

                    //using (TransactionScope tsInner = new TransactionScope())
                    //{
                    //    using (TransactionDebitService.Service1Client client = new TransactionDebitService.Service1Client())
                    //    {
                    //        debitResult = client.PerformDebitTransaction(debitAccountID, amount);
                    //    }

                    //    throw new Exception();

                    //    using (TransactionCreditService.Service1Client client = new TransactionCreditService.Service1Client())
                    //    {
                    //        creditResult = client.PerformCreditTransaction(creditAccountID, amount);
                    //    }

                    //    tsInner.Complete();
                    //} 
                    #endregion

                    #endregion

                    #region Client Call

                    //using (TransactionScope tsInner = new TransactionScope())
                    //{
                    //    debitResult = PerformDebitTransaction(debitAccountID, amount);

                    //    throw new Exception();

                    //    creditResult = PerformCreditTransaction(creditAccountID, amount);

                    //    tsInner.Complete();
                    //}

                    #endregion

                    if (debitResult && creditResult)
                    {
                        // To commit the transaction 
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                // rolling back
            }
        }

        private static void PresentTotalAmount()
        {
            using (TransactionCreditService.Service1Client client = new TransactionCreditService.Service1Client())
            {
                Console.WriteLine("Account 1: " + client.GetAccountDetails(1).ToString());
                Console.WriteLine("Account 2: " + client.GetAccountDetails(2).ToString());
                Console.WriteLine("");
            }
        }


        #region FromClientTransaction

        private static readonly string CONNECTION_STRING = ConfigurationManager.ConnectionStrings["SampleDbConnectionString1"].ConnectionString;

        public static bool PerformDebitTransaction(string debitAccountID, double amount)
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
                throw new Exception("Something went wring during debit");
            }
            return debitResult;
        }

        public static bool PerformCreditTransaction(string creditAccountID, double amount)
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
                throw new Exception("Something went wring during credit");
            }
            return creditResult;
        } 
        #endregion
    }
}
