using CmsData.Finance.Sage.Core;

namespace CmsData.Finance.Sage.Transaction
{
    internal abstract class TransactionProcessingRequest : Request
    {
        private const string BASE_ADDRESS = "https://gateway.sagepayments.net/web_services/vterm_extensions/transaction_processing.asmx/";

        protected TransactionProcessingRequest(string baseAddress, string operation, string id, string key)
            : base(baseAddress, operation, id, key)
        {
            // initialize standard transaction processing parameters.
            Data["T_ORDERNUM"] = string.Empty;
            Data["T_TAX"] = string.Empty;
            Data["T_SHIPPING"] = string.Empty;
            Data["C_SHIP_NAME"] = string.Empty;
            Data["C_SHIP_ADDRESS"] = string.Empty;
            Data["C_SHIP_CITY"] = string.Empty;
            Data["C_SHIP_STATE"] = string.Empty;
            Data["C_SHIP_ZIP"] = string.Empty;
            Data["C_SHIP_COUNTRY"] = string.Empty;
        }

        protected TransactionProcessingRequest(string id, string key, string operation)
            : this(BASE_ADDRESS, operation, id, key) { }

        public new TransactionProcessingResponse Execute()
        {
            var response = base.Execute();
            return new TransactionProcessingResponse(response);
        }
    }
}