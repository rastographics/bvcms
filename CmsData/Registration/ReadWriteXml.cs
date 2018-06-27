using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
    [Serializable]
    public partial class Settings : IXmlSerializable
    {
        public override string ToString()
        {
            return Util.Serialize(this);
        }

        public static Settings CreateSettings(string s, CMSDataContext db, Organization org)
        {
            var settings = !s.HasValue()
                ? new Settings()
                : Util.DeSerialize<Settings>(s);
            settings.Db = db;
            settings.OrgId = org.OrganizationId;
            settings.org = org;
            return settings;
        }
        public static Settings CreateSettings(string s, CMSDataContext db, int orgId)
        {
            var org = db.LoadOrganizationById(orgId);
            return CreateSettings(s, db, org);
        }

        public void ReadXml(XmlReader reader)
        {
            var xd = XDocument.Load(reader);
            if (xd.Root == null) return;

            foreach (var e in xd.Root.Elements())
            {
                var name = e.Name.ToString();
                switch (name)
                {
                    case "Confirmation":
                        Subject = e.Element("Subject")?.Value;
                        Body = e.Element("Body")?.Value;
                        break;
                    case "Reminder":
                        ReminderSubject = e.Element("Subject")?.Value;
                        ReminderBody = e.Element("Body")?.Value;
                        break;
                    case "SupportEmail":
                        SupportSubject = e.Element("Subject")?.Value;
                        SupportBody = e.Element("Body")?.Value;
                        break;
                    case "SenderEmail":
                        SenderSubject = e.Element("Subject")?.Value;
                        SenderBody = e.Element("Body")?.Value;
                        break;
                    case "Instructions":
                        InstructionLogin = e.Element("Login")?.Value;
                        InstructionSelect = e.Element("Select")?.Value;
                        InstructionFind = e.Element("Find")?.Value;
                        InstructionOptions = e.Element("Options")?.Value;
                        InstructionSpecial = e.Element("Special")?.Value;
                        InstructionSubmit = e.Element("Submit")?.Value;
                        InstructionSorry = e.Element("Sorry")?.Value;
                        ThankYouMessage = e.Element("Thanks")?.Value;
                        Terms = e.Element("Terms")?.Value;
                        break;
                    case "Fees":
                        Fee = e.Element("Fee").ToDecimal();
                        Deposit = e.Element("Deposit").ToDecimal();
                        ExtraFee = e.Element("ExtraFee").ToDecimal();
                        MaximumFee = e.Element("MaximumFee").ToDecimal();
                        ApplyMaxToOtherFees = e.Element("ApplyMaxToOtherFees").ToBool();
                        ExtraValueFeeName = e.Element("ExtraValueFeeName")?.Value;
                        AccountingCode = e.Element("AccountingCode")?.Value;
                        IncludeOtherFeesWithDeposit = e.Element("IncludeOtherFeesWithDeposit").ToBool();
                        OtherFeesAddedToOrgFee = e.Element("OtherFeesAddedToOrgFee").ToBool();
                        AskDonation = e.Element("AskDonation").ToBool();
                        DonationLabel = e.Element("DonationLabel")?.Value;
                        DonationFundId = e.Element("DonationFundId").ToInt2();
                        break;
                    case "AgeGroups":
                        if (AgeGroups == null)
                            AgeGroups = new List<AgeGroup>();
                        foreach (var ee in e.Elements("Group"))
                            AgeGroups.Add(new AgeGroup
                            {
                                StartAge = ee.Attribute("StartAge").ToInt2() ?? 0,
                                EndAge = ee.Attribute("EndAge").ToInt2() ?? 0,
                                Fee = ee.Attribute("Fee").ToDecimal(),
                                SmallGroup = ee.Value
                            });
                        break;
                    case "OrgFees":
                        if (OrgFees == null)
                            OrgFees = new List<OrgFee>();
                        foreach (var ee in e.Elements("Fee"))
                            OrgFees.Add(new OrgFee
                            {
                                OrgId = ee.Attribute("OrgId").ToInt2(),
                                Fee = ee.Attribute("Fee").ToDecimal()
                            });
                        break;
                    case "Options":
                        ConfirmationTrackingCode = e.Element("ConfirmationTrackingCode")?.Value;
                        ValidateOrgs = e.Element("ValidateOrgs")?.Value;
                        Shell = e.Element("Shell")?.Value;
                        ShellBs = e.Element("ShellBs")?.Value;
                        FinishRegistrationButton = e.Element("FinishRegistrationButton")?.Value;
                        SpecialScript = e.Element("SpecialScript")?.Value;
                        OnEnrollScript = e.Element("OnEnrollScript")?.Value;
                        GroupToJoin = e.Element("GroupToJoin")?.Value;
                        TimeOut = e.Element("TimeOut").ToInt2();
                        AllowOnlyOne = e.Element("AllowOnlyOne").ToBool();
                        TargetExtraValues = e.Element("TargetExtraValues").ToBool();
                        AllowReRegister = e.Element("AllowReRegister").ToBool();
                        AllowSaveProgress = e.Element("AllowSaveProgress").ToBool();
                        MemberOnly = e.Element("MemberOnly").ToBool();
                        AddAsProspect = e.Element("AddAsProspect").ToBool();
                        DisallowAnonymous = e.Element("DisallowAnonymous").ToBool();
                        break;
                    case "NotRequired":
                        NoReqBirthYear = e.Element("NoReqBirthYear").ToBool();
                        NotReqDOB = e.Element("NotReqDOB").ToBool();
                        NotReqAddr = e.Element("NotReqAddr").ToBool();
                        NotReqZip = e.Element("NotReqZip").ToBool();
                        NotReqPhone = e.Element("NotReqPhone").ToBool();
                        NotReqGender = e.Element("NotReqGender").ToBool();
                        NotReqMarital = e.Element("NotReqMarital").ToBool();
                        NotReqCampus = e.Element("NotReqCampus").ToBool();
                        break;
                    case "TimeSlots":
                        TimeSlots = TimeSlots.ReadXml(e);
                        if (TimeSlots.TimeSlotLockDays.HasValue)
                            TimeSlotLockDays = TimeSlots.TimeSlotLockDays;
                        break;
                    case "AskItems":
                        foreach (var ele in e.Elements())
                            AskItems.Add(Ask.ReadXml(ele));
                        break;

                }
            }
            SetUniqueIds("AskDropdown");
            SetUniqueIds("AskExtraQuestions");
            SetUniqueIds("AskCheckboxes");
            SetUniqueIds("AskText");
            SetUniqueIds("AskMenu");
        }

        public void WriteXml(XmlWriter writer)
        {
            var w = new APIWriter(writer);
            w.Attr("id", OrgId);
            w.AddComment($"{Util.UserPeopleId} {Util.Now:g}");

            w.StartPending("Confirmation")
                .Add("Subject", Subject)
                .AddCdata("Body", Body)
                .EndPending();

            w.StartPending("Reminder")
                .Add("Subject", ReminderSubject)
                .AddCdata("Body", ReminderBody)
                .EndPending();

            w.StartPending("SupportEmail")
                .Add("Subject", SupportSubject)
                .AddCdata("Body", SupportBody)
                .EndPending();

            w.StartPending("SenderEmail")
                .Add("Subject", SenderSubject)
                .AddCdata("Body", SenderBody)
                .EndPending();

            w.StartPending("Fees")
                .Add("Fee", Fee)
                .Add("Deposit", Deposit)
                .Add("ExtraFee", ExtraFee)
                .Add("MaximumFee", MaximumFee)
                .Add("ExtraValueFeeName", ExtraValueFeeName)
                .Add("AccountingCode", AccountingCode)
                .AddIfTrue("ApplyMaxToOtherFees", ApplyMaxToOtherFees)
                .AddIfTrue("IncludeOtherFeesWithDeposit", IncludeOtherFeesWithDeposit)
                .AddIfTrue("OtherFeesAddedToOrgFee", OtherFeesAddedToOrgFee)
                .AddIfTrue("AskDonation", AskDonation)
                .Add("DonationLabel", DonationLabel)
                .Add("DonationFundId", DonationFundId)
                .EndPending();


            w.StartPending("OrgFees");
            foreach (var i in OrgFees)
                w.Start("Fee")
                    .Attr("OrgId", i.OrgId)
                    .Attr("Fee", i.Fee)
                    .End();
            w.EndPending();


            w.StartPending("AgeGroups");
            foreach (var i in AgeGroups)
                w.Start("Group")
                    .Attr("StartAge", i.StartAge)
                    .Attr("EndAge", i.EndAge)
                    .Attr("Fee", i.Fee)
                    .AddText(i.SmallGroup)
                    .End();
            w.EndPending();

            w.StartPending("Instructions")
                .AddCdata("Login", InstructionLogin)
                .AddCdata("Select", InstructionSelect)
                .AddCdata("Find", InstructionFind)
                .AddCdata("Options", InstructionOptions)
                .AddCdata("Special", InstructionSpecial)
                .AddCdata("Submit", InstructionSubmit)
                .AddCdata("Sorry", InstructionSorry)
                .AddCdata("Thanks", ThankYouMessage)
                .AddCdata("Terms", Terms)
                .EndPending();

            w.StartPending("Options")
                .AddCdata("ConfirmationTrackingCode", ConfirmationTrackingCode)
                .Add("ValidateOrgs", ValidateOrgs)
                .Add("Shell", Shell)
                .Add("ShellBs", ShellBs)
                .Add("FinishRegistrationButton", FinishRegistrationButton)
                .Add("SpecialScript", SpecialScript)
                .Add("OnEnrollScript", OnEnrollScript)
                .Add("GroupToJoin", GroupToJoin)
                .Add("TimeOut", TimeOut)
                .AddIfTrue("AllowOnlyOne", AllowOnlyOne)
                .AddIfTrue("TargetExtraValues", TargetExtraValues)
                .AddIfTrue("AllowReRegister", AllowReRegister)
                .AddIfTrue("AllowSaveProgress", AllowSaveProgress)
                .AddIfTrue("MemberOnly", MemberOnly)
                .AddIfTrue("AddAsProspect", AddAsProspect)
                .AddIfTrue("DisallowAnonymous", DisallowAnonymous)
                .EndPending();

            w.StartPending("NotRequired")
                .AddIfTrue("NoReqBirthYear", NoReqBirthYear)
                .AddIfTrue("NotReqDOB", NotReqDOB)
                .AddIfTrue("NotReqAddr", NotReqAddr)
                .AddIfTrue("NotReqZip", NotReqZip)
                .AddIfTrue("NotReqPhone", NotReqPhone)
                .AddIfTrue("NotReqGender", NotReqGender)
                .AddIfTrue("NotReqMarital", NotReqMarital)
                .AddIfTrue("NotReqCampus", NotReqCampus)
                .EndPending();

            TimeSlots?.WriteXml(w);

            w.StartPending("AskItems");
            foreach (var a in AskItems)
                a.WriteXml(w);
            w.EndPending();
        }
        public class Messages
        {
            public bool Confirmation { get; set; }
            public bool Sender { get; set; }
            public bool Support { get; set; }
            public bool Reminder { get; set; }
            public bool Login { get; set; }
            public bool Find { get; set; }
            public bool Submit { get; set; }
            public bool Select { get; set; }
            public bool Sorry { get; set; }
            public bool Options { get; set; }
            public bool Special { get; set; }
        }
        public void WriteXmlMessages(XmlWriter writer, Messages messages)
        {
            var w = new APIWriter(writer);
            w.Start("Messages");
            w.Attr("id", OrgId);
            w.AddComment($"{Util.UserPeopleId} {Util.Now:g}");

            if(messages.Confirmation)
                w.Start("Confirmation")
                    .Add("Subject", Subject)
                    .AddCdata("Body", Body)
                    .End();

            if(messages.Reminder)
                w.Start("Reminder")
                    .Add("Subject", ReminderSubject)
                    .AddCdata("Body", ReminderBody)
                    .End();

            if(messages.Support)
                w.Start("SupportEmail")
                    .Add("Subject", SupportSubject)
                    .AddCdata("Body", SupportBody)
                    .End();

            if(messages.Support)
                w.Start("SenderEmail")
                    .Add("Subject", SenderSubject)
                    .AddCdata("Body", SenderBody)
                    .End();

            w.StartPending("Instructions");
            if(messages.Login)
                w.AddCdata("Login", InstructionLogin);
            if (messages.Select)
                w.AddCdata("Select", InstructionSelect);
            if (messages.Find)
                w.AddCdata("Find", InstructionFind);
            if (messages.Options)
                w.AddCdata("Options", InstructionOptions);
            if (messages.Special)
                w.AddCdata("Special", InstructionSpecial);
            if (messages.Submit)
                w.AddCdata("Submit", InstructionSubmit);
            if (messages.Sorry)
                w.AddCdata("Sorry", InstructionSorry);
            w.EndPending();

            w.End();
        }
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }
    }
}
