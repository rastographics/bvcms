using System.Collections.Generic;
using System.Linq;
using CmsData;

namespace CmsWeb.Areas.People.Models
{
    public class AttributesModel
    {
        private readonly CMSDataContext db;
        private readonly PeopleExtra ev;

        public int PeopleId => ev.PeopleId;
        public string Field => ev.Field;
        public string Data => ev.Data;
        public string Name { get; set; }

        public AttributesModel(CMSDataContext db, string field, int pid)
        {
            this.db = db;
            var i = (from ev in db.PeopleExtras
                     where ev.PeopleId == pid
                     where ev.Field == field
                     select new
                     {
                         ev,
                         ev.Person.Name
                     }).Single();
            Name = i.Name;
            ev = i.ev;
        }

        public void Update(string data)
        {
            ev.Data = data;
            db.SubmitChanges();
            DbUtil.LogActivity("Updated Attributes", peopleid: PeopleId);
        }

        public IEnumerable<CmsData.View.Attribute> Attributes()
        {
            return db.ViewAttributes.Where(vv => vv.Field == Field && vv.PeopleId == PeopleId);
        }
    }
}
