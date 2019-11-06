using CmsData;
using CmsWeb.Constants;
using System;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class EmailSentModel : EmailModel
    {
        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public EmailSentModel() : base() { }

        public EmailSentModel(CMSDataContext db) : base(db) { }

        public override IQueryable<EmailQueue> DefineModelList()
        {
            var q = from e in CurrentDatabase.EmailQueues
                    where !(e.Transactional ?? false)
                    where e.EmailQueueTos.Any()
                    where e.Sent != null
                    where e.QueuedBy == Person.PeopleId
                         || (e.FromAddr == Person.EmailAddress && Person.EmailAddress.HasValue())
                         || (e.FromAddr == Person.EmailAddress2 && Person.EmailAddress2.HasValue())
                    select e;
            return FilterForUser(q);
        }
    }
}
