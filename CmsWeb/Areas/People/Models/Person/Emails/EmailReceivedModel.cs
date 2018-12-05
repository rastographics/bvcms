using System.Linq;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models
{
    public class EmailReceivedModel : EmailModel
    {
        public EmailReceivedModel() { }
        public override IQueryable<EmailQueue> DefineModelList()
        {
            var q = from e in DbUtil.Db.EmailQueues
                    where e.Sent != null
                    where !(e.Transactional ?? false)
                    where e.EmailQueueTos.Any(ee => 
                        ee.PeopleId == Person.PeopleId
                        || ee.Parent1 == Person.PeopleId
                        || ee.Parent2 == Person.PeopleId
                        )
                    where e.QueuedBy != Person.PeopleId
                    where e.FromAddr != (Person.EmailAddress ?? "")
                    where e.FromAddr != (Person.EmailAddress2 ?? "")
                    select e;
            return FilterForUser(q);
        }
    }
}
