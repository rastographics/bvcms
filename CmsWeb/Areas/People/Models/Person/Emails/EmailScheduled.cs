using System.Linq;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class EmailScheduledModel : EmailModel
    {
        public EmailScheduledModel() { }
        
        override public IQueryable<EmailQueue> DefineModelList()
        {
            var q = from e in DbUtil.Db.EmailQueues
                    where !(e.Transactional ?? false)
                    where e.EmailQueueTos.Any()
                    where e.Sent == null && e.SendWhen != null
                    where e.QueuedBy == Person.PeopleId
                         || (e.FromAddr == Person.EmailAddress && Person.EmailAddress.HasValue())
                         || (e.FromAddr == Person.EmailAddress2 && Person.EmailAddress2.HasValue())
                    select e;
            return FilterForUser(q);
        }
    }
}
