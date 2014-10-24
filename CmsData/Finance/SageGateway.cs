using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using CmsData.Finance.Sage.Core;
using CmsData.Finance.Sage.Transaction.Refund;
using CmsData.Finance.Sage.Transaction.Sale;
using CmsData.Finance.Sage.Transaction.Void;
using CmsData.Finance.Sage.Vault;
using UtilityExtensions;

namespace CmsData.Finance
{
	internal class SageGateway : IGateway
	{
	    private readonly string _id;
	    private readonly string _key;
	    private readonly string _originatorId;
	    private readonly CMSDataContext db;

        public string GatewayType { get { return "Sage"; } }

		public SageGateway(CMSDataContext db, bool testing)
		{
			this.db = db;
		    var gatewayTesting = db.Setting("GatewayTesting", "false").ToLower() == "true";
			if (testing || gatewayTesting)
			{
                _id = "856423594649";
                _key = "M5Q4C9P2T4N5";
                _originatorId = "1111111111";
			}
			else
			{
				_id = db.Setting("M_id", "");
				_key = db.Setting("M_key", "");
                _originatorId = db.Setting("SageOriginatorId", "");
			}
		}
		
		public void StoreInVault(int peopleId, string type, string cardNumber, string expires, string cardCode, string routing, string account, bool giving)
		{
			var person = db.LoadPersonById(peopleId);
			var paymentInfo = person.PaymentInfo();
			if (paymentInfo == null)
			{
				paymentInfo = new PaymentInfo();
				person.PaymentInfos.Add(paymentInfo);
			}

            if (type == "C") // credit card
            {
                if (paymentInfo.SageCardGuid == null) // create new vault.
                    paymentInfo.SageCardGuid = CreateCreditCardVault(person, cardNumber, expires);
                else
                {
                    // update existing vault.
                    // check for updating the entire card or only expiration.
                    if (!cardNumber.StartsWith("X"))
                        UpdateCreditCardVault(paymentInfo.SageCardGuid.GetValueOrDefault(), person, cardNumber, expires);
                    else
                        UpdateCreditCardVault(paymentInfo.SageCardGuid.GetValueOrDefault(), person, expires);
                }
            }
            else // bank account
            {
                if (paymentInfo.SageBankGuid == null) // create new vault
                    paymentInfo.SageBankGuid = CreateAchVault(person, account, routing);
                else
                {
                    // we can only update the ach account if there is a full account number.
                    if (!account.StartsWith("X"))
                        UpdateAchVault(paymentInfo.SageBankGuid.GetValueOrDefault(), person, account, routing);
                }
            }

            paymentInfo.MaskedAccount = Util.MaskAccount(account);
			paymentInfo.Routing = Util.Mask(new StringBuilder(routing), 2);
			paymentInfo.MaskedCard = Util.MaskCC(cardNumber);
			paymentInfo.Ccv = cardCode;
			paymentInfo.Expires = expires;
			if (giving)
				paymentInfo.PreferredGivingType = type;
			else
				paymentInfo.PreferredPaymentType = type;
			db.SubmitChanges();
		}
        
        private Guid CreateCreditCardVault(Person person, string cardNumber, string expiration)
        {
            var createCreditCardVaultRequest = new CreateCreditCardVaultRequest(_id, _key, expiration, cardNumber);

            var response = createCreditCardVaultRequest.Execute();
            if (!response.Success)
                throw new Exception(
                    "Sage failed to create the credit card for people id: {0}".Fmt(person.PeopleId));

            return response.VaultGuid;
        }

        private void UpdateCreditCardVault(Guid vaultGuid, Person person, string cardNumber, string expiration)
        {
            var updateCreditCardVaultRequest = new UpdateCreditCardVaultRequest(_id,
                                                                                _key,
                                                                                vaultGuid,
                                                                                expiration,
                                                                                cardNumber);

            var response = updateCreditCardVaultRequest.Execute();
            if (!response.Success)
                throw new Exception(
                    "Sage failed to update the credit card for people id: {0}".Fmt(person.PeopleId));
        }

        private void UpdateCreditCardVault(Guid vaultGuid, Person person, string expiration)
        {
            var updateCreditCardVaultRequest = new UpdateCreditCardVaultRequest(_id, _key, vaultGuid, expiration);

            var response = updateCreditCardVaultRequest.Execute();
            if (!response.Success)
                throw new Exception(
                    "Sage failed to update the credit card expiration date for people id: {0}".Fmt(
                        person.PeopleId));
        }

        private Guid CreateAchVault(Person person, string accountNumber, string routingNumber)
        {
            var createAchVaultRequest = new CreateAchVaultRequest(_id, _key, accountNumber, routingNumber);

            var response = createAchVaultRequest.Execute();
            if (!response.Success)
                throw new Exception(
                    "Sage failed to create the ach account for people id: {0}".Fmt(person.PeopleId));

            return response.VaultGuid;
        }

        private void UpdateAchVault(Guid vaultGuid, Person person, string accountNumber, string routingNumber)
        {
            var updateAchVaultRequest = new UpdateAchVaultRequest(_id, _key, vaultGuid, accountNumber, routingNumber);

            var response = updateAchVaultRequest.Execute();
            if (!response.Success)
                throw new Exception(
                    "Sage failed to update the ach account for people id: {0}".Fmt(person.PeopleId));
        }
        
		public void RemoveFromVault(int peopleId)
		{
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
                return;

            if (paymentInfo.SageCardGuid.HasValue)
                DeleteVault(paymentInfo.SageCardGuid.GetValueOrDefault(), person);

            if (paymentInfo.SageBankGuid.HasValue)
                DeleteVault(paymentInfo.SageBankGuid.GetValueOrDefault(), person);

            // clear out local record and save changes.
            paymentInfo.SageCardGuid = null;
            paymentInfo.SageBankGuid = null;
            paymentInfo.MaskedCard = null;
            paymentInfo.MaskedAccount = null;
            paymentInfo.Ccv = null;
            db.SubmitChanges();
		}

        private void DeleteVault(Guid vaultGuid, Person person)
        {
            var deleteVaultRequest = new DeleteVaultRequest(_id, _key, vaultGuid);

            var success = deleteVaultRequest.Execute();
            if (!success)
                throw new Exception("Sage failed to delete the vault for people id: {0}".Fmt(person.PeopleId));
        }

		public TransactionResponse VoidCreditCardTransaction(string reference)
		{
            var voidRequest = new CreditCardVoidRequest(_id, _key, reference);
            var response = voidRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
		}

		public TransactionResponse VoidCheckTransaction(string reference)
		{
            var voidRequest = new AchVoidRequest(_id, _key, reference);
            var response = voidRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
		}

		public TransactionResponse RefundCreditCard(string reference, Decimal amt)
		{
            var refundRequest = new CreditCardRefundRequest(_id, _key, reference, amt);
            var response = refundRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
		}

		public TransactionResponse RefundCheck(string reference, Decimal amt)
		{
            var refundRequest = new AchRefundRequest(_id, _key, reference, amt);
            var response = refundRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
		}

		public TransactionResponse PayWithCreditCard(int peopleId, decimal amt, string cardnumber, string expires, string description, int tranid, string cardcode, string email, string first, string last, string addr, string city, string state, string zip, string phone)
		{
		    var creditCardSaleRequest = new CreditCardSaleRequest(
                _id,
		        _key,
		        new CreditCard
		        {
		            NameOnCard = "{0} {1}".Fmt(first, last),
		            CardNumber = cardnumber,
		            Expiration = expires,
		            CardCode = cardcode,
		            BillingAddress = new BillingAddress
		            {
		                Address1 = addr,
		                City = city,
		                State = state,
		                Zip = zip,
		                Email = email,
		                Phone = phone
		            }
		        },
		        amt,
		        tranid.ToString(CultureInfo.InvariantCulture),
		        peopleId.ToString(CultureInfo.InvariantCulture));

            var response = creditCardSaleRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
		}

		public TransactionResponse PayWithCheck(int peopleId, decimal amt, string routing, string acct, string description, int tranid, string email, string first, string middle, string last, string suffix, string addr, string city, string state, string zip, string phone)
		{
		    var achSaleRequest = new AchSaleRequest(_id,
		        _key,
		        _originatorId,
		        new Ach
		        {
		            FirstName = first,
		            MiddleInitial = middle.Truncate(1) ?? "",
		            LastName = last,
		            Suffix = suffix,
		            AccountNumber = acct,
		            RoutingNumber = routing,
		            BillingAddress = new BillingAddress
		            {
		                Address1 = addr,
		                City = city,
		                State = state,
		                Zip = zip,
		                Email = email,
		                Phone = phone
		            }

		        },
		        amt,
		        tranid.ToString(CultureInfo.InvariantCulture));

            var response = achSaleRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
		}

        public TransactionResponse PayWithVault(int peopleId, decimal amt, string description, int tranid, string type)
		{
            var person = db.LoadPersonById(peopleId);
            var paymentInfo = person.PaymentInfo();
            if (paymentInfo == null)
                return new TransactionResponse
                {
                    Approved = false,
                    Message = "missing payment info",
                };

            if (type == "C") // credit card
                return ChargeCreditCardVault(paymentInfo.SageCardGuid.GetValueOrDefault(), person, paymentInfo, amt, tranid);
            else // bank account
                return ChargeAchVault(paymentInfo.SageBankGuid.GetValueOrDefault(), person, paymentInfo, amt, tranid);

		}

        private TransactionResponse ChargeCreditCardVault(Guid vaultGuid, Person person, PaymentInfo paymentInfo, decimal amount, int tranid)
        {
            var creditCardVaultSaleRequest = new CreditCardVaultSaleRequest(_id,
                _key,
                vaultGuid,
                "{0} {1}".Fmt(paymentInfo.FirstName ?? person.FirstName, paymentInfo.LastName ?? person.LastName),
                new BillingAddress
                {
                    Address1 = paymentInfo.Address ?? person.PrimaryAddress,
                    City = paymentInfo.City ?? person.PrimaryCity,
                    State = paymentInfo.State ?? person.PrimaryState,
                    Zip = paymentInfo.Zip ?? person.PrimaryZip,
                    Email = person.EmailAddress,
                    Phone = paymentInfo.Phone ?? person.HomePhone
                },
                amount, tranid.ToString(CultureInfo.InvariantCulture),
                person.PeopleId.ToString(CultureInfo.InvariantCulture));

            var response = creditCardVaultSaleRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
        }

        private TransactionResponse ChargeAchVault(Guid vaultGuid, Person person, PaymentInfo paymentInfo, decimal amount, int tranid)
        {
            var achVaultSaleRequest = new AchVaultSaleRequest(_id,
                _key,
                _originatorId,
                vaultGuid,
                paymentInfo.FirstName ?? person.FirstName,
                (paymentInfo.MiddleInitial ?? person.MiddleName).Truncate(1) ?? "",
                paymentInfo.LastName ?? person.LastName,
                paymentInfo.Suffix ?? person.SuffixCode,
                new BillingAddress
                {
                    Address1 = paymentInfo.Address ?? person.PrimaryAddress,
                    City = paymentInfo.City ?? person.PrimaryCity,
                    State = paymentInfo.State ?? person.PrimaryState,
                    Zip = paymentInfo.Zip ?? person.PrimaryZip,
                    Email = person.EmailAddress,
                    Phone = paymentInfo.Phone ?? person.HomePhone
                },
                amount, tranid.ToString(CultureInfo.InvariantCulture));

            var response = achVaultSaleRequest.Execute();

            return new TransactionResponse
            {
                Approved = response.ApprovalIndicator == ApprovalIndicator.Approved,
                AuthCode = response.Code,
                Message = response.Message,
                TransactionId = response.Reference
            };
        }

		public BatchResponse SettledBatchSummary(DateTime start, DateTime end)
		{
			var wc = new WebClient();
			wc.BaseAddress = "https://www.sagepayments.net/web_services/vterm_extensions/reporting.asmx/";
			var coll = new NameValueCollection();
			coll["M_ID"] = _id;
			coll["M_KEY"] = _key;
			coll["START_DATE"] = start.ToShortDateString();
			coll["END_DATE"] = end.ToShortDateString();
			coll["INCLUDE_BANKCARD"] = "true";
			coll["INCLUDE_VIRTUAL_CHECK"] = "true";

			var b = wc.UploadValues("VIEW_SETTLED_BATCH_SUMMARY", "POST", coll);
			var ret = Encoding.ASCII.GetString(b);
			var ds = new DataSet();
			ds.ReadXml(new StringReader(ret));

            // TODO: IMPLEMENT BRIAN PLEASE?
            return new BatchResponse(new Batch[] {});
		}

		public DataSet SettledBatchListing(string batchref, string type)
		{
			var wc = new WebClient();
			wc.BaseAddress = "https://www.sagepayments.net/web_services/vterm_extensions/reporting.asmx/";
			var coll = new NameValueCollection();
			coll["M_ID"] = _id;
			coll["M_KEY"] = _key;
			coll["BATCH_REFERENCE"] = batchref;

			string method = null;
			switch (type)
			{
				case "eft":
					method = "VIEW_VIRTUAL_CHECK_SETTLED_BATCH_LISTING";
					break;
				case "bankcard":
					method = "VIEW_BANKCARD_SETTLED_BATCH_LISTING";
					break;
			}
			var b = wc.UploadValues(method, "POST", coll);
			var ret = Encoding.ASCII.GetString(b);
			var ds = new DataSet();
			ds.ReadXml(new StringReader(ret));
			return ds;
		}

		public DataSet VirtualCheckRejects(DateTime startdt, DateTime enddt)
		{
			var wc = new WebClient();
			wc.BaseAddress = "https://www.sagepayments.net/web_services/vterm_extensions/reporting.asmx/";
			var coll = new NameValueCollection();
			coll["M_ID"] = _id;
			coll["M_KEY"] = _key;
		    coll["START_DATE"] = "";
		    coll["END_DATE"] = "";

			const string method = "VIEW_VIRTUAL_CHECK_REJECTS_BY_DATE";

			var b = wc.UploadValues(method, "POST", coll);
			var ret = Encoding.ASCII.GetString(b);
			var ds = new DataSet();
			ds.ReadXml(new StringReader(ret));
			return ds;
		}

		public DataSet VirtualCheckRejects(DateTime rejectdate)
		{
			var wc = new WebClient {BaseAddress = "https://www.sagepayments.net/web_services/vterm_extensions/reporting.asmx/"};
		    var coll = new NameValueCollection();
			coll["M_ID"] = _id;
			coll["M_KEY"] = _key;
		    coll["REJECT_DATE"] = rejectdate.ToShortDateString();

			const string method = "VIEW_VIRTUAL_CHECK_REJECTS";

			var b = wc.UploadValues(method, "POST", coll);
			var ret = Encoding.ASCII.GetString(b);
			var ds = new DataSet();
			ds.ReadXml(new StringReader(ret));
			return ds;
		}

        public bool CanVoidRefund
        {
            get { return true; }
        }

        public bool CanGetSettlementDates
        {
            get { return true; }
        }

        public bool CanGetBounces
        {
            get { return true; }
        }
	}
}