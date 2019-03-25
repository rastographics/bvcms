using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using UtilityExtensions;
using System.Web;

namespace CmsData.Classes.ProtectMyMinistry
{
    public class ProtectMyMinistryHelper
    {
        public static string PMM_URL => IsSecureSearchFaithEnabled()
            ? "https://orders.securesearchfaith.com/webservice/default.cfm"
            : "https://services.priorityresearch.com/webservice/default.cfm";

        public static string PMM_Append => "/ExternalServices/PMMResults";

        public const int TYPE_BACKGROUND = 1;
        public const int TYPE_CREDIT = 2;

        public static string[] STATUSES => new[] { "Error", "Not Submitted", "Submitted", "Complete" };

        public static string[] BACKGROUND_TYPES_LABELS => new[] { "National Combo (Basic)", "National Combo (Plus County)", "National Combo (Plus State)", "Motor Vehicle Record (MVR)" };
        public static string[] BACKGROUND_TYPES => new[] { "Combo", "ComboPC", "ComboPS", "MVR" };

        public static string[] CREDIT_TYPES_LABELS => new[] { "Credit History" };
        public static string[] CREDIT_TYPES => new[] { IsSecureSearchFaithEnabled() ? "SS Credit" : "Credit" };

        public static void Create(int peopleId, string serviceCode, int reportTypeId, int reportLabelId)
        {
            var bcNew = new BackgroundCheck
            {
                StatusID = 1,
                UserID = Util.UserPeopleId ?? 0,
                PeopleID = peopleId,
                ServiceCode = serviceCode, // "Combo", "MVR", "Credit"
                Created = DateTime.Now,
                Updated = DateTime.Now,
                ReportTypeID = reportTypeId,
                ReportLabelID = reportLabelId
            };
            var db = DbUtil.Db;
            db.BackgroundChecks.InsertOnSubmit(bcNew);
            db.SubmitChanges();
        }

        public static bool Submit(int requestId, string SSN, string driversLicenseNumber, string responseURL, int stateId, string username, string password, string plusCounty, string plusState)
        {
            if (username == null || password == null) return false;
            var db = DbUtil.Db;

            // Get the already created (via create()) background check request
            var backgroundCheck = db.BackgroundChecks.Single(e => e.Id == requestId);
            if (backgroundCheck == null) return false;

            // Create XML
            var xws = new XmlWriterSettings
            {
                Indent = false,
                NewLineOnAttributes = false,
                NewLineChars = ""
            };

            // Create Bundle
            var bundle = new SubmitBundle
            {
                iPeopleID = backgroundCheck.PeopleID,
                sUser = username,
                sPassword = password,
                sBillingReference = backgroundCheck.Id.ToString(),
                sSSN = SSN,
                sServiceCode = backgroundCheck.ServiceCode,
                sResponseURL = responseURL,
                bTestMode = db.Setting("PMMTestMode"),
                sPlusCounty = plusCounty,
                sPlusState = plusState
            };

            // Get State (if MVR)
            if (backgroundCheck.ServiceCode == "MVR" && stateId > 0)
            {
                var bcmc = (from e in db.BackgroundCheckMVRCodes
                            where e.Id == stateId
                            select e).Single();

                if (bcmc == null) return false;

                bundle.sDNL = driversLicenseNumber;
                bundle.sStateCode = bcmc.Code;
                bundle.sStateAbbr = bcmc.StateAbbr;
            }

            // Main Request
            string sXML;
            using (var request = new MemoryStream())
            {
                using (var xwWriter = XmlWriter.Create(request, xws))
                {
                    XmlCreate(xwWriter, bundle);
                    sXML = Encoding.UTF8.GetString(request.ToArray()).Substring(1);
                }
            }

            // Submit Request to PMM
            var fields = new NameValueCollection();
            fields.Add("REQUEST", sXML);

            string response;
            using (var wc = new WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;
                response = Encoding.UTF8.GetString(wc.UploadValues(PMM_URL, "POST", fields));
            }

            var rbResponse = ProcessResponse(response);

            if (rbResponse.bHasErrors)
            {
                backgroundCheck.StatusID = 0;
                backgroundCheck.ErrorMessages = rbResponse.sErrors;
            }
            else
            {
                if (rbResponse.bHasInstant)
                {
                    backgroundCheck.StatusID = 3;
                    backgroundCheck.ErrorMessages = "";
                    backgroundCheck.ReportID = int.Parse(rbResponse.sReportID);
                    backgroundCheck.ReportLink = rbResponse.sReportLink;
                }
                else
                {
                    backgroundCheck.StatusID = 2;
                    backgroundCheck.ErrorMessages = "";
                    backgroundCheck.ReportID = int.Parse(rbResponse.sReportID);
                }
            }

            db.SubmitChanges();
            return true;
        }

        private static void XmlCreate(XmlWriter xwWriter, SubmitBundle bundle)
        {
            // Get Person Information
            var person = DbUtil.Db.People.FirstOrDefault(p => p.PeopleId == bundle.iPeopleID);

            // Compile Birthday per requested format
            var birthMonth = person.BirthMonth ?? 0;
            var birthDay = person.BirthDay ?? 0;
            var birthYear = person.BirthYear ?? 0;
            var dateOfBirth = birthMonth.ToString("D2") + "/" + birthDay.ToString("D2") + "/" + birthYear.ToString("D4");

            // Create OrderId
            var orderId = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            // Open Document
            xwWriter.WriteStartDocument();

            // Open OrderXML
            xwWriter.WriteStartElement("OrderXML");

            // Method (Inside OrderXML)
            xwWriter.WriteElementString("Method", "SEND ORDER");

            // Authentication Section (Inside OrderXML)
            xwWriter.WriteStartElement("Authentication");
            xwWriter.WriteElementString("Username", bundle.sUser);
            xwWriter.WriteElementString("Password", bundle.sPassword);
            xwWriter.WriteFullEndElement();

            if (bundle.bTestMode) xwWriter.WriteElementString("TestMode", "YES");

            // Return URL (Inside OrderXML)
            xwWriter.WriteElementString("ReturnResultURL", bundle.sResponseURL);

            // Order Section (Inside OrderXML)
            xwWriter.WriteStartElement("Order");

            // Our Billing Reference Code (Inside Order Section)
            xwWriter.WriteElementString("BillingReferenceCode", bundle.sBillingReference);

            // Subject Section (Inside Order Section)
            xwWriter.WriteStartElement("Subject");
            xwWriter.WriteElementString("Firstname", person.FirstName);

            if (person.MiddleName != null)
            {
                xwWriter.WriteElementString("Middlename", Util.PickFirst(person.MiddleName, GetNoMiddleNameCode()));
            }

            xwWriter.WriteElementString("Lastname", person.LastName);

            if (person.SuffixCode != null) xwWriter.WriteElementString("Generation", person.SuffixCode);

            xwWriter.WriteElementString("DOB", dateOfBirth);
            xwWriter.WriteElementString("SSN", bundle.sSSN);
            xwWriter.WriteElementString("Gender", person.Gender.Description);

            if (IsSecureSearchFaithEnabled())
            {
                xwWriter.WriteElementString("Email", person.EmailAddress);
            }

            // MVR Option
            if (bundle.sServiceCode == "MVR")
            {
                xwWriter.WriteElementString("DLNumber", bundle.sDNL);
            }

            xwWriter.WriteElementString("ApplicantPosition", "Volunteer");

            // CurrentAddress Section (Inside Subject Section)
            xwWriter.WriteStartElement("CurrentAddress");
            xwWriter.WriteElementString("StreetAddress", person.PrimaryAddress);
            xwWriter.WriteElementString("City", person.PrimaryCity);
            xwWriter.WriteElementString("State", person.PrimaryState);
            xwWriter.WriteElementString("Zipcode", person.PrimaryZip);
            xwWriter.WriteFullEndElement();

            // Close Subject Section
            xwWriter.WriteFullEndElement();

            if (bundle.sServiceCode == "Combo")
            {
                // Package Service Code - Only if a package (BASIC,PLUS) (Inside Order Section)
                xwWriter.WriteStartElement("PackageServiceCode");
                xwWriter.WriteAttributeString("OrderId", orderId);
                xwWriter.WriteString(GetBasicPackageName());
                xwWriter.WriteFullEndElement();
            }
            else if (bundle.sServiceCode == "ComboPC" || bundle.sServiceCode == "ComboPS")
            {
                // Package Service Code - Only if a package (BASIC,PLUS) (Inside Order Section)
                xwWriter.WriteStartElement("PackageServiceCode");
                xwWriter.WriteAttributeString("OrderId", orderId);
                xwWriter.WriteString(GetPlusPackageName());
                xwWriter.WriteFullEndElement();
            }

            if (bundle.sServiceCode == "ComboPC" || bundle.sServiceCode == "ComboPS")
            {
                // Basic Package
                xwWriter.WriteStartElement("OrderDetail");
                xwWriter.WriteAttributeString("serviceCode", "Combo");
                xwWriter.WriteAttributeString("OrderId", orderId);
                xwWriter.WriteEndElement();

                // Plus Package
                xwWriter.WriteStartElement("OrderDetail");

                if (bundle.sServiceCode == "ComboPC") xwWriter.WriteAttributeString("serviceCode", "CountyCrim");
                if (bundle.sServiceCode == "ComboPS") xwWriter.WriteAttributeString("serviceCode", "StateCriminal");

                xwWriter.WriteAttributeString("OrderId", orderId);

                if (bundle.sServiceCode == "ComboPC") xwWriter.WriteElementString("County", bundle.sPlusCounty);
                xwWriter.WriteElementString("State", bundle.sPlusState);
            }
            else
            {
                // Basic Package
                xwWriter.WriteStartElement("OrderDetail");
                xwWriter.WriteAttributeString("serviceCode", bundle.sServiceCode);
                xwWriter.WriteAttributeString("OrderId", orderId);
            }

            // MVR Option
            if (bundle.sServiceCode == "MVR")
            {
                xwWriter.WriteElementString("JurisdictionCode", bundle.sStateCode);
                xwWriter.WriteElementString("State", bundle.sStateAbbr);
            }

            xwWriter.WriteEndElement();

            // Close Order Section
            xwWriter.WriteFullEndElement();

            // Close OrderXML Section
            xwWriter.WriteFullEndElement();

            // Close Document
            xwWriter.WriteEndDocument();

            xwWriter.Flush();
        }

        private static string GetNoMiddleNameCode()
        {
            if (IsSecureSearchFaithEnabled())
            {
                return "NMN";
            }
            return "";
        }

        private static string GetPlusPackageName()
        {
            if (IsSecureSearchFaithEnabled())
            {
                return "Confidence";
            }
            return "PLUS";
        }

        private static string GetBasicPackageName()
        {
            if (IsSecureSearchFaithEnabled())
            {
                return "Protection";
            }
            return "Basic";
        }

        public static bool IsSecureSearchFaithEnabled()
        {
            return DbUtil.Db.Setting("EnableSecureSearchFaith");
        }

        private static ResponseBundle ProcessResponse(string response)
        {
            var bundle = new ResponseBundle();

            var xmldoc = XDocument.Parse(response, LoadOptions.None);

            if (xmldoc.Root.Element("Status").Value == "FAILED")
            {
                bundle.bHasErrors = true;

                var errors = xmldoc.Root.Element("Errors").Elements("Message");

                foreach (var error in errors)
                {
                    bundle.sErrors += HttpUtility.HtmlEncode(error.Value) + "<br>";
                }
            }
            else if (xmldoc.Root.Element("Status").Value == "ERROR")
            {
                bundle.bHasErrors = true;

                var errors = xmldoc.Root.Elements("Message");

                foreach (var error in errors)
                {
                    bundle.sErrors += HttpUtility.HtmlEncode(error.Value) + "<br>";
                }
            }
            else
            {
                if (xmldoc.Root.Element("Order").Element("ReportLink") != null)
                {
                    bundle.bHasInstant = true;
                    bundle.sReportLink = xmldoc.Root.Element("Order").Element("ReportLink").Value;
                }

                string reportId = xmldoc.Root.Element("ReferenceNumber").Value;
                if (reportId != null)
                {
                    bundle.sReportID = reportId;
                }
            }

            return bundle;
        }

        public static string getStatus(int iStatusID)
        {
            return STATUSES[iStatusID];
        }

        public static string getDescription(string sServiceCode)
        {
            for (var iX = 0; iX < BACKGROUND_TYPES.Length; iX++)
            {
                if (BACKGROUND_TYPES[iX] == sServiceCode) return BACKGROUND_TYPES_LABELS[iX];
            }

            for (int iX = 0; iX < CREDIT_TYPES.Length; iX++)
            {
                if (CREDIT_TYPES[iX] == sServiceCode) return CREDIT_TYPES_LABELS[iX];
            }

            return "";
        }

        public static List<CheckType> GetCheckTypes(int category)
        {
            var types = new List<CheckType>();

            if (category == TYPE_BACKGROUND)
            {
                for (var iX = 0; iX < BACKGROUND_TYPES.Length; iX++)
                {
                    var item = new CheckType { code = BACKGROUND_TYPES[iX], label = BACKGROUND_TYPES_LABELS[iX] };
                    types.Add(item);
                }
            }

            if (category == TYPE_CREDIT)
            {
                for (var iX = 0; iX < CREDIT_TYPES.Length; iX++)
                {
                    var item = new CheckType { code = CREDIT_TYPES[iX], label = CREDIT_TYPES_LABELS[iX] };
                    types.Add(item);
                }
            }

            return types;
        }
    }

    internal class SubmitBundle
    {
        // Internal
        public bool bTestMode = false;

        public string sUser = "";
        public string sPassword = "";
        public string sServiceCode = "";
        public string sBillingReference = "";
        public string sResponseURL = "";

        // Person
        public int iPeopleID = 0;
        public string sSSN = "";
        public string sDNL = "";
        public string sStateCode = "";
        public string sStateAbbr = "";

        // Plus
        public string sPlusCounty = "";
        public string sPlusState = "";
    }

    internal class ResponseBundle
    {
        public string sReportID = "0";

        public bool bHasErrors = false;
        public string sErrors = "";

        public bool bHasInstant = false;
        public string sReportLink = "";
    }

    public class CheckType
    {
        public string code { get; set; }
        public string label { get; set; }
    }
}
