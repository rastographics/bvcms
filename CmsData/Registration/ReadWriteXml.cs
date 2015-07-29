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
                    case "test":
                        break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            var w = new APIWriter(writer);
            var userPeopleId = Util.UserPeopleId;
            writer.WriteComment($"{userPeopleId} {DateTime.Now:g}");

            w.StartPending("Confirmation");
            w.Add("Subject", Subject);
            w.AddCdata("Body", Body);
            w.EndPending();

            w.StartPending("Reminder");
            w.Add("Subject", ReminderSubject);
            w.AddCdata("Body", ReminderBody);
            w.EndPending();

            w.StartPending("Support");
            w.Add("Subject", SupportSubject);
            w.AddCdata("Body", SupportBody);
            w.EndPending();

            w.StartPending("Sender");
            w.Add("Subject", SenderSubject);
            w.AddCdata("Body", SenderBody);
            w.EndPending();

            w.Add("Fee", Fee);
            w.Add("Deposit", Deposit);
            w.Add("ExtraFee", ExtraFee);
            w.Add("MaximumFee", MaximumFee);
            w.Add("ApplyMaxToOtherFees", ApplyMaxToOtherFees);
            w.Add("ExtraValueFeeName", ExtraValueFeeName);
            w.Add("AccountingCode", AccountingCode);
            w.AddIfTrue("IncludeOtherFeesWithDeposit", IncludeOtherFeesWithDeposit);

            w.StartPending("Donation");
            w.Add("Label", DonationLabel);
            w.Add("FundId", DonationFundId);
            w.EndPending();

            w.StartPending("AgeGroups");
            foreach (var i in AgeGroups)
            {
                w.Start("Group");
                w.Attr("StartAge", i.StartAge);
                w.Attr("EndAge", i.EndAge);
                w.Attr("Fee", i.Fee);
                w.AddText(i.SmallGroup);
                w.End();
            }
            w.EndPending();

            w.StartPending("OrgFees");
            foreach (var i in OrgFees)
            {
                w.Start("Fee");
                w.Attr("OrgId", i.OrgId);
                w.Attr("Fee", i.Fee);
                w.End();
            }
            w.EndPending();

            w.AddIfTrue("OtherFeesAddedToOrgFee", OtherFeesAddedToOrgFee);

            w.StartPending("Instructions");
            w.AddCdata("Login", InstructionLogin);
            w.AddCdata("Select", InstructionSelect);
            w.AddCdata("Find", InstructionFind);
            w.AddCdata("Options", InstructionOptions);
            w.AddCdata("Special", InstructionSpecial);
            w.AddCdata("Submit", InstructionSubmit);
            w.AddCdata("Sorry", InstructionSorry);
            w.AddCdata("Thanks", ThankYouMessage);
            w.EndPending();

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
