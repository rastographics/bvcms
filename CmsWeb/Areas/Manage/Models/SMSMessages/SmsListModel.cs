using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;

namespace CmsWeb.Areas.Manage.Models.SMSMessages
{
    public class SmsListModel
    {
        private CMSDataContext CurrentDatabase { get; set; }
        public SMSList SmsList { get; set; }
        public SmsListModel(CMSDataContext db, int id)
        {
            CurrentDatabase = db;
            SmsList = CurrentDatabase.SMSLists.Single(vv => vv.Id == id);
        }

        public IEnumerable<SMSItem> SmsItems()
        {
            return from i in SmsList.SMSItems
                   orderby i.Person.Name, i.Sent descending
                   select i;
        }
    }
}
