/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

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
