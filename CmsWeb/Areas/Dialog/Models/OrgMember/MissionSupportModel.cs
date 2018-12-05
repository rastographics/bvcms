using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Code;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class MissionSupportModel
    {
        private int? orgId;
        private int? peopleId;
        public MissionSupportModel() { }
        private void Populate()
        {
            var i = (from mm in DbUtil.Db.OrganizationMembers
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

        //public OrgMemberModel OrgMemberModel { get { return new OrgMemberModel() { PeopleId = PeopleId, OrgId = OrgId };} }

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
            var q = from om in DbUtil.Db.OrganizationMembers
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

            var org = DbUtil.Db.LoadOrganizationById(OrgId);
            var notifyIds = DbUtil.Db.NotifyIds(org.GiftNotifyIds);
            var person = DbUtil.Db.LoadPersonById(PeopleId ?? 0);
            var setting = DbUtil.Db.CreateRegistrationSettings(OrgId ?? 0);
            var fund = setting.DonationFundId;
            if (AmountGoer > 0)
            {
                var goerid = Goer.Value.ToInt();
                DbUtil.Db.GoerSenderAmounts.InsertOnSubmit(
                    new GoerSenderAmount
                    {
                        Amount = AmountGoer,
                        GoerId = goerid,
                        Created = DateTime.Now,
                        OrgId = org.OrganizationId,
                        SupporterId = PeopleId ?? 0,
                    });
                var c = person.PostUnattendedContribution(DbUtil.Db,
                    AmountGoer ?? 0, fund,
                    $"SupportMissionTrip: org={OrgId}; goer={Goer.Value}", typecode: BundleTypeCode.MissionTrip);
                c.CheckNo = (CheckNo ?? "").Trim().Truncate(20);
                if (PeopleId == goerid)
                {
                    var om = DbUtil.Db.OrganizationMembers.Single(
                        mm => mm.PeopleId == goerid && mm.OrganizationId == OrgId);
                    var descriptionForPayment = OnlineRegModel.GetDescriptionForPayment(OrgId);
                    om.AddTransaction(DbUtil.Db, "Payment", AmountGoer ?? 0, "Payment", pmtDescription: descriptionForPayment);
                }
                // send notices
                var goer = DbUtil.Db.LoadPersonById(goerid);
                ToGoerName = "to " + goer.Name;
                DbUtil.Db.Email(notifyIds[0].FromEmail, goer, org.OrganizationName + "-donation",
                    $"{AmountGoer:C} donation received from {person.Name}");
                DbUtil.LogActivity("OrgMem SupportMissionTrip goer=" + goerid, OrgId, PeopleId);
            }
            if (AmountGeneral > 0)
            {
                DbUtil.Db.GoerSenderAmounts.InsertOnSubmit(
                    new GoerSenderAmount
                    {
                        Amount = AmountGeneral,
                        Created = DateTime.Now,
                        OrgId = org.OrganizationId,
                        SupporterId = PeopleId ?? 0
                    });
                var c = person.PostUnattendedContribution(DbUtil.Db,
                    AmountGeneral ?? 0, fund,
                    $"SupportMissionTrip: org={OrgId}", typecode: BundleTypeCode.MissionTrip);
                if (CheckNo.HasValue())
                {
                    c.CheckNo = (CheckNo ?? "").Trim().Truncate(20);
                }

                DbUtil.LogActivity("OrgMem SupportMissionTrip", OrgId, PeopleId);
            }
            DbUtil.Db.SubmitChanges();
        }
    }
}
