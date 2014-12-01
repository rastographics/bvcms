using System.Linq;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models
{
    public class EmailReceivedModel : EmailModel
    {
        public EmailReceivedModel(int id, PagerModel2 pager)
            : base(id, pager)
        {
            
        }

        public override IQueryable<EmailQueue> DefineModelList()
        {
            var q = from e in DbUtil.Db.EmailQueues
                    where e.Sent != null
                    where !(e.Transactional ?? false)
                    where e.EmailQueueTos.Any(ee => 
                        ee.PeopleId == person.PeopleId
                        || ee.Parent1 == person.PeopleId
                        || ee.Parent2 == person.PeopleId
                        )
                    where e.QueuedBy != person.PeopleId
                    where e.FromAddr != (person.EmailAddress ?? "")
                    where e.FromAddr != (person.EmailAddress2 ?? "")
                    select e;
            return FilterForUser(q);
        }
    }
}
