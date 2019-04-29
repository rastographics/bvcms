using CmsData.Finance.Acceptiva.Core;
using Newtonsoft.Json;
using System;

namespace CmsData.Finance.Acceptiva.Transaction.Void
{
    internal class VoidResponse : Response
    {
        [JsonProperty(PropertyName = "trans_id_str")]
        public string TransIdStr { get; set; }
        [JsonProperty(PropertyName = "orig_trans_id_str")]
        public string OrigTransIdStr { get; set; }
        [JsonProperty(PropertyName = "refund_trans_id_strs")]
        public string RefundTransIdStrs { get; set; }
        [JsonProperty(PropertyName = "trans_datetime")]
        public DateTime TransDatetime { get; set; }
        [JsonProperty(PropertyName = "trans_status")]
        public int TransStatus { get; set; }
        [JsonProperty(PropertyName = "trans_status_msg")]
        public string TransStatusMsg { get; set; }
        [JsonProperty(PropertyName = "processor_response_code")]
        public string ProcessorResponseCode { get; set; }
        [JsonProperty(PropertyName = "processor_response_msg")]
        public string ProcessorResponseMsg { get; set; }
        [JsonProperty(PropertyName = "processor_response_msg_public")]
        public string ProcessorResponseMsgPublic { get; set; }
        [JsonProperty(PropertyName = "amt_processed")]
        public double AmtProcessed { get; set; }
        [JsonProperty(PropertyName = "payer_fname")]
        public string PayerFname { get; set; }
        [JsonProperty(PropertyName = "payer_lname")]
        public string PayerLname { get; set; }
        [JsonProperty(PropertyName = "payer_address")]
        public string PayerAddress { get; set; }
        [JsonProperty(PropertyName = "payer_address2")]
        public string PayerAddress2 { get; set; }
        [JsonProperty(PropertyName = "payer_city")]
        public string PayerCity { get; set; }
        [JsonProperty(PropertyName = "payer_state")]
        public string PayerState { get; set; }
        [JsonProperty(PropertyName = "payer_zip")]
        public string PayerZip { get; set; }
        [JsonProperty(PropertyName = "payer_country")]
        public string PayerCountry { get; set; }
        [JsonProperty(PropertyName = "payer_phone")]
        public string PayerPhone { get; set; }
        [JsonProperty(PropertyName = "payer_email")]
        public string PayerEmail { get; set; }
        [JsonProperty(PropertyName = "payment_type")]
        public string PaymentType { get; set; }
        [JsonProperty(PropertyName = "payer_id_str")]
        public string PayerIdStr { get; set; }
        [JsonProperty(PropertyName = "merch_acct_id_str")]
        public string MerchAcctIdStr { get; set; }
        [JsonProperty(PropertyName = "acct_name")]
        public string AcctName { get; set; }
        [JsonProperty(PropertyName = "acct_last_four")]
        public string AcctLastFour { get; set; }
        [JsonProperty(PropertyName = "cc_exp_mo")]
        public string CcExpMo { get; set; }
        [JsonProperty(PropertyName = "cc_exp_yr")]
        public string CcExpYr { get; set; }
        [JsonProperty(PropertyName = "settlement_length")]
        public string SettlementLength { get; set; }
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        [JsonProperty(PropertyName = "client_trans_id")]
        public string ClientTransId { get; set; }
        [JsonProperty(PropertyName = "client_payer_id")]
        public string ClientPayerId { get; set; }
    }
}
