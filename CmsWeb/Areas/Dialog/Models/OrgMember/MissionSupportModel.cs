using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Code;
using CmsWeb.Constants;
using CmsWeb.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class MissionSupportModel : IDbBinder
    {
        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public MissionSupportModel() { }
        public MissionSupportModel(CMSDataContext db ) { CurrentDatabase = db; }
        private CMSDataContext _currentDatabase;
        public CMSDataContext CurrentDatabase
        {
            get => _currentDatabase ?? DbUtil.Db;
            set{ _currentDatabase = value; }
        }

        private int? orgId;
        private int? peopleId;
        
        private void Populate()
        {
            var i = (from mm in CurrentDatabase.OrganizationMembers
                     where mm.OrganizationId == OrgId && mm.PeopleId == PeopleId
                     select new
                     {
                         mm.Person.Name,
                         mm.Organization.OrganizationName,
                     }).Single();
            Name = i.Name;
            OrgName = i.OrganizationName;
            if (Goer == null)
            {
                Goer = new CodeInfo(0, GoerList());
            }
        }

        public CodeInfo Goer { get; set; }

        public string Group { get; set; }

        public int? OrgId
        {
            get { return orgId; }
            set
            {
                orgId = value;
                if (peopleId.HasValue)
                {
                    Populate();
                }
            }
        }
        public int? PeopleId
        {
            get { return peopleId; }
            set
            {
                peopleId = value;
                if (orgId.HasValue)
                {
                    Populate();
                }
            }
        }
        public string Name { get; set; }
        public string OrgName { get; set; }
        public string CheckNo { get; set; }
        public decimal? AmountGoer { get; set; }
        public decimal? AmountGeneral { get; set; }

        public SelectList GoerList()
        {
            var q = from om in CurrentDatabase.OrganizationMembers
                    where om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == "Goer")
                    where om.OrganizationId == OrgId
                    orderby om.Person.Name2
                    select new CodeValueItem
                    {
                        Id = om.PeopleId,
                        Value = om.Person.Name2,
                    };
            var list = q.ToList();
            list.Insert(0, new CodeValueItem { Id = 0, Value = "(please select a Goer)" });
            return list.ToSelect();
        }

        public string ToGoerName;
        internal void PostContribution()
        {
            if (!(AmountGeneral > 0) && !(AmountGoer > 0))
            {
                return;
            }

            var org = CurrentDatabase.LoadOrganizationById(OrgId);
            var notifyIds = CurrentDatabase.NotifyIds(org.GiftNotifyIds);
            var person = CurrentDatabase.LoadPersonById(PeopleId ?? 0);
            var setting = CurrentDatabase.CreateRegistrationSettings(OrgId ?? 0);
            var fund = setting.DonationFundId;
            if (AmountGoer > 0)
            {
                var goerid = Goer.Value.ToInt();
                CurrentDatabase.GoerSenderAmounts.InsertOnSubmit(
                    new GoerSenderAmount
                    {
                        Amount = AmountGoer,
                        GoerId = goerid,
                        Created = DateTime.Now,
                        OrgId = org.OrganizationId,
                        SupporterId = PeopleId ?? 0,
                    });
                var c = person.PostUnattendedContribution(CurrentDatabase,
                    AmountGoer ?? 0, fund,
                    $"SupportMissionTrip: org={OrgId}; goer={Goer.Value}", typecode: BundleTypeCode.MissionTrip);
                c.CheckNo = (CheckNo ?? "").Trim().Truncate(20);
                if (PeopleId == goerid)
                {
                    var om = CurrentDatabase.OrganizationMembers.Single(
                        mm => mm.PeopleId == goerid && mm.OrganizationId == OrgId);
                    var descriptionForPayment = OnlineRegModel.GetDescriptionForPayment(OrgId, CurrentDatabase);
                    om.AddTransaction(CurrentDatabase, "Payment", AmountGoer ?? 0, "Payment", pmtDescription: descriptionForPayment);
                }
                // send notices
                var goer = CurrentDatabase.LoadPersonById(goerid);
                ToGoerName = "to " + goer.Name;
                CurrentDatabase.Email(notifyIds[0].FromEmail, goer, org.OrganizationName + "-donation",
                    $"{AmountGoer:C} donation received from {person.Name}");
                DbUtil.LogActivity("OrgMem SupportMissionTrip goer=" + goerid, OrgId, PeopleId);
            }
            if (AmountGeneral > 0)
            {
                CurrentDatabase.GoerSenderAmounts.InsertOnSubmit(
                    new GoerSenderAmount
                    {
                        Amount = AmountGeneral,
                        Created = DateTime.Now,
                        OrgId = org.OrganizationId,
                        SupporterId = PeopleId ?? 0
                    });
                var c = person.PostUnattendedContribution(CurrentDatabase,
                    AmountGeneral ?? 0, fund,
                    $"SupportMissionTrip: org={OrgId}", typecode: BundleTypeCode.MissionTrip);
                if (CheckNo.HasValue())
                {
                    c.CheckNo = (CheckNo ?? "").Trim().Truncate(20);
                }

                DbUtil.LogActivity("OrgMem SupportMissionTrip", OrgId, PeopleId);
            }
            CurrentDatabase.SubmitChanges();
        }
    }
}
