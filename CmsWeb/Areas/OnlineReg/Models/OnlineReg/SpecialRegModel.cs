using System.Linq;
using CmsData;
using CmsData.Registration;

namespace CmsWeb.Models
{
	public class SpecialRegModel
	{
		public int orgID { get; set; }
		public int peopleID { get; set; }

		private Organization _org;
		public Organization Org
		{
			get
			{
				return _org ??
					(_org = DbUtil.Db.Organizations.Single(oo => oo.OrganizationId == orgID));
			}
		}

		private Settings _regsettings;
		public Settings Regsettings
		{
			get
			{
				return _regsettings ??
					(_regsettings = new Settings(Org.RegSetting, DbUtil.Db, orgID));
			}
		}

		private Person _person;
		public Person Person
		{
			get { return _person ?? (_person = DbUtil.Db.People.Single(pp => pp.PeopleId == peopleID)); }
		}
	}
}