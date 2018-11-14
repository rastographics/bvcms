/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsData.Classes.RoleChecker;
using CmsWeb.Models;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class SearchResultsModel : PagedTableModel<Person, SearchResultInfo>
    {
        private readonly string[] usersOnlyContextTypes = { "taskdelegate", "taskowner", "taskdelegate2" };

        public SearchResultsModel()
            : base(null, null, true)
        {
            ShowPageSize = false;
        }

        public string AddContext { get; set; }
        public string Name { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        [DisplayName("Communication")]
        public string Communication { get; set; }

        public string Address { get; set; }

        [DisplayName("Date of Birth")]
        public string dob { get; set; }

        public bool UsersOnly => usersOnlyContextTypes.Contains(AddContext.ToLower());

        public bool ShowLimitedSearch => RoleChecker.HasSetting(SettingName.LimitedSearchPerson, false);
        public bool OnlineRegTypeSearch { get; set; }

        public string HelpLink(string page)
        {
            return Util.HelpLink($"_SearchAdd_{page}");
        }

        public override IQueryable<Person> DefineModelList()
        {
            if (ShowLimitedSearch)
            {
                return RunLimitedSearch();
            }

            //var db = Db;
            var q = Util2.OrgLeadersOnly
                ? DbUtil.Db.OrgLeadersOnlyTag2().People(DbUtil.Db)
                : DbUtil.Db.People.AsQueryable();

            if (UsersOnly)
            {
                q = q.Where(p => p.Users.Any(uu => uu.UserRoles.Any(ur => ur.Role.RoleName == "Access")));
            }

            if (OnlineRegTypeSearch)
            {
                return from pid in DbUtil.Db.FindPerson(FirstName, LastName, dob.ToDate(), Email, Phone)
                       join p in q on pid.PeopleId equals p.PeopleId
                       select p;
            }

            if (Name.HasValue())
            {
                string first, last;
                Util.NameSplit(Name, out first, out last);

                if (first.HasValue())
                {
                    q = from p in q
                        where (p.LastName.StartsWith(last) || p.MaidenName.StartsWith(last))
                              && (p.FirstName.StartsWith(first) || p.NickName.StartsWith(first) || p.MiddleName.StartsWith(first))
                        select p;
                }
                else if (last.AllDigits())
                {
                    q = from p in q
                        where p.PeopleId == last.ToInt()
                        select p;
                }
                else
                {
                    q = DbUtil.Db.Setting("UseAltnameContains")
                        ? from p in q
                          where p.LastName.StartsWith(last) || p.MaidenName.StartsWith(last) || p.AltName.Contains(last)
                          select p
                        : from p in q
                          where p.LastName.StartsWith(last) || p.MaidenName.StartsWith(last)
                          select p;
                }
            }

            if (Address.IsNotNull())
            {
                Address = Address.Trim();
                if (Address.HasValue())
                {
                    q = from p in q
                        where p.Family.AddressLineOne.Contains(Address)
                              || p.Family.AddressLineTwo.Contains(Address)
                              || p.Family.CityName.Contains(Address)
                              || p.Family.ZipCode.Contains(Address)
                        select p;
                }
            }
            if (Communication.IsNotNull())
            {
                Communication = Communication.Trim();
                if (Communication.HasValue())
                {
                    q = from p in q
                        where p.CellPhone.Contains(Communication) || p.EmailAddress.Contains(Communication)
                              || p.Family.HomePhone.Contains(Communication)
                              || p.WorkPhone.Contains(Communication)
                        select p;
                }
            }
            if (dob.HasValue())
            {
                DateTime dt;
                if (DateTime.TryParse(dob, out dt))
                {
                    if (Regex.IsMatch(dob, @"\d+/\d+/\d+"))
                    {
                        q = q.Where(p => p.BirthDay == dt.Day && p.BirthMonth == dt.Month && p.BirthYear == dt.Year);
                    }
                    else
                    {
                        q = q.Where(p => p.BirthDay == dt.Day && p.BirthMonth == dt.Month);
                    }
                }
                else
                {
                    int n;
                    if (int.TryParse(dob, out n))
                    {
                        if (n >= 1 && n <= 12)
                        {
                            q = q.Where(p => p.BirthMonth == n);
                        }
                        else
                        {
                            q = q.Where(p => p.BirthYear == n);
                        }
                    }
                }
            }
            return q;
        }

        public override IQueryable<Person> DefineModelSort(IQueryable<Person> q)
        {
            return q.OrderBy(p => p.Name2);
        }

        public override IEnumerable<SearchResultInfo> DefineViewList(IQueryable<Person> q)
        {
            return from p in q
                   orderby p.Name2
                   select new SearchResultInfo
                   {
                       PeopleId = p.PeopleId,
                       FamilyId = p.FamilyId,
                       Name = p.Name,
                       Middle = p.MiddleName,
                       GoesBy = p.NickName,
                       First = p.FirstName,
                       Maiden = p.MaidenName,
                       Address = p.PrimaryAddress,
                       CityStateZip = p.PrimaryCity + ", " + p.PrimaryState + " " + (p.PrimaryZip != null && p.PrimaryZip.Length >= 5 ? p.PrimaryZip.Substring(0, 5) : p.PrimaryZip),
                       Age = p.Age,
                       JoinDate = p.JoinDate,
                       BirthYear = p.BirthYear,
                       BirthMon = p.BirthMonth,
                       BirthDay = p.BirthDay,
                       HomePhone = p.HomePhone,
                       CellPhone = p.CellPhone,
                       WorkPhone = p.WorkPhone,
                       MemberStatus = p.MemberStatus.Description,
                       Email = p.EmailAddress
                   };
        }

        private IQueryable<Person> RunLimitedSearch()
        {
            var matches = new List<Person>();
            var results = DbUtil.Db.FindPerson(FirstName, LastName, null, Email, null);
            foreach (var result in results)
            {
                matches.Add(DbUtil.Db.LoadPersonById(result.PeopleId.GetValueOrDefault()));
            }
            return matches.AsQueryable();
        }
    }
}
