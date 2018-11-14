/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using CmsData;
using CmsWeb.Code;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class TagsModel : PagerModel2
    {
        //private readonly CMSDataContext Db;
        private int TagTypeId { get; set; }
        private string TagName { get; set; }
        private int? TagOwner { get; set; }

        public string tag { get; set; }
        public string tagname { get; set; }

        public TagsModel()
        {
            //Db = Db;
            Direction = "asc";
            GetCount = Count;
            SetCurrentTag();
        }
        public void SetCurrentTag()
        {
            if (tag.HasValue())
            {
                var a = tag.SplitStr(",", 2);
                if (a.Length > 1)
                {
                    Util2.CurrentTag = a[1];
                }
                else
                {
                    Util2.CurrentTag = tag;
                }
            }
            TagTypeId = DbUtil.TagTypeId_Personal;
            TagName = Util2.CurrentTagName;
            TagOwner = Util2.CurrentTagOwnerId;
            ShareIds = GetShareIds();
        }

        public bool usersonly { get; set; }

        private IQueryable<Person> people;
        private IQueryable<Person> FetchPeople()
        {
            if (people != null)
            {
                return people;
            }

            people = Util2.OrgLeadersOnly
                ? DbUtil.Db.OrgLeadersOnlyTag2().People(DbUtil.Db)
                : DbUtil.Db.People.Select(p => p);

            if (usersonly)
            {
                people = people.Where(p => p.Users.Any());
            }

            var tagid = DbUtil.Db.TagCurrent().Id;
            people = people.Where(p => p.Tags.Any(tp => tp.Id == tagid));
            return people;
        }
        public int SharedCount()
        {
            return DbUtil.Db.TagCurrent().TagShares.Count();
        }
        public IEnumerable<MailingController.TaggedPersonInfo> PeopleList()
        {
            var people = FetchPeople();
            if (!_count.HasValue)
            {
                _count = people.Count();
            }

            people = ApplySort(people)
                .Skip(StartRow).Take(PageSize);
            return PeopleList(people);
        }
        private IEnumerable<MailingController.TaggedPersonInfo> PeopleList(IQueryable<Person> query)
        {
            var q = from p in query
                    select new MailingController.TaggedPersonInfo
                    {
                        PeopleId = p.PeopleId,
                        Name = p.Name,
                        BirthYear = p.BirthYear,
                        BirthMon = p.BirthMonth,
                        BirthDay = p.BirthDay,
                        Address = p.PrimaryAddress,
                        Address2 = p.PrimaryAddress2,
                        CityStateZip = Util.FormatCSZ(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                        HomePhone = p.HomePhone,
                        CellPhone = p.CellPhone,
                        WorkPhone = p.WorkPhone,
                        PhonePref = p.PhonePrefId,
                        MemberStatus = p.MemberStatus.Description,
                        Email = p.EmailAddress,
                        BFTeacher = p.BFClass.LeaderName,
                        BFTeacherId = p.BFClass.LeaderId,
                        Age = Person.AgeDisplay(p.Age, p.PeopleId).ToString(),
                        Deceased = p.DeceasedDate.HasValue,
                        HasTag = p.Tags.Any(t => t.Tag.Name == TagName && t.Tag.PeopleId == TagOwner && t.Tag.TypeId == TagTypeId),
                    };
            return q;
        }

        public IQueryable<Person> ApplySort(IQueryable<Person> query)
        {
            if (!Sort.HasValue())
            {
                Sort = "Name";
            }

            switch (Direction)
            {
                case "asc":
                    switch (Sort)
                    {
                        case "Status":
                            query = from p in query
                                    orderby p.MemberStatus.Code,
                                    p.LastName,
                                    p.FirstName,
                                    p.PeopleId
                                    select p;
                            break;
                        case "Address":
                            query = from p in query
                                    orderby p.PrimaryState,
                                    p.PrimaryCity,
                                    p.PrimaryAddress,
                                    p.PeopleId
                                    select p;
                            break;
                        case "Fellowship Leader":
                            query = from p in query
                                    orderby p.BFClass.LeaderName,
                                    p.LastName,
                                    p.FirstName,
                                    p.PeopleId
                                    select p;
                            break;
                        case "DOB":
                            query = from p in query
                                    orderby p.BirthMonth, p.BirthDay,
                                    p.LastName, p.FirstName
                                    select p;
                            break;
                        case "Name":
                            query = from p in query
                                    orderby p.LastName,
                                    p.FirstName,
                                    p.PeopleId
                                    select p;
                            break;
                    }
                    break;
                case "desc":
                    switch (Sort)
                    {
                        case "Name":
                            query = from p in query
                                    orderby p.LastName descending,
                                    p.FirstName,
                                    p.PeopleId
                                    select p;
                            break;
                        case "Status":
                            query = from p in query
                                    orderby p.MemberStatus.Code descending,
                                    p.LastName descending,
                                    p.FirstName descending,
                                    p.PeopleId descending
                                    select p;
                            break;
                        case "Address":
                            query = from p in query
                                    orderby p.PrimaryState descending,
                                    p.PrimaryCity descending,
                                    p.PrimaryAddress descending,
                                    p.PeopleId descending
                                    select p;
                            break;
                        case "Fellowship Leader":
                            query = from p in query
                                    orderby p.BFClass.LeaderName descending,
                                    p.LastName descending,
                                    p.FirstName descending,
                                    p.PeopleId descending
                                    select p;
                            break;
                        case "DOB":
                            query = from p in query
                                    orderby p.BirthMonth descending, p.BirthDay descending,
                                    p.LastName descending, p.FirstName descending
                                    select p;
                            break;
                    }
                    break;
            }
            return query;
        }

        private CodeValueModel cv = new CodeValueModel();
        public List<CodeValueItem> Tags()
        {
            var t = DbUtil.Db.TagCurrent();
            var tags = cv.UserTags(Util.UserPeopleId);
            return tags;
        }
        public string GetShareIds()
        {
            var t = DbUtil.Db.TagCurrent();
            var s = string.Join(",", t.TagShares.Select(tt => tt.PeopleId).ToArray());
            return s;
        }
        public string ShareIds { get; set; }
        public void SetShareIds()
        {
            var tag = DbUtil.Db.TagCurrent();
            var selected_pids = ShareIds.SplitStr(",").Select(s => s.ToInt()).ToArray();
            var userDeletes = tag.TagShares.Where(ts => !selected_pids.Contains(ts.PeopleId));
            DbUtil.Db.TagShares.DeleteAllOnSubmit(userDeletes);
            var tag_pids = tag.TagShares.Select(ts => ts.PeopleId).ToArray();
            var userAdds = from pid in selected_pids
                           join tpid in tag_pids on pid equals tpid into j
                           from p in j.DefaultIfEmpty(-1)
                           where p == -1
                           select pid;
            foreach (var pid in userAdds)
            {
                tag.TagShares.Add(new TagShare { PeopleId = pid });
            }

            DbUtil.Db.SubmitChanges();
        }
        private int? _count;
        public int Count()
        {
            if (!_count.HasValue)
            {
                _count = FetchPeople().Count();
            }

            return _count.Value;
        }

        private int? currentTagId;
        public int CurrentTagId
        {
            get
            {
                if (!currentTagId.HasValue)
                {
                    currentTagId = DbUtil.Db.TagCurrent().Id;
                }

                return currentTagId ?? 0;
            }
        }
    }
}
