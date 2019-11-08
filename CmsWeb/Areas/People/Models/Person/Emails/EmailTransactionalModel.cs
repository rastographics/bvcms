using CmsData;
using CmsWeb.Constants;
using System;
using System.Linq;

namespace CmsWeb.Areas.People.Models
{
    public class EmailTransactionalModel : EmailModel
    {
        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public EmailTransactionalModel() : base() { }

        public EmailTransactionalModel(CMSDataContext db) : base(db) { }

        public override IQueryable<EmailQueue> DefineModelList()
        {
            var q = from e in CurrentDatabase.EmailQueues
                    where e.Sent != null
                    where e.Transactional ?? false
                    where e.EmailQueueTos.Any(ee => ee.PeopleId == Person.PeopleId)
                    select e;
            return FilterForUser(q);
        }
    }
}
