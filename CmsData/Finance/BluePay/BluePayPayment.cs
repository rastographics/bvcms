/*
 * Bluepay C#.NET Sample code.
 *
 * Developed by Joel Tosi, Chris Jansen, and Justin Slingerland of Bluepay.
 *
 * Updated: 2013-11-20
 *
 * This code is Free.  You may use it, modify it and redistribute it.
 * If you do make modifications that are useful, Bluepay would love it if you donated
 * them back to us!
 *
 * 
 * Documentation for Transactions: http://www.bluepay.com/sites/default/files/documentation/BluePay_bp10emu/BluePay%201-0%20Emulator.txt
 * 
 * Documentation for Reporting: http://www.bluepay.com/sites/default/files/documentation/BluePay_bpdailyreport2/dailyreport2.txt
 * 
 *
 */
using System;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Specialized;
using CsvHelper;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;


namespace BPCSharp
{
    ///<summary>
    ///Transaction Object
    /// </summary>
    /// 
    public class Transaction
    {

        public string invoice_id;
        public string trans_type;
        public string payment_type;
        public string name1;
        public string name2;

        public string id { get; private set; }

        public string settlement_id { get; private set; }

        public decimal amount { get; private set; }

        public string status { get; private set; }

        public string message { get; private set; }

        public DateTime issue_date { get; private set; }

        public DateTime settle_date { get; private set; }

        public string payment_account { get; set; }
    }

    ///<summary>
    ///BluePay Reporting Object
    /// </summary>
    /// 
    public class BluePayReport
    {
        public IEnumerable<Transaction> Transactions { get; private set; }
        private string _csvResponse;
         public BluePayReport(string csvResponse)
        {
            _csvResponse = csvResponse;
        }

         public IEnumerable<Transaction> GetTransactionList() {

             var stream = new MemoryStream(Encoding.UTF8.GetBytes(_csvResponse ?? ""));
             var reader = new StreamReader(stream);
             var csv = new CsvReader(reader);
             return csv.GetRecords<Transaction>().ToList();
         }

    }


    /// <summary>
    /// This is the BluePayPayment object.
    /// </summary>
    public class BluePayPayment
    {
        // required for every transaction
        public string accountID = "";
        public string URL = "";
        public string secretKey = "";
        public string ServiceMode = "";

        // required for auth or sale
        public string paymentAccount = "";
        public string cvv2 = "";
        public string cardExpire = "";
        public Regex track1And2 = new Regex(@"(%B)\d{0,19}\^([\w\s]*)\/([\w\s]*)([\s]*)\^\d{7}\w*\?;\d{0,19}=\d{7}\w*\?");
        public Regex track2Only = new Regex(@";\d{0,19}=\d{7}\w*\?");
        public string swipeData = "";
        public string routingNum = "";
        public string accountNum = "";
        public string accountType = "";
        public string docType = "";
        public string name1 = "";
        public string name2 = "";
        public string addr1 = "";
        public string city = "";
        public string state = "";
        public string zip = "";

        // optional for auth or sale
        public string addr2 = "";
        public string phone = "";
        public string email = "";
        public string country = "";

        // transaction variables
        public string amount = "";
        public string transType = "";
        public string paymentType = "";
        public string masterID = "";
        public string rebillID = "";

        // rebill variables
        public string doRebill = "";
        public string rebillAmount = "";
        public string rebillFirstDate = "";
        public string rebillExpr = "";
        public string rebillCycles = "";
        public string rebillNextAmount = "";
        public string rebillNextDate = "";
        public string rebillStatus = "";
        public string templateID = "";

        // level2 variables
        public string customID1 = "";
        public string customID2 = "";
        public string invoiceID = "";
        public string orderID = "";
        public string amountTip = "";
        public string amountTax = "";
        public string amountFood = "";
        public string amountMisc = "";
        public string memo = "";

        // rebill fields
        public string reportStartDate = "";
        public string reportEndDate = "";
        public string doNotEscape = "";
        public string queryBySettlement = "";
        public string queryByHierarchy = "";
        public string excludeErrors = "";

        public string response = "";
        public string TPS = "";
        public string BPheaderstring = "";


        private NameValueCollection postData = HttpUtility.ParseQueryString(string.Empty);

        public BluePayPayment(string accountID, string secretKey, string mode)
        {
            this.accountID = accountID;
            this.secretKey = secretKey;
            this.ServiceMode = mode;
        }

        /// <summary>
        /// Sets Customer Information
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        /// <param name="addr1"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="zip"></param>
        public void setCustomerInformation(string name1, string name2, string addr1, string city, string state, string zip)
        {
            this.name1 = name1;
            this.name2 = name2;
            this.addr1 = addr1;
            this.city = city;
            this.state = state;
            this.zip = zip;
        }

        /// <summary>
        /// Sets all transaction info for a credit card transaction
        /// </summary>
        /// 
        public void setupCCTransaction(int peopleId, string cardnumber, string expires, string description, int? tranid, string cardcode, string email, string first, string last, string addr, string city, string state, string zip, string phone)
        {
            this.setCCInformation(cardnumber, expires, cardcode);
            this.setCustomerInformation(first, last, addr, "", city, state, zip, peopleId);
            this.setPhone(phone);
            this.setEmail(email);
            if(tranid.HasValue)
                this.setInvoiceID(tranid.GetValueOrDefault().ToString());
            this.setMemo(description);
        
        }

        /// <summary>
        /// Sets all transaction info for an ACH transaction
        /// </summary>
        /// 
        public void setupACHTransaction(int peopleId, string routing, string account, string description, int? tranid, string email, string first, string last, string addr, string city, string state, string zip, string phone)
        {
            this.setACHInformation(routing, account, "C", "WEB");
            this.setCustomerInformation(first, last, addr, "",city, state, zip,peopleId);
            this.setPhone(phone);
            this.setEmail(email);
            if (tranid.HasValue)
                this.setInvoiceID(tranid.GetValueOrDefault().ToString());
            this.setMemo(description);
           

        }

        public void setupVaultTransaction(string description, int? tranid)
        {
            if (tranid.HasValue)
                this.setInvoiceID(tranid.GetValueOrDefault().ToString());
            this.setMemo(description);
        }

        /// <summary>
        /// Sets Customer Information
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        /// <param name="addr1"></param>
        /// <param name="addr2"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="zip"></param>
        public void setCustomerInformation(string name1, string name2, string addr1, string addr2, string city, string state, string zip, int? peopleId)
        {
            this.name1 = name1;
            this.name2 = name2;
            this.addr1 = addr1;
            this.addr2 = addr2;
            this.city = city;
            this.state = state;
            this.zip = zip;
            //No field for PeopleID in BluePay so save it in Custom1 field
            if(peopleId.HasValue)
                this.setCustomID1(peopleId.Value.ToString());
        }

        /// <summary>
        /// Sets Customer Information
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        /// <param name="addr1"></param>
        /// <param name="addr2"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="zip"></param>
        /// <param name="country"></param>
        public void setCustomerInformation(string name1, string name2, string addr1, string addr2, string city, string state, string zip, string country,int? peopleId)
        {
            setCustomerInformation(name1, name2, addr1, addr2, city, state, zip, peopleId);
            this.country = country;
        }

        /// <summary>
        /// Sets Credit Card Information
        /// </summary>
        /// <param name="cardNum"></param>
        /// <param name="cardExpire"></param>
        /// <param name="cvv2"></param>
        public void setCCInformation(string cardNum, string cardExpire, string cvv2 = null)
        {
            this.paymentType = "CREDIT";

            if (!cardNum.StartsWith("X")) //checks for masked number in case this is a request to update vault info besides card#
                this.paymentAccount = cardNum;

            if (cvv2 != null && !cvv2.StartsWith("X")) //checks for masked number in case this is a request to update vault info besides cvv2#
                this.cvv2 = cvv2;

            this.cardExpire = cardExpire;
            
        }

        /// <summary>
        /// Sets Swipe Information Using Either Both Track 1 2, Or Just Track 2
        /// </summary>
        /// <param name="swipe"></param> 
        public void swipe(string swipe)
        {
            this.paymentType = "CREDIT";
            this.swipeData = swipe;
        }

        /// <summary>
        /// Sets ACH Information
        /// </summary>
        /// <param name="routingNum"></param>
        /// <param name="accountNum"></param>
        /// <param name="accountType"></param>
        /// <param name="docType"></param>
        public void setACHInformation(string routingNum, string accountNum, string accountType, string docType = null)
        {
            this.paymentType = "ACH";
            this.routingNum = routingNum;
            this.accountNum = accountNum;
            this.accountType = accountType;
            this.docType = docType;
        }

        /// <summary>
        /// Sets Rebilling Cycle Information. To be used with other functions to create a transaction.
        /// </summary>
        /// <param name="rebAmount"></param>
        /// <param name="rebFirstDate"></param>
        /// <param name="rebExpr"></param>
        /// <param name="rebCycles"></param>
        public void setRebillingInformation(string rebAmount, string rebFirstDate, string rebExpr, string rebCycles)
        {
            this.doRebill = "1";
            this.rebillAmount = rebAmount;
            this.rebillFirstDate = rebFirstDate;
            this.rebillExpr = rebExpr;
            this.rebillCycles = rebCycles;
        }

        /// <summary>
        /// Updates Rebilling Cycle
        /// </summary>
        /// <param name="rebillID"></param>
        /// <param name="rebNextDate"></param>
        /// <param name="rebExpr"></param>
        /// <param name="rebCycles"></param>
        /// <param name="rebAmount"></param>
        /// <param name="rebNextAmount"></param>
        public void updateRebillingInformation(string rebillID, string rebNextDate, string rebExpr, string rebCycles, string rebAmount, string rebNextAmount)
        {
            this.transType = "SET";
            this.rebillID = rebillID;
            this.rebillNextDate = rebNextDate;
            this.rebillExpr = rebExpr;
            this.rebillCycles = rebCycles;
            this.rebillAmount = rebAmount;
            this.rebillNextAmount = rebNextAmount;
        }

        /// <summary>
        /// Updates a rebilling cycle's payment information
        /// </summary>
        /// <param name="templateID"></param>
        public void updateRebillPaymentInformation(string templateID)
        {
            this.templateID = templateID;
        }

        /// <summary>
        /// Cancels Rebilling Cycle
        /// </summary>
        /// <param name="rebillID"></param>
        public void cancelRebilling(string rebillID)
        {
            this.transType = "SET";
            this.rebillID = rebillID;
            this.rebillStatus = "stopped";
        }

        /// <summary>
        /// Gets a existing rebilling cycle's status
        /// </summary>
        /// <param name="rebillID"></param>
        public void getRebillStatus(string rebillID)
        {
            this.transType = "GET";
            this.rebillID = rebillID;
        }

        /// <summary>
        /// Gets Report of Transaction Data 
        /// </summary>
        /// <param name="reportStart"></param>
        /// <param name="reportEnd"></param>
        /// <param name="subaccountsSearched"></param>
        public void getTransactionReport(string reportStart, string reportEnd, string subaccountsSearched)
        {
            this.queryBySettlement = "0";
            this.reportStartDate = reportStart;
            this.reportEndDate = reportEnd;
            this.queryByHierarchy = subaccountsSearched;
        }

        /// <summary>
        /// Gets Report of Transaction Data
        /// </summary>
        /// <param name="reportStart"></param>
        /// <param name="reportEnd"></param>
        /// <param name="subaccountsSearched"></param>
        /// <param name="doNotEscape"></param>
        public void getTransactionReport(string reportStart, string reportEnd, string subaccountsSearched,
                string doNotEscape)
        {
            this.queryBySettlement = "0";
            this.reportStartDate = reportStart;
            this.reportEndDate = reportEnd;
            this.queryByHierarchy = subaccountsSearched;
            this.doNotEscape = doNotEscape;
        }

        /// <summary>
        /// Gets Report of Transaction Data
        /// </summary>
        /// <param name="reportStart"></param>
        /// <param name="reportEnd"></param>
        /// <param name="subaccountsSearched"></param>
        /// <param name="doNotEscape"></param>
        /// <param name="errors"></param>
        public void getTransactionReport(string reportStart, string reportEnd, string subaccountsSearched,
                string doNotEscape, string errors)
        {
            this.queryBySettlement = "0";
            this.reportStartDate = reportStart;
            this.reportEndDate = reportEnd;
            this.queryByHierarchy = subaccountsSearched;
            this.doNotEscape = doNotEscape;
            this.excludeErrors = errors;
        }

        /// <summary>
        /// Gets Report of Settled Transaction Data
        /// </summary>
        /// <param name="reportStart"></param>
        /// <param name="reportEnd"></param>
        /// <param name="subaccountsSearched"></param>
        public void getTransactionSettledReport(string reportStart, string reportEnd, string subaccountsSearched)
        {
            this.queryBySettlement = "1";
            this.reportStartDate = reportStart;
            this.reportEndDate = reportEnd;
            this.queryByHierarchy = subaccountsSearched;
        }

        /// <summary>
        /// Gets Report of Settled Transaction Data
        /// </summary>
        /// <param name="reportStart"></param>
        /// <param name="reportEnd"></param>
        /// <param name="subaccountsSearched"></param>
        /// <param name="doNotEscape"></param>
        public void getTransactionSettledReport(string reportStart, string reportEnd, string subaccountsSearched,
                string doNotEscape)
        {
            this.queryBySettlement = "1";
            this.reportStartDate = reportStart;
            this.reportEndDate = reportEnd;
            this.queryByHierarchy = subaccountsSearched;
            this.doNotEscape = doNotEscape;
        }

        /// <summary>
        /// Gets Report of Settled Transaction Data
        /// </summary>
        /// <param name="reportStart"></param>
        /// <param name="reportEnd"></param>
        /// <param name="subaccountsSearched"></param>
        /// <param name="doNotEscape"></param>
        /// <param name="includeErrors"></param>
        public void getTransactionSettledReport(DateTime reportStart, DateTime reportEnd, bool subaccountsSearched,
                bool doNotEscape, bool excludeErrors)
        {
            this.queryBySettlement = "1";
            this.reportStartDate = reportStart.ToString("yyyy-MM-dd");
            this.reportEndDate = reportEnd.ToString("yyyy-MM-dd");
            this.queryByHierarchy = subaccountsSearched ? "1":"0";
            this.doNotEscape = doNotEscape ? "1" : "0";
            this.excludeErrors = excludeErrors ? "1" : "0";
        }

        /// <summary>
        /// Gets Details of a Transaction
        /// </summary>
        /// <param name="reportStart"></param>
        /// <param name="reportEnd"></param>
        public void getSingleTransQuery(string reportStart, string reportEnd)
        {
            this.reportStartDate = reportStart;
            this.reportEndDate = reportEnd;
        }

        /// <summary>
        /// Gets Details of a Transaction
        /// </summary>
        /// <param name="reportStart"></param>
        /// <param name="reportEnd"></param>
        /// <param name="errors"></param>
        public void getSingleTransQuery(string reportStart, string reportEnd, string errors)
        {
            this.reportStartDate = reportStart;
            this.reportEndDate = reportEnd;
            this.excludeErrors = errors;
        }

        /// <summary>
        /// Queries by Transaction ID. To be used with getSingleTransQuery
        /// </summary>
        /// <param name="transID"></param>
        public void queryByTransactionID(string transID)
        {
            this.masterID = transID;
        }

        /// <summary>
        /// Queries by Payment Type. To be used with getSingleTransQuery
        /// </summary>
        /// <param name="payType"></param>
        public void queryByPaymentType(string payType)
        {
            this.paymentType = payType;
        }

        /// <summary>
        /// Queries by Transaction Type. To be used with getSingleTransQuery
        /// </summary>
        /// <param name="transType"></param>
        public void queryBytransType(string transType)
        {
            this.transType = transType;
        }

        /// <summary>
        /// Queries by Transaction Amount. To be used with getSingleTransQuery
        /// </summary>
        /// <param name="amount"></param>
        public void queryByAmount(string amount)
        {
            this.amount = amount;
        }

        /// <summary>
        /// Queries by First Name (NAME1) . To be used with getSingleTransQuery
        /// </summary>
        /// <param name="name1"></param>
        public void queryByName1(string name1)
        {
            this.name1 = name1;
        }

        /// <summary>
        /// Queries by Last Name (NAME2) . To be used with getSingleTransQuery
        /// </summary>
        /// <param name="name2"></param>
        public void queryByName2(string name2)
        {
            this.name2 = name2;
        }

        /// <summary>
        /// Runs a Sale Transaction
        /// </summary>
        /// <param name="amount"></param>
        public void sale(string amount)
        {
            this.transType = "SALE";
            this.amount = amount;
        }

        /// <summary>
        /// Runs a Sale Transaction
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="masterID"></param>
        public void sale(string amount, string masterID)
        {
            this.transType = "SALE";
            this.amount = amount;
            this.masterID = masterID;
        }

        /// <summary>
        /// Runs an Auth Transaction
        /// </summary>
        /// <param name="amount"></param>
        public void auth(string amount)
        {
            this.transType = "AUTH";
            this.amount = amount;
        }

        /// <summary>
        /// Runs an Auth Transaction
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="masterID"></param>
        public void auth(string amount, string masterID)
        {
            this.transType = "AUTH";
            this.amount = amount;
            this.masterID = masterID;
        }

        /// <summary>
        /// Runs a Refund Transaction
        /// </summary>
        /// <param name="masterID"></param>
        public void refund(string masterID)
        {
            this.transType = "REFUND";
            this.masterID = masterID;
        }

        /// <summary>
        /// Runs a Refund Transaction
        /// </summary>
        /// <param name="masterID"></param>
        /// <param name="amount"></param>
        public void refund(string masterID, string amount)
        {
            this.transType = "REFUND";
            this.masterID = masterID;
            this.amount = amount;
        }

        public void voidTransaction(string masterID)
        {
            this.transType = "VOID";
            this.masterID = masterID;
        }

        /// <summary>
        /// Runs a Capture Transaction
        /// </summary>
        /// <param name="masterID"></param>
        public void capture(string masterID)
        {
            this.transType = "CAPTURE";
            this.masterID = masterID;
        }

        /// <summary>
        /// Runs a Capture Transaction
        /// </summary>
        /// <param name="masterID"></param>
        /// <param name="amount"></param>
        public void capture(string masterID, string amount)
        {
            this.transType = "CAPTURE";
            this.masterID = masterID;
            this.amount = amount;
        }

        /// <summary>
        /// Sets Custom ID Field
        /// </summary>
        /// <param name="customID1"></param>
        public void setCustomID1(string customID1)
        {
            this.customID1 = customID1;
        }

        /// <summary>
        /// Sets Custom ID2 Field
        /// </summary>
        /// <param name="customID2"></param>
        public void setCustomID2(string customID2)
        {
            this.customID2 = customID2;
        }

        /// <summary>
        /// Sets Invoice ID Field
        /// </summary>
        /// <param name="invoiceID"></param>
        public void setInvoiceID(string invoiceID)
        {
            this.invoiceID = invoiceID;
        }

        /// <summary>
        /// Sets Order ID Field
        /// </summary>
        /// <param name="orderID"></param>
        public void setOrderID(string orderID)
        {
            this.orderID = orderID;
        }

        /// <summary>
        /// Sets Amount Tip Field
        /// </summary>
        /// <param name="amountTip"></param>
        public void setAmountTip(string amountTip)
        {
            this.amountTip = amountTip;
        }

        /// <summary>
        /// Sets Amount Tax Field
        /// </summary>
        /// <param name="amountTax"></param>
        public void setAmountTax(string amountTax)
        {
            this.amountTax = amountTax;
        }

        /// <summary>
        /// Sets Amount Food Field
        /// </summary>
        /// <param name="amountFood"></param>
        public void setAmountFood(string amountFood)
        {
            this.amountFood = amountFood;
        }

        /// <summary>
        /// Sets Amount Misc Field
        /// </summary>
        /// <param name="amountMisc"></param>
        public void setAmountMisc(string amountMisc)
        {
            this.amountMisc = amountMisc;
        }

        /// <summary>
        /// Sets Memo Field
        /// </summary>
        /// <param name="memo"></param>
        public void setMemo(string memo)
        {
            this.memo = memo;
        }

        /// <summary>
        /// Sets Phone Field
        /// </summary>
        /// <param name="Phone"></param>
        public void setPhone(string Phone)
        {
            this.phone = Phone;
        }

        /// <summary>
        /// Sets Email Field
        /// </summary>
        /// <param name="Email"></param>
        public void setEmail(string Email)
        {
            this.email = Email;
        }

        public void Set_Param(string Name, string Value)
        {
            Name = Value;
        }

        /// <summary>
        /// Calculates TAMPER_PROOF_SEAL for bp20post API
        /// </summary>
        public void calcTPS()
        {
            string tamper_proof_seal = this.secretKey
                                    + this.accountID
                                    + this.transType
                                    + this.amount
                                    + this.doRebill
                                    + this.rebillFirstDate
                                    + this.rebillExpr
                                    + this.rebillCycles
                                    + this.rebillAmount
                                    + this.masterID
                                    + this.ServiceMode;
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash;
            ASCIIEncoding encode = new ASCIIEncoding();

            byte[] buffer = encode.GetBytes(tamper_proof_seal);
            hash = md5.ComputeHash(buffer);
            this.TPS = ByteArrayToString(hash);
        }

        /// <summary>
        /// Calculates TAMPER_PROOF_SEAL for bp20rebadmin API
        /// </summary>
        public void calcRebillTPS()
        {
            string tamper_proof_seal = this.secretKey +
                                 this.accountID +
                                 this.transType +
                                 this.rebillID;

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash;
            ASCIIEncoding encode = new ASCIIEncoding();

            byte[] buffer = encode.GetBytes(tamper_proof_seal);
            hash = md5.ComputeHash(buffer);
            this.TPS = ByteArrayToString(hash);
        }

        /// <summary>
        /// Calculates TAMPER_PROOF_SEAL for bpdailyreport2 and stq APIs
        /// </summary>
        public void calcReportTPS()
        {
            string tamper_proof_seal = this.secretKey + this.accountID + this.reportStartDate + this.reportEndDate;
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash;
            ASCIIEncoding encode = new ASCIIEncoding();

            byte[] buffer = encode.GetBytes(tamper_proof_seal);
            hash = md5.ComputeHash(buffer);
            this.TPS = ByteArrayToString(hash);
        }

        /// <summary>
        /// Calculates BP_STAMP for trans notify post API
        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="transID"></param>
        /// <param name="transStatus"></param>
        /// <param name="transType"></param>
        /// <param name="amount"></param>
        /// <param name="batchID"></param>
        /// <param name="batchStatus"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalAmount"></param>
        /// <param name="batchUploadID"></param>
        /// <param name="rebillID"></param>
        /// <param name="rebillAmount"></param>
        /// <param name="rebillStatus"></param>
        /// <returns></returns>
        public static string calcTransNotifyTPS(string secretKey, string transID, string transStatus, string transType,
            string amount, string batchID, string batchStatus, string totalCount, string totalAmount,
            string batchUploadID, string rebillID, string rebillAmount, string rebillStatus)
        {
            string tamper_proof_seal = secretKey + transID + transStatus + transType + amount + batchID + batchStatus +
            totalCount + totalAmount + batchUploadID + rebillID + rebillAmount + rebillStatus;
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash;
            ASCIIEncoding encode = new ASCIIEncoding();

            byte[] buffer = encode.GetBytes(tamper_proof_seal);
            hash = md5.ComputeHash(buffer);
            tamper_proof_seal = ByteArrayToString(hash);
            return tamper_proof_seal;
        }

        //This is used to convert a byte array to a hex string
        static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }

     

        public string Process()
        {
            postData.Clear();
            
            if (this.queryByHierarchy != "")
            {
                calcReportTPS();
                this.URL = "https://secure.bluepay.com/interfaces/bpdailyreport2";
                addPostParam("ACCOUNT_ID", this.accountID);
                addPostParam("MODE", this.ServiceMode);
                addPostParam("TAMPER_PROOF_SEAL", this.TPS);
                addPostParam("REPORT_START_DATE", this.reportStartDate);
                addPostParam("REPORT_END_DATE", this.reportEndDate);
                addPostParam("DO_NOT_ESCAPE", this.doNotEscape);
                addPostParam("QUERY_BY_SETTLEMENT", this.queryBySettlement);
                addPostParam("QUERY_BY_HIERARCHY", this.queryByHierarchy);
                addPostParam("EXCLUDE_ERRORS", this.excludeErrors);
            }
            else if (this.reportStartDate != "")
            {
                calcReportTPS();
                this.URL = "https://secure.bluepay.com/interfaces/stq";
                addPostParam("ACCOUNT_ID", this.accountID);
                addPostParam("MODE", this.ServiceMode);
                addPostParam("TAMPER_PROOF_SEAL", this.TPS);
                addPostParam("REPORT_START_DATE", this.reportStartDate);
                addPostParam("REPORT_END_DATE", this.reportEndDate);
                addPostParam("DO_NOT_ESCAPE", this.doNotEscape);
                addPostParam("EXCLUDE_ERRORS", this.excludeErrors);
                addPostParam("id", this.masterID, true);
                addPostParam("payment_type", this.paymentType, true);
                addPostParam("trans_type", this.transType, true);
                addPostParam("amount", this.amount, true);
                addPostParam("name1", this.name1, true);
                addPostParam("name2", this.name2, true);
            }
            else if (this.transType != "SET" && this.transType != "GET")
            {
                calcTPS();
                this.URL = "https://secure.bluepay.com/interfaces/bp10emu";
                addPostParam("MERCHANT", this.accountID);
                addPostParam("MODE", this.ServiceMode);
                addPostParam("TRANSACTION_TYPE", this.transType);
                addPostParam("TAMPER_PROOF_SEAL", this.TPS);
                addPostParam("NAME1", this.name1);
                addPostParam("NAME2", this.name2);
                addPostParam("AMOUNT", this.amount);
                addPostParam("ADDR1", this.addr1);
                addPostParam("ADDR2", this.addr2);
                addPostParam("CITY", this.city);
                addPostParam("STATE", this.state);
                addPostParam("ZIPCODE", this.zip);
                addPostParam("COMMENT", this.memo);
                addPostParam("PHONE", this.phone);
                addPostParam("EMAIL", this.email);
                addPostParam("REBILLING", this.doRebill);
                addPostParam("REB_FIRST_DATE", this.rebillFirstDate);
                addPostParam("REB_EXPR", this.rebillExpr);
                addPostParam("REB_CYCLES", this.rebillCycles);
                addPostParam("REB_AMOUNT", this.rebillAmount);
                addPostParam("RRNO", this.masterID);
                addPostParam("PAYMENT_TYPE", this.paymentType);
                addPostParam("INVOICE_ID", this.invoiceID);
                addPostParam("ORDER_ID", this.orderID);
                addPostParam("CUSTOM_ID", this.customID1);
                addPostParam("CUSTOM_ID2", this.customID2);
                addPostParam("AMOUNT_TIP", this.amountTip);
                addPostParam("AMOUNT_TAX", this.amountTax);
                addPostParam("AMOUNT_FOOD", this.amountFood);
                addPostParam("AMOUNT_MISC", this.amountMisc);
                addPostParam("REMOTE_IP",System.Net.Dns.GetHostEntry("").AddressList[0].ToString());
                addPostParam("RESPONSEVERSION", "3");
               
                if (this.swipeData != "")
                {
                    Match matchTrack1And2 = track1And2.Match(this.swipeData);
                    Match matchTrack2 = track2Only.Match(this.swipeData);
                    if (matchTrack1And2.Success)
                        addPostParam("SWIPE", this.swipeData);
                    else if (matchTrack2.Success)
                        addPostParam("TRACK2", this.swipeData);
                }
                else if (this.paymentType == "CREDIT")
                {
                    addPostParam("CC_NUM", this.paymentAccount,true);
                    addPostParam("CC_EXPIRES", this.cardExpire,true);
                    addPostParam("CVCVV2", this.cvv2,true);
                }
                else
                {
                    addPostParam("ACH_ROUTING", this.routingNum);
                    addPostParam("ACH_ACCOUNT", this.accountNum);
                    addPostParam("ACH_ACCOUNT_TYPE", this.accountType);
                    addPostParam("DOC_TYPE", this.docType);
                }
            }
            else
            {
                calcRebillTPS();
                this.URL = "https://secure.bluepay.com/interfaces/bp20rebadmin";
                addPostParam("ACCOUNT_ID", this.accountID);
                addPostParam("TAMPER_PROOF_SEAL", this.TPS);
                addPostParam("TRANS_TYPE", this.transType);
                addPostParam("REBILL_ID", this.rebillID);
                addPostParam("TEMPLATE_ID", this.templateID);
                addPostParam("REB_EXPR", this.rebillExpr);
                addPostParam("REB_CYCLES", this.rebillCycles);
                addPostParam("REB_AMOUNT", this.rebillAmount);
                addPostParam("NEXT_AMOUNT", this.rebillNextAmount);
                addPostParam("NEXT_DATE", this.rebillNextDate);
                addPostParam("STATUS", this.rebillStatus);
            }

            //Create HTTPS POST object and send to BluePay
            ASCIIEncoding encoding = new ASCIIEncoding();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(this.URL));
            request.AllowAutoRedirect = false;

            byte[] data = encoding.GetBytes(postData.ToString());

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            Stream postdata = request.GetRequestStream();
            postdata.Write(data, 0, data.Length);
            postdata.Close();

            //get response    
            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse();
                getResponse(request);
                httpResponse.Close();
            }
            catch (WebException e)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)e.Response;
                getResponse(e);
                httpResponse.Close();
            }
            return getStatus();
        }

        private void addPostParam(string key, string value, bool doNotAddBlankValue = false)
        {
            if (!doNotAddBlankValue)
                this.postData[key] = value;
            else
                if (!String.IsNullOrWhiteSpace(value))
                    this.postData[key] = value;
        }


        public void getResponse(HttpWebRequest request)
        {
            HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse();
            responseParams(httpResponse);
        }

        public void getResponse(WebException request)
        {
            HttpWebResponse httpResponse = (HttpWebResponse)request.Response;
            responseParams(httpResponse);
        }

        public string responseParams(HttpWebResponse httpResponse)
        {
            Stream receiveStream = httpResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, encode);
            Char[] read = new Char[512];
            int count = readStream.Read(read, 0, 512);
            while (count > 0)
            {
                // Dumps the 256 characters on a string and displays the string to the console.
                String str = new String(read, 0, count);
                response = response + HttpUtility.UrlDecode(str);
                count = readStream.Read(read, 0, 512);
            }
            httpResponse.Close();
            return response;
        }

        /// <summary>
        /// Returns STATUS or status from response
        /// </summary>
        /// <returns></returns>
        public string getStatus()
        {
            Regex r = new Regex(@"Result=([^&$]*)");
            Match m = r.Match(response);
            if (m.Success)
                return (m.Value.Substring(7));
            r = new Regex(@"status=([^&$]*)");
            m = r.Match(response);
            if (m.Success)
                return (m.Value.Substring(7));
            else
                return "";
        }

        /// <summary>
        /// Returns bool whether Approved from response
        /// </summary>
        /// <returns></returns>
        public bool getIsApproved()
        {
            return getStatus().ToUpper() == "APPROVED";
        }



        /// <summary>
        /// Returns TRANS_ID from response
        /// </summary>
        /// <returns></returns>
        public string getTransID()
        {
            Regex r = new Regex(@"RRNO=([^&$]*)");
            Match m = r.Match(response);
            if (m.Success)
                return (m.Value.Substring(5));
            else
                return "";
        }

        /// <summary>
        /// Returns MESSAGE from Response
        /// </summary>
        /// <returns></returns>
        public string getMessage()
        {
            Regex r = new Regex(@"MESSAGE=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
            {
                string[] message = m.Value.Substring(8).Split('"');
                return message[0];
            }
            else
                return "";
        }

        /// <summary>
        /// Returns CVV2 from Response
        /// </summary>
        /// <returns></returns>
        public string getCVV2()
        {
            Regex r = new Regex(@"CVV2=([^&$]*)");
            Match m = r.Match(response);
            if (m.Success)
                return m.Value.Substring(5);
            else
                return "";
        }

        /// <summary>
        /// Returns AVS from Response
        /// </summary>
        /// <returns></returns>
        public string getAVS()
        {
            Regex r = new Regex(@"AVS=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
                return m.Value.Substring(4);
            else
                return "";
        }

        /// <summary>
        /// Returns PAYMENT_ACCOUNT from response
        /// </summary>
        /// <returns></returns>
        public string getMaskedPaymentAccount()
        {
            Regex r = new Regex("PAYMENT_ACCOUNT=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
                return m.Value.Substring(16);
            else
                return "";
        }

        /// <summary>
        /// Returns CARD_TYPE from response
        /// </summary>
        /// <returns></returns>
        public string getCardType()
        {
            Regex r = new Regex("CARD_TYPE=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
                return m.Value.Substring(10);
            else
                return "";
        }

        /// <summary>
        /// Returns BANK_NAME from Response
        /// </summary>
        /// <returns></returns>
        public string getBank()
        {
            Regex r = new Regex("BANK_NAME=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
                return m.Value.Substring(10);
            else
                return "";
        }

        /// <summary>
        /// Returns AUTH_CODE from Response
        /// </summary>
        /// <returns></returns>
        public string getAuthCode()
        {
            Regex r = new Regex(@"AUTH_CODE=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
                return (m.Value.Substring(10));
            else
                return "";
        }
        /// <summary>
        /// Returns REBID or rebill_id from Response
        /// </summary>
        /// <returns></returns>
        public string getRebillID()
        {
            Regex r = new Regex(@"REBID=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
                return (m.Value.Substring(6));
            r = new Regex(@"rebill_id=([^&$]+)");
            m = r.Match(response);
            if (m.Success)
                return (m.Value.Substring(10));
            else
                return "";
        }

        /// <summary>
        /// Returns creation_date from Response
        /// </summary>
        /// <returns></returns>
        public string getCreationDate()
        {
            Regex r = new Regex(@"creation_date=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
                return (m.Value.Substring(14));
            else
                return "";
        }

        /// <summary>
        /// Returns next_date from Response
        /// </summary>
        /// <returns></returns>
        public string getNextDate()
        {
            Regex r = new Regex(@"next_date=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
                return (m.Value.Substring(10));
            else
                return "";
        }

        /// <summary>
        /// Returns last_date from Response
        /// </summary>
        /// <returns></returns>
        public string getLastDate()
        {
            Regex r = new Regex(@"last_date=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
                return (m.Value.Substring(9));
            else
                return "";
        }

        /// <summary>
        /// Returns sched_expr from Response
        /// </summary>
        /// <returns></returns>
        public string getSchedExpr()
        {
            Regex r = new Regex(@"sched_expr=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
                return (m.Value.Substring(11));
            else
                return "";
        }

        /// <summary>
        /// Returns cycles_remain from Response
        /// </summary>
        /// <returns></returns>
        public string getCyclesRemain()
        {
            Regex r = new Regex(@"cycles_remain=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
                return (m.Value.Substring(14));
            else
                return "";
        }

        /// <summary>
        /// Returns reb_amount from Response
        /// </summary>
        /// <returns></returns>
        public string getRebillAmount()
        {
            Regex r = new Regex(@"reb_amount=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
                return (m.Value.Substring(11));
            else
                return "";
        }

        /// <summary>
        /// Returns next_amount from Response
        /// </summary>
        /// <returns></returns>
        public string getNextAmount()
        {
            Regex r = new Regex(@"next_amount=([^&$]+)");
            Match m = r.Match(response);
            if (m.Success)
                return (m.Value.Substring(12));
            else
                return "";
        }

        public string getBatchListResponse()
        {
            return response;
        }

       
    }
}
