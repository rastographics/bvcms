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
    public partial class OrganizationMember : ITableWithExtraValues
    {
        private const string STR_MeetingsToUpdate = "MeetingsToUpdate";

        public EnrollmentTransaction Drop(CMSDataContext db, bool skipTriggerProcessing = false)
        {
            return Drop(db, DateTime.Now, skipTriggerProcessing);
        }

        public EnrollmentTransaction Drop(CMSDataContext db, DateTime dropdate, bool skipTriggerProcessing = false)
        {
            return Drop(db, dropdate, Organization.OrganizationName, skipTriggerProcessing);
        }
        public EnrollmentTransaction Drop(CMSDataContext db, DateTime dropdate, string orgname, bool skipTriggerProcessing = false)
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
                    SmallGroups = string.Join("\n", sglist),
                    SkipInsertTriggerProcessing = skipTriggerProcessing
                };

                db.EnrollmentTransactions.InsertOnSubmit(droptrans);
                db.OrgMemberExtras.DeleteAllOnSubmit(this.OrgMemberExtras);
                db.OrgMemMemTags.DeleteAllOnSubmit(this.OrgMemMemTags);
                db.SubmitChanges();
                foreach (var ev in this.OrgMemberExtras)
                {
                    var ev2 = new PrevOrgMemberExtra()
                    {
                        EnrollmentTranId = droptrans.TransactionId,
                        OrganizationId = ev.OrganizationId,
                        PeopleId = ev.PeopleId,
                        Field = ev.Field,
                        StrValue = ev.StrValue,
                        Data = ev.Data,
                        BitValue = ev.BitValue,
                        IntValue = ev.IntValue,
                        DateValue = ev.DateValue,
                    };
                    db.PrevOrgMemberExtras.InsertOnSubmit(ev2);
                    db.SubmitChanges();
                }
                db.OrgMemberExtras.DeleteAllOnSubmit(this.OrgMemberExtras);
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
            db.OrgMemberExtras.DeleteAllOnSubmit(this.OrgMemberExtras);
            db.OrganizationMembers.DeleteOnSubmit(this);
            db.SubmitChanges();
        }

        public static void UpdateMeetingsToUpdate()
        {
            UpdateMeetingsToUpdate(DbUtil.Db);
        }

        public static void UpdateMeetingsToUpdate(CMSDataContext Db)
        {
            var mids = HttpContextFactory.Current.Items[STR_MeetingsToUpdate] as List<int>;
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

        public void MakeLeaderOfGroup(CMSDataContext Db, int sgId)
        {
            var omt = Db.OrgMemMemTags.SingleOrDefault(t =>
                                                      t.PeopleId == PeopleId
                                                      && t.MemberTagId == sgId
                                                      && t.OrgId == OrganizationId);
            if (omt == null)
            {
                Db.OrgMemMemTags.InsertOnSubmit(new OrgMemMemTag
                {
                    PeopleId = PeopleId,
                    OrgId = OrganizationId,
                    MemberTagId = sgId,
                    IsLeader = true
                });

            }
            else
            {
                omt.IsLeader = true;
            }
        }

        public void RemoveAsLeaderOfGroup(CMSDataContext Db, int sgId)
        {
            var omt = Db.OrgMemMemTags.SingleOrDefault(t =>
                                                      t.PeopleId == PeopleId
                                                      && t.MemberTagId == sgId
                                                      && t.OrgId == OrganizationId);
            if (omt != null)
            {
                omt.IsLeader = false;
            }
        }

        [Obsolete]
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

        [Obsolete]
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
                        if (m.MemberTypeId == MemberTypeCode.Member || m.MemberTypeId == 0 || m.MemberTypeId == MemberTypeCode.Prospect)
                        {
                            m.MemberTypeId = memberTypeId;
                        }
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
        public static OrganizationMember InsertOrgMembers(CMSDataContext db, int organizationId, int peopleId, int memberTypeId, DateTime enrollmentDate, DateTime? inactiveDate = null, bool pending = false, bool skipTriggerProcessing = false)
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
        public Transaction AddTransaction(CMSDataContext db, string reason, decimal payment, string description, decimal? amount = null, bool? adjustFee = false, string pmtDescription = null)
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
                Description = Util.PickFirst(pmtDescription, Organization.OrganizationName),
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
        public static decimal AmountDue(CMSDataContext db, int orgid, int pid)
        {
            var om = db.OrganizationMembers.SingleOrDefault(
                    vv => vv.OrganizationId == orgid && vv.PeopleId == pid);
            if (om == null)
                return 0;
            return om.AmountDue(db);
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

//        public Settings RegSetting()
//        {
//            return DbUtil.Db.CreateRegistrationSettings(OrganizationId);
//        }

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
            if (HttpContextFactory.Current.User.IsInRole("Admin") ||
                HttpContextFactory.Current.User.IsInRole("ManageVolunteers"))
                return true;
            var leaderorgs = db.GetLeaderOrgIds(Util.UserPeopleId);
            if (leaderorgs == null)
                return false;
            return leaderorgs.Contains(orgid.Value);
        }

        public IEnumerable<OrgMemberExtra> GetOrgMemberExtras()
        {
            return OrgMemberExtras.OrderBy(pp => pp.Field);
        }
        public OrgMemberExtra GetExtraValue(string field)
        {
            field = field.Trim();
            var ev = OrgMemberExtras.AsEnumerable().FirstOrDefault(ee => ee.Field == field);
            if (ev == null)
            {
                ev = new OrgMemberExtra()
                {
                    OrganizationId = OrganizationId,
                    PeopleId = PeopleId,
                    Field = field,
                };
                OrgMemberExtras.Add(ev);
            }
            return ev;
        }
        public static OrgMemberExtra GetExtraValue(CMSDataContext db, int oid, int pid, string field)
        {
            field = field.Trim();
            var q = from v in db.OrgMemberExtras
                    where v.Field == field
                    where v.OrganizationId == oid
                    where v.PeopleId == pid
                    select v;
            var ev = q.SingleOrDefault();
            if (ev == null)
            {
                ev = new OrgMemberExtra()
                {
                    OrganizationId = oid,
                    PeopleId = pid,
                    Field =  field,
                    TransactionTime = DateTime.Now
                };
                db.OrgMemberExtras.InsertOnSubmit(ev);
            }
            return ev;
        }

        public void AddEditExtra(CMSDataContext db, string field, string value, bool multiline = false)
        {
            var omev = db.OrgMemberExtras.SingleOrDefault(oe => oe.OrganizationId == OrganizationId && oe.PeopleId == PeopleId && oe.Field == field);
            if (omev == null)
            {
                omev = new OrgMemberExtra()
                {
                    OrganizationId = OrganizationId,
                    PeopleId = PeopleId,
                    Field = field,
                };
                db.OrgMemberExtras.InsertOnSubmit(omev);
            }
            omev.Data = value;
            omev.DataType = multiline ? "text" : null;
        }
        public void AddToExtraText(string field, string value)
        {
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.DataType = "text";
            if (ev.Data.HasValue())
                ev.Data = value + "\n" + ev.Data;
            else
                ev.Data = value;
        }

        public string GetExtra(CMSDataContext db, string field)
        {
            var oev = db.OrgMemberExtras.SingleOrDefault(oe => oe.OrganizationId == OrganizationId && oe.PeopleId == PeopleId && oe.Field == field);
            if (oev == null)
                return "";
            if (oev.StrValue.HasValue())
                return oev.StrValue;
            if (oev.Data.HasValue())
                return oev.Data;
            if (oev.DateValue.HasValue)
                return oev.DateValue.FormatDate();
            if (oev.IntValue.HasValue)
                return oev.IntValue.ToString();
            return oev.BitValue.ToString();
        }
        public void AddEditExtraCode(string field, string value, string location = null)
        {
            if (!field.HasValue())
                return;
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.StrValue = value;
            ev.TransactionTime = DateTime.Now;
        }

        public void AddEditExtraText(string field, string value, DateTime? dt = null)
        {
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.Data = value;
            ev.TransactionTime = dt ?? DateTime.Now;
        }

        public void AddEditExtraDate(string field, DateTime? value)
        {
            if (!value.HasValue)
                return;
            var ev = GetExtraValue(field);
            ev.DateValue = value;
            ev.TransactionTime = DateTime.Now;
        }

        public void AddEditExtraInt(string field, int value)
        {
            var ev = GetExtraValue(field);
            ev.IntValue = value;
            ev.TransactionTime = DateTime.Now;
        }

        public void AddEditExtraBool(string field, bool tf, string name = null, string location = null)
        {
            if (!field.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.BitValue = tf;
            ev.TransactionTime = DateTime.Now;
        }

        public void AddEditExtraValue(string field, string code, DateTime? date, string text, bool? bit, int? intn, DateTime? dt = null)
        {
            var ev = GetExtraValue(field);
            ev.StrValue = code;
            ev.Data = text;
            ev.DateValue = date;
            ev.IntValue = intn;
            ev.BitValue = bit;
            ev.UseAllValues = true;
            ev.TransactionTime = dt ?? DateTime.Now;
        }

        public void RemoveExtraValue(CMSDataContext db, string field)
        {
            var ev = OrgMemberExtras.AsEnumerable().FirstOrDefault(ee => string.Compare(ee.Field, field, StringComparison.OrdinalIgnoreCase) == 0);
            if (ev == null)
                return;
            db.OrgMemberExtras.DeleteOnSubmit(ev);
            ev.TransactionTime = DateTime.Now;
        }

        public void LogExtraValue(string op, string field)
        {
            DbUtil.LogActivity($"EVOrgMem {op}:{field}", orgid: OrganizationId, peopleid: PeopleId);
        }
        public static void MoveToOrg(CMSDataContext db, int pid, int fromOrg, int toOrg, bool? moveregdata = true, int toMemberTypeId = -1)
        {
            if (fromOrg == toOrg)
                return;
            var om = db.OrganizationMembers.SingleOrDefault(m => m.PeopleId == pid && m.OrganizationId == fromOrg);
            if (om == null)
                return;
            var tom = db.OrganizationMembers.SingleOrDefault(m => m.PeopleId == pid && m.OrganizationId == toOrg);
            if (tom == null)
            {
                tom = InsertOrgMembers(db,
                    toOrg, pid, MemberTypeCode.Member, DateTime.Now, null, om.Pending ?? false, skipTriggerProcessing:true);
                if (tom == null)
                    return;
            }
            tom.MemberTypeId = toMemberTypeId != -1 
                ? toMemberTypeId 
                : om.MemberTypeId;
            tom.UserData = om.UserData;

            if (om.Pending == true) // search for PromotingTo Extra Value to update
            {
                var fromev = (from vv in db.OrgMemberExtras
                             where vv.PeopleId == om.PeopleId
                             where vv.IntValue == om.OrganizationId
                             where vv.Field == "PromotingTo"
                             select vv).SingleOrDefault();
                if (fromev != null)
                    fromev.IntValue = tom.OrganizationId;
            }
            if (moveregdata == true)
            {
                tom.Request = om.Request;
                tom.Amount = om.Amount;
                tom.OnlineRegData = om.OnlineRegData;
                tom.RegistrationDataId = om.RegistrationDataId;
                tom.Grade = om.Grade;
                tom.RegisterEmail = om.RegisterEmail;
                tom.ShirtSize = om.ShirtSize;
                tom.TranId = om.TranId;
                tom.Tickets = om.Tickets;

                var sg = om.OrgMemMemTags.Select(mt => mt.MemberTag.Name).ToList();
                foreach (var s in sg)
                    tom.AddToGroup(db, s);
            }

            if (om.OrganizationId != tom.OrganizationId)
                tom.Moved = true;
            om.Drop(db, skipTriggerProcessing: true);            
            db.SubmitChanges();
        }
    }
}
