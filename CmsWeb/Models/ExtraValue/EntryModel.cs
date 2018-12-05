using CmsData;
using CmsWeb.Code;
using Newtonsoft.Json;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    public class EntryModel
    {
        public CodeInfo Origin { get; set; }
        public CodeInfo EntryPoint { get; set; }
        public CodeInfo InterestPoint { get; set; }
        public EntryModel() { }
        public EntryModel(int id)
        {
            var q = from p in DbUtil.Db.People
                    where p.PeopleId == id
                    select new { p.EntryPointId, p.OriginId, p.InterestPointId };
            var i = q.Single();
            Origin = new CodeInfo(i.OriginId, "Origin");
            EntryPoint = new CodeInfo(i.EntryPointId, "EntryPoint");
            InterestPoint = new CodeInfo(i.InterestPointId, "InterestPoint");
        }

        public static void EditValue(int id, string name, string value)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            if (name == "Origin")
            {
                p.OriginId = value.ToInt();
            }
            else if (name == "EntryPoint")
            {
                p.EntryPointId = value.ToInt();
            }
            else if (name == "InterestPoint")
            {
                p.InterestPointId = value.ToInt();
            }

            DbUtil.Db.SubmitChanges();
        }

        public static string OriginList()
        {
            var m = new CodeValueModel();
            var q = from o in m.OriginList()
                    select new { value = o.Id, text = o.Value };
            return JsonConvert.SerializeObject(q.ToArray());
        }

        public static string EntryPointList()
        {
            var m = new CodeValueModel();
            var q = from o in m.EntryPointList()
                    select new { value = o.Id, text = o.Value };
            return JsonConvert.SerializeObject(q.ToArray());
        }

        public static string InterestPointList()
        {
            var m = new CodeValueModel();
            var q = from o in m.InterestPoints()
                    select new { value = o.Id, text = o.Value };
            return JsonConvert.SerializeObject(q.ToArray());
        }
    }
}
