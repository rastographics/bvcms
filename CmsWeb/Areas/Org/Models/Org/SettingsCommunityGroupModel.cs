using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
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

        // TODO Fetch from the extras system instead of hard coding
        public IEnumerable<SelectListItem> GroupTypes()
        {
            var list = new List<SelectListItem>();

            list.Add(CreateBasicListItem("Women", GroupType));
            list.Add(CreateBasicListItem("Men", GroupType));
            list.Add(CreateBasicListItem("Co-ed", GroupType));
            list.Add(CreateBasicListItem("Women over 50", GroupType));

            return list;
        }

        // TODO Fetch from the extras system instead of hard coding
        public IEnumerable<SelectListItem> Terms()
        {
            var list = new List<SelectListItem>();

            list.Add(CreateBasicListItem("Community Group", Term));
            list.Add(CreateBasicListItem("Beta Group", Term));

            return list;
        }

        // TODO Fetch from the extras system instead of hard coding
        public IEnumerable<SelectListItem> Regions()
        {
            var list = new List<SelectListItem>();

            list.Add(CreateBasicListItem("Downtown", Region));
            list.Add(CreateBasicListItem("Bronx", Region));
            list.Add(CreateBasicListItem("Brooklyn", Region));

            return list;
        }

        private SelectListItem CreateBasicListItem(string name, string currentValue)
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
        }

        public void Update()
        {
            Org.AddEditExtraValue("Group Type", GroupType, null, GroupType, null, null);
            Org.AddEditExtraValue("Region", Region, null, Region, null, null);
            Org.AddEditExtraValue("Term", Term, null, Term, null, null);

            //Org.AddEditExtra(DbUtil.Db, "Group Type", GroupType);
            //Org.AddEditExtra(DbUtil.Db, "Term", Term);
            //Org.AddEditExtra(DbUtil.Db, "Region", Region);
            Org.AddEditExtra(DbUtil.Db, "Neighborhood", Neighborhood);
            Org.AddEditExtra(DbUtil.Db, "Cross-Streets", CrossStreets);
            Org.AddEditExtra(DbUtil.Db, "Visit Notes", VisitNotes, true);
            Org.AddEditExtraDate("Last Visit Date", LastVisitDate);
            Org.AddEditExtraInt("Size Limit", SizeLimit.GetValueOrDefault(0));
            Org.AddEditExtraBool("Has Childcare", HasChildcare);

            /*UpdateExtraValue("Group Type", GroupType);
            UpdateExtraValue("Term", Term);
            UpdateExtraValue("Region", Region);
            UpdateExtraValue("Neighborhood", Neighborhood);
            UpdateExtraValue("Cross-Streets", CrossStreets);
            UpdateExtraValue("Visit Notes", VisitNotes, true);
            UpdateExtraValue("Last Visit Date", LastVisitDate.ToString());
            UpdateExtraValue("Size Limit", SizeLimit.ToString());
            UpdateExtraValue("Has Childcare", HasChildcare.ToString());*/

            DbUtil.Db.SubmitChanges();
        }

        /*private void UpdateExtraValue(string field, string value, bool multiline = false)
        {
            var e = DbUtil.Db.OrganizationExtras.SingleOrDefault(ee => ee.OrganizationId == Id && ee.Field == field);
            if (e == null)
            {
                Org.AddEditExtra(DbUtil.Db, field, value, multiline);
            }
            else
            {
                e.Data = value;
            }
        }*/
    }
}