using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.ExtraValue;
using CmsWeb.Code;

namespace CmsWeb.Areas.Org.Models.Org
{
    public class SettingsCommunityGroupModel
    {
        public Organization Org;

        public int Id
        {
            get { return Org != null ? Org.OrganizationId : 0; }
            set
            {
                if (Org == null)
                {
                    Org = DbUtil.Db.LoadOrganizationById(value);
                    LoadValues();
                }
            }
        }

        public string GroupType { get; set; }
        public string Term { get; set; }
        public string Region { get; set; }
        public string Neighborhood { get; set; }
        public string CrossStreets { get; set; }
        public string VisitNotes { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public int? SizeLimit { get; set; }
        public bool HasChildcare { get; set; }

        private List<SelectListItem> _groupTypes;
        private List<SelectListItem> _terms;
        private List<SelectListItem> _regions;

        public IEnumerable<SelectListItem> GroupTypes()
        {
            return _groupTypes;
        }

        public IEnumerable<SelectListItem> Terms()
        {
            return _terms;
        }

        public IEnumerable<SelectListItem> Regions()
        {
            return _regions;
        }

        private static SelectListItem CreateBasicListItem(string name, string currentValue)
        {
            return new SelectListItem
            {
                Text = name,
                Value = name,
                Selected = currentValue == name
            };
        }

        public SettingsCommunityGroupModel()
        {
            
        }

        public SettingsCommunityGroupModel(int id)
        {
            Id = id;
        }

        private void LoadValues()
        {
            var extras = Org.GetOrganizationExtras().ToList();

            GroupType = (extras.FirstOrDefault(x => x.Field == "Group Type") ?? new OrganizationExtra()).StrValue;
            Term = (extras.FirstOrDefault(x => x.Field == "Term") ?? new OrganizationExtra()).StrValue;
            Region = (extras.FirstOrDefault(x => x.Field == "Region") ?? new OrganizationExtra()).StrValue;
            Neighborhood = (extras.FirstOrDefault(x => x.Field == "Neighborhood") ?? new OrganizationExtra()).Data;
            CrossStreets = (extras.FirstOrDefault(x => x.Field == "Cross-Streets") ?? new OrganizationExtra()).Data;
            VisitNotes = (extras.FirstOrDefault(x => x.Field == "Visit Notes") ?? new OrganizationExtra()).Data;
            LastVisitDate = (extras.FirstOrDefault(x => x.Field == "Last Visit Date") ?? new OrganizationExtra()).DateValue;
            SizeLimit = (extras.FirstOrDefault(x => x.Field == "Size Limit") ?? new OrganizationExtra()).IntValue;
            HasChildcare = (extras.FirstOrDefault(x => x.Field == "Has Childcare") ?? new OrganizationExtra()).BitValue ?? false;

            var fields = Views.GetStandardExtraValues(DbUtil.Db, "Organization");

            var groupTypeField = fields.FirstOrDefault(x => x.Name == "Group Type");
            _groupTypes = new List<SelectListItem>();
            groupTypeField?.Codes.ForEach(x => _groupTypes.Add(CreateBasicListItem(x, GroupType)));

            var termField = fields.FirstOrDefault(x => x.Name == "Term");
            _terms = new List<SelectListItem>();
            termField?.Codes.ForEach(x => _terms.Add(CreateBasicListItem(x, Term)));

            var regionField = fields.FirstOrDefault(x => x.Name == "Region");
            _regions = new List<SelectListItem>();
            regionField?.Codes.ForEach(x => _regions.Add(CreateBasicListItem(x, Region)));
        }

        public void Update()
        {
            Org.AddEditExtraValue("Group Type", GroupType, null, GroupType, null, null);
            Org.AddEditExtraValue("Region", Region, null, Region, null, null);
            Org.AddEditExtraValue("Term", Term, null, Term, null, null);

            Org.AddEditExtra(DbUtil.Db, "Neighborhood", Neighborhood);
            Org.AddEditExtra(DbUtil.Db, "Cross-Streets", CrossStreets);
            Org.AddEditExtra(DbUtil.Db, "Visit Notes", VisitNotes, true);
            Org.AddEditExtraDate("Last Visit Date", LastVisitDate);
            Org.AddEditExtraInt("Size Limit", SizeLimit.GetValueOrDefault(0));
            Org.AddEditExtraBool("Has Childcare", HasChildcare);

            DbUtil.Db.SubmitChanges();
        }
    }
}