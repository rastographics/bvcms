/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Data.Common;
using System.Linq;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using UtilityExtensions;
using System.Web;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Reflection;
using System.Collections.Generic;
using CmsData.Codes;
using System.Text.RegularExpressions;

namespace CmsData
{
    public partial class CMSDataContext
    {
        const string STR_System = "System";

        private int nextTagId = 10;

        public int NextTagId
        {
            get { return nextTagId++; }
        }

        partial void OnCreated()
        {
#if DEBUG
            CommandTimeout = 60;
#else
            CommandTimeout = 1200;
#endif
            Host = Util.Host;
        }
        private string _LogFile;
        public string LogFile
        {
            get
            {
                if (_LogFile == null)
                    _LogFile = Setting("LinqLogFile", null);
                return _LogFile;
            }
        }
        public override void SubmitChanges(System.Data.Linq.ConflictMode failureMode)
        {
            if (this.ObjectTrackingEnabled == true)
            {
                ChangeSet cs = this.GetChangeSet();
                var typesToCheck = new Type[] { typeof(string), typeof(System.Data.Linq.Binary) };//, typeof(DateTime) };
                var insertsUpdates = (
                    from i in cs.Inserts.Union(cs.Updates)
                    join m in this.Mapping.GetTables() on i.GetType() equals m.RowType.Type
                    select new
                    {
                        Entity = i,
                        Members = m.RowType.DataMembers.Where(dm => typesToCheck.Contains(dm.Type)).ToList()
                    }).Where(m => m.Members.Any()).ToList();
                foreach (var ins in insertsUpdates)
                    foreach (var mm in ins.Members)
                    {
                        //if (mm.DbType == "datetime NOT NULL")
                        //{
                        //    var dt = mm.MemberAccessor.GetBoxedValue(ins.Entity).ToDate();
                        //    if (dt < DateTime.Parse("1/1/1753"))
                        //    {
                        //        var iex = new InvalidOperationException(mm.Name + " in " + mm.DeclaringType.Name + " has a value that will not fit into " + mm.DbType + " " + dt.FormatDate());
                        //        throw iex;
                        //    }
                        //}
                        //else
                        {
                            var maxLength = GetMaxLength(mm.DbType);
                            if (mm.MemberAccessor.HasValue(ins.Entity))
                            {
                                var memberValueLength = GetMemberValueLength(mm.MemberAccessor.GetBoxedValue(ins.Entity));
                                if (maxLength > 0 && memberValueLength > maxLength)
                                {
                                    var iex = new InvalidOperationException(mm.Name + " in " + mm.DeclaringType.Name + " has a value that will not fit into " + mm.DbType);
                                    throw iex;
                                }
                            }
                        }
                    }
                base.SubmitChanges(failureMode);
            }
            else
                base.SubmitChanges(failureMode);
        }
        public void ClearCache2()
        {
            const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var method = this.GetType().GetMethod("ClearCache", Flags);
            method.Invoke(this, null);
        }

        private int GetMaxLength(string dbType)
        {
            int maxLength = 0;

            if (dbType == null)
                return maxLength;
            if (dbType.Contains("("))
                dbType = dbType.Substring(dbType.IndexOf("(") + 1);
            if (dbType.Contains(")"))
                dbType = dbType.Substring(0, dbType.IndexOf(")"));
            int.TryParse(dbType, out maxLength);
            return maxLength;
        }

        private int GetMemberValueLength(object value)
        {
            if (value == null)
                return 0;
            if (value.GetType() == typeof(string))
                return ((string)value).Length;
            else if (value.GetType() == typeof(Binary))
                return ((Binary)value).Length;
            else
                throw new ArgumentException("Unknown type.");
        }
        public Person LoadPersonById(int id)
        {
            return this.People.FirstOrDefault(p => p.PeopleId == id);
        }
        public Family LoadFamilyByPersonId(int id)
        {
            return this.Families.SingleOrDefault(ff => ff.People.Any(mm => mm.PeopleId == id));
        }
        public Organization LoadOrganizationById(int? id)
        {
            return this.Organizations.FirstOrDefault(o => o.OrganizationId == id);
        }
        public Organization LoadOrganizationByName(string name)
        {
            return this.Organizations.FirstOrDefault(o => o.OrganizationName == name);
        }
        public string FetchExtra(int pid, string field)
        {
            return this.PeopleExtras.OrderByDescending(e => e.TransactionTime)
                .First(e => e.Field == field && e.PeopleId == pid).StrValue;
        }
        public DateTime? FetchExtraDate(int pid, string field)
        {
            return this.PeopleExtras.OrderByDescending(e => e.TransactionTime)
                .First(e => e.Field == field && e.PeopleId == pid).DateValue;
        }
        private QueryBuilderClause CheckBadQuery(QueryBuilderClause qb)
        {
            if (qb != null && qb.Field == null) // bad query
            {
                DeleteQueryBuilderClauseOnSubmit(qb);
                SubmitChanges();
                return null;
            }
            return qb;
        }
        public QueryBuilderClause LoadQueryById(int? queryid)
        {
            var qb = QueryBuilderClauses.SingleOrDefault(c => c.QueryId == queryid);
            return CheckBadQuery(qb);
        }
        public IQueryable<Person> PeopleQuery(object id)
        {
            if (id == null)
                return null;
            IQueryable<Person> q = null;
            if (id is Guid)
            {
                var qid = (Guid)id;
                var query = LoadQueryById2(qid);
                if (query == null)
                    return null;
                var c = query.ToClause();
                q = People.Where(c.Predicate(this));
                if (c.ParentsOf)
                    q = PersonQueryParents(q);
            }
            else
            {
                var qid = (int)id;
                var qB = LoadQueryById(qid);
                if (qB == null)
                    return null;
                q = People.Where(qB.Predicate(this));
                if (qB.ParentsOf)
                    q = PersonQueryParents(q);
            }
            return q;
        }

        public IQueryable<Person> PeopleQuery(string name)
        {
            var qB = this.QueryBuilderClauses.FirstOrDefault(c => c.Description == name);
            if (qB == null)
                return null;
            var q = People.Where(qB.Predicate(this));
            if (qB.ParentsOf)
                q = PersonQueryParents(q);
            return q;
        }
        public IQueryable<Person> PersonQueryParents(IQueryable<Person> q)
        {
            var q2 = from p in q
                     from m in p.Family.People
                     where m.PositionInFamilyId == 10
                     //					 where (m.PositionInFamilyId == 10 && p.PositionInFamilyId != 10)
                     //					 || (m.PeopleId == p.PeopleId && p.PositionInFamilyId == 10)
                     where m.DeceasedDate == null
                     select m.PeopleId;
            var tag = PopulateTemporaryTag(q2.Distinct());
            var q3 = from p in q
                     let ev = p.PeopleExtras.SingleOrDefault(ee => ee.Field == "Parent" && ee.IntValue > 0)
                     where ev != null
                     where !q2.Any(pp => pp == ev.IntValue)
                     select ev.IntValue;

            foreach (var i in q3.Distinct())
                tag.PersonTags.Add(new TagPerson { PeopleId = i.Value });
            SubmitChanges();
            return tag.People(this);
        }
        public QueryBuilderClause QueryBuilderScratchPad()
        {
            var qb = LoadQueryById(Util.QueryBuilderScratchPadId);
            if (qb == null)
            {
                qb = QueryBuilderClauses.FirstOrDefault(c => c.SavedBy == Util.UserName
                    && c.Description == Util.ScratchPad);
                qb = CheckBadQuery(qb);
                if (qb == null)
                {
                    qb = QueryBuilderClause.NewGroupClause();
                    qb.Description = Util.ScratchPad;
                    qb.SavedBy = Util.UserName;
                    QueryBuilderClauses.InsertOnSubmit(qb);
                    SubmitChanges();
                }
            }
            Util.QueryBuilderScratchPadId = qb.QueryId;
            return qb;
        }
        public QueryBuilderClause QueryBuilderHasCurrentTag()
        {
            const string STR_HasCurrentTag = "HasCurrentTag";
            var qb = QueryBuilderClauses.FirstOrDefault(c => c.SavedBy == STR_System
                && c.Description == STR_HasCurrentTag);
            if (qb == null)
            {
                qb = QueryBuilderClause.NewGroupClause();
                qb.Description = STR_HasCurrentTag;
                qb.SavedBy = STR_System;
                qb.AddNewClause(QueryType.HasCurrentTag, CompareType.Equal, "1,T");
                QueryBuilderClauses.InsertOnSubmit(qb);
                SubmitChanges();
            }
            return qb;
        }
        public QueryBuilderClause QueryBuilderInCurrentOrg()
        {
            const string STR_InCurrentOrg = "InCurrentOrg";
            var qb = QueryBuilderClauses.FirstOrDefault(c => c.SavedBy == STR_System
                && c.Description == STR_InCurrentOrg);
            if (qb == null)
            {
                qb = QueryBuilderClause.NewGroupClause();
                qb.Description = STR_InCurrentOrg;
                qb.SavedBy = STR_System;
                qb.AddNewClause(QueryType.InCurrentOrg, CompareType.Equal, "1,T");
                QueryBuilderClauses.InsertOnSubmit(qb);
                SubmitChanges();
            }
            return qb;
        }
        public QueryBuilderClause QueryBuilderLeadersUnderCurrentOrg()
        {
            const string STR_LeadersUnderCurrentOrg = "LeadersUnderCurrentOrg";
            var qb = QueryBuilderClauses.FirstOrDefault(c => c.SavedBy == STR_System
                && c.Description == STR_LeadersUnderCurrentOrg);
            if (qb == null)
            {
                qb = QueryBuilderClause.NewGroupClause();
                qb.Description = STR_LeadersUnderCurrentOrg;
                qb.SavedBy = STR_System;
                qb.AddNewClause(QueryType.LeadersUnderCurrentOrg, CompareType.Equal, "1,T");
                QueryBuilderClauses.InsertOnSubmit(qb);
                SubmitChanges();
            }
            return qb;
        }
        public QueryBuilderClause QueryBuilderMembersUnderCurrentOrg()
        {
            const string STR_MembersUnderCurrentOrg = "MembersUnderCurrentOrg";
            var qb = QueryBuilderClauses.FirstOrDefault(c => c.SavedBy == STR_System
                && c.Description == STR_MembersUnderCurrentOrg);
            if (qb == null)
            {
                qb = QueryBuilderClause.NewGroupClause();
                qb.Description = STR_MembersUnderCurrentOrg;
                qb.SavedBy = STR_System;
                qb.AddNewClause(QueryType.MembersUnderCurrentOrg, CompareType.Equal, "1,T");
                QueryBuilderClauses.InsertOnSubmit(qb);
                SubmitChanges();
            }
            return qb;
        }
        public QueryBuilderClause QueryBuilderInactiveCurrentOrg()
        {
            const string STR_InactiveCurrentOrg = "InactiveCurrentOrg";
            var qb = QueryBuilderClauses.FirstOrDefault(c => c.SavedBy == STR_System
                && c.Description == STR_InactiveCurrentOrg);
            if (qb == null)
            {
                qb = QueryBuilderClause.NewGroupClause();
                qb.Description = STR_InactiveCurrentOrg;
                qb.SavedBy = STR_System;
                qb.AddNewClause(QueryType.InactiveCurrentOrg, CompareType.Equal, "1,T");
                QueryBuilderClauses.InsertOnSubmit(qb);
                SubmitChanges();
            }
            return qb;
        }
        public QueryBuilderClause QueryBuilderProspectCurrentOrg()
        {
            const string STR_ProspectCurrentOrg = "ProspectCurrentOrg";
            var qb = QueryBuilderClauses.FirstOrDefault(c => c.SavedBy == STR_System
                && c.Description == STR_ProspectCurrentOrg);
            if (qb == null)
            {
                qb = QueryBuilderClause.NewGroupClause();
                qb.Description = STR_ProspectCurrentOrg;
                qb.SavedBy = STR_System;
                qb.AddNewClause(QueryType.ProspectCurrentOrg, CompareType.Equal, "1,T");
                QueryBuilderClauses.InsertOnSubmit(qb);
                SubmitChanges();
            }
            return qb;
        }
        public QueryBuilderClause QueryBuilderPendingCurrentOrg()
        {
            const string STR_PendingCurrentOrg = "PendingCurrentOrg";
            var qb = QueryBuilderClauses.FirstOrDefault(c => c.SavedBy == STR_System
                && c.Description == STR_PendingCurrentOrg);
            if (qb == null)
            {
                qb = QueryBuilderClause.NewGroupClause();
                qb.Description = STR_PendingCurrentOrg;
                qb.SavedBy = STR_System;
                qb.AddNewClause(QueryType.PendingCurrentOrg, CompareType.Equal, "1,T");
                QueryBuilderClauses.InsertOnSubmit(qb);
                SubmitChanges();
            }
            return qb;
        }
        public QueryBuilderClause QueryBuilderPreviousCurrentOrg()
        {
            const string STR_PreviousCurrentOrg = "PreviousCurrentOrg";
            var qb = QueryBuilderClauses.FirstOrDefault(c => c.SavedBy == STR_System
                && c.Description == STR_PreviousCurrentOrg);
            if (qb == null)
            {
                qb = QueryBuilderClause.NewGroupClause();
                qb.Description = STR_PreviousCurrentOrg;
                qb.SavedBy = STR_System;
                qb.AddNewClause(QueryType.PreviousCurrentOrg, CompareType.Equal, "1,T");
                QueryBuilderClauses.InsertOnSubmit(qb);
                SubmitChanges();
            }
            return qb;
        }
        public QueryBuilderClause QueryBuilderVisitedCurrentOrg()
        {
            const string STR_VisitedCurrentOrg = "VisitedCurrentOrg";
            var qb = QueryBuilderClauses.FirstOrDefault(c => c.SavedBy == STR_System
                && c.Description == STR_VisitedCurrentOrg);
            if (qb == null)
            {
                qb = QueryBuilderClause.NewGroupClause();
                qb.Description = STR_VisitedCurrentOrg;
                qb.SavedBy = STR_System;
                qb.AddNewClause(QueryType.VisitedCurrentOrg, CompareType.Equal, "1,T");
                QueryBuilderClauses.InsertOnSubmit(qb);
                SubmitChanges();
            }
            return qb;
        }
        public QueryBuilderClause QueryBuilderIsCurrentPerson()
        {
            const string STR_IsCurrentPerson = "IsCurrentPerson";
            var qb = QueryBuilderClauses.FirstOrDefault(c => c.SavedBy == STR_System
                && c.Description == STR_IsCurrentPerson);
            if (qb == null)
            {
                qb = QueryBuilderClause.NewGroupClause();
                qb.Description = STR_IsCurrentPerson;
                qb.SavedBy = STR_System;
                qb.AddNewClause(QueryType.IsCurrentPerson, CompareType.Equal, "1,T");
                QueryBuilderClauses.InsertOnSubmit(qb);
                SubmitChanges();
            }
            return qb;
        }
        public void DeleteQueryBuilderClauseOnSubmit(QueryBuilderClause qb)
        {
            foreach (var c in qb.Clauses)
                DeleteQueryBuilderClauseOnSubmit(c);
            this.QueryBuilderClauses.DeleteOnSubmit(qb);
        }
        public void TagAll(IQueryable<Person> list)
        {
            var tag = TagCurrent();
            TagAll(list, tag);
        }
        public void TagAll(IQueryable<Person> list, Tag tag)
        {
            var q = from p in list
                    where !p.Tags.Any(tp => tp.Id == tag.Id)
                    select p.PeopleId;
            foreach (var id in q)
                tag.PersonTags.Add(new TagPerson { PeopleId = id });
            SubmitChanges();
        }
        public void TagAll2(IQueryable<Person> list, Tag tag)
        {
            ExecuteCommand("delete TagPerson where Id = {0}", tag.Id);
            var q2 = list.Select(pp => pp.PeopleId);
            var cmd = GetCommand(q2);
            var s = cmd.CommandText;
            var plist = new List<DbParameter>();
            var n = 0;
            foreach (var p in cmd.Parameters)
            {
                s = s.Replace("@p" + n, "{{{0}}}".Fmt(n));
                n++;
                plist.Add(p as DbParameter);
            }
            s = Regex.Replace(s, "^SELECT( DISTINCT)?",
                @"INSERT INTO TagPerson (Id, PeopleId) $0 " + tag.Id + ",");
            ExecuteCommand(s, plist.Select(pp => pp.Value).ToArray());
        }
        public void TagAll(IEnumerable<int> list, Tag tag)
        {
            foreach (var id in list)
                tag.PersonTags.Add(new TagPerson { PeopleId = id });
            SubmitChanges();
        }
        public void UnTagAll(IQueryable<Person> list)
        {
            var person = list.FirstOrDefault();
            var tag = TagCurrent();
            var q = from p in list
                    from t in p.Tags
                    where t.Id == tag.Id
                    select t;

            foreach (var t in q)
                TagPeople.DeleteOnSubmit(t);
            SubmitChanges();
        }
        public Tag PopulateSpecialTag(object QueryId, int TagTypeId)
        {
            IQueryable<Person> q;
            if(QueryId is Guid)
                q = PeopleQuery((Guid)QueryId);
            else
                q = PeopleQuery((int)QueryId);
            return PopulateSpecialTag(q, TagTypeId);
        }
        public Tag PopulateSpecialTag(IQueryable<Person> q, int TagTypeId)
        {
            var tag = FetchOrCreateTag(Util.SessionId, Util.UserPeopleId ?? Util.UserId1, TagTypeId);
            ExecuteCommand("delete TagPerson where Id = {0}", tag.Id);
            TagAll(q, tag);
            return tag;
        }
        public Tag PopulateTemporaryTag(IQueryable<int> q)
        {
            var tag = FetchOrCreateTag(Util.SessionId, Util.UserPeopleId ?? Util.UserId1, NextTagId);
            ExecuteCommand("delete TagPerson where Id = {0}", tag.Id);
            var cmd = GetCommand(q);
            var s = cmd.CommandText;
            var plist = new List<DbParameter>();
            var n = 0;
            foreach (var p in cmd.Parameters)
            {
                s = s.Replace("@p" + n, "{{{0}}}".Fmt(n));
                n++;
                plist.Add(p as DbParameter);
            }
            s = Regex.Replace(s, "^SELECT( DISTINCT)?",
                @"INSERT INTO TagPerson (Id, PeopleId) $0 " + tag.Id + ",");
            ExecuteCommand(s, plist.Select(pp => pp.Value).ToArray());
            return tag;
        }
        public Tag PopulateTemporaryTag(IEnumerable<int> a)
        {
            var tag = FetchOrCreateTag(Util.SessionId, Util.UserPeopleId ?? Util.UserId1, NextTagId);
            ExecuteCommand("delete TagPerson where Id = {0}", tag.Id);
            TagAll(a, tag);
            return tag;
        }
        public void ClearTag(Tag tag)
        {
            ExecuteCommand("delete TagPerson where Id = {0}", tag.Id);
        }
        public void PopulateSpecialTag(IQueryable<Person> q, string tagname)
        {
            var tag = FetchOrCreateTag(tagname, Util.UserPeopleId ?? Util.UserId1, DbUtil.TagTypeId_Personal);
            TagPeople.DeleteAllOnSubmit(tag.PersonTags);
            SubmitChanges();
            TagAll(q, tag);
        }
        public void DePopulateSpecialTag(IQueryable<Person> q, int TagTypeId)
        {
            var tag = FetchOrCreateTag(Util.SessionId, Util.UserPeopleId ?? Util.UserId1, TagTypeId);
            TagPeople.DeleteAllOnSubmit(tag.PersonTags);
            SubmitChanges();
        }
        public Tag TagById(int id)
        {
            return Tags.SingleOrDefault(t => t.Id == id);
        }
        public Tag TagCurrent()
        {
            return FetchOrCreateTag(Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
        }
        //public string NewPeopleEmailAddressx
        //{
        //    get
        //    {
        //        var em = DbUtil.SystemEmailAddress;
        //        var npm = People.SingleOrDefault(p => p.PeopleId == DbUtil.NewPeopleManagerId);
        //        if (npm != null && npm.EmailAddress.HasValue())
        //            em = npm.EmailAddress;
        //        var s = Settings.Where(ss => ss.Id == "NewPeopleEmailAddress").Select(ss => ss.SettingX).SingleOrDefault();
        //        return Util.PickFirst(s, em);
        //    }
        //}
        public int NewPeopleManagerId
        {
            get
            {
                var s = Setting("NewPeopleManagerIds", "1");
                if (s.HasValue())
                    return s.SplitStr(",;")[0].ToInt();
                var q = from u in Users
                        where u.UserRoles.Any(ur => ur.Role.RoleName == "Admin")
                        select u.PeopleId.Value;
                return q.First();
            }
        }
        public IEnumerable<Person> GetNewPeopleManagers()
        {
            var s = Setting("NewPeopleManagerIds", "");
            IEnumerable<Person> q = null;
            if (s.HasValue())
            {
                var a = s.SplitStr(",").Select(ss => ss.ToInt());
                q = from p in People
                    where a.Contains(p.PeopleId)
                    select p;
                return q;
            }
            return CMSRoleProvider.provider.GetAdmins();
        }
        private User _currentuser;
        public User CurrentUser
        {
            get
            {
                if (_currentuser != null)
                    return _currentuser;
                GetCurrentUser();
                return _currentuser;
            }
        }
        private void GetCurrentUser()
        {
            var q = from u in Users
                    where u.UserId == Util.UserId
                    select new
                           {
                               u,
                               roleids = u.UserRoles.Select(uu => uu.RoleId).ToArray(),
                               roles = u.UserRoles.Select(uu => uu.Role.RoleName).ToArray(),
                           };
            var i = q.SingleOrDefault();
            if (i == null)
                return;
            _roles = i.roles;
            _roleids = i.roleids;
            _currentuser = i.u;
        }

        private string[] _roles;
        public string[] CurrentRoles()
        {
            if (_roles == null)
                GetCurrentUser();
            return _roles;
        }
        private int[] _roleids;
        public int[] CurrentRoleIds()
        {
            if (_roleids == null)
                GetCurrentUser();
            return _roleids;
        }

        public Person CurrentUserPerson
        {
            get
            {
                return Users.Where(u => u.UserId == Util.UserId).Select(u => u.Person).SingleOrDefault();
            }
        }
        public Tag OrgMembersOnlyTag2()
        {
            return FetchOrCreateTag(Util.SessionId, Util.UserPeopleId ?? Util.UserId1, DbUtil.TagTypeId_OrgMembersOnly);
        }
        public Tag OrgLeadersOnlyTag2()
        {
            return FetchOrCreateTag(Util.SessionId, Util.UserPeopleId ?? Util.UserId1, DbUtil.TagTypeId_OrgLeadersOnly);
        }

        public Tag FetchOrCreateTag(string tagname, int? OwnerId, int tagtypeid)
        {
            var tag = FetchTag(tagname, OwnerId, tagtypeid);
            if (tag == null)
            {
                tag = new Tag { Name = tagname.Replace('!', '*'), PeopleId = OwnerId, TypeId = tagtypeid };
                Tags.InsertOnSubmit(tag);
                SubmitChanges();
            }
            return tag;
        }
        public Tag FetchTag(string tagname, int? OwnerId, int tagtypeid)
        {
            var tag = Tags.FirstOrDefault(t =>
                t.Name == tagname && t.PeopleId == OwnerId && t.TypeId == tagtypeid);
            return tag;
        }
        public Tag FetchOrCreateSystemTag(string tagname)
        {
            var tag = Tags.FirstOrDefault(t => t.Name == tagname && t.TypeId == 100);
            if (tag == null)
            {
                tag = new Tag { Name = tagname, TypeId = 100 };
                Tags.InsertOnSubmit(tag);
                SubmitChanges();
            }
            return tag;
        }

        public void SetOrgMembersOnly()
        {
            var me = Util.UserPeopleId;
            var dt = Util.Now.AddYears(-1);

            // members of any of my orgs excluding unshared orgs
            var q = from p in People
                    where p.OrganizationMembers.Any(m =>
                        OrganizationMembers.Any(um =>
                            (um.Organization.SecurityTypeId != 3
                                || um.MemberTypeId == MemberTypeCode.Teacher)
                             && um.OrganizationId == m.OrganizationId && um.PeopleId == me))
                    select p;
            var tag = PopulateSpecialTag(q, DbUtil.TagTypeId_OrgMembersOnly);
            //PopulateSpecialTag(q, "OrgMemberOnlyInMyOrg");

            // prev members of any of my orgs excluding unshared orgs

            q = from p in People
                where p.EnrollmentTransactions.Any(et =>
                        et.TransactionDate > dt
                        && et.TransactionTypeId >= 4
                        && et.Organization.SecurityTypeId != 3
                        && OrganizationMembers.Any(um =>
                            um.OrganizationId == et.OrganizationId && um.PeopleId == me))
                select p;
            TagAll(q, tag);
            //PopulateSpecialTag(q, "OrgMemberOnlyPrevInMyOrg");

            // members of my family
            q = from p in People
                where p.FamilyId == CurrentUser.Person.FamilyId
                select p;
            TagAll(q, tag);
            //PopulateSpecialTag(q, "OrgMemberOnlyInMyFamily");

            // visitors in the last year to one of my orgs excluding unshared
            var attype = new int[] { 40, 50, 60 };
            q = from p in People
                where p.Attends.Any(a =>
                    OrganizationMembers.Any(um =>
                        um.Organization.SecurityTypeId != 3 &&
                        um.OrganizationId == a.Meeting.OrganizationId && um.PeopleId == me)
                    && attype.Contains(a.AttendanceTypeId.Value) && a.MeetingDate > dt)
                select p;
            TagAll(q, tag);
            //PopulateSpecialTag(q, "OrgMemberOnlyVisitMyOrg");

            // people assigned to me in one of my tasks
            q = from p in People
                where p.TasksAboutPerson.Any(at => at.OwnerId == me || at.CoOwnerId == me)
                select p;
            TagAll(q, tag);
            //PopulateSpecialTag(q, "OrgMemberOnlyInMyTask");

            // people I have visited in a contact
            q = from p in People
                where p.contactsHad.Any(c => c.contact.contactsMakers.Any(cm => cm.PeopleId == me))
                select p;
            TagAll(q, tag);
            //PopulateSpecialTag(q, "OrgMemberOnlyIHaveContacted");
        }
        public int[] GetLeaderOrgIds(int? me)
        {
            var o1 = from o in Organizations
                     where o.OrganizationMembers.Any(om => om.MemberType.AttendanceTypeId == AttendTypeCode.Leader && om.PeopleId == me)
                     select o.OrganizationId;
            var o2 = from o in Organizations
                     where o1.Contains(o.OrganizationId)
                     from co in o.ChildOrgs
                     select co.OrganizationId;
            var o3 = from o in Organizations
                     where o1.Contains(o.OrganizationId)
                     from co in o.ChildOrgs
                     from cco in co.ChildOrgs
                     select cco.OrganizationId;
            var oids = o1.Union(o2).Union(o3).ToArray();
            return oids;
        }
        public int[] GetParentChildOrgIds(int? parent)
        {
            var o1 = from o in Organizations
                     where o.OrganizationId == parent
                     select o.OrganizationId;
            var o2 = from o in Organizations
                     where o1.Contains(o.OrganizationId)
                     from co in o.ChildOrgs
                     select co.OrganizationId;
            var o3 = from o in Organizations
                     where o1.Contains(o.OrganizationId)
                     from co in o.ChildOrgs
                     from cco in co.ChildOrgs
                     select cco.OrganizationId;
            var oids = o1.Union(o2).Union(o3).ToArray();
            return oids;
        }
        public void SetOrgLeadersOnly()
        {
            var me = Util.UserPeopleId;
            var dt = Util.Now.AddYears(-1);

            var oids = GetLeaderOrgIds(Util.UserPeopleId);
            // current members of one of my orgs I lead
            var q = from p in People
                    where p.OrganizationMembers.Any(m => oids.Contains(m.OrganizationId))
                    select p;
            var tag = PopulateSpecialTag(q, DbUtil.TagTypeId_OrgLeadersOnly);

            // previous members of my org
            q = from p in People
                where p.EnrollmentTransactions.Any(et =>
                        et.TransactionDate > dt
                        && et.TransactionTypeId >= 4
                        && oids.Contains(et.OrganizationId)
                        && et.Organization.SecurityTypeId != 3
                        && OrganizationMembers.Any(um =>
                            um.OrganizationId == et.OrganizationId && um.PeopleId == me))
                select p;
            TagAll(q, tag);

            // members of my family
            q = from p in People
                where p.FamilyId == CurrentUser.Person.FamilyId
                select p;
            TagAll(q, tag);

            // visitors in the last year to one of my orgs excluding unshared
            var attype = new int[] { 40, 50, 60 };
            q = from p in People
                where p.Attends.Any(a =>
                    OrganizationMembers.Any(um =>
                        um.MemberType.AttendanceTypeId == AttendTypeCode.Leader
                        && um.Organization.SecurityTypeId != 3
                        && oids.Contains(um.OrganizationId)
                        && um.OrganizationId == a.Meeting.OrganizationId
                        && um.PeopleId == me)
                    && attype.Contains(a.AttendanceTypeId.Value) && a.MeetingDate > dt)
                select p;
            TagAll(q, tag);

            // people assigned to me in one of my tasks
            q = from p in People
                where p.TasksAboutPerson.Any(at => at.OwnerId == me || at.CoOwnerId == me)
                select p;
            TagAll(q, tag);

            // people I have visited in a contact
            q = from p in People
                where p.contactsHad.Any(c => c.contact.contactsMakers.Any(cm => cm.PeopleId == me))
                select p;
            TagAll(q, tag);
        }
        [Function(Name = "dbo.AddAbsents")]
        public int AddAbsents([Parameter(DbType = "Int")] int? meetid, [Parameter(DbType = "Int")] int? userid)
        {
            var result = this.ExecuteMethodCall(this, (MethodInfo)(MethodInfo.GetCurrentMethod()), meetid, userid);
            return (int)(result.ReturnValue);
        }
        [Function(Name = "dbo.UpdateAttendStr")]
        public int UpdateAttendStr([Parameter(DbType = "Int")] int? orgid, [Parameter(DbType = "Int")] int? pid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), orgid, pid);
            return ((int)(result.ReturnValue));
        }
        public class TopGiver
        {
            public int PeopleId;
            public string Name;
            public decimal Amount;
        }
        public class AttendMeetingInfo1
        {
            public AttendMeetingInfo2 info;
            public Attend AttendanceOrg;
            public Attend Attendance;
            public Meeting Meeting;
            public List<Attend> VIPAttendance;
            public OrganizationMember BFCMember;
            public Attend BFCAttendance;
            public Meeting BFCMeeting;
            public int path;
        }
        public class AttendMeetingInfo2
        {
            public int? AttendedElsewhere { get; set; }
            public int? MemberTypeId { get; set; }
            //public bool? IsRegularHour { get; set; }
            //public int? ScheduleId { get; set; }
            //public bool? IsSameHour { get; set; }
            public bool? IsOffSite { get; set; }
            public bool? IsRecentVisitor { get; set; }
            public string Name { get; set; }
            public int? BFClassId { get; set; }
        }

        [Function(Name = "dbo.AttendMeetingInfo")]
        [ResultType(typeof(AttendMeetingInfo2))]
        [ResultType(typeof(Attend))] // Attendance
        [ResultType(typeof(Meeting))] // Meeting Attended
        [ResultType(typeof(Attend))] // VIP Attendance
        [ResultType(typeof(OrganizationMember))] // BFC membership
        [ResultType(typeof(Attend))] // BFC Attendance at same time
        [ResultType(typeof(Meeting))] // BFC Meeting Attended
        public IMultipleResults AttendMeetingInfo(int MeetingId, int PeopleId)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), MeetingId, PeopleId);
            return (IMultipleResults)result.ReturnValue;
        }
        public AttendMeetingInfo1 AttendMeetingInfo0(int MeetingId, int PeopleId)
        {
            var r = AttendMeetingInfo(MeetingId, PeopleId);
            var o = new AttendMeetingInfo1();
            o.info = r.GetResult<CMSDataContext.AttendMeetingInfo2>().First();
            o.Attendance = r.GetResult<Attend>().FirstOrDefault();
            if (o.Attendance != null)
            {
                o.AttendanceOrg = new Attend
                {
                    AttendanceFlag = o.Attendance.AttendanceFlag,
                    AttendanceTypeId = o.Attendance.AttendanceTypeId,
                    AttendId = o.Attendance.AttendId,
                    CreatedBy = o.Attendance.CreatedBy,
                    CreatedDate = o.Attendance.CreatedDate,
                    EffAttendFlag = o.Attendance.EffAttendFlag,
                    MeetingDate = o.Attendance.MeetingDate,
                    MeetingId = o.Attendance.MeetingId,
                    MemberTypeId = o.Attendance.MemberTypeId,
                    OrganizationId = o.Attendance.OrganizationId,
                    OtherOrgId = o.Attendance.OtherOrgId,
                    PeopleId = o.Attendance.PeopleId,
                };
            }

            o.Meeting = r.GetResult<Meeting>().First();
            o.VIPAttendance = r.GetResult<Attend>().ToList();
            o.BFCMember = r.GetResult<OrganizationMember>().FirstOrDefault();
            o.BFCAttendance = r.GetResult<Attend>().FirstOrDefault();
            o.BFCMeeting = r.GetResult<Meeting>().FirstOrDefault();
            return o;
        }
        public class RecordAttendInfo
        {
            public string ret { get; set; }
        }
        [Function(Name = "dbo.RecordAttend")]
        [ResultType(typeof(RecordAttendInfo))]
        private IMultipleResults RecordAttend([Parameter(DbType = "Int")] int? meetingId, [Parameter(DbType = "Int")] int? peopleId, [Parameter(DbType = "Bit")] bool? present)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), meetingId, peopleId, present);
            return ((IMultipleResults)(result.ReturnValue));
        }
        [Function(Name = "dbo.RecordAttendance")]
        [ResultType(typeof(RecordAttendInfo))]
        private IMultipleResults RecordAttendance(
            [Parameter(DbType = "Int")] int? orgId,
            [Parameter(DbType = "Int")] int? peopleId,
            [Parameter(DbType = "DateTime")] DateTime meetingDate,
            [Parameter(DbType = "bit")] bool present,
            [Parameter(DbType = "Varchar(50)")] string location,
            [Parameter(DbType = "Int")] int? userId)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), orgId, peopleId, meetingDate, present, location, userId);
            return ((IMultipleResults)(result.ReturnValue));
        }

        public string RecordAttendance(
            int? orgId, int? peopleId, DateTime meetingDate, bool present, string location)
        {
            var r = RecordAttendance(orgId, peopleId, meetingDate, present, location, Util.UserId1);
            var s = r.GetResult<RecordAttendInfo>().First();
            return s.ret;
        }

        public string RecordAttendance(int MeetingId, int PeopleId, bool present)
        {
            var r = RecordAttend(MeetingId, PeopleId, present);
            var s = r.GetResult<RecordAttendInfo>().First();
            return s.ret;
        }
        public string UserPreference(string pref)
        {
            return UserPreference(pref, string.Empty);
        }
        public string UserPreference(string pref, string def)
        {
            var d = HttpContext.Current.Session["preferences"] as Dictionary<string, string>;
            if (d != null && d.ContainsKey(pref))
                return d[pref] ?? def;
            if (d == null)
            {
                d = new Dictionary<string, string>();
                HttpContext.Current.Session["preferences"] = d;
            }
            Preference p = null;
            if (CurrentUser != null)
                p = CurrentUser.Preferences.SingleOrDefault(up => up.PreferenceX == pref);
            if (p != null)
                return d[pref] = p.ValueX;
            d[pref] = null;
            return def;
        }
        public void SetUserPreference(string pref, object value)
        {
            if (UserPreference(pref) == value.ToString())
                return;
            if (CurrentUser == null)
                return;
            var p = CurrentUser.Preferences.SingleOrDefault(up => up.PreferenceX == pref);
            if (p != null)
                p.ValueX = value.ToString();
            else
            {
                p = new Preference { UserId = Util.UserId1, PreferenceX = pref, ValueX = value.ToString() };
                Preferences.InsertOnSubmit(p);
            }
            var d = HttpContext.Current.Session["preferences"] as Dictionary<string, string>;
            if (d == null)
            {
                d = new Dictionary<string, string>();
                HttpContext.Current.Session["preferences"] = d;
            }
            d[pref] = p.ValueX;
            SubmitChanges();
        }
        public void SetUserPreference(int id, string pref, object value)
        {
            var u = Users.Single(uu => uu.UserId == id);
            var p = u.Preferences.SingleOrDefault(up => up.PreferenceX == pref);
            if (p != null)
            {
                if (p.ValueX == value.ToString())
                    return;
                p.ValueX = value.ToString();
            }
            else
            {
                p = new Preference {UserId = id, PreferenceX = pref, ValueX = value.ToString()};
                Preferences.InsertOnSubmit(p);
            }
            SubmitChanges();
        }

        [Function(Name = "dbo.LinkEnrollmentTransaction")]
        public int LinkEnrollmentTransaction([Parameter(DbType = "Int")] int? tid, [Parameter(DbType = "DateTime")] DateTime? trandt, [Parameter(DbType = "Int")] int? typeid, [Parameter(DbType = "Int")] int? orgid, [Parameter(DbType = "Int")] int? pid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), tid, trandt, typeid, orgid, pid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.FlagOddTransaction")]
        public int FlagOddTransaction([Parameter(DbType = "Int")] int? pid, [Parameter(DbType = "Int")] int? orgid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), pid, orgid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.PurgePerson")]
        public int PurgePerson([Parameter(DbType = "Int")] int? pid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), pid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.PurgeOrganization")]
        public int PurgeOrganization([Parameter(DbType = "Int")] int? oid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), oid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.UpdateMainFellowship")]
        public int UpdateMainFellowship([Parameter(DbType = "Int")] int? orgid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), orgid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.UpdateMeetingCounters")]
        public int UpdateMeetingCounters([Parameter(DbType = "Int")] int? mid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), mid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.DeletePeopleExtras")]
        public int DeletePeopleExtras([Parameter(DbType = "varchar(50)")] string field)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), field);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.DeleteSpecialTags")]
        public int DeleteSpecialTags([Parameter(DbType = "Int")] int? pid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), pid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.UpdateResCodes")]
        public int UpdateResCodes()
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())));
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.PurgeAllPeopleInCampus")]
        public int PurgeAllPeopleInCampus([Parameter(DbType = "Int")] int? cid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), cid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.PopulateComputedEnrollmentTransactions")]
        public int PopulateComputedEnrollmentTransactions([Parameter(DbType = "Int")] int? orgid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), orgid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.UpdateSchoolGrade")]
        public int UpdateSchoolGrade([Parameter(DbType = "Int")] int? pid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), pid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.UpdateLastActivity")]
        public int UpdateLastActivity([Parameter(DbType = "Int")] int? userid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.PurgeUser")]
        public int PurgeUser([Parameter(DbType = "Int")] int? uid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), uid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.DeleteQBTree")]
        public int DeleteQBTree([Parameter(DbType = "Int")] int? qid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), qid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.SetMainDivision")]
        public int SetMainDivision([Parameter(DbType = "Int")] int? orgid, [Parameter(DbType = "Int")] int? divid)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), orgid, divid);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.DeleteQueryBitTags")]
        public int DeleteQueryBitTags()
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())));
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.CreateMeeting")]
        public int CreateMeeting([Parameter(DbType = "Int")] int oid, [Parameter(DbType = "DateTime")] DateTime? mdt)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), oid, mdt);
            return ((int)(result.ReturnValue));
        }
        [Function(Name = "dbo.InsertDuplicate")]
        public int InsertDuplicate([Parameter(DbType = "Int")] int i1, [Parameter(DbType = "Int")] int i2)
        {
            var result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), i1, i2);
            return ((int)(result.ReturnValue));
        }
        public OrganizationMember LoadOrgMember(int PeopleId, string OrgName, bool orgmustexist)
        {
            if (orgmustexist)
            {
                var org = Organizations.SingleOrDefault(oo => oo.OrganizationName == OrgName);
                if (org == null)
                    throw new Exception("Org Named '" + OrgName + "' does not exist");
            }
            return OrganizationMember.Load(this, PeopleId, OrgName);
        }
        public IEnumerable<string[]> StatusFlags()
        {
            var q = from c in QueryBuilderClauses
                    where c.GroupId == null && c.Field == "Group"
                    where c.Description.StartsWith("F") && c.Description.Contains(":")
                    orderby c.Description
                    select c.Description;

            const string FindPrefix = @"^F\d+:.*";
            var re = new Regex(FindPrefix, RegexOptions.Singleline | RegexOptions.Multiline);
            var q2 = from s in q.ToList()
                     where re.Match(s).Success
                     let a = s.SplitStr(":", 2)
                     select new string[] { a[0], a[1] };
            return q2;
        }
        public IEnumerable<string[]> QueryStatClauses()
        {
            var q = from c in QueryBuilderClauses
                    where c.GroupId == null && c.Field == "Group"
                    where c.Description.StartsWith("S") && c.Description.Contains(":")
                    select c.Description;

            const string FindPrefix = @"^S\d+:.*";
            var re = new Regex(FindPrefix, RegexOptions.Singleline | RegexOptions.Multiline);
            var q2 = from s in q.ToList()
                     where re.Match(s).Success
                     let a = s.SplitStr(":", 2)
                     select new string[] { a[0], a[1] };
            return q2;
        }
        public Content Content(string name)
        {
            return Contents.SingleOrDefault(c => c.Name == name);
        }
        public string Content(string name, string def)
        {
            var content = Contents.SingleOrDefault(c => c.Name == name);
            if (content != null)
                return content.Body;
            return def;
        }
        public string Content2(string name, string def, int ContentTypeId)
        {
            var c = Contents.SingleOrDefault(cc => cc.Name == name);
            if (c == null)
            {
                c = new Content()
                        {
                            Name = name,
                            Title = name,
                            Body = def,
                            TypeID = ContentTypeId
                        };
                Contents.InsertOnSubmit(c);
                SubmitChanges();
            }
            return c.Body;
        }
        public string ContentHtml(string name, string def)
        {
            return Content2(name, def, ContentTypeCode.TypeHtml);
        }
        public string ContentText(string name, string def)
        {
            return Content2(name, def, ContentTypeCode.TypeText);
        }
        public void SetNoLock()
        {
            //ExecuteCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED"); 
        }

        public int FetchOrCreateCampusId(string name)
        {
            var c = Campus.SingleOrDefault(pp => pp.Description == name);
            if (c == null)
            {
                var max = 10;
                if(Campus.Any())
                    max = Campus.Max(mm => mm.Id) + 10;
                c = new Campu() { Id = max, Description = name, Code = name.Truncate(20) };
                Campus.InsertOnSubmit(c);
                SubmitChanges();
            }
            return c.Id;
        }

        public ContributionFund FetchOrCreateFund(string Description)
        {
            return FetchOrCreateFund(0, Description);
        }
        public EntryPoint FetchOrCreateEntryPoint(string type)
        {
            var ep = EntryPoints.SingleOrDefault(pp => pp.Description == type);
            if (ep == null)
            {
                var max = EntryPoints.Max(mm => mm.Id) + 10;
                if (max < 1000)
                    max = 1010;
                ep = new EntryPoint { Id = max, Description = type, Code = type.Truncate(20) };
                EntryPoints.InsertOnSubmit(ep);
                SubmitChanges();
            }
            return ep;
        }
        public ContributionFund FetchOrCreateFund(int FundId, string Description)
        {
            ContributionFund fund;
            if (FundId > 0)
                fund = ContributionFunds.SingleOrDefault(f => f.FundId == FundId);
            else
                fund = ContributionFunds.FirstOrDefault(f => f.FundName == Description && f.FundStatusId == 1);
            if (fund == null)
            {
                int nextfundid;
                if (FundId > 0)
                    nextfundid = FundId;
                else
                    nextfundid = ContributionFunds.Max(ff => ff.FundId) + 1;
                fund = new ContributionFund
                {
                    FundName = Description,
                    FundId = nextfundid,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now,
                    FundStatusId = 1,
                    FundTypeId = 1,
                    FundPledgeFlag = false,
                };
                ContributionFunds.InsertOnSubmit(fund);
                SubmitChanges();
            }
            return fund;
        }
        public int ActiveRecords()
        {
            const string name = "ActiveRecords";
            var qb = QueryBuilderClauses.FirstOrDefault(c => c.IsPublic && c.Description == name && c.SavedBy == "public");
            if (qb == null)
            {
                qb = QueryBuilderScratchPad();
                qb.CleanSlate(this);
                qb.SetComparisonType(CompareType.AnyTrue);

                var clause = qb.AddNewClause(QueryType.RecentAttendCount, CompareType.GreaterEqual, "1");
                clause.Days = 365;
                clause = qb.AddNewClause(QueryType.RecentHasIndContributions, CompareType.Equal, "1,T");
                clause.Days = 365;
                qb.AddNewClause(QueryType.IncludeDeceased, CompareType.Equal, "1,T");
                qb.SaveTo(this, name, "public", true);
            }
            FromActiveRecords = true;
            var n = PeopleQuery(qb.QueryId).Count();
            FromActiveRecords = false;
            return n;
        }


        internal bool FromActiveRecords { get; set; }

    }
}
