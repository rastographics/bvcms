using System.Linq;
using CmsData;

namespace CmsWeb.Areas.People.Models
{
    public class EmailSentModel : EmailModel
    {
        public EmailSentModel(int id) : base(id) { }

        override public IQueryable<EmailQueue> DefineModelList()
        {
            var q = from e in DbUtil.Db.EmailQueues
                    where !(e.Transactional ?? false)
                    where e.EmailQueueTos.Any()
                    where e.QueuedBy == person.PeopleId
                         || (e.FromAddr == person.EmailAddress && person.EmailAddress.Length > 0)
                         || (e.FromAddr == person.EmailAddress2 && person.EmailAddress2.Length > 0)
                    select e;
            return FilterForUser(q);
        }
    }
}