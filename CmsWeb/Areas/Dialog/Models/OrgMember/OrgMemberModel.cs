using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CmsData;
using CmsData.Codes;
using CmsData.OnlineRegSummaryText;
using CmsData.Registration;
using CmsData.View;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class OrgMemberModel
    {
        public Organization Organization;
        public List<OrgMemMemTag> OrgMemMemTags;
        public bool IsMissionTrip;
        public TransactionSummary TransactionSummary;
        private int? orgId;
        private int? peopleId;

        private const string AutoOrgLeaderPromotion = "AutoOrgLeaderPromotion";
        private const int DefaultLeaderMemberTypeId = 140;
        private const string AccessRole = "Access";
        private const string OrgLeadersOnlyRole = "OrgLeadersOnly";

        public OrgMemberModel()
        {
        }

        public OrgMemberModel(string group, int oid, int pid)
        {
            OrgId = oid;
            PeopleId = pid;
            Group = group;
        }
        public OrgMemberModel(int oid, int pid) : this("", oid, pid) { }

        public OrganizationMember OrgMember;
        private void Populate()
        {
            if (dopopulate)
            {
                dopopulate = false;
                if (!OrgId.HasValue || !PeopleId.HasValue)
                    return;
            }
            var i = (from mm in DbUtil.Db.OrganizationMembers
                     where mm.OrganizationId == OrgId && mm.PeopleId == PeopleId
                     select new
                     {
                         mm,
                         mm.Person.Name,
                         mm.Organization.OrganizationName,
                         mm.Organization,
                         mm.OrgMemMemTags,
                         mm.Organization.IsMissionTrip,
                         ts = DbUtil.Db.ViewTransactionSummaries.SingleOrDefault(tt => tt.RegId == mm.TranId && tt.PeopleId == PeopleId && tt.OrganizationId == OrgId)
                     }).SingleOrDefault();
            if (i == null)
                throw new Exception($"missing OrgMember at oid={OrgId}, pid={PeopleId}");
            OrgMember = i.mm;
            TransactionSummary = i.ts;
            this.CopyPropertiesFrom(OrgMember);
            Name = i.Name;

            IsMissionTrip = i.IsMissionTrip ?? false;
            AmtFee = i.ts?.IndPaid + i.ts?.IndDue;
            AmtDonation = i.ts?.IndAmt - AmtFee;
            AmtCoupon = i.ts?.TotCoupon;
            AmtPaid = OrgMember.AmountPaidTransactions(DbUtil.Db);
            AmtDue = OrgMember.AmountDueTransactions(DbUtil.Db);

            OrgName = i.OrganizationName;
            Organization = i.Organization;
            OrgMemMemTags = i.OrgMemMemTags.ToList();
            Setting = DbUtil.Db.CreateRegistrationSettings(OrgId ?? 0);
        }


        public string Group { get; set; }
        public string GroupName
        {
            get
            {
                switch (Group)
                {
                    case GroupSelectCode.Member:
                        return "Member";
                    case GroupSelectCode.Pending:
                        return "Pending";
                    case GroupSelectCode.Prospect:
                        return "Prospect";
                    case GroupSelectCode.Inactive:
                        return "Inactive";
                    case GroupSelectCode.Previous:
                        return "Previous";
                }
                return null;
            }
        }

        private bool dopopulate = false;
        public int? OrgId
        {
            get { return orgId; }
            set
            {
                if (orgId != value)
                    dopopulate = true;
                orgId = value;
                Populate();
            }
        }

        [SkipFieldOnCopyProperties]
        public int? PeopleId
        {
            get { return peopleId; }
            set
            {
                if (peopleId != value)
                    dopopulate = true;
                peopleId = value;
                Populate();
            }
        }

        public string Name { get; set; }
        public string OrgName { get; set; }

        public string AttendStr { get; set; }

        public Settings Setting { get; set; }

        [TrackChanges]
        public CodeInfo MemberType { get; set; }

        [TrackChanges]
        public DateTime? InactiveDate { get; set; }

        [TrackChanges]
        //        [RegularExpression(@"\d{1,2}/\d{1,2}/(\d\d){1,2}( \d{1,2}:\d\d [AP]M){0,1}")]
        [DisplayName("Enrollment Date")]
        public DateTime? EnrollmentDate { get; set; }

        [TrackChanges]
        public bool Pending { get; set; }

        [TrackChanges]
        public string RegisterEmail { get; set; }

        [TrackChanges]
        public string Request { get; set; }

        [TrackChanges]
        public int? Grade { get; set; }

        [TrackChanges]
        public int? TranId { get; set; }

        [TrackChanges]
        public int? Tickets { get; set; }

        [TrackChanges, StringLength(50)]
        public string ShirtSize { get; set; }

        [TrackChanges]
        [DisplayName("Extra Member Info")]
        public string UserData { get; set; }

        [TrackChanges]
        public DateTime? DropDate { get; set; }


//        [DisplayName("Total Amount")]
//        public decimal? Amount { get; set; }

        [DisplayName("Fee")]
        public decimal? AmtFee { get; set; }

        [DisplayName("Paid")]
        public decimal? AmtPaid { get; set; }

        [DisplayName("Due")]
        public decimal? AmtDue { get; set; }

        [DisplayName("Donation")]
        public decimal? AmtDonation { get; set; }

        [DisplayName("Coupon")]
        public decimal? AmtCoupon { get; set; }

        private string transactionsLink;
        public string TransactionsLink
        {
            get
            {
                if (transactionsLink.HasValue())
                    return transactionsLink;

                if (OrgMember == null)
                    OrgMember = DbUtil.Db.OrganizationMembers.Single(mm => mm.OrganizationId == OrgId && mm.PeopleId == PeopleId);

                if (!IsMissionTrip)
                    return transactionsLink = OrgMember.TranId.HasValue ? $"/Transactions/{OrgMember.TranId}" : null;

                if (OrgMember.IsInGroup("Goer") && OrgMember.IsInGroup("Sender"))
                    return transactionsLink = OrgMember.TranId.HasValue ? $"/Transactions/{OrgMember.TranId}?goerid={OrgMember.PeopleId}&senderid={OrgMember.PeopleId}" : null;

                if (OrgMember.IsInGroup("Goer"))
                    return transactionsLink = OrgMember.TranId.HasValue ? $"/Transactions/{OrgMember.TranId}?goerid={OrgMember.PeopleId}"
                        : null;

                if (OrgMember.IsInGroup("Sender"))
                    return transactionsLink = $"/Transactions/{0}?senderid={OrgMember.PeopleId}";

                return transactionsLink = OrgMember.TranId.HasValue ? $"/Transactions/{OrgMember.TranId}" : null;
            }
        }

        public void UpdateModel()
        {
            if (OrgMember == null)
                OrgMember = DbUtil.Db.OrganizationMembers.Single(mm => mm.OrganizationId == OrgId && mm.PeopleId == PeopleId);

            var leaderTypeId = OrgMember.Organization.LeaderMemberTypeId > 0 
                ? OrgMember.Organization.LeaderMemberTypeId : DefaultLeaderMemberTypeId;
            
            if (OrgMember.MemberTypeId == leaderTypeId && MemberType.Name != leaderTypeId.ToString())
                CheckForAutoDemotion();
            if(MemberType.Value == leaderTypeId.ToString() && OrgMember.MemberTypeId != leaderTypeId)
                CheckForAutoPrOrgMemberotion();

            var changes = this.CopyPropertiesTo(OrgMember);


            DbUtil.Db.SubmitChanges();
            foreach (var g in changes)
                DbUtil.LogActivity($"OrgMem {GroupName} change {g.Field}", OrgId, PeopleId);
            Populate();
        }

        private string payLink;
        public string PayLink
        {
            get
            {
                if (payLink.HasValue())
                    return payLink;
                return payLink = GetPayLink(OrgId, PeopleId);
            }
        }
        public static string GetPayLink(int? oid, int? pid)
        {
            var om = DbUtil.Db.OrganizationMembers.Single(mm => mm.OrganizationId == oid && mm.PeopleId == pid);
            return om.PayLink2(DbUtil.Db);
        }
        private string supportLink;
        public string SupportLink
        {
            get
            {
                if (supportLink.HasValue())
                    return supportLink;
                if (OrgMember == null)
                    OrgMember = DbUtil.Db.OrganizationMembers.Single(mm => mm.OrganizationId == OrgId && mm.PeopleId == PeopleId);
                if (IsMissionTrip && OrgMember.MemberTypeId == MemberTypeCode.Member)
                    return supportLink = DbUtil.Db.ServerLink($"/OnlineReg/{OrgId}?goerid={PeopleId}");
                return null;
            }
        }

        [Display(Description = @"
Checking the Remove From Enrollment History box will erase all enrollment history (but not attendance) of this person in this organization.
**There is no undo.**")]
        public bool RemoveFromEnrollmentHistory { get; set; }

        public void Drop()
        {
            if (OrgMember == null)
                OrgMember = DbUtil.Db.OrganizationMembers.Single(mm => mm.OrganizationId == OrgId && mm.PeopleId == PeopleId);
            if (DropDate.HasValue)
                OrgMember.Drop(DbUtil.Db, DropDate.Value);
            else
                OrgMember.Drop(DbUtil.Db);
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("OrgMem Drop", OrgId, PeopleId);
            if (RemoveFromEnrollmentHistory)
            {
                DbUtil.DbDispose();
                DbUtil.Db = DbUtil.Create(Util.Host);
                var q = DbUtil.Db.EnrollmentTransactions.Where(tt => tt.OrganizationId == OrgId && tt.PeopleId == PeopleId);
                DbUtil.Db.EnrollmentTransactions.DeleteAllOnSubmit(q);
                DbUtil.Db.SubmitChanges();
                DbUtil.LogActivity("OrgMem RemoveEnrollmentHistory", OrgId, PeopleId);
            }
        }
        public string SmallGroupChanged(int sgtagid, bool ck)
        {
            if (OrgMember == null)
                OrgMember = DbUtil.Db.OrganizationMembers.Single(mm => mm.OrganizationId == OrgId && mm.PeopleId == PeopleId);

            if (OrgMember == null)
                return "error";
            if (ck)
            {
                var name = (from mt in DbUtil.Db.MemberTags
                            where mt.Id == sgtagid
                            where mt.OrgId == OrgId
                            select mt.Name).Single();
                OrgMember.OrgMemMemTags.Add(new OrgMemMemTag { MemberTagId = sgtagid });
                DbUtil.LogActivity("OrgMem AddSubGroup " + name, OrgId, PeopleId);
            }
            else
            {
                var i = (from mt in OrgMember.OrgMemMemTags
                         where mt.MemberTagId == sgtagid
                         select new
                         {
                             mt,
                             name = mt.MemberTag.Name
                         }).SingleOrDefault();
                if (i == null)
                    return "not found";
                DbUtil.Db.OrgMemMemTags.DeleteOnSubmit(i.mt);
                DbUtil.LogActivity("OrgMem RemoveSubGroup " + i.name, OrgId, PeopleId);
            }
            DbUtil.Db.SubmitChanges();
            return "ok";
        }

        public IEnumerable<OrgMemberQuestion> RegQuestions()
        {
            var q = DbUtil.Db.OrgMemberQuestions(OrgId, PeopleId);
            return q;
        }

        public void AddQuestions()
        {
            if (OrgId == null)
                return;
            var r = OnlineRegPersonModel0.CreateFromSettings(DbUtil.Db, OrgId.Value);
            OrgMember.OnlineRegData = r.WriteXml();
            DbUtil.Db.SubmitChanges();
        }

        public void UpdateQuestion(int n, string answer)
        {
            var q = DbUtil.Db.OrgMemberQuestions(OrgId, PeopleId).ToList();
            var rq = q.SingleOrDefault(vv => vv.Row == n);
            if (rq == null)
                throw new Exception("question not found");

            var question = rq.Question;

            var r = new OnlineRegPersonModel0(OrgMember.OnlineRegData);
            r.ExtraQuestion[rq.SetX ?? 0][question] = answer;
            OrgMember.OnlineRegData = r.WriteXml();
            DbUtil.Db.SubmitChanges();
        }

        private void CheckForAutoPromotion()
        {
            var autoLeaderOrgPromotionSetting = DbUtil.Db.Setting(AutoOrgLeaderPromotion, "false").ToLower() == "true";
            if (!autoLeaderOrgPromotionSetting) return;

            var users = DbUtil.Db.Users.Where(us => us.PeopleId == PeopleId);
            if (users.Any())
            {
                foreach (var user in users)
                {
                    if (!user.Roles.Any())
                    {
                        user.AddRoles(DbUtil.Db, !user.InRole(AccessRole) ? new[] { AccessRole, OrgLeadersOnlyRole } : new[] { OrgLeadersOnlyRole });
                    }
                }
            }
        }

        private void CheckForAutoDemotion()
        {
            var autoLeaderOrgPromotionSetting = DbUtil.Db.Setting(AutoOrgLeaderPromotion, "false").ToLower() == "true";
            if (!autoLeaderOrgPromotionSetting) return;

            var users = DbUtil.Db.Users.Where(us => us.PeopleId == PeopleId);
            if (users.Any())
            {
                foreach (var user in users)
                {
                    if (user.Roles.Count() == 2 && user.InRole(OrgLeadersOnlyRole) && user.InRole(AccessRole) &&
                            !DbUtil.Db.OrganizationMembers.Any(x => x.MemberType.Id == (om.Organization.LeaderMemberTypeId > 0 ? om.Organization.LeaderMemberTypeId : DefaultLeaderMemberTypeId)
                                && x.PeopleId == PeopleId && x.OrganizationId != Organization.OrganizationId))
                    {
                        // Resets their roles back to a "MyData" user
                        user.SetRoles(DbUtil.Db, new string[] { });
                    }
                }
            }
        }
    }
}
