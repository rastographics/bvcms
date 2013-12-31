using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data.Linq;
using System.Web;
using CmsData;
using CmsWeb.Code;
using DocumentFormat.OpenXml.Drawing.Charts;
using UtilityExtensions;
using System.Web.Mvc;
using System.Text;
using System.Net.Mail;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data.Linq.SqlClient;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using CmsData.Codes;

namespace CmsWeb.Models
{
    public class HomeModel
    {
        public class BirthdayInfo
        {
            public DateTime Birthday { get; set; }
            public string Name { get; set; }
            public int PeopleId { get; set; }
        }
        public IEnumerable<BirthdayInfo> Birthdays()
        {
            var up = DbUtil.Db.CurrentUserPerson;
            if (up == null)
                return new List<BirthdayInfo>();

            var tag = DbUtil.Db.FetchOrCreateTag("TrackBirthdays", up.PeopleId, DbUtil.TagTypeId_Personal);
            var q = tag.People(DbUtil.Db);
            if (q.Count() == 0)
                q = from p in DbUtil.Db.People
                    where p.OrganizationMembers.Any(om => om.OrganizationId == up.BibleFellowshipClassId)
                    select p;

            var q2 = from p in q
                     let nextbd = DbUtil.Db.NextBirthday(p.PeopleId)
                     where SqlMethods.DateDiffDay(UtilityExtensions.Util.Now, nextbd) <= 15
                     where p.DeceasedDate == null
                     orderby nextbd
                     select new BirthdayInfo { Birthday = nextbd.Value, Name = p.Name, PeopleId = p.PeopleId };
            return q2;
        }
        public class MyInvolvementInfo
        {
            public string Name { get; set; }
            public string MemberType { get; set; }
            public int OrgId { get; set; }
            public string OrgType { get; set; }
        }
        public IEnumerable<MyInvolvementInfo> MyInvolvements()
        {
            var u = DbUtil.Db.CurrentUser;
            if (u == null)
                return new List<MyInvolvementInfo>();

            var pid = u.PeopleId;

            var limitvisibility = Util2.OrgMembersOnly || Util2.OrgLeadersOnly;
            var oids = new int[0];
            if (Util2.OrgLeadersOnly)
                oids = DbUtil.Db.GetLeaderOrgIds(pid);

            var roles = DbUtil.Db.CurrentUser.UserRoles.Select(uu => uu.Role.RoleName).ToArray();
            var orgmembers = from om in DbUtil.Db.OrganizationMembers
                             where om.Organization.LimitToRole == null || roles.Contains(om.Organization.LimitToRole)
                             select om;

            var q = from om in orgmembers
                    where om.PeopleId == pid
                    where (om.Pending ?? false) == false
                    where oids.Contains(om.OrganizationId) || !(limitvisibility && om.Organization.SecurityTypeId == 3)
                    orderby om.Organization.OrganizationType.Code ?? "z", om.Organization.OrganizationName
                    select new MyInvolvementInfo
                    {
                        Name = om.Organization.OrganizationName,
                        MemberType = om.MemberType.Description,
                        OrgId = om.OrganizationId,
                        OrgType = om.Organization.OrganizationType.Description ?? "Other",
                    };

            return q;
        }
        public class NewsInfo
        {
            public string Title { get; set; }
            public DateTime Published { get; set; }
            public string Url { get; set; }
        }
        public IEnumerable<NewsInfo> BVCMSNews()
        {
            var feedurl = "http://feeds.feedburner.com/BvcmsBlog";

            var wr = new WebClient();
            var feed = DbUtil.Db.RssFeeds.FirstOrDefault(r => r.Url == feedurl);

            HttpWebRequest req = null;
            try
            {
                req = WebRequest.Create(feedurl) as HttpWebRequest;
            }
            catch
            {
            }

            if (feed != null)
            {
                if (feed.LastModified.HasValue)
                {
                    req.IfModifiedSince = feed.LastModified.Value;
                    req.Headers.Add("If-None-Match", feed.ETag);
                }
            }
            else
            {
                feed = new RssFeed();
                DbUtil.Db.RssFeeds.InsertOnSubmit(feed);
                feed.Url = feedurl;
            }

            if (req != null)
            {
                try
                {
                    var resp = req.GetResponse() as HttpWebResponse;
                    feed.LastModified = resp.LastModified;
                    feed.ETag = resp.Headers["ETag"];
                    var sr = new StreamReader(resp.GetResponseStream());
                    feed.Data = sr.ReadToEnd();
                    sr.Close();
                    DbUtil.Db.SubmitChanges();
                }
                catch (WebException)
                {
                }
                if (feed.Data != null)
                {
                    try
                    {
                        var reader = XmlReader.Create(new StringReader(feed.Data));
                        var f = SyndicationFeed.Load(reader);
                        var posts = from item in f.Items
                                    select new NewsInfo
                                    {
                                        Title = item.Title.Text,
                                        Published = item.PublishDate.DateTime,
                                        Url = item.Links.Single(i => i.RelationshipType == "alternate").GetAbsoluteUri().AbsoluteUri
                                    };
                        return posts;
                    }
                    catch
                    {
                        return new List<NewsInfo>();
                    }
                }
            }
            return null;
        }
        public IEnumerable<NewsInfo> ChurchNews()
        {
            var feedurl = DbUtil.Db.Setting("ChurchFeedUrl", "");

            var feed = DbUtil.Db.RssFeeds.FirstOrDefault(r => r.Url == feedurl);


            HttpWebRequest req = null;
            try
            {
                req = WebRequest.Create(feedurl) as HttpWebRequest;
            }
            catch
            {
            }

            if (feed != null)
            {
                if (feed.LastModified.HasValue)
                {
                    req.IfModifiedSince = feed.LastModified.Value;
                    req.Headers.Add("If-None-Match", feed.ETag);
                }
            }
            else
            {
                feed = new RssFeed();
                DbUtil.Db.RssFeeds.InsertOnSubmit(feed);
                feed.Url = feedurl;
            }

            if (req != null)
            {
                try
                {
                    var resp = req.GetResponse() as HttpWebResponse;
                    feed.LastModified = resp.LastModified;
                    feed.ETag = resp.Headers["ETag"];
                    var sr = new StreamReader(resp.GetResponseStream());
                    feed.Data = sr.ReadToEnd();
                    sr.Close();
                    DbUtil.Db.SubmitChanges();
                }
                catch (WebException)
                {
                }
                if (feed.Data != null)
                {
                    try
                    {
                        var reader = XmlReader.Create(new StringReader(feed.Data));
                        var f = SyndicationFeed.Load(reader);
                        var posts = from item in f.Items
                                    let a = item.Authors.FirstOrDefault()
                                    let au = a == null ? "" : a.Name
                                    select new NewsInfo
                                    {
                                        Title = item.Title.Text,
                                        Published = item.PublishDate.DateTime,
                                        Url = item.Links.Single(i => i.RelationshipType == "alternate").GetAbsoluteUri().AbsoluteUri
                                    };
                        return posts;
                    }
                    catch
                    {
                        return new NewsInfo[] { };
                    }
                }
            }
            return new NewsInfo[] { };
        }

        public class MySavedQueryInfo
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
        public IEnumerable<MySavedQueryInfo> MyQueries()
        {
            var up = DbUtil.Db.CurrentUserPerson;
            if (up == null)
                return new List<MySavedQueryInfo>();

            return from c in DbUtil.Db.Queries
                   where c.Owner == Util.UserName
                   where c.Name != Util.ScratchPad2
                   orderby c.Name
                   select new MySavedQueryInfo
                   {
                       Name = c.Name,
                       Url = "/QueryBuilder2/Main/" + c.QueryId
                   };
        }
        public class TaskInfo
        {
            public int TaskId { get; set; }
            public int PeopleId { get; set; }
            public string Who { get; set; }
            public string Description { get; set; }
        }
        public IEnumerable<TaskInfo> Tasks()
        {
            var up = DbUtil.Db.CurrentUserPerson;
            if (up == null)
                return new List<TaskInfo>();
            var completedcode = TaskStatusCode.Complete;
            var pid = DbUtil.Db.CurrentUser.PeopleId;
            var q = from t in DbUtil.Db.Tasks
                    where t.Archive == false // not archived
                    where t.OwnerId == pid || t.CoOwnerId == pid
                    where t.WhoId != null && t.StatusId != completedcode
                    where !(t.OwnerId == pid && t.CoOwnerId != null)
                    orderby t.CreatedOn
                    select new TaskInfo
                    {
                        TaskId = t.Id,
                        PeopleId = t.AboutWho.PeopleId,
                        Who = t.AboutWho.Name,
                        Description = t.Description,
                    };
            return q;
        }
        public IEnumerable<CodeValueItem> Tags()
        {
            var up = DbUtil.Db.CurrentUserPerson;
            if (up == null)
                return new List<CodeValueItem>();
            var ctl = new CodeValueModel();
            var pid = DbUtil.Db.CurrentUser.PeopleId;
            var list = ctl.UserTags(pid);
            return list;
        }
        public class SearchInfo
        {
            public string line1 { get; set; }
            public string line2 { get; set; }
            public bool isOrg { get; set; }
            public int id { get; set; }
        }
        public static IEnumerable<SearchInfo> Names(string text)
        {
            string First, Last;
            var qp = DbUtil.Db.People.AsQueryable();
            var qo = from o in DbUtil.Db.Organizations
                     where o.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Active
                     select o;
            if (Util2.OrgLeadersOnly)
                qp = DbUtil.Db.OrgLeadersOnlyTag2().People(DbUtil.Db);

            qp = from p in qp
                 where p.DeceasedDate == null
                 select p;

            Util.NameSplit(text, out First, out Last);

            var hasfirst = First.HasValue();
            if (text.AllDigits())
            {
                string phone = null;
                if (text.HasValue() && text.AllDigits() && text.Length == 7)
                    phone = text;
                if (phone.HasValue())
                {
                    var id = Last.ToInt();
                    qp = from p in qp
                         where
                             p.PeopleId == id
                             || p.CellPhone.Contains(phone)
                             || p.Family.HomePhone.Contains(phone)
                             || p.WorkPhone.Contains(phone)
                         select p;
                    qo = from o in qo
                         where o.OrganizationId == id
                         select o;
                }
                else
                {
                    var id = Last.ToInt();
                    qp = from p in qp
                         where p.PeopleId == id
                         select p;
                    qo = from o in qo
                         where o.OrganizationId == id
                         select o;
                }
            }
            else
            {
                qp = from p in qp
                     where
                         (
                             (p.LastName.StartsWith(Last) || p.MaidenName.StartsWith(Last)
                              || p.LastName.StartsWith(text) || p.MaidenName.StartsWith(text))
                             &&
                             (!hasfirst || p.FirstName.StartsWith(First) || p.NickName.StartsWith(First) ||
                              p.MiddleName.StartsWith(First)
                              || p.LastName.StartsWith(text) || p.MaidenName.StartsWith(text))
                         )
                     select p;

                qo = from o in qo
                     where o.OrganizationName.Contains(text) || o.LeaderName.Contains(text)
                     select o;
            }

            var rp = from p in qp
                     let age = p.Age.HasValue ? " (" + p.Age + ")" : ""
                     orderby p.Name2
                     select new SearchInfo()
                                {
                                    id = p.PeopleId,
                                    line1 = p.Name2 + age,
                                    line2 = p.PrimaryAddress ?? "",
                                    isOrg = false,
                                };
            var ro = from o in qo
                     orderby o.OrganizationName
                     select new SearchInfo()
                                {
                                    id = o.OrganizationId,
                                    line1 = o.OrganizationName,
                                    line2 = o.Division.Name,
                                    isOrg = true
                                };

            var list = new List<SearchInfo>();
            list.AddRange(rp.Take(6));
            if (list.Count > 0)
                list.Add(new SearchInfo() { id = 0 });
            var roTake = ro.Take(4).ToList();
            list.AddRange(roTake);
            if (roTake.Count > 0)
                list.Add(new SearchInfo() { id = 0 });
            list.AddRange(new List<SearchInfo>() 
            { 
                new SearchInfo() { id = -1, line1 = "People Search"  }, 
                new SearchInfo() { id = -2, line1 = "Search Builder" }, 
                new SearchInfo() { id = -3, line1 = "Organization Search" }, 
            });
            return list;
        }
        public class SearchInfo22
        {
            public string line1 { get; set; }
            public string line2 { get; set; }
            public string url { get; set; }
            public bool addmargin { get; set; }
        }

        public static IEnumerable<SearchInfo22> PrefetchSearch()
        {
            var list = (from c in DbUtil.Db.Queries
                        where c.Name != Util.ScratchPad2
                        where c.Owner == Util.UserName
                        orderby c.LastRun descending
                        select new SearchInfo22()
                        {
                            url = "/Query/" + c.QueryId,
                            line1 = c.Name,
                        }).Take(3).ToList();
            list.InsertRange(0, new List<SearchInfo22>() 
            {
                //new SearchInfo22() { url = "/PeopleSearch?name=%QUERY", line1 = "Find Person"  }, 
                new SearchInfo22() { url = "/PeopleSearch", line1 = "Find Person"  }, 
                new SearchInfo22() { url = "/OrgSearch", line1 = "Organization Search" }, 
                new SearchInfo22() { url = "/Query", line1 = "Advanced Search" }, 
                new SearchInfo22() { url = "/SavedQueryList", line1 = "Saved Searches" }, 
                new SearchInfo22() { url = "/Query/NewQuery", line1 = "New Search", 
                    addmargin = true }, 
            });
            return list;
        }

        public static IEnumerable<SearchInfo22> FastSearch(string text)
        {
            string First, Last;
            var qp = DbUtil.Db.People.AsQueryable();
            var qo = from o in DbUtil.Db.Organizations
                     where o.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Active
                     select o;
            if (Util2.OrgLeadersOnly)
                qp = DbUtil.Db.OrgLeadersOnlyTag2().People(DbUtil.Db);

            qp = from p in qp
                 where p.DeceasedDate == null
                 select p;

            Util.NameSplit(text, out First, out Last);

            var hasfirst = First.HasValue();
            if (text.AllDigits())
            {
                string phone = null;
                if (text.HasValue() && text.AllDigits() && text.Length == 7)
                    phone = text;
                if (phone.HasValue())
                {
                    var id = Last.ToInt();
                    qp = from p in qp
                         where
                             p.PeopleId == id
                             || p.CellPhone.Contains(phone)
                             || p.Family.HomePhone.Contains(phone)
                             || p.WorkPhone.Contains(phone)
                         select p;
                    qo = from o in qo
                         where o.OrganizationId == id
                         select o;
                }
                else
                {
                    var id = Last.ToInt();
                    qp = from p in qp
                         where p.PeopleId == id
                         select p;
                    qo = from o in qo
                         where o.OrganizationId == id
                         select o;
                }
            }
            else
            {
                qp = from p in qp
                     where
                         (
                             (p.LastName.StartsWith(Last) || p.MaidenName.StartsWith(Last)
                              || p.LastName.StartsWith(text) || p.MaidenName.StartsWith(text))
                             &&
                             (!hasfirst || p.FirstName.StartsWith(First) || p.NickName.StartsWith(First) ||
                              p.MiddleName.StartsWith(First)
                              || p.LastName.StartsWith(text) || p.MaidenName.StartsWith(text))
                         )
                     select p;

                qo = from o in qo
                     where o.OrganizationName.Contains(text) || o.LeaderName.Contains(text)
                     select o;
            }

            var rp = from p in qp
                     let age = p.Age.HasValue ? " (" + p.Age + ")" : ""
                     orderby p.Name2
                     select new SearchInfo22()
                                {
                                    url = "/Person2/" + p.PeopleId,
                                    line1 = p.Name2 + age,
                                    line2 = p.PrimaryAddress ?? "",
                                };
            var ro = from o in qo
                     orderby o.OrganizationName
                     select new SearchInfo22()
                                {
                                    url = "/Organization/Index/" + o.OrganizationId,
                                    line1 = o.OrganizationName,
                                    line2 = o.Division.Name,
                                };

            var list = new List<SearchInfo22>();
            list.AddRange(rp.Take(6));
            if (list.Count > 0)
                list[list.Count - 1].addmargin = true;
            var roTake = ro.Take(4).ToList();
            list.AddRange(roTake);
            if (roTake.Count > 0)
                list[list.Count - 1].addmargin = true;
            list.AddRange(new List<SearchInfo22>() 
            {
                new SearchInfo22() { url = "/PeopleSearch?name=%QUERY", line1 = "Find Person"  }, 
                new SearchInfo22() { url = "/Query", line1 = "Advanced Search Builder" }, 
                new SearchInfo22() { url = "/OrgSearch", line1 = "Organization Search" }, 
            });
            return list;
        }
    }
}
