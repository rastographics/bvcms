using System;
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
            var s = reader.ReadOuterXml();
            var x = XDocument.Parse(s);
            if (x.Root == null) return;

            foreach (var e in x.Root.Elements())
            {
                var name = e.Name.ToString();
                switch (name)
                {
                    case "Confirmation":
                        break;
                    case "Reminder":
                        break;
                    case "Support":
                        break;
                    case "Sender":
                        break;
                    case "Fee":
                        break;
                    case "Deposit":
                        break;
                    case "ExtraFee":
                        break;
                    case "MaximumFee":
                        break;
                    case "ApplyMaxToOtherFees":
                        break;
                    case "ExtraValueFeeName":
                        break;
                    case "AccountingCode":
                        break;
                    case "IncludeOtherFeesWithDeposit":
                        break;
                    case "Donation":
                        break;
                    case "AgeGroups":
                        break;
                    case "OrgFees":
                        break;
                    case "OtherFeesAddedToOrgFee":
                        break;
                    case "Instructions":
                        break;
                    case "Terms":
                        break;
                    case "ConfirmationTrackingCode":
                        break;
                    case "ValidateOrgs":
                        break;
                    case "Shell":
                        break;
                    case "ShellBs":
                        break;
                    case "FinishRegistrationButton":
                        break;
                    case "SpecialScript":
                        break;
                    case "GroupToJoin":
                        break;
                    case "TimeOut":
                        break;
                    case "AllowOnlyOne":
                        break;
                    case "TargetExtraValues":
                        break;
                    case "AllowReRegister":
                        break;
                    case "AllowSaveProgress":
                        break;
                    case "MemberOnly":
                        break;
                    case "AddAsProspect":
                        break;
                    case "NoReqBirthYear":
                        break;
                    case "NotReqDOB":
                        break;
                    case "NotReqAddr":
                        break;
                    case "NotReqZip":
                        break;
                    case "NotReqPhone":
                        break;
                    case "NotReqGender":
                        break;
                    case "NotReqMarital":
                        break;
                    case "DisallowAnonymous":
                        break;
                    case "TimeSlots":
                        break;
                    case "Ask":
                        break;
                    case "AskCheckboxes":
                        break;
                    case "AskDropdown":
                        break;
                    case "AskExtraQuestions":
                        break;
                    case "AskGradeOptions":
                        break;
                    case "AskHeader":
                        break;
                    case "AskInstruction":
                        break;
                    case "AskMenu":
                        break;
                    case "AskRequest":
                        break;
                    case "AskSize":
                        break;
                    case "AskSuggestedFee":
                        break;
                    case "AskText":
                        break;
                    case "AskTickets":
                        break;
                    case "AskYesNoQuestions":
                        break;
                        

                    case "AskSMS":
                    case "AskParents":
                    case "AskDoctor":
                    case "AskInsurance":
                    case "AskEmContact":
                    case "AskAllergies":
                    case "AskChurch":
                    case "AskTylenolEtc":
                    case "AskCoaching":
                    case "AskGrade":
                    case "AskDonation":
                    case "AskMedical":
                        AskItems.Add(new Ask(name));
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

            w.StartPending("Support")
                .Add("Subject", SupportSubject)
                .AddCdata("Body", SupportBody)
                .EndPending();

            w.StartPending("Sender")
                .Add("Subject", SenderSubject)
                .AddCdata("Body", SenderBody)
                .EndPending();

            w.Add("Fee", Fee);
            w.Add("Deposit", Deposit);
            w.Add("ExtraFee", ExtraFee);
            w.Add("MaximumFee", MaximumFee);
            w.Add("ApplyMaxToOtherFees", ApplyMaxToOtherFees);
            w.Add("ExtraValueFeeName", ExtraValueFeeName);
            w.Add("AccountingCode", AccountingCode);
            w.AddIfTrue("IncludeOtherFeesWithDeposit", IncludeOtherFeesWithDeposit);

            w.StartPending("Donation")
                .Add("Label", DonationLabel)
                .Add("FundId", DonationFundId)
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

            w.AddIfTrue("OtherFeesAddedToOrgFee", OtherFeesAddedToOrgFee);

            w.StartPending("Instructions")
                .AddCdata("Login", InstructionLogin)
                .AddCdata("Select", InstructionSelect)
                .AddCdata("Find", InstructionFind)
                .AddCdata("Options", InstructionOptions)
                .AddCdata("Special", InstructionSpecial)
                .AddCdata("Submit", InstructionSubmit)
                .AddCdata("Sorry", InstructionSorry)
                .AddCdata("Thanks", ThankYouMessage)
                .EndPending();

            w.Add("Terms", Terms);
            w.Add("ConfirmationTrackingCode", ConfirmationTrackingCode);

            w.Add("ValidateOrgs", ValidateOrgs);
            w.Add("Shell", Shell);
            w.Add("ShellBs", ShellBs);
            w.Add("FinishRegistrationButton", FinishRegistrationButton);
            w.Add("SpecialScript", SpecialScript);
            w.Add("GroupToJoin", GroupToJoin);
            w.Add("TimeOut", TimeOut);

            w.AddIfTrue("AllowOnlyOne", AllowOnlyOne);
            w.AddIfTrue("TargetExtraValues", TargetExtraValues);
            w.AddIfTrue("AllowReRegister", AllowReRegister);
            w.AddIfTrue("AllowSaveProgress", AllowSaveProgress);
            w.AddIfTrue("MemberOnly", MemberOnly);
            w.AddIfTrue("AddAsProspect", AddAsProspect);
            w.AddIfTrue("NoReqBirthYear", NoReqBirthYear);
            w.AddIfTrue("NotReqDOB", NotReqDOB);
            w.AddIfTrue("NotReqAddr", NotReqAddr);
            w.AddIfTrue("NotReqZip", NotReqZip);
            w.AddIfTrue("NotReqPhone", NotReqPhone);
            w.AddIfTrue("NotReqGender", NotReqGender);
            w.AddIfTrue("NotReqMarital", NotReqMarital);
            w.AddIfTrue("DisallowAnonymous", DisallowAnonymous);

            TimeSlots.WriteXml(writer);

            foreach (var a in AskItems)
                a.WriteXml(writer);

        }
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

    }
}
