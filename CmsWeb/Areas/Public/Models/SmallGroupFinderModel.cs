using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using CmsData;
using CmsData.Classes.SmallGroupFinder;
using System.Web;
using System;
using MoreLinq;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Models
{
	public class SmallGroupFinderModel
	{
		public const int TYPE_SETTING = 1;
		public const int TYPE_FILTER = 2;

		public const string SHOW_ALL = "-- All --";

		SmallGroupFinder sgf;
		Dictionary<string, string> search;

		string sTemplate;
		string sGutter;

		public void load(string sName)
		{
			var xml = DbUtil.Content("SGF-" + sName + ".xml", "");

			var xs = new XmlSerializer(typeof(SmallGroupFinder), new XmlRootAttribute("SGF"));
			var sr = new StringReader(xml);
			sgf = (SmallGroupFinder)xs.Deserialize(sr);

			sTemplate = DbUtil.Content(sgf.layout, "");
			sGutter = DbUtil.Content(sgf.gutter, "");
		}

		public void setSearch(Dictionary<string, string> newserach)
		{
			search = newserach;
		}

		public void setDefaultSearch()
		{
			search = new Dictionary<string, string>();

			foreach (var item in getFilters())
			{
				if (item.locked)
					search.Add(item.name, item.lockedvalue);
			}
		}

		public bool IsSelectedValue(string key, string value)
		{
			if (search.ContainsKey(key))
			{
				if (search[key] == value)
					return true;
				else
					return false;
			}
			else
				return false;
		}

		public Division getDivision()
		{
			var d = (from e in DbUtil.Db.Divisions
						where e.Id == sgf.divisionid
						select e).SingleOrDefault();

			return d;
		}

		public int getCount(int type)
		{
			if (sgf == null) return 0;

			switch (type)
			{
				case TYPE_SETTING:
					{
						return sgf.SGFSettings.Count();
					}

				case TYPE_FILTER:
					{
						return sgf.SGFFilters.Count();
					}

				default: return 0;
			}
		}

		public List<SGFSetting> getSettings()
		{
			return sgf.SGFSettings;
		}

		public SGFSetting getSetting(int id)
		{
			return sgf.SGFSettings[id];
		}

		public SGFSetting getSetting(string name)
		{
			return (from s in sgf.SGFSettings where s.name == name select s).FirstOrDefault();
		}

		public List<SGFFilter> getFilters()
		{
			return sgf.SGFFilters;
		}

		public SGFFilter getFilter(int id)
		{
			return sgf.SGFFilters[id];
		}

		public List<FilterItem> getFilterItems(int id)
		{
			var f = getFilter(id);
			List<FilterItem> i = new List<FilterItem>();

			if (f.locked)
			{
				i.Add(new FilterItem { value = f.lockedvalue });
			}
			else
			{
				i = (from e in DbUtil.Db.OrganizationExtras
					  where e.Organization.DivOrgs.Any(ee => ee.DivId == sgf.divisionid)
					  where e.Field == f.name
					  select new FilterItem
					  {
						  value = e.Data
					  }).DistinctBy(n => n.value).ToList<FilterItem>();

				i.Insert(0, new FilterItem { value = SHOW_ALL });
			}

			return i;
		}

		public List<Organization> getGroups()
		{
			var orgs = from o in DbUtil.Db.Organizations
						  where o.DivOrgs.Any(ee => ee.DivId == sgf.divisionid)
						  select o;

			foreach (var filter in search)
			{
				if (filter.Value == SHOW_ALL) continue;

				orgs = from g in orgs
						 where g.OrganizationExtras.Any(oe => oe.Field == filter.Key && oe.Data == filter.Value)
						 select g;
			}

			return orgs.ToList<Organization>();
		}

		public string ReplaceAndWrite(GroupLookup gl)
		{
			string temp = HttpUtility.HtmlDecode(string.Copy(sTemplate));

			foreach (var item in gl.values)
			{
				temp = temp.Replace("[" + item.Key + "]", item.Value);
			}

			temp = Regex.Replace(temp, GroupLookup.PATTERN_CLEAN, "");

			return temp;
		}

		public string GetGutter()
		{
			return sGutter;
		}
	}

	public class FilterItem
	{
		public string value;
	}

	public class GroupLookup
	{
		public const string PATTERN_CLEAN = @"\[SGF:\w*\]";
		public static string[] DAY_LAST = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "", "", "", "Any" };

		public Dictionary<string, string> values = new Dictionary<string, string>();

		public void populateFromOrg(Organization org)
		{
			values["SGF:OrgID"] = org.OrganizationId.ToString();
			values["SGF:Name"] = org.OrganizationName;
			values["SGF:Description"] = org.Description;
			values["SGF:Room"] = org.Location;
			values["SGF:Leader"] = org.LeaderName;
			values["SGF:DateStamp"] = DateTime.Now.ToString("yyyy-MM-dd");
			values["SGF:Schedule"] = "";

			if (org.OrgSchedules.Count > 0)
			{
				int count = 0;
				foreach (var schedule in org.OrgSchedules)
				{
					if (count > 0) values["SGF:Schedule"] += "; ";
					values["SGF:Schedule"] += DAY_LAST[schedule.SchedDay ?? 0] + ", " + schedule.SchedTime.ToString2("t"); ;
					count++;
				}
			}

			foreach (var extra in org.OrganizationExtras)
			{
				if (extra.Field.StartsWith("SGF:"))
					values[extra.Field] = extra.Data;
			}
		}
	}
}