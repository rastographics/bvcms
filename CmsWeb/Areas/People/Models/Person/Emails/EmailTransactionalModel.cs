using System.Linq;
using CmsData;

namespace CmsWeb.Areas.People.Models
{
    public class EmailTransactionalModel : EmailModel
    {
        public EmailTransactionalModel(int id) : base(id) { }

        public override IQueryable<EmailQueue> DefineModelList()
        {
            var q = from e in DbUtil.Db.EmailQueues
                    where e.Sent != null
                    where e.Transactional ?? false
                    where e.EmailQueueTos.Any(ee => ee.PeopleId == person.PeopleId)
                    select e;
            return FilterForUser(q);
        }
    }
}
