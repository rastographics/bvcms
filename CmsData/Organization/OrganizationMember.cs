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
                                    OrganizationName = Organization.OrganizationName,
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
                db.ExecuteCommand("DELETE FROM dbo.SubRequest WHERE EXISTS(SELECT NULL FROM Attend a WHERE a.AttendId = AttendId AND a.OrganizationId = {0} AND a.MeetingDate > {1} AND a.PeopleId = {2})", OrganizationId, Util.Now, PeopleId);
                db.ExecuteCommand("DELETE dbo.Attend WHERE OrganizationId = {0} AND MeetingDate > {1} AND PeopleId = {2} AND ISNULL(Commitment, 1) = 1", OrganizationId, Util.Now, PeopleId);
                db.ExecuteCommand("UPDATE dbo.GoerSenderAmounts SET InActive = 1 WHERE OrgId = {0} AND (GoerId = {1} OR SupporterId = {1})", OrganizationId, PeopleId);
                return droptrans;
            }
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

        public void AddToGroup(CMSDataContext Db, string name, int? n)
        {
            if (!name.HasValue())
                return;
            var mt = Db.MemberTags.SingleOrDefault(t => t.Name == name.Trim() && t.OrgId == OrganizationId);
            if (mt == null)
            {
                mt = new MemberTag { Name = name.Trim(), OrgId = OrganizationId };
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

        public static OrganizationMember InsertOrgMembers(CMSDataContext db, int organizationId, int peopleId, int memberTypeId, DateTime enrollmentDate, DateTime? inactiveDate, bool pending, bool skipTriggerProcessing = false)
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
                    var org = db.Organizations.SingleOrDefault(oo => oo.OrganizationId == organizationId);
                    if (org == null)
                        return null;
                    var name = org.OrganizationName;

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

        private bool transactionSummaryLoaded;
        private TransactionSummary transactionSummary;
        public TransactionSummary TransactionSummary(CMSDataContext db)
        {
            if(transactionSummaryLoaded)
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
                    TransactionId = "{0} ({1})".Fmt(reason, Util.UserPeopleId ?? Util.UserId1),
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
                    State = Person.PrimaryState,
                    Zip = Person.PrimaryZip,
                    LoginPeopleId = Util.UserPeopleId,
                    Approved = true,
                    Amt = payment,
                    Amtdue = (amount ?? payment) - payment,
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
            //                var settings = new Settings(Organization.RegSetting, db, OrganizationId);
            //            }

            return ti2;
        }

        public decimal AmountDue(CMSDataContext db)
        {
            var ts = TransactionSummary(db);
            if (ts == null)
                return 0;
            return FeePaid(db) - TotalPaid(db);
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
            return ts.IndAmt ?? 0;
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

        public Registration.Settings RegSetting()
        {
            return new Settings(Organization.RegSetting, DbUtil.Db, OrganizationId);
        }

        public string PayLink2(CMSDataContext db)
        {
            if (!TranId.HasValue)
                return null;
            var estr = HttpUtility.UrlEncode(Util.Encrypt(TranId.ToString()));
            return db.ServerLink("/OnlineReg/PayAmtDue?q=" + estr);
        }
    }
}
