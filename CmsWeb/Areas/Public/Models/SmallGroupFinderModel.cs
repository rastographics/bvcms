using CmsData;
using CmsData.Classes.SmallGroupFinder;
using CmsWeb.Models;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Models
{
    public class SmallGroupFinderModel
    {
        public const int TYPE_SETTING = 1;
        public const int TYPE_FILTER = 2;

        public const string SHOW_ALL = "-- All --";

        private SmallGroupFinder _sgf;
        private Dictionary<string, SearchItem> _search;
        private readonly List<int> _divList = new List<int>();

        private string _template;
        private string _gutter;
        private string _shell;
        private readonly Controller _controller;

        public string Title
        {
            get
            {
                string title = null;
                if (_sgf != null)
                {
                    title = getSetting("Title")?.value;
                }

                return title ?? "Small Group Finder";
            }
        }

        public string Subtitle
        {
            get
            {
                string subtitle = null;
                if (_sgf != null)
                {
                    var subtitleContent = getSetting("Subtitle-Content")?.value;
                    subtitle = DbUtil.Db.Content(subtitleContent)?.Body;
                }

                return subtitle ?? "";
            }
        }

        public bool UseShell { get; private set; }

        public SmallGroupFinderModel() { }

        public SmallGroupFinderModel(Controller controller, bool useShell = true)
        {
            _controller = controller;
            UseShell = useShell;
        }

        public void load(string sName)
        {
            var xml = DbUtil.Content("SGF-" + sName + ".xml", "");

            var xs = new XmlSerializer(typeof(SmallGroupFinder), new XmlRootAttribute("SGF"));
            var sr = new StringReader(xml);
            _sgf = (SmallGroupFinder)xs.Deserialize(sr);

            var divs = _sgf.divisionid?.Split(',') ?? new string[] { };
            foreach (var div in divs)
            {
                _divList.Add(Convert.ToInt32(div));
            }

            _shell = DbUtil.Content(_sgf.shell, "");
            _template = DbUtil.Content(_sgf.layout, "");
            _gutter = DbUtil.Content(_sgf.gutter, "");
        }

        public bool hasShell()
        {
            return !string.IsNullOrEmpty(_shell);
        }

        public string createFromShell()
        {
            _shell = _shell.Replace("[SGF:Gutter]", getGutter());
            _shell = _shell.Replace("[SGF:Form]", getForm());
            _shell = _shell.Replace("[SGF:Groups]", getGroupList());

            if (_search != null)
            {
                foreach (var entry in _search)
                {
                    if (entry.Value.parse)
                    {
                        foreach (var value in entry.Value.values)
                        {
                            _shell = _shell.Replace("[" + entry.Key + ":" + value + "]", "checked=\"checked\"");
                        }
                    }
                    else
                    {
                        _shell = _shell.Replace("[" + entry.Key + ":" + entry.Value.values[0] + "]", "checked=\"checked\"");
                    }
                }

                _shell = Regex.Replace(_shell, GroupLookup.PATTERN_CLEAN_CHECKED, "");
            }

            return _shell;
        }

        public void setSearch(Dictionary<string, SearchItem> newSearch)
        {
            _search = newSearch;
        }

        public bool IsSelectedValue(string key, string value)
        {
            if (_search == null)
            {
                return false;
            }

            if (_search.ContainsKey(key))
            {
                if (_search[key].values.Contains(value))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public List<Division> getDivisions()
        {
            return (from e in DbUtil.Db.Divisions
                    where _divList.Contains(e.Id)
                    select e).ToList();
        }

        public int getCount(int type)
        {
            if (_sgf == null)
            {
                return 0;
            }

            switch (type)
            {
                case TYPE_SETTING:
                    {
                        return _sgf.SGFSettings.Count();
                    }

                case TYPE_FILTER:
                    {
                        return _sgf.SGFFilters.Count();
                    }

                default: return 0;
            }
        }

        public List<SGFSetting> getSettings()
        {
            return _sgf.SGFSettings;
        }

        public SGFSetting getSetting(int id)
        {
            return _sgf.SGFSettings[id];
        }

        public SGFSetting getSetting(string name)
        {
            return (from s in _sgf.SGFSettings where s.name == name select s).FirstOrDefault();
        }

        public List<SGFFilter> getFilters()
        {
            return _sgf.SGFFilters;
        }

        public SGFFilter getFilter(int id)
        {
            return _sgf.SGFFilters[id];
        }

        private static readonly List<string> weekdayList = new List<string>()
        {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"
        };

        public List<FilterItem> getFilterItems(int id)
        {
            var orgTypes = DbUtil.Db.Setting("SGF-OrgTypes", "").Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x));

            var f = getFilter(id);
            var i = new List<FilterItem>();

            if (f.locked)
            {
                i.Add(new FilterItem { value = f.lockedvalue });
            }
            else
            {
                if (f.weekdays)
                {
                    i.AddRange(weekdayList.Select(w => new FilterItem
                    {
                        value = w
                    }));
                }
                else if (f.timeofdayonly)
                {
                    i.AddRange(new[] { "AM", "PM" }.Select(x => new FilterItem
                    {
                        value = x
                    }));
                }
                else if (f.name == "Campus")
                {
                    var campusExclusions = f.exclude?.Split(',') ?? new string[] { };
                    i = (from campus in DbUtil.Db.Campus
                         orderby campus.Description
                         where !campusExclusions.Contains(campus.Description)
                         select new FilterItem
                         {
                             value = campus.Description
                         }).ToList();
                }
                else
                {
                    i = (from e in DbUtil.Db.OrganizationExtras
                         where e.Organization.DivOrgs.Any(ee => _divList.Contains(ee.DivId)) || orgTypes.Contains(e.Organization.OrganizationType.Description)
                         where e.Field == f.name
                         orderby e.Data ??
                             e.StrValue ??
                             (e.DateValue != null ? e.DateValue.ToString() : null) ??
                             (e.IntValue != null ? e.IntValue.ToString() : null) ??
                             (e.BitValue != null ? e.BitValue.ToString() : null)
                         select e)
                         .ToList()
                         .Select(x => new FilterItem
                         {
                             value = x.Data ??
                                     x.StrValue ??
                                     x.DateValue?.ToString() ??
                                     x.IntValue?.ToString() ??
                                     x.BitValue?.ToString()
                         }).DistinctBy(n => n.value).ToList();
                }

                i.Insert(0, new FilterItem { value = SHOW_ALL });
            }

            return i;
        }

        public SmallGroupSearchResult getGroups()
        {
            if (_search == null)
            {
                return new SmallGroupSearchResult { Organizations = new List<Organization>(), IsInitialSearch = true };
            }

            var orgTypes = DbUtil.Db.Setting("SGF-OrgTypes", "").Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();

            IQueryable<Organization> orgs;
            if (!orgTypes.Any())
            {
                orgs = from o in DbUtil.Db.Organizations
                       where o.OrganizationStatusId == 30
                       && o.DivOrgs.Any(ee => _divList.Contains(ee.DivId))
                       select o;
            }
            else
            {
                orgs = from o in DbUtil.Db.Organizations
                       where o.OrganizationStatusId == 30
                       && orgTypes.Contains(o.OrganizationType.Description)
                       select o;
            }

            foreach (var filter in _search)
            {
                if (filter.Value.values.Contains(SHOW_ALL))
                {
                    continue;
                }

                if (filter.Key == "Campus")
                {
                    orgs = from g in orgs
                           where g.Campu.Description == filter.Value.values[0]
                           select g;
                }
                else if (filter.Key.EndsWith("Time"))
                {
                    var val = filter.Value.values[0];
                    orgs = from g in orgs
                           where g.OrganizationExtras
                              .Any(oe => oe.Field == filter.Key &&
                                  (
                                      oe.StrValue.ToLower().EndsWith(val) ||
                                      oe.Data.ToLower().EndsWith(val) ||
                                      oe.DateValue != null && oe.DateValue.ToString().EndsWith(val) ||
                                      oe.IntValue != null && oe.IntValue.ToString().EndsWith(val) ||
                                      oe.BitValue != null && oe.BitValue.ToString().EndsWith(val)
                                  )
                              )
                           select g;
                }
                else if (filter.Value.parse)
                {
                    var vals = filter.Value.values;
                    orgs = from g in orgs
                           where g.OrganizationExtras
                               .Any(oe => oe.Field == filter.Key &&
                                   (
                                      vals.Contains(oe.StrValue) ||
                                      vals.Contains(oe.Data) ||
                                      oe.DateValue != null && vals.Contains(oe.DateValue.ToString()) ||
                                      oe.IntValue != null && vals.Contains(oe.IntValue.ToString()) ||
                                      oe.BitValue != null && vals.Contains(oe.BitValue.ToString())
                                   )
                               )
                           select g;
                }
                else
                {
                    var val = filter.Value.values[0];
                    orgs = from g in orgs
                           where g.OrganizationExtras
                              .Any(oe => oe.OrganizationId == g.OrganizationId && oe.Field == filter.Key &&
                                  (
                                      oe.StrValue == val ||
                                      oe.Data.ToLower() == val ||
                                      oe.DateValue != null && oe.DateValue.ToString() == val ||
                                      oe.IntValue != null && oe.IntValue.ToString() == val ||
                                      oe.BitValue != null && oe.BitValue.ToString() == val
                                  )
                              )
                           select g;
                }

            }

            return new SmallGroupSearchResult
            {
                Organizations = orgs.OrderBy(gg => gg.OrganizationName).ToList(),
                IsInitialSearch = false
            };
        }

        public string replaceAndWrite(GroupLookup gl)
        {
            var temp = HttpUtility.HtmlDecode(string.Copy(_template));

            foreach (var item in gl.values)
            {
                temp = temp.Replace("[" + item.Key + "]", item.Value);
            }

            temp = Regex.Replace(temp, GroupLookup.PATTERN_CLEAN, "");

            return temp;
        }

        public string getGutter()
        {
            return _gutter;
        }

        public string RenderViewToString(string viewName, object model)
        {
            _controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(_controller.ControllerContext, viewName);
                var viewContext = new ViewContext(_controller.ControllerContext, viewResult.View, _controller.ViewData, _controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(_controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public string getForm()
        {
            return RenderViewToString("MapForm", this);
        }

        public string getGroupList()
        {
            var sList = "";

            foreach (var group in getGroups().Organizations)
            {
                var gl = new GroupLookup();
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
            values["SGF:Campus"] = org.Campu?.Description;

            if (leader != null && leader.PictureId != null)
            {
                values["SGF:LeaderPicSrc"] = "/Portrait/" + leader.Picture.SmallId.Value + "?v=" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
            }
            else
            {
                values["SGF:LeaderPicSrc"] = "/Portrait/-3";
            }

            if (org.OrgSchedules.Count > 0)
            {
                int count = 0;
                foreach (var schedule in org.OrgSchedules)
                {
                    if (count > 0)
                    {
                        values["SGF:Schedule"] += "; ";
                    }

                    values["SGF:Schedule"] += DAY_LAST[schedule.SchedDay ?? 0] + ", " + schedule.SchedTime.ToString2("t"); ;
                    count++;
                }
            }

            var loadAllValues = DbUtil.Db.Setting("SGF-LoadAllExtraValues");

            foreach (var extra in org.OrganizationExtras)
            {
                var val = extra.Data ??
                          extra.StrValue ??
                          extra.DateValue?.ToString() ??
                          extra.IntValue?.ToString() ??
                          extra.BitValue?.ToString();

                if (extra.Field.StartsWith("SGF:"))
                {
                    values[extra.Field] = val;
                }
                else if (loadAllValues)
                {
                    values[$"SGF:{extra.Field}"] = val;
                }
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
