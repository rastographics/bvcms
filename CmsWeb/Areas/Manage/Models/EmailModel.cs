using CmsData;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                {
                    _Queue = DbUtil.Db.EmailQueues.SingleOrDefault(ee => ee.Id == Id);
                }

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

        //        public string FormattedHtmlBody => TidyLib.FormatHtml(queue.Body);

        public EmailModel() { }

        public FilterType FilterType { get; private set; }

        public int CountOfAllRecipients { get; private set; }

        public int CountOfOpenedRecipients { get; private set; }

        public int CountOfNotOpenedRecipients { get; private set; }

        public int CountOfFailedRecipients { get; private set; }

        public IEnumerable<RecipientInfo> Recipients { get; private set; }

        public PagerModel2 Pager { get; set; }

        private int _count;
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
            var cn = DbUtil.Db.Connection;
            dynamic counts = cn.Query(@"
SELECT
	COUNT(*) AS Total
	,(SELECT COUNT(DISTINCT PeopleId) FROM dbo.EmailResponses WHERE EmailQueueId = @emailQueueId) AS NumberOpened
	,(SELECT COUNT(*) FROM dbo.EmailQueueToFail WHERE Id = @emailQueueId) AS NumberFailed
FROM dbo.EmailQueueTo eqt
WHERE eqt.Id = @emailQueueId
", new { emailQueueId = Id }).Single();

            CountOfAllRecipients = counts.Total;
            CountOfOpenedRecipients = counts.NumberOpened;
            CountOfFailedRecipients = counts.NumberFailed;
            CountOfNotOpenedRecipients = CountOfAllRecipients - CountOfOpenedRecipients;

            switch (FilterType)
            {
                case FilterType.All:
                    _count = CountOfAllRecipients;
                    break;
                case FilterType.Opened:
                    _count = CountOfOpenedRecipients;
                    break;
                case FilterType.NotOpened:
                    _count = CountOfNotOpenedRecipients;
                    break;
                case FilterType.Failed:
                    _count = CountOfFailedRecipients;
                    break;
                default:
                    _count = CountOfAllRecipients;
                    Pager = new PagerModel2(Count);
                    break;
            }

            SetPager(page, pageSize);
            Recipients = GetRecipients();
        }

        private void SetPager(int? page, int? pageSize)
        {
            Pager = new PagerModel2(Count);
            if (pageSize.HasValue)
            {
                Pager.PageSize = pageSize.Value;
            }

            Pager.Page = page;
        }

        private IEnumerable<RecipientInfo> GetRecipients()
        {
            const string sql = @"
;WITH Emails AS (
	SELECT
		p.Name
		,p.PeopleId
		,p.EmailAddress
		,p.Name2
		,eqt.Id
		,eqt.Parent1
		,eqt.Parent2
	FROM dbo.EmailQueueTo eqt
	JOIN dbo.People p ON p.PeopleId = eqt.PeopleId
	WHERE eqt.Id = @emailQueueId
)
,Responses AS (
	SELECT
		Id
		,PeopleId
	FROM dbo.EmailResponses
	WHERE EmailQueueId = @emailQueueId
)
,AllEmails AS (
	SELECT
		e.PeopleId
		,e.Name
		,e.Name2
		,e.EmailAddress
		,FailedEmail = ef.email
		,COUNT(r.Id) AS NumberOpened
		,p1.Name AS Parent1
		,p2.Name AS Parent2
		,(CASE WHEN ef.Id IS NULL THEN 0 ELSE 1 END) AS Failed
	FROM Emails e
	LEFT JOIN Responses r ON r.PeopleId = e.PeopleId
	LEFT JOIN dbo.People p1 ON p1.PeopleId = e.Parent1
	LEFT JOIN dbo.People p2 ON p2.PeopleId = e.Parent2
	LEFT JOIN dbo.EmailQueueToFail ef ON ef.Id = e.Id AND ef.PeopleId = e.PeopleId
	WHERE
		@currentPeopleId = (
			CASE
				WHEN @isAdmin = 1 THEN @currentPeopleId
				WHEN (SELECT QueuedBy FROM dbo.EmailQueue WHERE Id = @emailQueueId) = @currentPeopleId THEN @currentPeopleId
				ELSE e.PeopleId
			END
		)
	GROUP BY
		e.PeopleId
		,e.Name
		,e.Name2
		,e.EmailAddress
		,ef.email
		,p1.Name
		,p2.Name
		,ef.Id
)
SELECT
	PeopleId
	,Name
	,EmailAddress = IIF(@filter = 'failed', FailedEmail, EmailAddress)
	,NumberOpened
	,Parent1
	,Parent2
	,Failed
FROM AllEmails
WHERE
	(((CASE
		WHEN @filter = 'failed' AND Failed = 1 THEN 1
		WHEN @filter = 'opened' AND NumberOpened > 0 THEN 1
		WHEN @filter = 'unopened' AND NumberOpened = 0 THEN 1
		ELSE 0
	END) = 1) OR (@filter = 'all'))
ORDER BY Name2
OFFSET (@currentPage-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY
";
            var roles = DbUtil.Db.CurrentRoles();
            var isAdmin = roles.Contains("Admin") || roles.Contains("ManageEmails") || roles.Contains("Finance");
            string filter;

            switch (FilterType)
            {
                case FilterType.All:
                    filter = "all";
                    break;
                case FilterType.Failed:
                    filter = "failed";
                    break;
                case FilterType.NotOpened:
                    filter = "unopened";
                    break;
                default:
                    filter = "opened";
                    break;
            }

            var args = new
            {
                emailQueueId = Id,
                currentPage = Pager.Page,
                pageSize = Pager.PageSize,
                isAdmin,
                currentPeopleId = Util.UserPeopleId,
                filter
            };

            return DbUtil.Db.Connection.Query<RecipientInfo>(sql, args);
        }

        public bool CanDelete()
        {
            if (HttpContextFactory.Current.User.IsInRole("Admin"))
            {
                return true;
            }

            if (queue.QueuedBy == Util.UserPeopleId)
            {
                return true;
            }

            var u = DbUtil.Db.LoadPersonById(Util.UserPeopleId ?? 0);
            return queue.FromAddr == u.EmailAddress;
        }

        public string SendFromOrgName { get; set; }

        public bool SendFromOrg
        {
            get
            {
                var sendFromOrg = queue.SendFromOrgId.HasValue && !Recipients.Any();
                if (sendFromOrg)
                {
                    var i = from o in DbUtil.Db.Organizations
                            where o.OrganizationId == queue.SendFromOrgId
                            select o.OrganizationName;
                    SendFromOrgName = i.Single();
                }
                return sendFromOrg;
            }
        }

        public Guid CurrentPersonQueryId
        {
            get { return DbUtil.Db.QueryIsCurrentPerson().QueryId; }
        }

        private int? _emptyTemplateId;
        public int EmptyTemplateId
        {
            get
            {
                if (!_emptyTemplateId.HasValue)
                {
                    var emptyTemplate = (from content in DbUtil.Db.Contents
                                         where content.Name == "Empty Template"
                                         select content).SingleOrDefault();

                    _emptyTemplateId = (emptyTemplate == null) ? 0 : emptyTemplate.Id;
                }

                return _emptyTemplateId.Value;
            }
        }
    }

    public class RecipientInfo
    {
        public string Name { get; set; }
        public int PeopleId { get; set; }
        public string EmailAddress { get; set; }
        public int NumberOpened { get; set; }
        public string Parent1 { get; set; }
        public string Parent2 { get; set; }
    }

    public enum FilterType
    {
        All = 0,
        Opened = 1,
        NotOpened = 2,
        Failed = 3
    }
}
