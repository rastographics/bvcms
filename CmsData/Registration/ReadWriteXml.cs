using System;
using System.Collections.Generic;
using System.Web;
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
        public void ReadXml(XmlReader reader)
        {
            var x = XDocument.Load(reader);
            if (x.Root == null) return;

            foreach (var e in x.Root.Elements())
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
                    case "Fees":
                        Fee = e.Element("Fee")?.Value.ToDecimal();
                        Deposit = e.Element("Deposit")?.Value.ToDecimal();
                        ExtraFee = e.Element("ExtraFee")?.Value.ToDecimal();
                        MaximumFee = e.Element("MaximumFee")?.Value.ToDecimal();
                        ApplyMaxToOtherFees = e.Element("ApplyMaxToOtherFees")?.Value.ToBool2() ?? false;
                        ExtraValueFeeName = e.Element("ExtraValueFeeName")?.Value;
                        AccountingCode = e.Element("AccountingCode")?.Value;
                        IncludeOtherFeesWithDeposit = e.Element("IncludeOtherFeesWithDeposit")?.Value.ToBool2() ?? false;
                        OtherFeesAddedToOrgFee = e.Element("OtherFeesAddedToOrgFee")?.Value.ToBool2() ?? false;
                        DonationLabel = e.Element("AskDonation")?.Element("Label")?.Value;
                        DonationFundId = e.Element("AskDonation")?.Element("FundId")?.Value.ToInt2();
                        break;
                    case "AgeGroups":
                        if(AgeGroups == null)
                            AgeGroups = new List<AgeGroup>();
                        var ageGroup = new AgeGroup
                        {
                            StartAge = e.Attribute("StartAge")?.Value.ToInt2() ?? 0,
                            EndAge = e.Attribute("EndAge")?.Value.ToInt2() ?? 0,
                            Fee = e.Attribute("Fee")?.Value.ToDecimal(),
                            SmallGroup = e.Value
                        };
                        AgeGroups.Add(ageGroup);
                        break;
                    case "OrgFees":
                        if(OrgFees == null)
                            OrgFees = new List<OrgFee>();
                        var orgfee = new OrgFee
                        {
                            OrgId = e.Attribute("OrgId").Value.ToInt(),
                            Fee = e.Attribute("Fee").Value.ToDecimal()
                        };
                        OrgFees.Add(orgfee);
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
                    case "Options":
                        ConfirmationTrackingCode = e.Element("ConfirmTrackingCode")?.Value;
                        ValidateOrgs = e.Element("ValidateOrgs")?.Value;
                        Shell = e.Element("Shell")?.Value;
                        ShellBs = e.Element("ShellBs")?.Value;
                        FinishRegistrationButton = e.Element("FinishRegistrationButton")?.Value;
                        SpecialScript = e.Element("SpecialScript")?.Value;
                        GroupToJoin = e.Element("GroupToJoin")?.Value;
                        TimeOut = e.Element("TimeOut")?.Value.ToInt2();
                        AllowOnlyOne = e.Element("AllowOnlyOne")?.Value.ToBool2() ?? false;
                        TargetExtraValues = e.Element("TargetExtraValues")?.Value.ToBool2() ?? false;
                        AllowReRegister = e.Element("AllowReRegister")?.Value.ToBool2() ?? false;
                        AllowSaveProgress = e.Element("AllowSaveProgress")?.Value.ToBool2() ?? false;
                        MemberOnly = e.Element("MemberOnly")?.Value.ToBool2() ?? false;
                        AddAsProspect = e.Element("AddAsProspect")?.Value.ToBool2() ?? false;
                        DisallowAnonymous = e.Element("DisallowAnonymous")?.Value.ToBool2() ?? false;
                        break;
                    case "NotRequired":
                        NoReqBirthYear = e.Element("NoReqBirthYear")?.Value.ToBool2() ?? false;
                        NotReqDOB = e.Element("NotReqDOB")?.Value.ToBool2() ?? false;
                        NotReqAddr = e.Element("NotReqAddr")?.Value.ToBool2() ?? false;
                        NotReqZip = e.Element("NotReqZip")?.Value.ToBool2() ?? false;
                        NotReqPhone = e.Element("NotReqPhone")?.Value.ToBool2() ?? false;
                        NotReqGender = e.Element("NotReqGender")?.Value.ToBool2() ?? false;
                        NotReqMarital = e.Element("NotReqMarital")?.Value.ToBool2() ?? false;
                        break;
                    case "TimeSlots":
                        TimeSlotLockDays = e.Attribute("LockDays")?.Value.ToInt2();
                        TimeSlots = new TimeSlots();
                        foreach (var ele in e.Elements("Slot"))
                            TimeSlots.list.Add(new TimeSlots.TimeSlot()
                            {
                                Time = ele.Attribute("Time")?.Value.ToDate(),
                                DayOfWeek = ele.Attribute("DayOfWeek")?.Value.ToInt2() ?? 0,
                                Limit = ele.Attribute("Limit")?.Value.ToInt2(),
                                Description = ele.Value,
                            });
                        break;
                    case "AskItems":
                        foreach (var ele in e.Elements())
                            AskItems.Add(Ask.ReadXml(ele));
                        break;

                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            var w = new APIWriter(writer);
            var userPeopleId = Util.UserPeopleId;
            writer.WriteComment($"{userPeopleId} {DateTime.Now:g}");

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
                .Add("ApplyMaxToOtherFees", ApplyMaxToOtherFees)
                .Add("ExtraValueFeeName", ExtraValueFeeName)
                .Add("AccountingCode", AccountingCode)
                .AddIfTrue("IncludeOtherFeesWithDeposit", IncludeOtherFeesWithDeposit)
                .AddIfTrue("OtherFeesAddedToOrgFee", OtherFeesAddedToOrgFee)
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
                .Add("ConfirmationTrackingCode", ConfirmationTrackingCode)
                .Add("ValidateOrgs", ValidateOrgs)
                .Add("Shell", Shell)
                .Add("ShellBs", ShellBs)
                .Add("FinishRegistrationButton", FinishRegistrationButton)
                .Add("SpecialScript", SpecialScript)
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
                .EndPending();

            TimeSlots?.WriteXml(w);

            w.StartPending("AskItems");
            foreach (var a in AskItems)
                a.WriteXml(w);
            w.EndPending();

        }
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

    }
}
