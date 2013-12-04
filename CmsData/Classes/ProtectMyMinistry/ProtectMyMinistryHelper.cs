using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsData.Classes.ProtectMyMinistry
{
    public class ProtectMyMinistryHelper
    {
        public const string PMM_URL = "https://services.priorityresearch.com/webservice/default.cfm";
        public const string PMM_Append = "/ExternalServices/PMMResults";

        public const int TYPE_BACKGROUND = 1;
        public const int TYPE_CREDIT = 2;

        public static readonly string[] STATUSES = { "Error", "Not Submitted", "Submitted", "Complete" };

        public static readonly string[] BACKGROUND_TYPES_LABELS = { "National Combo (Basic)", "National Combo (Plus County)", "National Combo (Plus State)", "Motor Vehicle Record (MVR)" };
        public static readonly string[] BACKGROUND_TYPES = { "Combo", "ComboPC", "ComboPS", "MVR" };

        public static readonly string[] CREDIT_TYPES_LABELS = { "Credit History" };
        public static readonly string[] CREDIT_TYPES = { "Credit" };

        public static void create( int iPeopleID, string sServiceCode, int iType, int iLabel )
        {
            BackgroundCheck bcNew = new BackgroundCheck();

            bcNew.StatusID = 1;
            bcNew.UserID = Util.UserPeopleId ?? 0;
            bcNew.PeopleID = iPeopleID;
            bcNew.ServiceCode = sServiceCode; // "Combo", "MVR"
            bcNew.Created = DateTime.Now;
            bcNew.Updated = DateTime.Now;
            bcNew.ReportTypeID = iType;
            bcNew.ReportLabelID = iLabel;

            DbUtil.Db.BackgroundChecks.InsertOnSubmit(bcNew);
            DbUtil.Db.SubmitChanges();
        }

        public static bool submit( int iRequestID, string sSSN, string sDLN, string sResponseURL, int iStateID, string sUser, string sPassword, string sPlusCounty, string sPlusState )
        {
            if (sUser == null || sPassword == null) return false;

            // Get the already created (via create()) background check request
            BackgroundCheck bc = (from e in DbUtil.Db.BackgroundChecks
                                  where e.Id == iRequestID
                                  select e).Single();
            if (bc == null) return false;

            // Create XML
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = false;
            xws.NewLineOnAttributes = false;
            xws.NewLineChars = "";

            // Create Bundle
            SubmitBundle sb = new SubmitBundle();
            sb.iPeopleID = bc.PeopleID;
            sb.sUser = sUser;
            sb.sPassword = sPassword;
            sb.sBillingReference = bc.Id.ToString();
            sb.sSSN = sSSN;
            sb.sServiceCode = bc.ServiceCode;
            sb.sResponseURL = sResponseURL;
            sb.bTestMode = (DbUtil.Db.Setting("PMMTestMode", "false") == "true");
            sb.sPlusCounty = sPlusCounty;
            sb.sPlusState = sPlusState;

            // Get State (if MVR)
            if (bc.ServiceCode == "MVR" && iStateID > 0)
            {
                BackgroundCheckMVRCode bcmc = (from e in DbUtil.Db.BackgroundCheckMVRCodes
                                               where e.Id == iStateID
                                               select e).Single();
                
                if (bcmc == null) return false;

                sb.sDNL = sDLN;
                sb.sStateCode = bcmc.Code;
                sb.sStateAbbr = bcmc.StateAbbr;
            }

            // Main Request
            MemoryStream msRequest = new MemoryStream();
            XmlWriter xwWriter = XmlWriter.Create(msRequest, xws);
            xmlCreate(xwWriter, sb);
            string sXML = Encoding.UTF8.GetString(msRequest.ToArray()).Substring(1);
            msRequest.Close();

            // Submit Request to PMM
            var fields = new NameValueCollection();
            fields.Add("REQUEST", sXML);

            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            var response = Encoding.UTF8.GetString(wc.UploadValues(PMM_URL, "POST", fields));

            ResponseBundle rbResponse = processResponse(response);

            if (rbResponse.bHasErrors)
            {
                bc.StatusID = 0;
                bc.ErrorMessages = rbResponse.sErrors;
            }
            else
            {
                bc.StatusID = 2;
                bc.ErrorMessages = "";
                bc.ReportID = Int32.Parse( rbResponse.sReportID );
            }

            DbUtil.Db.SubmitChanges();
            return true;
        }

        public static bool xmlCreate( XmlWriter xwWriter, SubmitBundle sb )
        {
            // Get Person Information
            var pPerson = (from e in DbUtil.Db.People
                          where e.PeopleId == sb.iPeopleID
                          select e).FirstOrDefault();

            // Compile Birthday per requested format
            int iBirthMonth = pPerson.BirthMonth ?? 0;
            int iBirthDay = pPerson.BirthDay ?? 0;
            int iBirthYear = pPerson.BirthYear ?? 0;
            String sDOB = iBirthMonth.ToString("D2") + "/" + iBirthDay.ToString("D2") + "/" + iBirthYear.ToString("D4");

            // Create OrderId
            String sOrderID = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            // Open Document
            xwWriter.WriteStartDocument();

            // Open OrderXML
            xwWriter.WriteStartElement("OrderXML");

            // Method (Inside OrderXML)
            xwWriter.WriteElementString("Method", "SEND ORDER");

            // Authentication Section (Inside OrderXML)
            xwWriter.WriteStartElement("Authentication");
            xwWriter.WriteElementString("Username", sb.sUser);
            xwWriter.WriteElementString("Password", sb.sPassword);
            xwWriter.WriteFullEndElement();

            if( sb.bTestMode ) xwWriter.WriteElementString("TestMode", "YES");

            // Return URL (Inside OrderXML)
            xwWriter.WriteElementString("ReturnResultURL", sb.sResponseURL);

            // Order Section (Inside OrderXML)
            xwWriter.WriteStartElement("Order");

            // Our Billing Reference Code (Inside Order Section)
            xwWriter.WriteElementString("BillingReferenceCode", sb.sBillingReference);

            // Subject Section (Inside Order Section)
            xwWriter.WriteStartElement("Subject");
            xwWriter.WriteElementString("Firstname", pPerson.FirstName);

            if (pPerson.MiddleName != null) xwWriter.WriteElementString("Middlename", pPerson.MiddleName);

            xwWriter.WriteElementString("Lastname", pPerson.LastName);

            if (pPerson.SuffixCode != null) xwWriter.WriteElementString("Generation", pPerson.SuffixCode);

            xwWriter.WriteElementString("DOB", sDOB);
            xwWriter.WriteElementString("SSN", sb.sSSN);
            xwWriter.WriteElementString("Gender", pPerson.Gender.Description);
            //xwWriter.WriteElementString("Ethnicity", "Caucasian");

            // MVR Option
            if (sb.sServiceCode == "MVR")
            {
                xwWriter.WriteElementString("DLNumber", sb.sDNL);
            }

            xwWriter.WriteElementString("ApplicantPosition", "Volunteer");

            // CurrentAddress Section (Inside Subject Section)
            xwWriter.WriteStartElement("CurrentAddress");
            xwWriter.WriteElementString("StreetAddress", pPerson.PrimaryAddress);
            xwWriter.WriteElementString("City", pPerson.PrimaryCity);
            xwWriter.WriteElementString("State", pPerson.PrimaryState);
            xwWriter.WriteElementString("Zipcode", pPerson.PrimaryZip);
            xwWriter.WriteFullEndElement();

            // Close Subject Section
            xwWriter.WriteFullEndElement();

            if (sb.sServiceCode == "Combo")
            {
                // Package Service Code - Only if a package (BASIC,PLUS) (Inside Order Section)
                xwWriter.WriteStartElement("PackageServiceCode");
                xwWriter.WriteAttributeString("OrderId", sOrderID);
                xwWriter.WriteString("Basic");
                xwWriter.WriteFullEndElement();
            }
            else if (sb.sServiceCode == "ComboPC" || sb.sServiceCode == "ComboPS")
            {
                // Package Service Code - Only if a package (BASIC,PLUS) (Inside Order Section)
                xwWriter.WriteStartElement("PackageServiceCode");
                xwWriter.WriteAttributeString("OrderId", sOrderID);
                xwWriter.WriteString("PLUS");
                xwWriter.WriteFullEndElement();
            }

            if (sb.sServiceCode == "ComboPC" || sb.sServiceCode == "ComboPS")
            {
                // Basic Package
                xwWriter.WriteStartElement("OrderDetail");
                xwWriter.WriteAttributeString("serviceCode", "Combo");
                xwWriter.WriteAttributeString("OrderId", sOrderID);
                xwWriter.WriteEndElement();

                // Plus Package
                xwWriter.WriteStartElement("OrderDetail");

                if (sb.sServiceCode == "ComboPC") xwWriter.WriteAttributeString("serviceCode", "CountyCrim");
                if (sb.sServiceCode == "ComboPS") xwWriter.WriteAttributeString("serviceCode", "StateCriminal");

                xwWriter.WriteAttributeString("OrderId", sOrderID);

                if (sb.sServiceCode == "ComboPC") xwWriter.WriteElementString("County", sb.sPlusCounty);
                xwWriter.WriteElementString("State", sb.sPlusState);
            }
            else
            {
                // Basic Package
                xwWriter.WriteStartElement("OrderDetail");
                xwWriter.WriteAttributeString("serviceCode", sb.sServiceCode);
                xwWriter.WriteAttributeString("OrderId", sOrderID);
            }

            // MVR Option
            if (sb.sServiceCode == "MVR")
            {
                xwWriter.WriteElementString("JurisdictionCode", sb.sStateCode);
                xwWriter.WriteElementString("State", sb.sStateAbbr);
            }

            xwWriter.WriteEndElement();

            // Close Order Section
            xwWriter.WriteFullEndElement();

            // Close OrderXML Section
            xwWriter.WriteFullEndElement();

            // Close Document
            xwWriter.WriteEndDocument();

            xwWriter.Flush();
            return true;
        }

        public static ResponseBundle processResponse(string sResponse)
        {
            ResponseBundle sReturn = new ResponseBundle();

            XDocument xd = XDocument.Parse(sResponse, LoadOptions.None);

            if (xd.Root.Element("Status").Value == "FAILED")
            {
                sReturn.bHasErrors = true;

                var errors = xd.Root.Element("Errors").Elements("Message");

                foreach (var item in errors)
                {
                    sReturn.sErrors += item.Value.Replace("<", "&lt;").Replace(">", "&gt;") + "<br>";
                }
            }
            else if (xd.Root.Element("Status").Value == "ERROR")
            {
                sReturn.bHasErrors = true;

                var errors = xd.Root.Elements("Message");

                foreach (var item in errors)
                {
                    sReturn.sErrors += item.Value.Replace("<", "&lt;").Replace(">", "&gt;") + "<br>";
                }
            }
            else
            {
                string sReportID = xd.Root.Element("ReferenceNumber").Value;

                if (sReportID != null) sReturn.sReportID = sReportID;
            }

            return sReturn;
        }

        public static string getStatus(int iStatusID)
        {
            return STATUSES[iStatusID];
        }

        public static string getDescription(string sServiceCode)
        {
            for (int iX = 0; iX < BACKGROUND_TYPES.Length; iX++)
            {
                if (BACKGROUND_TYPES[iX] == sServiceCode) return BACKGROUND_TYPES_LABELS[iX];
            }

            for (int iX = 0; iX < CREDIT_TYPES.Length; iX++)
            {
                if (CREDIT_TYPES[iX] == sServiceCode) return CREDIT_TYPES_LABELS[iX];
            }

            return "";
        }

        public static List<CheckType> getCheckTypes(int category)
        {
            var types = new List<CheckType>();

            if( category == TYPE_BACKGROUND )
            {
                for (int iX = 0; iX < BACKGROUND_TYPES.Length; iX++)
                {
                    var item = new CheckType { code = BACKGROUND_TYPES[iX], label = BACKGROUND_TYPES_LABELS[iX] };
                    types.Add(item);
                }
            }

            if( category == TYPE_CREDIT )
            {
                for (int iX = 0; iX < CREDIT_TYPES.Length; iX++)
                {
                    var item = new CheckType { code = CREDIT_TYPES[iX], label = CREDIT_TYPES_LABELS[iX] };
                    types.Add(item);
                }
            }

            return types;
        }
    }

    public class SubmitBundle
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

    public class ResponseBundle
    {
        public bool bHasErrors = false;
        public string sErrors = "";
        public string sReportID = "0";        
    }

    public class CheckType
    {
        public string code { get; set; }
        public string label { get; set; }
    }
}
