
using CmsData.Finance.TransNational.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsData.Finance.TransNational.Query
{
    internal class QueryRequest : Request
    {
        private const string URL = "https://secure.networkmerchants.com/api/query.php";

        public QueryRequest(string userName, string password, IEnumerable<string> transactionIds)
            : base(URL, userName, password)
        {
            Data["transaction_id"] = string.Join(",", transactionIds);
        }

        public QueryRequest(string userName, string password, DateTime startDate, DateTime endDate)
            : base(URL, userName, password)
        {
            Data["start_date"] = startDate.ToUniversalTime().ToString("yyyyMMddHHmmss");
            Data["end_date"] = endDate.ToUniversalTime().ToString("yyyyMMddHHmmss");
        }

        public QueryRequest(string userName, string password, IEnumerable<string> transactionIds, DateTime startDate, DateTime endDate)
            : this(userName, password, transactionIds)
        {
            Data["start_date"] = startDate.ToUniversalTime().ToString("yyyyMMddHHmmss");
            Data["end_date"] = endDate.ToUniversalTime().ToString("yyyyMMddHHmmss");
        }

        public QueryRequest(string userName, string password, DateTime startDate, DateTime endDate, IEnumerable<Condition> conditions)
            : this(userName, password, startDate, endDate)
        {
            Data["condition"] = string.Join(",", conditions.Select(GetConditionValue));
        }
        
        public QueryRequest(string userName, string password, DateTime startDate, DateTime endDate, IEnumerable<Condition> conditions, IEnumerable<TransactionType> transactionTypes)
            : this(userName, password, startDate, endDate, conditions)
        {
            Data["transaction_type"] = string.Join(",", transactionTypes.Select(GetTransactionTypeValue));
        }

        public QueryRequest(string userName, string password, DateTime startDate, DateTime endDate, IEnumerable<Condition> conditions, IEnumerable<ActionType> actionTypes)
            : this(userName, password, startDate, endDate, conditions)
        {
            Data["action_type"] = string.Join(",", actionTypes.Select(GetActionTypeValue));
        }

        public QueryRequest(string userName, string password, DateTime startDate, DateTime endDate, IEnumerable<Condition> conditions, IEnumerable<TransactionType> transactionTypes, IEnumerable<ActionType> actionTypes)
            : this(userName, password, startDate, endDate, conditions, transactionTypes)
        {
            Data["action_type"] = string.Join(",", actionTypes.Select(GetActionTypeValue));
        }

        public new QueryResponse Execute()
        {
            var response = base.Execute();
            return new QueryResponse(response);
        }

        private string GetConditionValue(Condition condition)
        {
            switch (condition)
            {
                case Condition.Pending:
                    return "pending";
                case Condition.PendingSettlement:
                    return "pendingsettlement";
                case Condition.Failed:
                    return "failed";
                case Condition.Canceled:
                    return "canceled";
                case Condition.Complete:
                    return "complete";
                case Condition.Unknown:
                    return "unknown";
                default:
                    return "unknown";
            }
        }

        private string GetTransactionTypeValue(TransactionType transactionType)
        {
            switch (transactionType)
            {
                case TransactionType.CreditCard:
                    return "cc";
                case TransactionType.Ach:
                    return "ck";
                default:
                    return "unknown";
            }
        }

        private string GetActionTypeValue(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.Sale:
                    return "sale";
                case ActionType.Refund:
                    return "refund";
                case ActionType.Credit:
                    return "credit";
                case ActionType.Auth:
                    return "auth";
                case ActionType.Capture:
                    return "capture";
                case ActionType.Void:
                    return "void";
                case ActionType.Settle:
                    return "settle";
                case ActionType.CheckReturn:
                    return "check_return";
                case ActionType.CheckLateReturn:
                    return "check_late_return";
                default:
                    return "unknown";
            }
        }
    }
}
