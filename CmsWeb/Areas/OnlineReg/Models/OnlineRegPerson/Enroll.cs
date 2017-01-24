using System;
using System.Linq;
using CmsData;
using CmsData.Registration;
using UtilityExtensions;
using System.Text;
using System.Web;
using CmsData.Codes;
using CmsData.View;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        public OrganizationMember Enroll(Transaction transaction, string payLink)
        {
            var om = GetOrganizationMember(transaction);

            if (Parent.SupportMissionTrip)
                return AddSender(om);

            if (RecordFamilyAttendance())
                return RecordAllFamilyAttends(om);

            om.Amount = TotalAmount();

            SaveAnswers(om);
            SaveAgeGroupChoice(om);
            DoLinkGroupsFromOrgs(om);
            LogRegistrationOnOrgMember(transaction, om, payLink);

            DbUtil.Db.SubmitChanges();
            return om;
        }

        private static string DefaultMessage => DbUtil.Db.ContentHtml("DefaultConfirmation", 
                Resource1.SettingsRegistrationModel_DefaulConfirmation);

        public string SummaryTransaction()
        {
            var ts = Parent.TransactionSummary();
            if (ts == null)
                return "";
            var summary = DbUtil.Db.RenderTemplate(@"
<table>
    <tr>
        <td style='{{LabelStyle}}'>Total Paid</td>
        {{#if TotCoupon}}
            <td style='{{LabelStyle}}'>Coupon</td>
        {{/if}}
        <td style='{{LabelStyle}}'>Total Due</td>
    </tr>
    <tr>
        <td style='{{DataStyle}}{{AlignRight}}'>{{Fmt TotPaid 'c'}}</td>
        {{#if TotCoupon}}
            <td style='{{DataStyle}}{{AlignRight}}'>{{Fmt TotCoupon 'c'}}</td>
        {{/if}}
        <td style='{{DataStyle}}{{AlignRight}}'>{{Fmt TotDue 'c'}}</td>
    </tr>
</table>
", ts);
            return summary;
        }
        public string GetMessage()
        {
            var payLink = GetPayLink();
            var body = settings[org.OrganizationId].Body;
            if (masterorgid.HasValue && !body.HasValue())
                body = settings[masterorgid.Value].Body;
            var message = Util.PickFirst(body, DefaultMessage);

            var location = org.Location;
            if (!location.HasValue() && masterorg != null)
                location = masterorg.Location;

            message = CmsData.API.APIOrganization.MessageReplacements(DbUtil.Db,
                person, org.DivisionName, org.OrganizationId, org.OrganizationName, location, message);

            var ts = Parent.TransactionSummary();
            message = message.Replace("{phone}", org.PhoneNumber.FmtFone7())
                .Replace("{tickets}", ntickets.ToString())
                .Replace("{paid}", ts?.TotPaid.ToString("c"))
                .Replace("{sessiontotal}", ts?.TotPaid.ToString("c"));
            if (ts?.TotDue > 0)
                message = message.Replace("{paylink}",
                    $"<a href='{payLink}'>Click this link to make a payment on your balance of {ts.TotDue:C}</a>.");
            else
                message = message.Replace("{paylink}", "You have a zero balance.");
            return message;
        }

        private string cachedPayLink;
        public string GetPayLink()
        {
            if (cachedPayLink.HasValue())
                return cachedPayLink;
            var estr = HttpUtility.UrlEncode(Util.Encrypt(Parent.Transaction.OriginalId.ToString()));
            return cachedPayLink = DbUtil.Db.ServerLink("/OnlineReg/PayAmtDue?q=" + estr);
        }

        private void LogRegistrationOnOrgMember(Transaction transaction, OrganizationMember om, string paylink)
        {
            var reg = person.GetRecReg();
            var sb = new StringBuilder();

            sb.AppendFormat("{0:g} ----------------\n", DateTime.Now);
            if (om.AmountPaid > 0)
            {
                var others = GetOthersInTransaction(transaction);
                sb.AppendFormat("{0:c} ({1} id) transaction amount\n", transaction.Amt, transaction.Id);
                sb.AppendFormat("{0:c} applied to this registrant\n", AmountToPay());
                sb.AppendFormat("{0:c} total due all registrants\n", transaction.Amtdue);
                if (others.HasValue())
                    sb.AppendFormat("Others: {0}\n", others);
            }
            om.AddToMemberDataBelowComments(sb.ToString());

            var sbreg = new StringBuilder();
            sbreg.Append($"{org.OrganizationName}\n");
            sbreg.AppendFormat("{0:g} ----------------\n", DateTime.Now);
            if (om.AmountPaid > 0)
            {
                sbreg.AppendFormat("{0:c} ({1} id) transaction amount\n", transaction.Amt, transaction.Id);
                sbreg.AppendFormat("{0:c} applied to this registrant\n", AmountToPay());
                sbreg.AppendFormat("{0:c} total due all registrants\n", transaction.Amtdue);
            }
            if (paylink.HasValue())
            {
                sbreg.AppendLine(paylink);
                om.PayLink = paylink;
            }
            if (request.HasValue())
            {
                sbreg.AppendFormat("Request: {0}\n", request);
                om.Request = request.Truncate(140);
            }
            sbreg.AppendFormat("{0}\n", EmailAddress);

            reg.AddToComments(sbreg.ToString());
        }

        private void DoLinkGroupsFromOrgs(OrganizationMember om)
        {
            if (setting.LinkGroupsFromOrgs.Count > 0)
            {
                var q = from omt in DbUtil.Db.OrgMemMemTags
                    where setting.LinkGroupsFromOrgs.Contains(omt.OrgId)
                    where omt.PeopleId == om.PeopleId
                    select omt.MemberTag.Name;
                foreach (var name in q)
                    om.AddToGroup(DbUtil.Db, name);
            }
            if (om.Organization.IsMissionTrip == true)
                om.AddToGroup(DbUtil.Db, "Goer");
        }

        private void SaveAgeGroupChoice(OrganizationMember om)
        {
            if (setting.TargetExtraValues)
            {
                foreach (var ag in setting.AgeGroups)
                    person.RemoveExtraValue(DbUtil.Db, ag.SmallGroup);
                if (setting.AgeGroups.Count > 0)
                    person.AddEditExtraCode(AgeGroup(), "true");
            }
            else
            {
                foreach (var ag in setting.AgeGroups)
                    om.RemoveFromGroup(DbUtil.Db, ag.SmallGroup);
                if (setting.AgeGroups.Count > 0)
                    om.AddToGroup(DbUtil.Db, AgeGroup());
            }
        }

        private void SaveAnswers(OrganizationMember om)
        {
            om.OnlineRegData = Util.Serialize(this); // saves all answers

            var reg = person.SetRecReg();
            foreach (var ask in setting.AskItems)
                switch (ask.Type)
                {
                    case "AskSize":
                        om.ShirtSize = shirtsize;
                        reg.ShirtSize = shirtsize;
                        break;
                    case "AskChurch":
                        reg.ActiveInAnotherChurch = otherchurch;
                        reg.Member = memberus;
                        break;
                    case "AskAllergies":
                        reg.MedAllergy = medical.HasValue();
                        reg.MedicalDescription = medical;
                        break;
                    case "AskParents":
                        reg.Mname = mname;
                        reg.Fname = fname;
                        break;
                    case "AskEmContact":
                        reg.Emcontact = emcontact;
                        reg.Emphone = emphone;
                        break;
                    case "AskTylenolEtc":
                        reg.Tylenol = tylenol;
                        reg.Advil = advil;
                        reg.Robitussin = robitussin;
                        reg.Maalox = maalox;
                        break;
                    case "AskDoctor":
                        reg.Docphone = docphone;
                        reg.Doctor = doctor;
                        break;
                    case "AskCoaching":
                        reg.Coaching = coaching;
                        break;
                    case "AskSMS":
                        SaveSMSChoice();
                        break;
                    case "AskInsurance":
                        reg.Insurance = insurance;
                        reg.Policy = policy;
                        break;
                    case "AskTickets":
                        om.Tickets = ntickets;
                        break;
                    case "AskYesNoQuestions":
                        SaveYesNoChoices(om, ask);
                        break;
                    case "AskCheckboxes":
                        SaveCheckboxChoices(om, ask);
                        break;
                    case "AskExtraQuestions":
                        foreach (var g in ExtraQuestion[ask.UniqueId])
                            if (g.Value.HasValue())
                                if (setting.TargetExtraValues)
                                    person.AddEditExtraText(g.Key, g.Value);
                        break;
                    case "AskText":
                        foreach (var g in Text[ask.UniqueId])
                            if (g.Value.HasValue())
                                if (setting.TargetExtraValues)
                                    person.AddEditExtraText(g.Key, g.Value);
                        break;
                    case "AskMenu":
                        SaveMenuChoices(om, ask);
                        break;
                    case "AskDropdown":
                        SaveDropdownChoice(om, ask);
                        break;
                    case "AskGradeOptions":
                        SaveGradeChoice(om);
                        break;
                }
        }

        private void SaveSMSChoice()
        {
            if (sms.HasValue && LoggedIn == true)
            {
                person.UpdateValue("ReceiveSMS", sms.Value);
                Log("ReceiveSMS " + sms.Value);
            }
        }

        private void SaveGradeChoice(OrganizationMember om)
        {
            if (setting.TargetExtraValues)
                person.Grade = gradeoption.ToInt();
            else
            {
                om.Grade = gradeoption.ToInt();
                person.UpdateValue("Grade", gradeoption.ToInt());
                Log("GradeUpdate " + person.Grade);
            }
        }

        private void SaveDropdownChoice(OrganizationMember om, Ask ask)
        {
            if (setting.TargetExtraValues)
            {
                foreach (var op in ((AskDropdown) ask).list)
                    person.RemoveExtraValue(DbUtil.Db, op.SmallGroup);
                person.AddEditExtraCode(((AskDropdown) ask).SmallGroupChoice(option).SmallGroup, "true");
            }
            else
            {
                foreach (var op in ((AskDropdown) ask).list)
                    op.RemoveFromSmallGroup(DbUtil.Db, om);
                ((AskDropdown) ask).SmallGroupChoice(option).AddToSmallGroup(DbUtil.Db, om, PythonModel);
            }
        }

        private void SaveMenuChoices(OrganizationMember om, Ask ask)
        {
            foreach (var i in MenuItem[ask.UniqueId])
                om.AddToGroup(DbUtil.Db, i.Key, i.Value);
            {
                var menulabel = ((AskMenu) ask).Label;
                foreach (var i in ((AskMenu) ask).MenuItemsChosen(MenuItem[ask.UniqueId]))
                {
                    om.AddToMemberDataBelowComments(menulabel);
                    var desc = i.amt > 0 
                        ? $"{i.number} {i.desc} (at {i.amt:N2})" 
                        : $"{i.number} {i.desc}";
                    om.AddToMemberDataBelowComments(desc);
                    menulabel = string.Empty;
                }
            }
        }

        private void SaveCheckboxChoices(OrganizationMember om, Ask ask)
        {
            if (setting.TargetExtraValues)
            {
                foreach (var ck in ((AskCheckboxes) ask).list)
                    person.RemoveExtraValue(DbUtil.Db, ck.SmallGroup);
                foreach (var g in ((AskCheckboxes) ask).CheckboxItemsChosen(Checkbox))
                    person.AddEditExtraBool(g.SmallGroup, true);
            }
            else
            {
                foreach (var ck in ((AskCheckboxes) ask).list)
                    ck.RemoveFromSmallGroup(DbUtil.Db, om);
                foreach (var i in ((AskCheckboxes) ask).CheckboxItemsChosen(Checkbox))
                    i.AddToSmallGroup(DbUtil.Db, om, PythonModel);
            }
        }

        private void SaveYesNoChoices(OrganizationMember om, Ask ask)
        {
            if (setting.TargetExtraValues == false)
            {
                foreach (var yn in ((AskYesNoQuestions) ask).list)
                {
                    om.RemoveFromGroup(DbUtil.Db, "Yes:" + yn.SmallGroup);
                    om.RemoveFromGroup(DbUtil.Db, "No:" + yn.SmallGroup);
                }
                foreach (var g in YesNoQuestion)
                    om.AddToGroup(DbUtil.Db, (g.Value == true ? "Yes:" : "No:") + g.Key);
            }
            else
                foreach (var g in YesNoQuestion)
                    person.AddEditExtraCode(g.Key, g.Value == true ? "Yes" : "No");
        }

        private OrganizationMember GetOrganizationMember(Transaction transaction)
        {
            var membertype = setting.AddAsProspect ? MemberTypeCode.Prospect : MemberTypeCode.Member;
            var om = OrganizationMember.InsertOrgMembers(DbUtil.Db, org.OrganizationId, person.PeopleId,
                membertype, DateTime.Now, null, false);
            if (om.TranId == null)
                om.TranId = transaction.OriginalId;
            om.RegisterEmail = EmailAddress;
            om.RegistrationDataId = Parent.DatumId;
            Log("JoinedOrg");
            return om;
        }

        private static OrganizationMember AddSender(OrganizationMember om)
        {
            if (!om.IsInGroup("Goer"))
                om.MemberTypeId = MemberTypeCode.InActive;
            om.AddToGroup(DbUtil.Db, "Sender");
            return om;
        }

        private OrganizationMember RecordAllFamilyAttends(OrganizationMember om)
        {
            om.AddToGroup(DbUtil.Db, "Attending");
            om.AddToGroup(DbUtil.Db, "Registered");
            foreach (var fm in FamilyAttend)
            {
                if (fm.PeopleId == -1)
                    continue;
                Person pp = null;
                OrganizationMember omm = null;
                if (!fm.PeopleId.HasValue && fm.Attend)
                {
                    if (!fm.Name.HasValue())
                        continue;
                    string first, last;
                    Util.NameSplit(fm.Name, out first, out last);
                    if (!first.HasValue())
                    {
                        first = last;
                        last = LastName;
                    }
                    var uperson = DbUtil.Db.LoadPersonById(PeopleId ?? 0);
                    var p = new OnlineRegPersonModel()
                    {
                        FirstName = first,
                        LastName = last,
                        DateOfBirth = fm.Birthday,
                        EmailAddress = fm.Email,
                        gender = fm.GenderId,
                        married = fm.MaritalId
                    };
                    p.AddPerson(uperson, org.EntryPointId ?? 0);
                    pp = p.person;
                    omm = OrganizationMember.InsertOrgMembers(DbUtil.Db, org.OrganizationId, pp.PeopleId,
                        MemberTypeCode.Member, DateTime.Now, null, false);
                }
                else
                {
                    pp = DbUtil.Db.LoadPersonById(fm.PeopleId ?? 0);
                    if (fm.Attend)
                        omm = OrganizationMember.InsertOrgMembers(DbUtil.Db, org.OrganizationId, pp.PeopleId,
                            MemberTypeCode.Member, DateTime.Now, null, false);
                    else
                    {
                        omm = OrganizationMember.Load(DbUtil.Db, pp.PeopleId, org.OrganizationId);
                        omm?.RemoveFromGroup(DbUtil.Db, "Attending");
                    }
                }
                if (omm == null)
                    continue;
                if (fm.Attend)
                    omm.AddToGroup(DbUtil.Db, "Attending");
                if (!fm.PeopleId.HasValue)
                    omm.AddToGroup(DbUtil.Db, "Added");
            }
            return om;
        }

        public string AgeGroup()
        {
            foreach (var i in setting.AgeGroups)
                if (person.Age >= i.StartAge && person.Age <= i.EndAge)
                    return i.SmallGroup;
            return string.Empty;
        }
    }
}
