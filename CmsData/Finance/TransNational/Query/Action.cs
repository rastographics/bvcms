using System;
using System.Xml.Linq;

namespace CmsData.Finance.TransNational.Query
{
    internal class Action
    {
        public ActionType ActionType { get; private set; }

        public decimal Amount { get; private set; }

        public DateTime Date { get; private set; }

        public bool Success { get; private set; }

        public string ResponseText { get; private set; }

        public string BatchId { get; private set; }

        public string ProcessorBatchId { get; private set; }

        public Action(XElement data)
        {
            ActionType = GetActionType(data.Element("action_type").Value);
            Amount = decimal.Parse(data.Element("amount").Value);
            Date = DateTime.ParseExact(data.Element("date").Value, "yyyyMMddHHmmss", null).ToLocalTime();
            Success = data.Element("success").Value == "1";
            ResponseText = data.Element("response_text").Value;
            BatchId = data.Element("batch_id").Value;
            ProcessorBatchId = data.Element("processor_batch_id").Value;
        }

        private ActionType GetActionType(string actionType)
        {
            switch (actionType)
            {
                case "sale":
                    return ActionType.Sale;
                case "refund":
                    return ActionType.Refund;
                case "credit":
                    return ActionType.Credit;
                case "auth":
                    return ActionType.Auth;
                case "capture":
                    return ActionType.Capture;
                case "void":
                    return ActionType.Void;
                case "settle":
                    return ActionType.Settle;
                case "check_return":
                    return ActionType.CheckReturn;
                case"check_late_return" :
                    return ActionType.CheckLateReturn;
                default:
                    return ActionType.Unknown;
            }
        }
    }
}
