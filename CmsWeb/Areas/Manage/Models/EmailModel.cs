using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using Elmah;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class EmailModel
    {
        public int Id { get; set; }
        private EmailQueue _Queue;
        public EmailQueue queue
        {
            get
            {
                if (_Queue == null)
                    _Queue = DbUtil.Db.EmailQueues.SingleOrDefault(ee => ee.Id == Id);
                return _Queue;
            }
        }

        private bool? _hasTracking;
        public bool HasTracking
        {
            get
            {
                if (!_hasTracking.HasValue)
                {
                    _hasTracking = _Queue.Body.Contains("{track}");
                }
                return _hasTracking.Value;
            }
        }

        private bool? _hasTrackLinks;
        public bool HasTrackLinks
        {
            get
            {
                if (!_hasTrackLinks.HasValue)
                {
                    _hasTrackLinks = _Queue.Body.Contains("{tracklinks}");
                }
                return _hasTrackLinks.Value;
            }
        }

        public FilterType FilterType { get; private set; }

        public int CountOfAllRecipients { get; private set; }

        public int CountOfOpenedRecipients { get; private set; }

        public int CountOfNotOpenedRecipients { get; private set; }

        public int CountOfFailedRecipients { get; private set; }

        public IEnumerable<RecipientInfo> Recipients { get; private set; }

        public PagerModel2 Pager { get; set; }

        int _count;
        public int Count()
        {
            return _count;
        }
        
        public EmailModel(int id)
        {
            Id = id;
            FilterType = FilterType.All;
            LoadRecipients();
        }

        public EmailModel(int id, FilterType filterType, int? page, int pageSize)
        {
            Id = id;
            FilterType = filterType;
            LoadRecipients(page, pageSize);
        }

        private void LoadRecipients(int? page = null, int? pageSize = null)
        {
            var emailTos = GetEmailTos().ToList();
            CountOfAllRecipients = emailTos.Count();
            CountOfOpenedRecipients = emailTos.Count(e => e.Opened);
            CountOfNotOpenedRecipients = emailTos.Count(e => !e.Opened);
            CountOfFailedRecipients = emailTos.Count(e => e.Failed);

            switch (FilterType)
            {
                case FilterType.All:
                    _count = CountOfAllRecipients;
                    SetPager(page, pageSize);
                    Recipients = GetRecipients(emailTos);
                    break;
                case FilterType.Opened:
                    _count = CountOfOpenedRecipients;
                    SetPager(page, pageSize);
                    Recipients = GetRecipients(emailTos.Where(e => e.Opened));
                    break;
                case FilterType.NotOpened:
                    _count = CountOfNotOpenedRecipients;
                    SetPager(page, pageSize);
                    Recipients = GetRecipients(emailTos.Where(e => !e.Opened));
                    break;
                case FilterType.Failed:
                    _count = CountOfFailedRecipients;
                    SetPager(page, pageSize);
                    Recipients = GetRecipients(emailTos.Where(e => e.Failed));
                    break;
                default:
                    _count = CountOfAllRecipients;
                    Pager = new PagerModel2(Count);
                    SetPager(page, pageSize);
                    Recipients = GetRecipients(emailTos);
                    break;
            }
        }

        private void SetPager(int? page, int? pageSize)
        {
            Pager = new PagerModel2(Count);
            if (pageSize.HasValue)
                Pager.PageSize = pageSize.Value;
            Pager.Page = page; 
        }

        private IEnumerable<EmailTo> GetEmailTos()
        {
            var q = from t in DbUtil.Db.EmailQueueTos
                    let opened = t.Person.EmailResponses.Any(er => er.EmailQueueId == t.Id)
                    let fail = DbUtil.Db.EmailQueueToFails.FirstOrDefault(ff => ff.Id == t.Id && ff.PeopleId == t.PeopleId)
                    where t.Id == Id
                    select new EmailTo
                    {
                        EmailQueueTo = t,
                        Opened = opened,
                        Failed = fail != null
                    };

            var roles = DbUtil.Db.CurrentRoles();
            var isadmin = roles.Contains("Admin") || roles.Contains("ManageEmails");
            if (isadmin || queue.QueuedBy == Util.UserPeopleId)
                return q;
            return q.Where(ee => ee.EmailQueueTo.PeopleId == Util.UserPeopleId);
        }

        public IEnumerable<RecipientInfo> GetRecipients(IEnumerable<EmailTo> emailTos)
        {
            var q = emailTos;
            var q2 = queue.CCParents == true
                ? from e in q.OrderBy(ee => ee.EmailQueueTo.Person.Name2).Skip(Pager.StartRow).Take(Pager.PageSize)
                  let p1 = DbUtil.Db.People.Where(pp => pp.PeopleId == e.EmailQueueTo.Parent1).Select(pp => pp.Name).SingleOrDefault()
                  let p2 = DbUtil.Db.People.Where(pp => pp.PeopleId == e.EmailQueueTo.Parent2).Select(pp => pp.Name).SingleOrDefault()
                  select new RecipientInfo
                  {
                      peopleid = e.EmailQueueTo.PeopleId,
                      name = e.EmailQueueTo.Person.Name,
                      address = e.EmailQueueTo.Person.EmailAddress,
                      nopens = e.EmailQueueTo.Person.EmailResponses.Count(er => er.EmailQueueId == e.EmailQueueTo.Id),
                      parent1name = p1,
                      parent2name = p2,
                  }
                : from e in q.OrderBy(ee => ee.EmailQueueTo.Person.Name2).Skip(Pager.StartRow).Take(Pager.PageSize)
                  select new RecipientInfo
                  {
                      peopleid = e.EmailQueueTo.PeopleId,
                      name = e.EmailQueueTo.Person.Name,
                      address = e.EmailQueueTo.Person.EmailAddress,
                      nopens = e.EmailQueueTo.Person.EmailResponses.Count(er => er.EmailQueueId == e.EmailQueueTo.Id),
                  };

            return q2;
        }

        public bool CanDelete()
        {
            if (HttpContext.Current.User.IsInRole("Admin"))
                return true;
            if (queue.QueuedBy == Util.UserPeopleId)
                return true;
            var u = DbUtil.Db.LoadPersonById(Util.UserPeopleId.Value);
            if (queue.FromAddr == u.EmailAddress)
                return true;
            return false;
        }

        public string SendFromOrgName { get; set; }
        public bool SendFromOrg
        {
            get
            {
                var sendfromorg = queue.SendFromOrgId.HasValue && !GetEmailTos().Any();
                if (sendfromorg)
                {
                    var i = from o in DbUtil.Db.Organizations
                            where o.OrganizationId == queue.SendFromOrgId
                            select o.OrganizationName;
                    SendFromOrgName = i.Single();
                }
                return sendfromorg;
            }
        }

    }

    public class EmailTo
    {
        public EmailQueueTo EmailQueueTo { get; set; }
        public bool Opened { get; set; }
        public bool Failed { get; set; }
    }

    public class RecipientInfo
    {
        public string name { get; set; }
        public int peopleid { get; set; }
        public string address { get; set; }
        public int nopens { get; set; }
        public string failtype { get; set; }
        public int? parent1id { get; set; }
        public int? parent2id { get; set; }
        public string parent1name { get; set; }
        public string parent2name { get; set; }
    }

    public enum FilterType
    {
        All = 0,
        Opened = 1,
        NotOpened = 2,
        Failed = 3
    }

}
