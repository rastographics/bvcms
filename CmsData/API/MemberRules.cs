using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using UtilityExtensions;
using System.Text.RegularExpressions;
using System.Data.Linq;
using System.Data.Linq.SqlClient;
using IronPython.Hosting;
using System.IO;
using CmsData.Codes;
using System.Web;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting.Providers;

namespace CmsData
{
    public partial class Person
    {
        public bool FamilyHasPrimaryMemberForMoreThanDays(int days)
        {
            return Family.People.Any(p =>
                p.PositionInFamilyId == PositionInFamily.PrimaryAdult
                   && p.MemberStatusId == MemberStatusCode.Member
                   && SqlMethods.DateDiffDay(p.JoinDate, Util.Now) >= days);
        }
        public void DropAllMemberships(CMSDataContext Db)
        {
            var list = (from om in Db.OrganizationMembers
                        where om.PeopleId == PeopleId
                        select om).ToList();
            foreach (var om in list)
                om.Drop(Db, DateTime.Now);
        }
        public string errorReturn;

        public string MemberProfileAutomation(CMSDataContext Db)
        {
			if (Db.Setting("UseMemberProfileAutomation", "true") != "true")
				return "ok";
            var script = Db.Content("MemberProfileAutomation");
            if (script == null)
                return "ok";
            var path = HttpContextFactory.Current.Server.MapPath("/Content/");
#if DEBUG2
            var options = new Dictionary<string, object>();
            options["Debug"] = true;
            var engine = Python.CreateEngine(options);
            var paths = engine.GetSearchPaths();
            paths.Add(path);
            engine.SetSearchPaths(paths);
            var sc = engine.CreateScriptSourceFromFile(HttpContextFactory.Current.Server.MapPath("/Content/MembershipAutomation2.py"));
#else
            var engine = Python.CreateEngine();
            var pc = HostingHelpers.GetLanguageContext(engine) as PythonContext;
            var hooks = pc?.SystemState.Get__dict__()["path_hooks"] as List;
            hooks?.Clear();
            var searchPaths = engine.GetSearchPaths();
            searchPaths.Add(ConfigurationManager.AppSettings["PythonLibPath"]);
            engine.SetSearchPaths(searchPaths);
            var sc = engine.CreateScriptSourceFromString(script.Body);
#endif

            try
            {
                var code = sc.Compile();
                var scope = engine.CreateScope();
                code.Execute(scope);

                dynamic MembershipAutomation = scope.GetVariable("MembershipAutomation");
                dynamic m = MembershipAutomation();
                var ret = m.Run(Db, this);

                //var m = scope.GetVariable("MembershipAutomation");
                //dynamic instance = engine.Operations.CreateInstance(m);
                //var value = instance.Run(Db, this);
                return errorReturn;
            }
            catch (Exception ex)
            {
                return "MemberProfileAutomation script error: " + ex.Message;
            }
        }
        //public void MemberProfileAutomation0(CMSDataContext Db)
        //{
        //    if (DecisionTypeIdChanged)
        //        switch (DecisionTypeId ?? 0)
        //        {
        //            case DecisionCode.ProfessionForMembership:
        //                MemberStatusId = MemberStatusCode.Pending;
        //                if (NewMemberClassStatusId != NewMemberClassStatusCode.Attended)
        //                    NewMemberClassStatusId = NewMemberClassStatusCode.Pending;
        //                if (Age <= 12 && Family.People.Any(p =>
        //                        p.PositionInFamilyId == PositionInFamily.PrimaryAdult
        //                        && p.MemberStatusId == MemberStatusCode.Member
        //                        && SqlMethods.DateDiffMonth(p.JoinDate, Util.Now) >= 12))
        //                    BaptismTypeId = BaptismTypeCode.Biological;
        //                else
        //                    BaptismTypeId = BaptismTypeCode.Original;
        //                BaptismStatusId = BaptismStatusCode.NotScheduled;
        //                break;
        //            case DecisionCode.ProfessionNotForMembership:
        //                MemberStatusId = MemberStatusCode.NotMember;
        //                if (NewMemberClassStatusId != NewMemberClassStatusCode.Attended)
        //                    NewMemberClassStatusId = NewMemberClassStatusCode.NotSpecified;
        //                if (BaptismStatusId != BaptismStatusCode.Completed)
        //                {
        //                    BaptismTypeId = BaptismTypeCode.NonMember;
        //                    BaptismStatusId = BaptismStatusCode.NotScheduled;
        //                }
        //                break;
        //            case DecisionCode.Letter:
        //                MemberStatusId = MemberStatusCode.Pending;
        //                if (NewMemberClassStatusId != NewMemberClassStatusCode.Attended)
        //                    NewMemberClassStatusId = NewMemberClassStatusCode.Pending;
        //                if (BaptismStatusId != BaptismStatusCode.Completed)
        //                {
        //                    BaptismTypeId = BaptismTypeCode.NotSpecified;
        //                    BaptismStatusId = BaptismStatusCode.NotSpecified;
        //                }
        //                break;
        //            case DecisionCode.Statement:
        //                MemberStatusId = MemberStatusCode.Pending;
        //                if (NewMemberClassStatusId != NewMemberClassStatusCode.Attended)
        //                    NewMemberClassStatusId = NewMemberClassStatusCode.Pending;
        //                if (BaptismStatusId != BaptismStatusCode.Completed)
        //                {
        //                    BaptismTypeId = BaptismTypeCode.NotSpecified;
        //                    BaptismStatusId = BaptismStatusCode.NotSpecified;
        //                }
        //                break;
        //            case DecisionCode.StatementReqBaptism:
        //                MemberStatusId = MemberStatusCode.Pending;
        //                if (NewMemberClassStatusId != NewMemberClassStatusCode.Attended)
        //                    NewMemberClassStatusId = NewMemberClassStatusCode.Pending;
        //                if (BaptismStatusId != BaptismStatusCode.Completed)
        //                {
        //                    BaptismTypeId = BaptismTypeCode.Required;
        //                    BaptismStatusId = BaptismStatusCode.NotScheduled;
        //                }
        //                break;
        //            case DecisionCode.Cancelled:
        //                MemberStatusId = MemberStatusCode.NotMember;
        //                if (NewMemberClassStatusId != NewMemberClassStatusCode.Attended)
        //                    NewMemberClassStatusId = NewMemberClassStatusCode.NotSpecified;
        //                if (BaptismStatusId != BaptismStatusCode.Completed)
        //                    if (BaptismStatusId != BaptismStatusCode.Completed)
        //                    {
        //                        BaptismTypeId = BaptismTypeCode.NotSpecified;
        //                        BaptismStatusId = BaptismStatusCode.Canceled;
        //                    }
        //                EnvelopeOptionsId = EnvelopeOptionCode.None;
        //                break;
        //        }
        //    // This section sets join codes
        //    if (NewMemberClassStatusIdChanged || BaptismStatusIdChanged)
        //        switch (DecisionTypeId ?? 0)
        //        {
        //            case DecisionCode.ProfessionForMembership:
        //                if (DiscClassStatusCompletedCodes.Contains(NewMemberClassStatusId ?? 0)
        //                    && BaptismStatusId == BaptismStatusCode.Completed)
        //                {
        //                    MemberStatusId = MemberStatusCode.Member;
        //                    if (BaptismTypeId == BaptismTypeCode.Biological)
        //                        JoinCodeId = JoinTypeCode.BaptismBIO;
        //                    else
        //                        JoinCodeId = JoinTypeCode.BaptismPOF;
        //                    if (NewMemberClassDate.HasValue && BaptismDate.HasValue)
        //                        JoinDate = NewMemberClassDate.Value > BaptismDate.Value ?
        //                            NewMemberClassDate.Value : BaptismDate.Value;
        //                }
        //                break;
        //            case DecisionCode.Letter:
        //                if (NewMemberClassStatusIdChanged)
        //                    if (DiscClassStatusCompletedCodes.Contains(NewMemberClassStatusId ?? 0)
        //                        || NewMemberClassStatusId == NewMemberClassStatusCode.AdminApproval)
        //                    {
        //                        MemberStatusId = MemberStatusCode.Member;
        //                        JoinCodeId = JoinTypeCode.Letter;
        //                        JoinDate = NewMemberClassDate.HasValue ? NewMemberClassDate : DecisionDate;
        //                    }
        //                break;
        //            case DecisionCode.Statement:
        //                if (NewMemberClassStatusIdChanged)
        //                    if (DiscClassStatusCompletedCodes.Contains(NewMemberClassStatusId ?? 0))
        //                    {
        //                        MemberStatusId = MemberStatusCode.Member;
        //                        JoinCodeId = JoinTypeCode.Statement;
        //                        JoinDate = NewMemberClassDate.HasValue ? NewMemberClassDate : DecisionDate;
        //                    }
        //                break;
        //            case DecisionCode.StatementReqBaptism:
        //                if (DiscClassStatusCompletedCodes.Contains(NewMemberClassStatusId ?? 0)
        //                    && BaptismStatusId == BaptismStatusCode.Completed)
        //                {
        //                    MemberStatusId = MemberStatusCode.Member;
        //                    JoinCodeId = JoinTypeCode.BaptismSRB;
        //                    if (NewMemberClassDate.HasValue)
        //                        JoinDate = NewMemberClassDate.Value > BaptismDate.Value ?
        //                            NewMemberClassDate.Value : BaptismDate.Value;
        //                    else
        //                        JoinDate = BaptismDate;
        //                }
        //                break;
        //        }
        //    if (DeceasedDateChanged)
        //    {
        //        if (DeceasedDate.HasValue)
        //            DeceasePerson(Db);
        //    }
        //    else if (DropCodeIdChanged)
        //    {
        //        switch (DropCodeId)
        //        {
        //            case DropTypeCode.Administrative:
        //                DropMembership(Db);
        //                break;
        //            case DropTypeCode.AnotherDenomination:
        //                DropMembership(Db);
        //                break;
        //            case DropTypeCode.Duplicate:
        //                DropMembership(Db);
        //                MemberStatusId = MemberStatusCode.NotMember;
        //                break;
        //            case DropTypeCode.LetteredOut:
        //                DropMembership(Db);
        //                break;
        //            case DropTypeCode.Other:
        //                DropMembership(Db);
        //                break;
        //            case DropTypeCode.Requested:
        //                DropMembership(Db);
        //                break;
        //        }
        //    }
        //    if (NewMemberClassStatusIdChanged
        //        && NewMemberClassStatusId == NewMemberClassStatusCode.Attended)
        //    {
        //        var q = from om in Db.OrganizationMembers
        //                where om.PeopleId == PeopleId
        //                where om.Organization.OrganizationName == "Step 1"
        //                select om;
        //        foreach (var om in q)
        //            om.Drop(Db, addToHistory: true);
        //    }
        //}
        public void DropMemberships(CMSDataContext Db)
        {
            if (MemberStatusId == MemberStatusCode.Member)
            {
                if (Deceased)
                    DropCodeId = DropTypeCode.Deceased;
                MemberStatusId = MemberStatusCode.Previous;
                DropDate = Util.Now.Date;
            }
            if (Deceased)
            {
                EmailAddress = null;
                DoNotCallFlag = true;
                DoNotMailFlag = true;
                DoNotVisitFlag = true;
            }
            if (SpouseId.HasValue)
            {
                var spouse = Db.LoadPersonById(SpouseId.Value);
                if (Deceased)
                {
                    spouse.MaritalStatusId = MaritalStatusCode.Widowed;
                    if (spouse.EnvelopeOptionsId.HasValue)
                        if (spouse.EnvelopeOptionsId != StatementOptionCode.None)
                            spouse.EnvelopeOptionsId = StatementOptionCode.Individual;
                    spouse.ContributionOptionsId = StatementOptionCode.Individual;
                }

                if (spouse.MemberStatusId == MemberStatusCode.Member)
                    if (spouse.EnvelopeOptionsId == StatementOptionCode.Joint)
                        spouse.EnvelopeOptionsId = StatementOptionCode.Individual;
            }
            EnvelopeOptionsId = StatementOptionCode.None;

            DropAllMemberships(Db);
        }
    }
}
