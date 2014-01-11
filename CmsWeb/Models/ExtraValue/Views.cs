using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    public class Views
    {
        [XmlElement("View")]
        public List<View> List;

        public void Save()
        {
            var newxml = Util.Serialize(this);
            DbUtil.SetStandardExtraValues2(newxml);
            DbUtil.Db.SubmitChanges();
        }

        public static Views GetViews(bool nocache = false)
        {
            var xml = DbUtil.StandardExtraValues2(nocache);

            var f = Util.DeSerialize<Views>(xml);
            if (f == null)
                return new Views();
            return f;
        }
        public static List<Value> GetStandardExtraValues(string table, bool nocache = false)
        {
            return (from vv in GetViews(nocache).List
                    where vv.Table == table
                    from v in vv.Values
                    select v).ToList();
        }

        public class StandardValueNameType
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public bool CanView { get; set; }
        }

        public static List<StandardValueNameType> GetViewableNameTypes(string table, bool nocache = false)
        {
            var list = (from vv in GetStandardExtraValues(table, nocache)
                        where vv.Type != "Bits"
                        select new StandardValueNameType()
                        { 
                            Name = vv.Name, 
                            Type = vv.Type,
                            CanView = vv.UserCanView()
                        }).ToList();

            var list2 = (from vv in GetStandardExtraValues(table, nocache)
                         where vv.Type == "Bits"
                         from v in vv.Codes
                         select new StandardValueNameType()
                         { 
                             Name = v, 
                             Type = vv.Type,
                             CanView = vv.UserCanView()
                         }).ToList();

            return list.Union(list2).OrderBy(vv => vv.Name).ToList();
        }

        public class ViewValue
        {
            public Views views;
            public View view;
            public Value value;
        }
        public static ViewValue GetViewsViewValue(string table, string name)
        {
            var views = GetViews(nocache: true);
            var i = from view in views.List
                    where view.Table == table
                    from value in view.Values
                    where value.Name == name
                    select new ViewValue
                    {
                        views = views,
                        view = view,
                        value = value
                    };
            return i.Single();
        }
        public static ViewValue GetViewsView(string table, string location)
        {
            var views = GetViews(nocache: true);
            var i = from view in views.List
                    where view.Table == table
                    where view.Location == location
                    select new ViewValue { views = views, view = view };
            var v = i.SingleOrDefault();
            if (v != null) 
                return v;
            var vv = new View() {Location = location, Table = table};
            views.List.Add(vv);
            return new ViewValue() {views = views, view = vv};
        }
        public static List<Value> GetStandardExtraValues(string Table, string Location)
        {
            var views = GetViews();
            var q = from vv in views.List
                    where vv.Table == Table
                    where vv.Location == Location
                    from v in vv.Values
                    select v;
            var values = q.ToList();
            var n = 1;
            foreach (var ff in values)
                ff.Order = n++;
            return values;
        }
    }
}