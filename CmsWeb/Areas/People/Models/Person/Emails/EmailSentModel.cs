using System.Linq;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class EmailSentModel : EmailModel
    {
        public EmailSentModel(int id, PagerModel2 pager)
            : base(id, pager)
        {
            
        }

        override public IQueryable<EmailQueue> DefineModelList()
        {
            var q = from e in DbUtil.Db.EmailQueues
                    where !(e.Transactional ?? false)
                    where e.EmailQueueTos.Any()
                    where e.Sent != null
                    where e.QueuedBy == person.PeopleId
                         || (e.FromAddr == person.EmailAddress && person.EmailAddress.HasValue())
                         || (e.FromAddr == person.EmailAddress2 && person.EmailAddress2.HasValue())
                    select e;
            return FilterForUser(q);
        }
    }
}