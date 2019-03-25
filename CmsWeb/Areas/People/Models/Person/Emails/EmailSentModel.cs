using CmsData;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class EmailSentModel : EmailModel
    {
        public EmailSentModel() { }
        public override IQueryable<EmailQueue> DefineModelList()
        {
            var q = from e in DbUtil.Db.EmailQueues
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
