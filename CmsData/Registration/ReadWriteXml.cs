using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using CmsData.API;
using UtilityExtensions;
using RegKeywords = CmsData.Registration.Parser.RegKeywords;

namespace CmsData.Registration
{
    [Serializable]
    public partial class Settings : IXmlSerializable
    {
        private Parser.RegKeywords Parse(string s)
        {
            return (RegKeywords)Enum.Parse(typeof(RegKeywords), s);
        }
        public void ReadXml(XmlReader reader)
        {
            var s = reader.ReadOuterXml();
            var x = XDocument.Parse(s);
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
                        AccountingCode = e.Element("")?.Value;
                        IncludeOtherFeesWithDeposit = e.Element("")?.Value.ToBool2() ?? false;
                        OtherFeesAddedToOrgFee = e.Element("")?.Value.ToBool2() ?? false;
                        AskDonation = e.Element("AskDonation") != null;
                        if (AskDonation)
                        {
                            DonationLabel = e.Element("AskDonation")?.Element("Label")?.Value;
                            DonationFundId = e.Element("AskDonation")?.Element("FundId")?.Value.ToInt2();
                        }
                        break;
                    case "AgeGroups":
                        if(AgeGroups == null)
                            AgeGroups = new List<AgeGroup>();
                        var ageGroup = new AgeGroup()
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
                        var orgfee = new OrgFee()
                        {
                            OrgId = e.Attribute("OrgId").Value.ToInt(),
                            Fee = e.Attribute("Fee").Value.ToDecimal(),
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
                        break;
                    case "NotReqired":
                        NoReqBirthYear = e.Element("NoReqBirthYear")?.Value.ToBool2() ?? false;
                        NotReqDOB = e.Element("NotReqDOB")?.Value.ToBool2() ?? false;
                        NotReqAddr = e.Element("NotReqAddr")?.Value.ToBool2() ?? false;
                        NotReqZip = e.Element("NotReqZip")?.Value.ToBool2() ?? false;
                        NotReqPhone = e.Element("NotReqPhone")?.Value.ToBool2() ?? false;
                        NotReqGender = e.Element("NotReqGender")?.Value.ToBool2() ?? false;
                        NotReqMarital = e.Element("NotReqMarital")?.Value.ToBool2() ?? false;
                        DisallowAnonymous = e.Element("DisallowAnonymous")?.Value.ToBool2() ?? false;
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
                    case "Ask":
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

            w.StartPending(RegKeywords.Confirmation)
                .Add("Subject", Subject)
                .AddCdata("Body", Body)
                .EndPending();

            w.StartPending(RegKeywords.Reminder)
                .Add("Subject", ReminderSubject)
                .AddCdata("Body", ReminderBody)
                .EndPending();

            w.StartPending(RegKeywords.SupportEmail)
                .Add("Subject", SupportSubject)
                .AddCdata("Body", SupportBody)
                .EndPending();

            w.StartPending(RegKeywords.SenderEmail)
                .Add("Subject", SenderSubject)
                .AddCdata("Body", SenderBody)
                .EndPending();

            w.Start("Fees")
                .Add(RegKeywords.Fee, Fee)
                .Add(RegKeywords.Deposit, Deposit)
                .Add(RegKeywords.ExtraFee, ExtraFee)
                .Add(RegKeywords.MaximumFee, MaximumFee)
                .Add(RegKeywords.ApplyMaxToOtherFees, ApplyMaxToOtherFees)
                .Add(RegKeywords.ExtraValueFeeName, ExtraValueFeeName)
                .Add(RegKeywords.AccountingCode, AccountingCode)
                .AddIfTrue(RegKeywords.IncludeOtherFeesWithDeposit, IncludeOtherFeesWithDeposit)
                .AddIfTrue(RegKeywords.OtherFeesAddedToOrgFee, OtherFeesAddedToOrgFee)
                .StartPending(RegKeywords.AskDonation)
                    .Add("Label", DonationLabel)
                    .Add("FundId", DonationFundId)
                    .EndPending()
                .EndPending();


            w.StartPending(RegKeywords.OrgFees);
            foreach (var i in OrgFees)
                w.Start("Fee")
                    .Attr("OrgId", i.OrgId)
                    .Attr("Fee", i.Fee)
                    .End();
            w.EndPending();


            w.StartPending(RegKeywords.AgeGroups);
            foreach (var i in AgeGroups)
                w.Start("Group")
                    .Attr("StartAge", i.StartAge)
                    .Attr("EndAge", i.EndAge)
                    .Attr("Fee", i.Fee)
                    .AddText(i.SmallGroup)
                    .End();
            w.EndPending();

            w.StartPending(RegKeywords.Instructions)
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
                .Add(RegKeywords.ConfirmationTrackingCode, ConfirmationTrackingCode)
                .Add(RegKeywords.ValidateOrgs, ValidateOrgs)
                .Add(RegKeywords.Shell, Shell)
                .Add(RegKeywords.ShellBs, ShellBs)
                .Add(RegKeywords.FinishRegistrationButton, FinishRegistrationButton)
                .Add(RegKeywords.SpecialScript, SpecialScript)
                .Add(RegKeywords.GroupToJoin, GroupToJoin)
                .Add(RegKeywords.TimeOut, TimeOut)
                .AddIfTrue(RegKeywords.AllowOnlyOne, AllowOnlyOne)
                .AddIfTrue(RegKeywords.TargetExtraValues, TargetExtraValues)
                .AddIfTrue(RegKeywords.AllowReRegister, AllowReRegister)
                .AddIfTrue(RegKeywords.AllowSaveProgress, AllowSaveProgress)
                .AddIfTrue(RegKeywords.MemberOnly, MemberOnly)
                .AddIfTrue(RegKeywords.AddAsProspect, AddAsProspect)
                .EndPending();

            w.StartPending("NotReq")
                .AddIfTrue(RegKeywords.NoReqBirthYear, NoReqBirthYear)
                .AddIfTrue(RegKeywords.NotReqDOB, NotReqDOB)
                .AddIfTrue(RegKeywords.NotReqAddr, NotReqAddr)
                .AddIfTrue(RegKeywords.NotReqZip, NotReqZip)
                .AddIfTrue(RegKeywords.NotReqPhone, NotReqPhone)
                .AddIfTrue(RegKeywords.NotReqGender, NotReqGender)
                .AddIfTrue(RegKeywords.NotReqMarital, NotReqMarital)
                .AddIfTrue(RegKeywords.DisallowAnonymous, DisallowAnonymous)
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
