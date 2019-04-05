using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UtilityExtensions;

namespace CmsData.ExtraValue
{
    // Views are a set of places for extravalues to be seen (see View which is a single place)
    public class Views
    {
        [XmlElement("View")]
        public List<View> List;

        public void Save(CMSDataContext db)
        {
            var newxml = Util.Serialize(this);
            DbUtil.SetStandardExtraValues2(db, newxml);
            db.SubmitChanges();
        }

        public static Views GetViews(CMSDataContext db, bool nocache = false)
        {
            var xml = DbUtil.StandardExtraValues2(db, nocache);

            var f = Util.DeSerialize<Views>(xml);
            if (f == null)
                return new Views();
            return f;
        }
        public static List<Value> GetStandardExtraValues(CMSDataContext db, string table, bool nocache = false, string location = null)
        {
            return (from vv in GetViews(db, nocache).List
                    where vv.Table == table
                    where vv.Location == location || location == null
                    from v in vv.Values
                    select v).ToList();
        }

        public class StandardValueNameType
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public bool CanView { get; set; }
        }

        public static List<StandardValueNameType> GetViewableNameTypes(CMSDataContext db, string table, bool nocache = false)
        {
            var list = (from vv in GetStandardExtraValues(db, table, nocache)
                        where vv.Type != "Bits"
                        select new StandardValueNameType()
                        { 
                            Name = vv.Name, 
                            Type = vv.Type,
                            CanView = vv.UserCanView(db)
                        }).ToList();

            var list2 = (from vv in GetStandardExtraValues(db, table, nocache)
                         where vv.Type == "Bits"
                         from v in vv.Codes
                         select new StandardValueNameType()
                         { 
                             Name = v.Text, 
                             Type = vv.Type,
                             CanView = vv.UserCanView(db)
                         }).ToList();

            return list.Union(list2).OrderBy(vv => vv.Name).ToList();
        }
        public static List<StandardValueNameType> GetViewableDataTypes(CMSDataContext db, bool nocache = false)
        {
            var list = (from vv in GetStandardExtraValues(db, "People", nocache)
                        where vv.Type == "Data"
                        select new StandardValueNameType()
                        { 
                            Name = vv.Name, 
                            Type = vv.Type,
                            CanView = vv.UserCanView(db)
                        }).ToList();
            return list;
        }

        public class ViewValue
        {
            public Views views;
            public View view;
            public Value value;
        }
        public static ViewValue GetViewsViewValue(CMSDataContext db, string table, string name, string location = null)
        {
            var views = GetViews(db, nocache: true);
            var i = from view in views.List
                    where view.Table == table
                    where location == null || view.Location == location
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
        public static ViewValue GetViewsView(CMSDataContext db, string table, string location)
        {
            var views = GetViews(db, nocache: true);
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
        public static List<Value> GetStandardExtraValuesOrdered(CMSDataContext db, string table, string location)
        {
            var views = GetViews(db, nocache: true);
            var q = from vv in views.List
                    where vv.Table == table
                    where location == null || vv.Location == location
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
