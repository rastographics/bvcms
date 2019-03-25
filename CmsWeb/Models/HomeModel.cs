using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Caching;
using System.Xml;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class HomeModel
    {
        public string UserUrl => "/Person2/" + Util.UserPeopleId;

        public class BirthdayInfo
        {
            public DateTime Birthday { get; set; }
            public string Name { get; set; }
            public int PeopleId { get; set; }
            public string Url => "/Person2/" + PeopleId;
        }

        public HomeModel() { }

        public IEnumerable<BirthdayInfo> Birthdays()
        {
            if (Util.UserPeopleId == null)
            {
                return new List<BirthdayInfo>();
            }

            var qB = DbUtil.Db.Queries.FirstOrDefault(cc => cc.Name == "TrackBirthdays" && cc.Owner == Util.UserName);
            var tagq = DbUtil.Db.FetchTag("FromTrackBirthdaysQuery", Util.UserPeopleId, DbUtil.TagTypeId_System);
            if (qB != null)
            {
                if (tagq?.Created == null || tagq.Created < DateTime.Today)
                {
                    DbUtil.Db.PopulateSpecialTag(DbUtil.Db.PeopleQuery(qB.QueryId), "FromTrackBirthdaysQuery", DbUtil.TagTypeId_System);
                }

                tagq = DbUtil.Db.FetchTag("FromTrackBirthdaysQuery", Util.UserPeopleId, DbUtil.TagTypeId_System);
                if (tagq != null)
                {
                    var q0 = from p in tagq.People(DbUtil.Db)
                             let bd = p.BirthDay
                             let bm = p.BirthMonth
                             where bd != null && bm != null
                             let bd2 = bd == 29 && bm == 2 ? bd - 1 : bd
                             let bdate = new DateTime(DateTime.Now.Year, bm.Value, bd2.Value)
                             let nextbd = bdate < DateTime.Today ? bdate.AddYears(1) : bdate
                             orderby nextbd
                             select new BirthdayInfo
                             {
                                 Birthday = nextbd,
                                 Name = p.Name,
                                 PeopleId = p.PeopleId
                             };
                    return q0.Take(100);
                }
            }
            tagq?.DeleteTag(DbUtil.Db);
            var tag = DbUtil.Db.FetchOrCreateTag("TrackBirthdays", Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            var q = qB != null
                ? DbUtil.Db.PeopleQuery(qB.QueryId)
                : tag.People(DbUtil.Db);


            if (!q.Any())
            {
                q = from p in DbUtil.Db.People
                    let up = DbUtil.Db.People.Single(pp => pp.PeopleId == Util.UserPeopleId)
                    where p.OrganizationMembers.Any(om => om.OrganizationId == up.BibleFellowshipClassId)
                    select p;
            }

            var q2 = from p in q
                     let bd = p.BirthDay
                     let bm = p.BirthMonth
                     where bd != null && bm != null
                     let bd2 = bd == 29 && bm == 2 ? bd - 1 : bd
                     let bdate = new DateTime(DateTime.Now.Year, bm.Value, bd2.Value)
                     let nextbd = bdate < DateTime.Today ? bdate.AddYears(1) : bdate
                     where SqlMethods.DateDiffDay(Util.Now, nextbd) <= 15
                     where p.DeceasedDate == null
                     orderby nextbd
                     select new BirthdayInfo
                     {
                         Birthday = nextbd,
                         Name = p.Name,
                         PeopleId = p.PeopleId
                     };
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
            {
                return new List<MyInvolvementInfo>();
            }

            var pid = u.PeopleId;

            var limitvisibility = Util2.OrgLeadersOnly;
            var oids = new int[0];
            if (Util2.OrgLeadersOnly)
            {
                oids = DbUtil.Db.GetLeaderOrgIds(pid);
            }

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
        public IEnumerable<NewsInfo> BvcmsNews()
        {
            return CachedNewsInfo("BvcmsNews", "http://www.touchpointsoftware.com/feed/", 10);
        }

        public IEnumerable<NewsInfo> ChurchNews()
        {
            var feedurl = DbUtil.Db.Setting("ChurchFeedUrl", "");
            return CachedNewsInfo(DbUtil.Db.Host + "ChurchFeed", feedurl, 120);
        }

        public class MySavedQueryInfo
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
        public IEnumerable<MySavedQueryInfo> MyQueries()
        {
            if (Util.UserPeopleId == null)
            {
                return new List<MySavedQueryInfo>();
            }

            return from c in DbUtil.Db.Queries
                   where c.Owner == Util.UserName
                   where c.Name != Util.ScratchPad2
                   orderby c.Name
                   select new MySavedQueryInfo
                   {
                       Name = c.Name,
                       Url = "/Query/" + c.QueryId
                   };
        }
        public class TaskInfo
        {
            public int TaskId { get; set; }
            public int PeopleId { get; set; }
            public string Who { get; set; }
            public string Description { get; set; }
            public string Url => "/Person2/" + PeopleId;
        }
        public IEnumerable<TaskInfo> Tasks()
        {
            if (Util.UserPeopleId == null)
            {
                return new List<TaskInfo>();
            }

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
            if (Util.UserPeopleId == null)
            {
                return new List<CodeValueItem>();
            }

            var ctl = new CodeValueModel();
            var list = ctl.UserTags(Util.UserPeopleId);
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
            {
                qp = DbUtil.Db.OrgLeadersOnlyTag2().People(DbUtil.Db);
            }

            qp = from p in qp
                 where p.DeceasedDate == null
                 select p;

            Util.NameSplit(text, out First, out Last);

            var hasfirst = First.HasValue();
            if (text.AllDigits())
            {
                string phone = null;
                if (text.HasValue() && text.AllDigits() && text.Length == 7)
                {
                    phone = text;
                }

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
                     orderby p.Name2
                     select new SearchInfo()
                     {
                         id = p.PeopleId,
                         line1 = p.Name2 + (p.Age.HasValue ? $" ({Person.AgeDisplay(p.Age, p.PeopleId)})" : ""),
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
            {
                list.Add(new SearchInfo() { id = 0 });
            }

            var roTake = ro.Take(4).ToList();
            list.AddRange(roTake);
            if (roTake.Count > 0)
            {
                list.Add(new SearchInfo() { id = 0 });
            }

            list.AddRange(new List<SearchInfo>()
            {
                new SearchInfo() { id = -1, line1 = "People Search"  },
                new SearchInfo() { id = -2, line1 = "Last Search" },
                new SearchInfo() { id = -3, line1 = "Organization Search" },
            });
            return list;
        }
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class SearchInfo22
        {
            public SearchInfo22()
            {
                showaltname = DbUtil.Db.Setting("ShowAltNameOnSearchResults");
            }

            public string line1 => nonPersonName.HasValue()
                ? nonPersonName
                : displayname + (age.HasValue ? $" ({Person.AgeDisplay(age, peopleid)})" : "");

            public string line2 { get; set; }
            public string url { get; set; }
            public bool addmargin { get; set; }

            internal int peopleid;
            internal int? age;

            internal string displayname => (showaltname ? $"{(name2.HasValue()? name2:lastname)} {altname}" : name2.HasValue()?name2:lastname);
            internal string name2;
            internal string lastname;
            internal string nonPersonName;
            internal string altname;
            internal bool showaltname;
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
                            nonPersonName = c.Name,
                        }).Take(3).ToList();
            list.InsertRange(0, new List<SearchInfo22>()
            {
                new SearchInfo22() { url = "/PeopleSearch", nonPersonName = "Find Person"  },
                new SearchInfo22() { url = "/OrgSearch", nonPersonName = "Organization Search" },
                new SearchInfo22() { url = "/NewQuery", nonPersonName = "New Search", addmargin = true },
                new SearchInfo22() { url = "/Query", nonPersonName = "Last Search" },
                new SearchInfo22() { url = "/SavedQueryList", nonPersonName = "Saved Searches" },
            });
            return list;
        }

        public static IEnumerable<SearchInfo22> FastSearch(string text)
        {
            string first, last;
            var qp = DbUtil.Db.People.AsQueryable();
            var qo = from o in DbUtil.Db.Organizations
                     where o.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Active
                     select o;
            if (Util2.OrgLeadersOnly)
            {
                qp = DbUtil.Db.OrgLeadersOnlyTag2().People(DbUtil.Db);
            }

            qp = from p in qp
                 where p.DeceasedDate == null
                 select p;

            Util.NameSplit(text, out first, out last);
            IEnumerable<SearchInfo22> rp = null;

            if (text.AllDigits())
            {
                string phone = null;
                if (text.HasValue() && text.AllDigits() && text.Length == 7)
                {
                    phone = text;
                }

                if (phone.HasValue())
                {
                    var id = last.ToInt();
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
                    var id = last.ToInt();
                    qp = from p in qp
                         where p.PeopleId == id
                         select p;
                    qo = from o in qo
                         where o.OrganizationId == id
                         select o;
                }
                rp = (from p in qp
                      orderby p.Name2
                      select new SearchInfo22()
                      {
                          url = "/Person2/" + p.PeopleId,
                          line2 = p.PrimaryAddress ?? "",
                          peopleid = p.PeopleId,
                          age = p.Age,
                          name2 = p.Name2,
                          altname = p.AltName,
                          lastname = p.LastName
                      }).Take(6);
            }
            else
            {
                if (first.HasValue())
                {
                    //Util.NameSplit(text, out first, out last) returns both first and last.
                    qp = from p in qp
                         where p.LastName.StartsWith(last) || p.MaidenName.StartsWith(last)
                             || p.LastName.StartsWith(text) || p.MaidenName.StartsWith(text) // gets Bob St Clair
                         where
                             p.FirstName.StartsWith(first) || p.NickName.StartsWith(first) || p.MiddleName.StartsWith(first) || p.AltName.StartsWith(first)
                             || p.FirstName.StartsWith(text) || p.NickName.StartsWith(text) || p.MiddleName.StartsWith(text) || p.AltName.StartsWith(text) // gets Bob St Clair
                         select p;
                    rp = (from p in qp
                          orderby p.Name2
                          select new SearchInfo22()
                          {
                              url = "/Person2/" + p.PeopleId,
                              line2 = p.PrimaryAddress ?? "",
                              peopleid = p.PeopleId,
                              age = p.Age,
                              name2 = p.Name2,
                              altname = p.AltName,
                              lastname = p.LastName
                          }).Take(6);
                }
                else
                {
                    //Util.NameSplit(text, out first, out last) returns only last.
                    var qp2 = DbUtil.Db.Setting("UseAltnameContains")
                        ? from p in qp
                          where p.LastName.StartsWith(text) || p.MaidenName.StartsWith(text) || p.AltName.Contains(text)
                          select p
                        : from p in qp
                          where p.LastName.StartsWith(text) || p.MaidenName.StartsWith(text) || p.AltName.StartsWith(text)
                          select p;
                    var qp1 = from p in qp
                              where !qp2.Select(pp => pp.PeopleId).Contains(p.PeopleId)
                              where p.FirstName.StartsWith(text) || p.NickName.StartsWith(text) || p.MiddleName.StartsWith(text)
                              select p;
                    var rp2 = (from p in qp2
                               orderby p.Name2
                               select new SearchInfo22()
                               {
                                   url = "/Person2/" + p.PeopleId,
                                   line2 = p.PrimaryAddress ?? "",
                                   peopleid = p.PeopleId,
                                   age = p.Age,
                                   name2 = p.Name2,
                                   altname = p.AltName,
                                   lastname = p.LastName
                               }).Take(6).ToList();
                    var rp1 = (from p in qp1
                               orderby p.Name2
                               select new SearchInfo22()
                               {
                                   url = "/Person2/" + p.PeopleId,
                                   line2 = p.PrimaryAddress ?? "",
                                   peopleid = p.PeopleId,
                                   age = p.Age,
                                   name2 = p.Name2,
                                   altname = p.AltName,
                                   lastname = p.LastName
                               }).Take(6).ToList();
                    rp = rp2.Union(rp1).Take(6);
                }

                qo = from o in qo
                     where o.OrganizationName.Contains(text) || o.LeaderName.Contains(text)
                     select o;
            }

            var ro = from o in qo
                     orderby o.OrganizationName
                     select new SearchInfo22()
                     {
                         url = $"/Org/{o.OrganizationId}",
                         line2 = o.Division.Name,
                         nonPersonName = o.OrganizationName,
                     };

            var list = new List<SearchInfo22>();
            list.AddRange(rp);
            if (list.Count > 0)
            {
                list[list.Count - 1].addmargin = true;
            }

            var roTake = ro.Take(4).ToList();
            list.AddRange(roTake);
            if (roTake.Count > 0)
            {
                list[list.Count - 1].addmargin = true;
            }

            list.AddRange(new List<SearchInfo22>()
            {
                new SearchInfo22() { url = "/PeopleSearch", nonPersonName = "Find Person"  },
                new SearchInfo22() { url = "/OrgSearch", nonPersonName = "Organization Search" },
                new SearchInfo22() { url = "/NewQuery", nonPersonName = "New Search", addmargin = true },
                new SearchInfo22() { url = "/Query", nonPersonName = "Last Search" },
                new SearchInfo22() { url = "/SavedQueryList", nonPersonName = "Saved Searches" },
            });
            return list;
        }

        public string ChurchBlogUrl => DbUtil.Db.Setting("ChurchBlogUrl", "#");
        public bool ShowTip => !DbUtil.Db.UserPreference("hide-tip-home", "false").ToBool();
        public string BlogAppUrl => DbUtil.Db.Setting("BlogAppUrl", "#");

        private static IEnumerable<NewsInfo> GetNewsInfo(string feedurl)
        {
            var feed = DbUtil.Db.RssFeeds.FirstOrDefault(r => r.Url == feedurl);
            if (!feedurl.HasValue())
            {
                return new List<NewsInfo>();
            }

            var req = WebRequest.Create(feedurl) as HttpWebRequest;
            if (feed != null && req != null)
            {
                if (feed.LastModified.HasValue)
                {
                    req.IfModifiedSince = feed.LastModified.Value;
                }
            }
            else
            {
                feed = new RssFeed();
                DbUtil.Db.RssFeeds.InsertOnSubmit(feed);
                feed.Url = feedurl;
            }

            if (req == null)
            {
                return new List<NewsInfo>();
            }

            try
            {
                var resp = req.GetHttpResponse();
                if (resp == null)
                {
                    return new List<NewsInfo>();
                }

                if (resp.StatusCode != HttpStatusCode.NotModified)
                {
                    feed.LastModified = resp.LastModified;
                    var rs = resp.GetResponseStream();
                    if (rs == null)
                    {
                        return new List<NewsInfo>();
                    }

                    var sr = new StreamReader(rs);
                    feed.Data = sr.ReadToEnd();
                    sr.Close();
                    DbUtil.Db.SubmitChanges();
                }
            }
            catch
            {
                return new List<NewsInfo>();
            }
            if (feed.Data == null)
            {
                return new List<NewsInfo>();
            }

            try
            {
                var reader = XmlReader.Create(new StringReader(feed.Data));
                var f = SyndicationFeed.Load(reader);
                if (f == null)
                {
                    return new List<NewsInfo>();
                }

                var posts = from item in f.Items
                            select new NewsInfo
                            {
                                Title = item.Title.Text,
                                Published = item.PublishDate.DateTime,
                                Url = item.Links.Single(i => i.RelationshipType == "alternate")?.GetAbsoluteUri()?.AbsoluteUri
                            };
                return posts;
            }
            catch
            {
                return new List<NewsInfo>();
            }
        }
        private static IEnumerable<NewsInfo> CachedNewsInfo(string bvcmsnews, string feedurl, int minutes)
        {
            var list = HttpRuntime.Cache[bvcmsnews] as List<NewsInfo>;
            if (list != null)
            {
                return list;
            }

            list = GetNewsInfo(feedurl).ToList();
            HttpRuntime.Cache.Insert(bvcmsnews, list, null,
                DateTime.Now.AddMinutes(minutes), Cache.NoSlidingExpiration);
            return list;
        }
    }
}
