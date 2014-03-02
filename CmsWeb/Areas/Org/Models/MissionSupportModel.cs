using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsData.Registration;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class MissionSupportModel
    {
        private int? orgId;
        private int? peopleId;
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
            if(Goer == null)
                Goer = new CodeInfo(0, GoerList());
        }

        //public OrgMemberModel OrgMemberModel { get { return new OrgMemberModel() { PeopleId = PeopleId, OrgId = OrgId };} }

        public CodeInfo Goer { get; set; }

        public int? OrgId
        {
            get { return orgId; }
            set
            {
                orgId = value;
                if (peopleId.HasValue)
                    Populate();
            }
        }
        public int? PeopleId
        {
            get { return peopleId; }
            set
            {
                peopleId = value;
                if (orgId.HasValue)
                    Populate();
            }
        }
        public string Name { get; set; }
        public string OrgName { get; set; }
        public string CheckNo { get; set; }
        public decimal? AmountGoer { get; set; }
        public decimal? AmountGeneral { get; set; }

        public IEnumerable<CodeValueItem> GoerList()
        {
            var q = from om in DbUtil.Db.OrganizationMembers
                where om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == "Goer")
                where om.OrganizationId == OrgId
                select new CodeValueItem()
                {
                    Id = om.PeopleId,
                    Value = om.Person.Name,
                };
            var list = q.ToList();
            list.Insert(0, new CodeValueItem() { Id=0, Value = "(please select a Goer)" });
            return list;
        }

        public string ToGoerName;
        internal void PostContribution()
        {
            if (AmountGeneral > 0 || AmountGoer > 0)
            {

                var org = DbUtil.Db.LoadOrganizationById(OrgId);
                var notifyIds = DbUtil.Db.NotifyIds(org.OrganizationId, org.GiftNotifyIds);
                var person = DbUtil.Db.LoadPersonById(PeopleId ?? 0);
                var setting = new Settings(org.RegSetting, DbUtil.Db, org.OrganizationId);
                var fund = setting.DonationFundId;
                if (AmountGoer > 0)
                {
                    DbUtil.Db.GoerSenderAmounts.InsertOnSubmit(
                        new GoerSenderAmount 
                        {
                            Amount = AmountGoer,
                            GoerId = Goer.Value.ToInt(),
                            Created = DateTime.Now,
                            OrgId = org.OrganizationId,
                            SupporterId = PeopleId ?? 0,
                        });
                    var c = person.PostUnattendedContribution(DbUtil.Db,
                        AmountGoer ?? 0, fund, 
                        "SupportMissionTrip: org={0}; goer={1}".Fmt(OrgId, Goer.Value));
                    c.CheckNo = CheckNo;
                    // send notices
                    var goer = DbUtil.Db.LoadPersonById(Goer.Value.ToInt());
                    ToGoerName = "to " + goer.Name;
                    DbUtil.Db.Email(notifyIds[0].FromEmail, goer, org.OrganizationName + "-donation",
                        "{0:C} donation received from {1}".Fmt(AmountGoer, person.Name));
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
                        "SupportMissionTrip: org={0}".Fmt(OrgId));
                    c.CheckNo = CheckNo;
                }
                DbUtil.Db.SubmitChanges();
            }
        }
    }
}
