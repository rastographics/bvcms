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
		Dictionary<string, SearchItem> search;
		List<int> divList = new List<int>();

		string sTemplate;
		string sGutter;
		string sShell;

		public void load(string sName)
		{
			var xml = DbUtil.Content("SGF-" + sName + ".xml", "");

			var xs = new XmlSerializer(typeof(SmallGroupFinder), new XmlRootAttribute("SGF"));
			var sr = new StringReader(xml);
			sgf = (SmallGroupFinder)xs.Deserialize(sr);

			string[] divs = sgf.divisionid.Split(',');
			foreach (var div in divs)
			{
				divList.Add(Convert.ToInt32(div));
			}

			sShell = DbUtil.Content(sgf.shell, "");
			sTemplate = DbUtil.Content(sgf.layout, "");
			sGutter = DbUtil.Content(sgf.gutter, "");
		}

		public Boolean hasShell()
		{
			if (sShell != null && sShell.Length > 0)
				return true;
			else
				return false;
		}

		public String createFromShell()
		{
			sShell = sShell.Replace("[SGF:Gutter]", getGutter());
			sShell = sShell.Replace("[SGF:Form]", getForm());
			sShell = sShell.Replace("[SGF:Groups]", getGroupList());

			if (search != null)
			{
				foreach (var entry in search)
				{
					if (entry.Value.parse)
					{
						foreach (var value in entry.Value.values)
						{
							sShell = sShell.Replace("[" + entry.Key + ":" + value + "]", "checked=\"checked\"");
						}
					}
					else
					{
						sShell = sShell.Replace("[" + entry.Key + ":" + entry.Value.values[0] + "]", "checked=\"checked\"");
					}
				}

				sShell = Regex.Replace(sShell, GroupLookup.PATTERN_CLEAN_CHECKED, "");
			}

			return sShell;
		}

		public void setSearch(Dictionary<string, SearchItem> newserach)
		{
			search = newserach;
		}

		/*
		public void setDefaultSearch()
		{
			search = new Dictionary<string, string>();

			foreach (var item in getFilters())
			{
				if (item.locked)
					search.Add(item.name, item.lockedvalue);
			}
		}
		*/

		public bool IsSelectedValue(string key, string value)
		{
			if (search == null) return false;

			if (search.ContainsKey(key))
			{
				if (search[key].values.Contains(value))
					return true;
				else
					return false;
			}
			else
				return false;
		}

		public List<Division> getDivisions()
		{
			return (from e in DbUtil.Db.Divisions
					  where divList.Contains(e.Id)
					  select e).ToList();
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
					  //where e.Organization.DivOrgs.Any(ee => divList.Contains(ee.DivId))
					  where e.Field == f.name
                      orderby e.Data
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
			if (search == null) return new List<Organization>();

		    var orgTypes = DbUtil.Db.Setting("SGF-OrgTypes", "").Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x));

		    IQueryable<Organization> orgs;
		    if (!orgTypes.Any())
		    {
                orgs = from o in DbUtil.Db.Organizations
                           where o.DivOrgs.Any(ee => divList.Contains(ee.DivId))
                           //where o.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Active
                           select o;
            }
		    else
		    {
                orgs = from o in DbUtil.Db.Organizations
                           where orgTypes.Contains(o.OrganizationType.Description)
                           //where o.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Active
                           select o;
            }

			foreach (var filter in search)
			{
				if (filter.Value.values.Contains(SHOW_ALL)) continue;

				if (filter.Value.parse)
				{
					orgs = from g in orgs
							 where g.OrganizationExtras.Any(oe => oe.Field == filter.Key && filter.Value.values.Contains(oe.Data))
							 select g;
				}
				else
				{
					orgs = from g in orgs
							 where g.OrganizationExtras.Any(oe => oe.OrganizationId == g.OrganizationId && oe.Field == filter.Key && oe.Data == filter.Value.values[0])
							 select g;
				}
			}

			return orgs.OrderBy(gg => gg.OrganizationName).ToList<Organization>();
		}

		public string replaceAndWrite(GroupLookup gl)
		{
			string temp = HttpUtility.HtmlDecode(string.Copy(sTemplate));

			foreach (var item in gl.values)
			{
				temp = temp.Replace("[" + item.Key + "]", item.Value);
			}

			temp = Regex.Replace(temp, GroupLookup.PATTERN_CLEAN, "");

			return temp;
		}

		public string getGutter()
		{
			return sGutter;
		}

		public string getForm()
		{
			string sForm = "<form method=\"post\"><table class=\"sgfform\">";

			for (var iX = 0; iX < getCount(SmallGroupFinderModel.TYPE_FILTER); iX++)
			{
				var f = getFilter(iX);
				var fi = getFilterItems(iX);

				sForm += "<tr class=\"sgftr\">";
				sForm += "<td class=\"sgftdlabel\">" + f.title + ":</td>";
				sForm += "<td class=\"sgftdfield\">";
				sForm += "<select name=" + f.name + ">";

				foreach (var item in fi)
				{
					sForm += "<option " + (IsSelectedValue(f.name, item.value) ? "selected" : "") + ">" + item.value + "</option>";
				}

				sForm += "</select></td></tr>";
			}

			var submitText = getSetting("SubmitText");

			sForm += "<tr><td colspan=\"2\" class=\"sgfsubmitholder\"><input class=\"sgfsubmitbutton\" type=\"submit\" value=\"" + (submitText != null ? submitText.value : "Find Groups") + "\" /></td></tr>";
			sForm += "</table></form>";

			return sForm;
		}

		public string getGroupList()
		{
			string sList = "";

			foreach (var group in getGroups())
			{
				GroupLookup gl = new GroupLookup();
				gl.populateFromOrg(group);
				sList += replaceAndWrite(gl);
			}

			return sList;
		}
	}

	public class FilterItem
	{
		public string value;
	}

	public class GroupLookup
	{
		public const string PATTERN_CLEAN = @"\[SGF:\w*\]";
		public const string PATTERN_CLEAN_CHECKED = @"\[SGF:\w*:\w*\]";
		public static string[] DAY_LAST = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "", "", "", "Any" };

		public Dictionary<string, string> values = new Dictionary<string, string>();

		public void populateFromOrg(Organization org)
		{
			var leader = (from e in DbUtil.Db.People
							  where e.PeopleId == org.LeaderId
							  select e).SingleOrDefault();

			values["SGF:OrgID"] = org.OrganizationId.ToString();
			values["SGF:Name"] = org.OrganizationName;
			values["SGF:Description"] = org.Description;
			values["SGF:Room"] = org.Location;
			values["SGF:Leader"] = org.LeaderName;
			values["SGF:DateStamp"] = DateTime.Now.ToString("yyyy-MM-dd");
			values["SGF:Schedule"] = "";

			if (leader != null && leader.PictureId != null)
				values["SGF:LeaderPicSrc"] = "/Portrait/" + leader.Picture.SmallId.Value + "?v=" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
			else
				values["SGF:LeaderPicSrc"] = "/Portrait/-3";

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

		    var loadAllValues = DbUtil.Db.Setting("SGF-LoadAllExtraValues", false);

			foreach (var extra in org.OrganizationExtras)
			{
				if (extra.Field.StartsWith("SGF:"))
					values[extra.Field] = extra.Data;
                else if(loadAllValues)
                    values[$"SGF:{extra.Field.Replace(" ", "")}"] = extra.Data;
            }
		}
	}

	public class SearchItem
	{
		public string name = "";
		public List<string> values = new List<string>();

		public bool parse = false;
	}
}