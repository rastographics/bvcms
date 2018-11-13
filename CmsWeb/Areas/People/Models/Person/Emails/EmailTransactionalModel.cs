using System.Linq;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models
{
    public class EmailTransactionalModel : EmailModel
    {
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
