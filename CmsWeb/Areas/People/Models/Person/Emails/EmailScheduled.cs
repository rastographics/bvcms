using System;
using System.Linq;
using CmsData;
using CmsWeb.Constants;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class EmailScheduledModel : EmailModel
    {
        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public EmailScheduledModel() : base() { }

        public EmailScheduledModel(CMSDataContext db) : base(db) { }

        override public IQueryable<EmailQueue> DefineModelList()
        {
            var q = from e in CurrentDatabase.EmailQueues
                    where !(e.Transactional ?? false)
                    where e.Sent == null && e.SendWhen != null
                    where e.QueuedBy == Person.PeopleId
                         || (e.FromAddr == Person.EmailAddress && Person.EmailAddress.HasValue())
                         || (e.FromAddr == Person.EmailAddress2 && Person.EmailAddress2.HasValue())
                    select e;
            return FilterForUser(q);
        }
    }
}
