using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models.Communications
{
    public class OptOutsModel : IDbBinder
    {
        public CMSDataContext CurrentDatabase { get; set; }

        public OptOutsModel() { }

        public OptOutsModel(CMSDataContext db, int id)
        {
            CurrentDatabase = db;
            PeopleId = id;
        }

        public int PeopleId { get; set; }

        public IEnumerable<SelectListItem> PersonSmsGroups()
        {
            var q = from g in CurrentDatabase.SMSGroups
                where !g.IsDeleted
                where g.SMSLists.Any(vv => vv.SMSItems.Any(v => v.PeopleID == PeopleId))
                    where !g.IsDeleted
                    select new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.Name
                    };
            var groups = q.ToList();
            groups.Insert(0, new SelectListItem { Text = "(select group)", Value = "0" });
            return groups;
        }

        public class OptOutViewModel
        {
            public DateTime? CreatedDt { get; set; }
            public int PeopleId { get; set; }
            public string OptedOutOf { get; set; }
            public bool IsText { get; set; }
        }
        public List<OptOutViewModel> OptOuts()
        {
            var person = CurrentDatabase.LoadPersonById(PeopleId);
            var qe = from oe in person.EmailOptOuts
                select new OptOutViewModel
                {
                    CreatedDt = oe.DateX,
                    OptedOutOf = oe.FromEmail,
                    PeopleId = oe.ToPeopleId,
                    IsText = false
                };
            var qt = from ot in person.SmsGroupOptOuts
                join g in CurrentDatabase.SMSGroups on ot.FromGroup equals g.Id
                select new OptOutViewModel
                {
                    CreatedDt = ot.DateX,
                    OptedOutOf = g.Name,// + " (" + g.Id + ")",
                    PeopleId = ot.ToPeopleId,
                    IsText = true
                };
            var q = qe.Union(qt).OrderByDescending(vv => vv.CreatedDt);
            return q.ToList();
        }
    }
}
