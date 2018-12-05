using CmsData;
using CmsWeb.Areas.Search.Models;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.XPath;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models.Org
{
    public class OrgSearchField
    {
        public string Field { get; set; }
        public string Label { get; set; }
        public bool Dropdown { get; set; }
        public bool ExtraValue { get; set; }
        public bool Display { get; set; }
        public bool Weekdays { get; set; }
        public string OrgType { get; set; }
        public IEnumerable<SelectListItem> SelectList { get; set; }
        public OrgSearchModel OrgSearchModel { get; set; }

        public string DivClass
        {
            get
            {
                if (string.IsNullOrEmpty(OrgType))
                {
                    return "";
                }

                return $"{OrgType.ToLower().Replace(" ", "")}-cell ev-orgtype-cell";
            }
        }

        public string FieldClass
        {
            get { return ExtraValue ? "form-control ev-input" : "form-control"; }
        }

        public string Value
        {
            get
            {
                if (Field == "Name")
                {
                    return OrgSearchModel.Name;
                }
                else
                {
                    var value = "";
                    OrgSearchModel.ExtraValuesDict?.TryGetValue(Field, out value);
                    return value;
                }
            }
        }
    }

    public class OrgSearchFieldsModel
    {
        public OrgSearchFieldsModel() { }
        private static readonly List<string> standardFields = new List<string>()
        {
            "Name",
            "TypeId",
            "ProgramId",
            "DivisionId",
            "StatusId",
            "CampusId",
            "ScheduleId",
            "OnlineReg"
        };

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

        public static List<OrgSearchField> GetFields(OrgSearchModel osm)
        {
            string customTextName = "OrgSearchFields";
            //var db = Db;
            var list = new List<OrgSearchField>();

            var s = HttpRuntime.Cache[DbUtil.Db.Host + customTextName] as string;
            if (s == null)
            {
                s = DbUtil.Db.ContentText(customTextName, Resource1.OrgSearchFields);
                HttpRuntime.Cache.Insert(DbUtil.Db.Host + customTextName, s, null,
                    DateTime.Now.AddMinutes(Util.IsDebug() ? 0 : 1), Cache.NoSlidingExpiration);
            }
            if (!s.HasValue())
            {
                return list;
            }

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Parse(s);
            }
            catch (Exception)
            {
                xdoc = XDocument.Parse(Resource1.OrgSearchFields);
            }

            if (xdoc?.Root == null)
            {
                return list;
            }

            string selectedOrgType = null;

            foreach (var e in xdoc.XPathSelectElements("/OrgSearch/Fields").Elements())
            {
                if (e.Name.LocalName.ToLower() == "field")
                {
                    var field = new OrgSearchField();
                    field.OrgSearchModel = osm;
                    field.Field = e.Value;
                    field.OrgType = e.Attribute("orgtype")?.Value;
                    field.Display = string.IsNullOrEmpty(field.OrgType);

                    if (field.Field == "Campus")
                    {
                        field.Label = Util2.CampusLabel;
                    }
                    else
                    {
                        field.Label = e.Attribute("label")?.Value ?? field.Field;
                    }

                    bool dropdown;
                    bool.TryParse(e.Attribute("dropdown")?.Value ?? "false", out dropdown);
                    field.Dropdown = dropdown;

                    bool weekdays;
                    bool.TryParse(e.Attribute("weekdays")?.Value ?? "false", out weekdays);
                    field.Weekdays = weekdays;

                    field.ExtraValue = !standardFields.Contains(field.Field);

                    if (dropdown && field.ExtraValue)
                    {
                        IEnumerable<string> values;

                        if (field.Weekdays)
                        {
                            values = weekdayList;
                        }
                        else
                        {
                            values = DbUtil.Db.OrganizationExtras.Where(x => x.Field == field.Field)
                                .Select(x => x.Data ?? x.StrValue).Distinct().OrderBy(x => x).ToList()
                                .Where(x => !string.IsNullOrEmpty(x));
                            // IsNullOrEmpty() doesn't work in LINQ-to-SQL, so this comes after materializing query
                        }

                        var items = new List<SelectListItem>();
                        items.Add(new SelectListItem
                        {
                            Text = "(not specified)",
                            Value = ""
                        });

                        values.ForEach(x => items.Add(new SelectListItem
                        {
                            Text = x,
                            Value = x,
                            Selected = field.Value == x
                        }));

                        field.SelectList = items;
                    }
                    else
                    {
                        switch (field.Field)
                        {
                            case "TypeId":
                                field.SelectList = OrgSearchModel.OrgTypeFilters();
                                selectedOrgType = field.SelectList.FirstOrDefault(x => x.Selected)?.Text;
                                break;
                            case "ProgramId":
                                field.SelectList = osm.ProgramIds();
                                break;
                            case "DivisionId":
                                field.SelectList = osm.DivisionIds();
                                break;
                            case "StatusId":
                                field.SelectList = OrgSearchModel.StatusIds();
                                break;
                            case "CampusId":
                                field.SelectList = osm.CampusIds();
                                break;
                            case "ScheduleId":
                                field.SelectList = osm.ScheduleIds();
                                break;
                            case "OnlineReg":
                                field.SelectList = OrgSearchModel.RegistrationTypeIds();
                                break;
                        }
                    }

                    list.Add(field);
                }

                // Handles cases where a pre-existing OrgType value will mean we need to
                // display some OrgType-specific fields on load.
                if (!string.IsNullOrEmpty(selectedOrgType))
                {
                    list.Where(x => x.OrgType == selectedOrgType).ForEach(x => x.Display = true);
                }
            }
            return list;
        }
    }
}
