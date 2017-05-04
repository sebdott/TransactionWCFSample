using System;
using System.Collections.Generic;
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
                using (TransactionScope ts = new TransactionScope())
                {
                    using (TransactionDebitService.Service1Client client = new TransactionDebitService.Service1Client())
                    {
                        debitResult = client.PerformDebitTransaction(debitAccountID, amount);
                    }

                    using (TransactionCreditService.Service1Client client = new TransactionCreditService.Service1Client())
                    {
                        creditResult = client.PerformCreditTransaction(creditAccountID, amount);
                    }

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
    }
}
