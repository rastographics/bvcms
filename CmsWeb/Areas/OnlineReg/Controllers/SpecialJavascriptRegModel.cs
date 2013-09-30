using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmsData;
using CmsData.Registration;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    class SpecialJavascriptRegModel
    {
		public int OrgId { get; set; }
		public int PeopleId { get; set; }

        public SpecialJavascriptRegModel(int orgId, int peopleId)
        {
            
        }
		private Organization _org;

		public Organization Org
		{
			get
			{
				return _org ??
					(_org = DbUtil.Db.Organizations.Single(oo => oo.OrganizationId == OrgId));
			}
		}

		private Settings _regsettings;

		public Settings Regsettings
		{
			get
			{
				return _regsettings ??
					(_regsettings = new Settings(Org.RegSetting, DbUtil.Db, OrgId));
			}
		}

		private Person _person;

		public Person Person
		{
			get { return _person ?? (_person = DbUtil.Db.People.Single(pp => pp.PeopleId == PeopleId)); }
		}

    }
}
