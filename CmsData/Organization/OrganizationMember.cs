/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */
using System;
using System.Collections.Generic;
using System.Linq;
using CmsData.Registration;
using UtilityExtensions;
using System.Web;
using System.Data.SqlClient;
using CmsData.Codes;
using CmsData.View;

namespace CmsData
{
    public partial class OrganizationMember
    {
        private const string STR_MeetingsToUpdate = "MeetingsToUpdate";

        public EnrollmentTransaction Drop(CMSDataContext db)
        {
            return Drop(db, DateTime.Now);
        }

        public EnrollmentTransaction Drop(CMSDataContext db, DateTime dropdate)
        {
            return Drop(db, dropdate, Organization.OrganizationName);
        }
        public EnrollmentTransaction Drop(CMSDataContext db, DateTime dropdate, string orgname)
        {
            db.SubmitChanges();
            while (true)
            {
                if (!EnrollmentDate.HasValue)
                    EnrollmentDate = CreatedDate;
                var sglist = (from mt in db.OrgMemMemTags
                              where mt.PeopleId == PeopleId
                              where mt.OrgId == OrganizationId
                              select mt.MemberTag.Name
                    ).ToList();
                var droptrans = new EnrollmentTransaction
                {
                    OrganizationId = OrganizationId,
                    PeopleId = PeopleId,
                    MemberTypeId = MemberTypeId,
                    OrganizationName = orgname,
                    TransactionDate = dropdate,
                    TransactionTypeId = 5, // drop
                    CreatedBy = Util.UserId1,
                    CreatedDate = Util.Now,
                    Pending = Pending,
                    AttendancePercentage = AttendPct,
                    InactiveDate = InactiveDate,
                    UserData = UserData,
                    Request = Request,
                    ShirtSize = ShirtSize,
                    Grade = Grade,
                    Tickets = Tickets,
                    RegisterEmail = RegisterEmail,
                    Score = Score,
                    SmallGroups = string.Join("\n", sglist)
                };

                db.EnrollmentTransactions.InsertOnSubmit(droptrans);
                db.OrgMemMemTags.DeleteAllOnSubmit(this.OrgMemMemTags);
                db.OrganizationMembers.DeleteOnSubmit(this);
                db.ExecuteCommand(@"
DELETE dbo.SubRequest
FROM dbo.SubRequest sr
JOIN dbo.Attend a ON a.AttendId = sr.AttendId
WHERE a.OrganizationId = {0}
AND a.MeetingDate > {1}
AND a.PeopleId = {2}
", OrganizationId, Util.Now, PeopleId);
                db.ExecuteCommand("DELETE dbo.Attend WHERE OrganizationId = {0} AND MeetingDate > {1} AND PeopleId = {2} AND ISNULL(Commitment, 1) = 1", OrganizationId, Util.Now, PeopleId);
                db.ExecuteCommand("DELETE dbo.Attend WHERE OrganizationId = {0} AND DATEADD(DAY, DATEDIFF(DAY, 0, MeetingDate), 0) = DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), 0) AND PeopleId = {1} AND AttendanceFlag = 0", OrganizationId, PeopleId);
                db.ExecuteCommand("UPDATE dbo.GoerSenderAmounts SET InActive = 1 WHERE OrgId = {0} AND (GoerId = {1} OR SupporterId = {1})", OrganizationId, PeopleId);
                return droptrans;
            }
        }
        public void FastDrop(CMSDataContext db, DateTime dropdate, string orgname)
        {
            if (!EnrollmentDate.HasValue)
                EnrollmentDate = CreatedDate;
            var droptrans = new EnrollmentTransaction
            {
                OrganizationId = OrganizationId,
                PeopleId = PeopleId,
                MemberTypeId = MemberTypeId,
                OrganizationName = orgname,
                TransactionDate = dropdate,
                TransactionTypeId = 5, // drop
                CreatedBy = Util.UserId1,
                CreatedDate = Util.Now,
                Pending = Pending,
                AttendancePercentage = AttendPct,
                InactiveDate = InactiveDate,
                UserData = UserData,
                Request = Request,
                ShirtSize = ShirtSize,
                Grade = Grade,
                Tickets = Tickets,
                RegisterEmail = RegisterEmail,
                Score = Score,
            };

            db.EnrollmentTransactions.InsertOnSubmit(droptrans);
            db.OrgMemMemTags.DeleteAllOnSubmit(this.OrgMemMemTags);
            db.OrganizationMembers.DeleteOnSubmit(this);
            db.SubmitChanges();
        }

        public static void UpdateMeetingsToUpdate()
        {
            UpdateMeetingsToUpdate(DbUtil.Db);
        }

        public static void UpdateMeetingsToUpdate(CMSDataContext Db)
        {
            var mids = HttpContext.Current.Items[STR_MeetingsToUpdate] as List<int>;
            if (mids != null)
                foreach (var mid in mids)
                    Db.UpdateMeetingCounters(mid);
        }

        public static OrganizationMember Load(CMSDataContext Db, int PeopleId, string OrgName)
        {
            var q = from om in Db.OrganizationMembers
                    where om.PeopleId == PeopleId
                    where om.Organization.OrganizationName == OrgName
                    select om;
            return q.SingleOrDefault();
        }
        public static OrganizationMember Load(CMSDataContext Db, int PeopleId, int OrgId)
        {
            var q = from om in Db.OrganizationMembers
                    where om.PeopleId == PeopleId
                    where om.Organization.OrganizationId == OrgId
                    select om;
            return q.SingleOrDefault();
        }

        public bool ToggleGroup(CMSDataContext Db, int groupid)
        {
            var group = OrgMemMemTags.SingleOrDefault(g =>
                                                      g.OrgId == OrganizationId && g.PeopleId == PeopleId &&
                                                      g.MemberTagId == groupid);
            if (group == null)
            {
                OrgMemMemTags.Add(new OrgMemMemTag { MemberTagId = groupid });
                return true;
            }
            OrgMemMemTags.Remove(group);
            Db.OrgMemMemTags.DeleteOnSubmit(group);
            return false;
        }
        public bool IsInGroup(string name)
        {
            var mt = Organization.MemberTags.SingleOrDefault(t => t.Name == name);
            if (mt == null)
                return false;
            var omt = OrgMemMemTags.SingleOrDefault(t => t.MemberTagId == mt.Id);
            return omt != null;
        }

        public void AddToGroup(CMSDataContext Db, string name)
        {
            int? n = null;
            AddToGroup(Db, name, n);
        }
        public int AddToGroup(CMSDataContext Db, int n)
        {
            var omt = Db.OrgMemMemTags.SingleOrDefault(t =>
                                                       t.PeopleId == PeopleId
                                                       && t.MemberTagId == n
                                                       && t.OrgId == OrganizationId);
            if (omt == null)
            {
                Db.OrgMemMemTags.InsertOnSubmit(new OrgMemMemTag
                {
                    PeopleId = PeopleId,
                    OrgId = OrganizationId,
                    MemberTagId = n
                });
                Db.SubmitChanges();
                return 1;
            }
            return 0;
        }

        public void AddToGroup(CMSDataContext Db, string name, int? n)
        {
            if (!name.HasValue())
                return;
            var name2 = name.Trim().Truncate(200);
            var mt = Db.MemberTags.SingleOrDefault(t => t.Name == name2 && t.OrgId == OrganizationId);
            if (mt == null)
            {
                mt = new MemberTag { Name = name2, OrgId = OrganizationId };
                Db.MemberTags.InsertOnSubmit(mt);
                Db.SubmitChanges();
            }
            var omt = Db.OrgMemMemTags.SingleOrDefault(t =>
                                                       t.PeopleId == PeopleId
                                                       && t.MemberTagId == mt.Id
                                                       && t.OrgId == OrganizationId);
            if (omt == null)
                mt.OrgMemMemTags.Add(new OrgMemMemTag
                {
                    PeopleId = PeopleId,
                    OrgId = OrganizationId,
                    Number = n
                });
            Db.SubmitChanges();
        }

        public void RemoveFromGroup(CMSDataContext Db, string name)
        {
            var mt = Db.MemberTags.SingleOrDefault(t => t.Name == name && t.OrgId == OrganizationId);
            if (mt == null)
                return;
            var omt =
                Db.OrgMemMemTags.SingleOrDefault(t => t.PeopleId == PeopleId && t.MemberTagId == mt.Id && t.OrgId == OrganizationId);
            if (omt != null)
            {
                OrgMemMemTags.Remove(omt);
                Db.OrgMemMemTags.DeleteOnSubmit(omt);
                Db.SubmitChanges();
            }
        }

        public void AddToMemberData(string s)
        {
            if (UserData.HasValue())
                UserData += "\n";
            UserData += s;
        }
        public void AddToMemberDataBelowComments(string s)
        {
            if (UserData.HasValue())
                UserData += "\n";
            else
                UserData = "--Add comments above this line--\n";
            UserData += s;
        }

        public static OrganizationMember InsertOrgMembers(CMSDataContext db, int organizationId, int peopleId, int memberTypeId, DateTime enrollmentDate, DateTime? inactiveDate, bool pending, string name, bool skipTriggerProcessing = false)
        {
            db.SubmitChanges();
            var ntries = 2;
            while (true)
            {
                try
                {
                    var m = db.OrganizationMembers.SingleOrDefault(m2 => m2.PeopleId == peopleId && m2.OrganizationId == organizationId);
                    if (m != null)
                    {
                        m.Pending = pending;
                        m.MemberTypeId = memberTypeId;
                        db.SubmitChanges();
                        return m;
                    }
                    var om = new OrganizationMember
                    {
                        OrganizationId = organizationId,
                        PeopleId = peopleId,
                        MemberTypeId = memberTypeId,
                        EnrollmentDate = enrollmentDate,
                        InactiveDate = inactiveDate,
                        CreatedDate = Util.Now,
                        Pending = pending,
                        SkipInsertTriggerProcessing = skipTriggerProcessing
                    };

                    var et = new EnrollmentTransaction
                    {
                        OrganizationId = om.OrganizationId,
                        PeopleId = om.PeopleId,
                        MemberTypeId = om.MemberTypeId,
                        OrganizationName = name,
                        TransactionDate = enrollmentDate,
                        EnrollmentDate = enrollmentDate,
                        TransactionTypeId = 1,
                        // join
                        CreatedBy = Util.UserId1,
                        CreatedDate = Util.Now,
                        Pending = pending,
                        AttendancePercentage = om.AttendPct,
                        SkipInsertTriggerProcessing = skipTriggerProcessing
                    };

                    db.OrganizationMembers.InsertOnSubmit(om);
                    db.EnrollmentTransactions.InsertOnSubmit(et);

                    db.SubmitChanges();
                    return om;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205)
                        if (--ntries > 0)
                        {
                            System.Threading.Thread.Sleep(500);
                            continue;
                        }
                    throw;
                }
            }
        }
        public static OrganizationMember InsertOrgMembers(CMSDataContext db, int organizationId, int peopleId, int memberTypeId, DateTime enrollmentDate, DateTime? inactiveDate, bool pending, bool skipTriggerProcessing = false)
        {
            var org = db.LoadOrganizationById(organizationId);
            return InsertOrgMembers(db, organizationId, peopleId, memberTypeId, enrollmentDate, inactiveDate, pending, org.OrganizationName, skipTriggerProcessing);
        }
        public static OrganizationMember AddOrgMember(CMSDataContext db, int organizationId, int peopleId, int memberTypeId, DateTime enrollmentDate, string name)
        {
            var om = new OrganizationMember
            {
                OrganizationId = organizationId,
                PeopleId = peopleId,
                MemberTypeId = memberTypeId,
                EnrollmentDate = enrollmentDate,
                CreatedDate = Util.Now,
                SkipInsertTriggerProcessing = true
            };

            var et = new EnrollmentTransaction
            {
                OrganizationId = om.OrganizationId,
                PeopleId = om.PeopleId,
                MemberTypeId = om.MemberTypeId,
                OrganizationName = name,
                TransactionDate = enrollmentDate,
                EnrollmentDate = enrollmentDate,
                TransactionTypeId = 1,
                // join
                CreatedBy = Util.UserId1,
                CreatedDate = Util.Now,
                AttendancePercentage = om.AttendPct,
                SkipInsertTriggerProcessing = true
            };

            db.OrganizationMembers.InsertOnSubmit(om);
            db.EnrollmentTransactions.InsertOnSubmit(et);

            db.SubmitChanges();
            return om;
        }


        private bool transactionSummaryLoaded;
        private TransactionSummary transactionSummary;
        public TransactionSummary TransactionSummary(CMSDataContext db)
        {
            if (transactionSummaryLoaded)
                return transactionSummary;
            transactionSummary = db.ViewTransactionSummaries.SingleOrDefault(tt => tt.RegId == TranId && tt.PeopleId == PeopleId);
            transactionSummaryLoaded = true;
            return transactionSummary;
        }
        public Transaction AddTransaction(CMSDataContext db, string reason, decimal payment, string description, decimal? amount = null, bool? adjustFee = false)
        {
            var ts = TransactionSummary(db);
            var ti = db.Transactions.SingleOrDefault(tt => tt.Id == TranId);
            if (ti == null)
            {
                ti = (from t in db.Transactions
                      where t.OriginalTransaction.TransactionPeople.Any(pp => pp.PeopleId == PeopleId)
                      where t.OriginalTransaction.OrgId == OrganizationId
                      orderby t.Id descending
                      select t).FirstOrDefault();
                if (ti != null)
                    TranId = ti.Id;
            }

            var ti2 = new Transaction
            {
                TransactionId = $"{reason} ({Util.UserPeopleId ?? Util.UserId1})",
                Description = Organization.OrganizationName,
                TransactionDate = DateTime.Now,
                OrgId = OrganizationId,
                Name = Person.Name,
                First = Person.PreferredName,
                Last = Person.LastName,
                MiddleInitial = Person.MiddleName.Truncate(1),
                Suffix = Person.SuffixCode,
                Address = Person.PrimaryAddress,
                City = Person.PrimaryCity,
                Emails = Person.EmailAddress,
                State = Person.PrimaryState,
                Zip = Person.PrimaryZip,
                LoginPeopleId = Util.UserPeopleId,
                Approved = true,
                Amt = payment,
                //Amtdue = (amount ?? payment) - payment,
                Amtdue = (amount ?? 0) - payment,
                AdjustFee = adjustFee,
                Message = description,
            };

            db.Transactions.InsertOnSubmit(ti2);
            db.SubmitChanges();
            if (ts == null)
            {
                TranId = ti2.Id;
                ti2.TransactionPeople.Add(new TransactionPerson { PeopleId = PeopleId, OrgId = OrganizationId, Amt = amount ?? payment });
                if (ti != null)
                    ti.OriginalId = ti.Id;
            }
            ti2.OriginalId = TranId;
            db.SubmitChanges();

            //            if (Organization.IsMissionTrip == true)
            //            {
            //                var settings = Settings.CreateSettings(Organization.RegSetting, db, OrganizationId);
            //            }

            return ti2;
        }

        public decimal AmountDue(CMSDataContext db)
        {
            var ts = TransactionSummary(db);
            if (ts == null)
                return 0;
            return (ts.IndAmt ?? 0) - TotalPaid(db);
        }

        public decimal? AmountPaidTransactions(CMSDataContext db)
        {
            var ts = TransactionSummary(db);
            if (ts == null)
                return null;
            return Organization.IsMissionTrip == true
                ? TotalPaid(db)
                : ts.IndPaid;
        }
        public decimal? AmountDueTransactions(CMSDataContext db)
        {
            var ts = TransactionSummary(db);
            if (ts == null)
                return null;
            return Organization.IsMissionTrip == true 
                ? AmountDue(db)
                : ts.IndDue;
        }
        private decimal? totalPaid;
        public decimal TotalPaid(CMSDataContext db)
        {
            if (totalPaid.HasValue)
                return totalPaid.Value;
            totalPaid = db.TotalPaid(OrganizationId, PeopleId);
            return totalPaid ?? 0;
        }
        public decimal FeePaid(CMSDataContext db)
        {
            var ts = TransactionSummary(db);
            if (ts == null)
                return 0;
            return ts.IndPaid ?? 0;
        }

        public class SmallGroupItem
        {
            public MemberTag mt { get; set; }
            public int cnt { get; set; }
        }
        public IEnumerable<SmallGroupItem> SmallGroupList()
        {
            var sglist = (from mt in Organization.MemberTags
                          let cnt = mt.OrgMemMemTags.Count()
                          orderby mt.Name
                          select new SmallGroupItem() { mt = mt, cnt = cnt }).ToList();
            return sglist;
        }

        public bool HasSmallGroup(int id)
        {
            return OrgMemMemTags.Any(omt => omt.MemberTagId == id);
        }

        public Settings RegSetting()
        {
            return DbUtil.Db.CreateRegistrationSettings(OrganizationId);
        }

        public string PayLink2(CMSDataContext db)
        {
            if (!TranId.HasValue)
                return null;
            var estr = HttpUtility.UrlEncode(Util.Encrypt(TranId.ToString()));
            return db.ServerLink("/OnlineReg/PayAmtDue?q=" + estr);
        }
        public static bool VolunteerLeaderInOrg(CMSDataContext db, int? orgid)
        {
            if (orgid == null)
                return false;
            var o = db.LoadOrganizationById(orgid);
            if (o == null || o.RegistrationTypeId != RegistrationTypeCode.ChooseVolunteerTimes)
                return false;
            if (HttpContext.Current.User.IsInRole("Admin") ||
                HttpContext.Current.User.IsInRole("ManageVolunteers"))
                return true;
            var leaderorgs = db.GetLeaderOrgIds(Util.UserPeopleId);
            if (leaderorgs == null)
                return false;
            return leaderorgs.Contains(orgid.Value);
        }
    }
}
