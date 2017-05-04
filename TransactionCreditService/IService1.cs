using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;

namespace TransactionCreditService
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract, TransactionFlow(TransactionFlowOption.Allowed)]
        bool PerformCreditTransaction(string creditAccountID, double amount);

        //[OperationContract, TransactionFlow(TransactionFlowOption.Mandatory)]
        //bool PerformDebitTransaction(string debitAccountID, double amount);

        [OperationContract]
        decimal GetAccountDetails(int id);
    }
}
