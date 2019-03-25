/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Classes.ProtectMyMinistry;
using CmsData.Codes;
using Dapper;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public partial class CodeValueModel
    {
        private CMSDataContext Db;
        private static readonly CodeValueItem[] top =
        {
            new CodeValueItem
            {
                Id = 0,
                Value = "(not specified)",
                Code = "0"
            }
        };

        public CodeValueModel()
        {
            Db = DbUtil.Db;
        }

        public CodeValueModel(CMSDataContext db)
        {
            Db = db;
        }
        public List<CodeValueItem> Activities()
        {
            var q = from a in Db.CheckInActivities
                    group a.Activity by a.Activity
                into g
                    select new CodeValueItem
                    {
                        Code = g.Key,
                        Value = g.Key
                    };
            var list = q.ToList();
            return list;
        }

        public IEnumerable<CodeValueItem> AddressTypeCodes()
        {
            return from at in Db.AddressTypes
                   select new CodeValueItem
                   {
                       Id = at.Id,
                       Code = at.Code,
                       Value = at.Description
                   };
        }

        public IEnumerable<CodeValueItem> AdhocExtraValueTypeCodes()
        {
            yield return new CodeValueItem { Code = "Text", Value = "Text (multi line)" };
            yield return new CodeValueItem { Code = "Code", Value = "Code" };
            yield return new CodeValueItem { Code = "Bit", Value = "Checkbox" };
            yield return new CodeValueItem { Code = "Int", Value = "Integer" };
            yield return new CodeValueItem { Code = "Date", Value = "Date" };
        }

        public IEnumerable<CodeValueItem> AllCampuses()
        {
            var qc = Db.Campus.AsQueryable();
            qc = Db.Setting("SortCampusByCode")
                ? qc.OrderBy(cc => cc.Code)
                : qc.OrderBy(cc => cc.Description);
            return from c in qc
                   select new CodeValueItem
                   {
                       Id = c.Id,
                       Code = c.Code,
                       Value = c.Description
                   };
        }

        public IEnumerable<CodeValueItem> AllCampuses0()
        {
            return AllCampuses().AddNotSpecified();
        }

        public IEnumerable<CodeValueItem> AllCampusesNo()
        {
            return AllCampuses().AddNotSpecified("No Campus");
        }

        public IEnumerable<CodeValueItem> AllOrgDivTags()
        {
            var q = from program in Db.Programs
                    from div in program.Divisions
                    orderby program.Name, div.Name
                    select new CodeValueItem
                    {
                        Id = div.Id,
                        Value = $"{program.Name}: {div.Name}"
                    };
            return top.Union(q);
        }

        public IEnumerable<DropDownItem> AllOrgDivTags2()
        {
            var q = from program in Db.Programs
                    from div in program.Divisions
                    orderby program.Name, div.Name
                    select new DropDownItem
                    {
                        Value = $"{program.Id}:{div.Id}",
                        Text = $"{program.Name}: {div.Name}"
                    };
            return new[]
            {
                new DropDownItem
                {
                    Text = "(not specified)",
                    Value = "0"
                }
            }.Union(q);
        }

        public IEnumerable<CodeValueItem> AttendanceTypeCodes()
        {
            return from c in Db.AttendTypes
                   select new CodeValueItem
                   {
                       Id = c.Id,
                       Code = c.Code,
                       Value = c.Description
                   };
        }

        public static IEnumerable<CodeValueItem> AttendCommitmentCodes()
        {
            yield return new CodeValueItem { Id = AttendCommitmentCode.Attending, Code = "AT", Value = "Attending" };
            yield return new CodeValueItem { Id = AttendCommitmentCode.FindSub, Code = "FS", Value = "Find Sub" };
            yield return new CodeValueItem { Id = AttendCommitmentCode.SubFound, Code = "SF", Value = "Sub Found" };
            yield return new CodeValueItem { Id = AttendCommitmentCode.Substitute, Code = "SB", Value = "Substitute" };
            yield return new CodeValueItem { Id = AttendCommitmentCode.Regrets, Code = "RG", Value = "Regrets" };
        }

        public static IEnumerable<CodeValueItem> AttendCredits()
        {
            return from ms in DbUtil.Db.AttendCredits
                   orderby ms.Id
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public List<CodeValueItem> BackgroundStatuses()
        {
            var list = new List<CodeValueItem>();
            for (var i = 0; i < ProtectMyMinistryHelper.STATUSES.Length; i++)
                list.Add(new CodeValueItem
                {
                    Id = i,
                    Code = i.ToString(),
                    Value = ProtectMyMinistryHelper.STATUSES[i]
                });
            list.Insert(0, new CodeValueItem { Id = 99, Code = "99", Value = "(not specified)" });
            return list;
        }

        public List<CodeValueItem> BadETCodes()
        {
            return new List<CodeValueItem>
            {
                new CodeValueItem {Id = 11, Value = "Enroll-Enroll", Code = "N"},
                new CodeValueItem {Id = 55, Value = "Drop-Drop", Code = "C"},
                new CodeValueItem {Id = 15, Value = "Same Time", Code = "C"},
                new CodeValueItem {Id = 10, Value = "Missing Drop", Code = "B"}
            };
        }

        public IEnumerable<CodeValueItem> BundleHeaderTypes()
        {
            return from ms in Db.BundleHeaderTypes
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> BundleHeaderTypes0()
        {
            return BundleHeaderTypes().AddNotSpecified();
        }

        public static IEnumerable<CodeValueItem> BundleStatusTypes()
        {
            return from bs in DbUtil.Db.BundleStatusTypes
                   let hasDataEntryRole = DbUtil.Db.Roles.Any(rr => rr.RoleName == "FinanceDataEntry")
                   where bs.Id < 2 || hasDataEntryRole
                   select new CodeValueItem
                   {
                       Id = bs.Id,
                       Code = bs.Code,
                       Value = bs.Description
                   };
        }

        public IEnumerable<CodeValueItem> ContactReasonCodes()
        {
            return from c in Db.ContactReasons
                   orderby c.Description.StartsWith("-") ? "Z" + c.Description : c.Description
                   select new CodeValueItem
                   {
                       Id = c.Id,
                       Code = c.Code,
                       Value = c.Description
                   };
        }

        public IEnumerable<CodeValueItem> ContactTypeCodes()
        {
            return from c in Db.ContactTypes
                   orderby c.Description.StartsWith("-") ? "Z" + c.Description : c.Description
                   select new CodeValueItem
                   {
                       Id = c.Id,
                       Code = c.Code,
                       Value = c.Description
                   };
        }

        public IEnumerable<CodeValueItem> ContributionStatuses()
        {
            return from ms in Db.ContributionStatuses
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> ContributionStatuses99()
        {
            return ContributionStatuses().AddNotSpecified(99);
        }

        public IEnumerable<CodeValueItem> ContributionTypes()
        {
            return from ms in Db.ContributionTypes
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> ContributionTypes0()
        {
            return ContributionTypes().AddNotSpecified();
        }

        public static List<SelectListItem> ConvertToSelect(object items, string valuefield)
        {
            var list = items as IEnumerable<CodeValueItem>;
            if (list == null)
                throw new Exception("items are null in ConvertToSelect");
            List<SelectListItem> list2;
            switch (valuefield)
            {
                case "IdCode":
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.IdCode }).ToList();
                    break;
                case "Id":
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.Id.ToString() }).ToList();
                    break;
                case "Code":
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.Code }).ToList();
                    break;
                default:
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.Value }).ToList();
                    break;
            }
            if (list2.Count > 0)
                list2[0].Selected = true;
            return list2;
        }

        //public List<CodeValueItem> MeetingStatusCodes()
        //{
        //    var list = HttpRuntime.Cache[DbUtil.Db.Host + NAME] as List<CodeValueItem>;
        //    if (list == null)
        //    {
        //        return from ms in DbUtil.Db.MeetingStatuses
        //                select new CodeValueItem
        //                {
        //                    Id = ms.Id,
        //                    Code = ms.Code,
        //                    Value = ms.Description
        //                };
        //        list = q.ToList();
        //        HttpRuntime.Cache[DbUtil.Db.Host + NAME] = list;
        //    }
        //    return list;
        //}
        public List<CodeValueItem> DateFields()
        {
            return new List<CodeValueItem>
            {
                new CodeValueItem {Id = 1, Value = "Joined", Code = "JoinDate"},
                new CodeValueItem {Id = 2, Value = "Dropped", Code = "DropDate"},
                new CodeValueItem {Id = 3, Value = "Decision", Code = "DecisionDate"},
                new CodeValueItem {Id = 4, Value = "Baptism", Code = "BaptismDate"},
                new CodeValueItem {Id = 5, Value = "Wedding", Code = "WeddingDate"},
                new CodeValueItem {Id = 6, Value = "New Member Class", Code = "NewMemberClassDate"}
            };
        }

        public static SelectList DaysOfWeek()
        {
            return new SelectList(new[]
            {
                new {Text = "Sunday", Value = "0"},
                new {Text = "Monday", Value = "1"},
                new {Text = "Tuesday", Value = "2"},
                new {Text = "Wednesday", Value = "3"},
                new {Text = "Thursday", Value = "4"},
                new {Text = "Friday", Value = "5"},
                new {Text = "Saturday", Value = "6"},
                new {Text = "Any Day", Value = "10"}
            }, "Value", "Text");
        }

        public IEnumerable<CodeValueItem> Employers()
        {
            return from p in Db.People
                   group p by p.EmployerOther
                into g
                   orderby g.Key
                   select new CodeValueItem
                   {
                       Value = g.Key
                   };
        }

        public IEnumerable<CodeValueItem> EntryPoints()
        {
            return from ms in Db.EntryPoints
                   orderby ms.Description
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> ExtraValueTypeCodes()
        {
            yield return new CodeValueItem { Code = "Header", Value = "Header" };
            yield return new CodeValueItem { Code = "HTML", Value = "HTML" };
            yield return new CodeValueItem { Code = "Link", Value = "Link" };
            yield return new CodeValueItem { Code = "Text", Value = "Text (single line)" };
            yield return new CodeValueItem { Code = "Text2", Value = "Text (multi line)" };
            yield return new CodeValueItem { Code = "Code", Value = "Dropdown" };
            yield return new CodeValueItem { Code = "Bit", Value = "Checkbox" };
            yield return new CodeValueItem { Code = "Bits", Value = "Checkboxes" };
            yield return new CodeValueItem { Code = "Int", Value = "Integer" };
            yield return new CodeValueItem { Code = "Date", Value = "Date" };
            yield return new CodeValueItem { Code = "Data", Value = "Data" };
        }

        public IEnumerable<CodeValueItem> FamilyPositionCodes()
        {
            return from ms in Db.FamilyPositions
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> Funds()
        {
            var q = from f in Db.ContributionFunds
                    where f.FundStatusId == 1
                    orderby f.FundId
                    select new CodeValueItem
                    {
                        Id = f.FundId,
                        Value = f.FundName
                    };
            var list = q.ToList();
            list.Insert(0, new CodeValueItem { Id = 0, Value = "(not specified)" });
            return list;
        }

        public IEnumerable<CodeValueItem> FundsScopedByRoleMembership()
        {
            const int openFundStatusId = 1;

            return Db.ContributionFunds.ScopedByRoleMembership()
                .Where(fund => fund.FundStatusId == openFundStatusId)
                .OrderBy(fund => fund.FundId)
                .Select(fund => new CodeValueItem { Id = fund.FundId, Value = fund.FundName })
                .ToList();
        }

        public IEnumerable<CodeValueItem> FundsScopedByRoleMembershipWithUnspecified()
        {
            var funds = new List<CodeValueItem>(FundsScopedByRoleMembership());
            funds.Insert(0, new CodeValueItem { Id = 0, Value = "(not specified)" });

            return funds;
        }

        public IEnumerable<CodeValueItem> GenderCodes()
        {
            return from ms in Db.Genders
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public List<CodeValueItem> GenderCodesWithUnspecified()
        {
            var u = new CodeValueItem { Id = 99, Code = "99", Value = "(not specified)" };
            var list = GenderCodes().ToList();
            list.Insert(0, u);
            return list;
        }

        public static IEnumerable<CodeValueItem> GetCountryList()
        {
            return from c in DbUtil.Db.Countries
                   select new CodeValueItem
                   {
                       Code = c.Code,
                       Value = c.Description
                   };
        }

        //--------------------------------------------------
        //--------------Organizations---------------------

        public IEnumerable<CodeValueItem> GetOrganizationList(int DivId)
        {
            return from ot in Db.DivOrgs
                   where (ot.DivId == DivId)
                         && ((SqlMethods.DateDiffMonth(ot.Organization.OrganizationClosedDate, Util.Now) < 14)
                             || (ot.Organization.OrganizationStatusId == 30))
                   orderby ot.Organization.OrganizationStatusId, ot.Organization.OrganizationName
                   select new CodeValueItem
                   {
                       Id = ot.OrgId,
                       Value = Organization.FormatOrgName(ot.Organization.OrganizationName,
                           ot.Organization.LeaderName, ot.Organization.Location)
                   };
        }

        public static List<CodeValueItem> GetStateList()
        {
            var q = from s in DbUtil.Db.StateLookups
                    orderby s.StateCode
                    select new CodeValueItem
                    {
                        Code = s.StateCode,
                        Value = s.StateCode + " - " + s.StateName
                    };
            var list = q.ToList();
            list.Insert(0, new CodeValueItem { Code = "", Value = "(not specified)" });
            return list;
        }


        public List<CodeValueItem> GetStateListUnknown()
        {
            var list = GetStateList().ToList();
            list.Insert(1, new CodeValueItem { Code = "na", Value = "(Unknown)" });
            return list;
        }

        public IEnumerable<CodeValueItem> InterestPoints()
        {
            return from ms in Db.InterestPoints
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public List<CodeValueItem> LetterStatusCodes()
        {
            var q = from ms in Db.MemberLetterStatuses
                    orderby ms.Description
                    select new CodeValueItem
                    {
                        Id = ms.Id,
                        Code = ms.Code,
                        Value = ms.Description
                    };
            return q.ToList();
        }

        public IEnumerable<CodeValueItem> MaritalStatusCodes()
        {
            return from ms in Db.MaritalStatuses
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> MaritalStatusCodes99()
        {
            return MaritalStatusCodes().AddNotSpecified(99);
        }

        public IEnumerable<CodeValueItem> MemberStatusCodes()
        {
            return from ms in Db.MemberStatuses
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> MemberStatusCodes0()
        {
            return MemberStatusCodes().AddNotSpecified();
        }

        public static IEnumerable<CodeValueItem> MemberTypeCodes()
        {
            var list = MemberTypeCodes2();
            return list.Select(c => new CodeValueItem { Code = c.Code, Id = c.Id, Value = c.Value });
        }

        public static List<MemberTypeItem> MemberTypeCodes0()
        {
            var list = MemberTypeCodes2().ToList();
            list.Insert(0, new MemberTypeItem { Id = 0, Value = "(not specified)" });
            return list;
        }

        public static IEnumerable<MemberTypeItem> MemberTypeCodes2()
        {
            return from mt in DbUtil.Db.MemberTypes
                   where mt.Id != MemberTypeCode.Visitor
                   where mt.Id != MemberTypeCode.VisitingMember
                   orderby mt.Description
                   select new MemberTypeItem
                   {
                       Id = mt.Id,
                       Code = mt.Code,
                       Value = mt.Description,
                       AttendanceTypeId = mt.AttendanceTypeId
                   };
        }

        public IEnumerable<CodeValueItem> MemberTypeCodesByFreq()
        {
            var q = from mt in Db.OrganizationMembers
                    group mt by mt.MemberTypeId
                into g
                    orderby g.Count()
                    select new { g.Key, count = g.Count() };

            var q2 = from mt in Db.MemberTypes
                     join g in q on mt.Id equals g.Key
                     orderby g.count descending
                     select new CodeValueItem
                     {
                         Id = mt.Id,
                         Code = mt.Code,
                         Value = mt.Description
                     };
            return q2;
        }

        public IEnumerable<CodeValueItem> Ministries()
        {
            return from m in Db.Ministries
                   orderby m.MinistryName
                   select new CodeValueItem
                   {
                       Id = m.MinistryId,
                       Code = m.MinistryName,
                       Value = m.MinistryName
                   };
        }

        public IEnumerable<CodeValueItem> Occupations()
        {
            return from p in Db.People
                   group p by p.OccupationOther
                into g
                   orderby g.Key
                   select new CodeValueItem
                   {
                       Value = g.Key
                   };
        }

        public IEnumerable<CodeValueItem> Organizations(int SubDivId)
        {
            return top.Union(GetOrganizationList(SubDivId));
        }

        public IEnumerable<CodeValueItem> OrganizationStatusCodes()
        {
            return from c in Db.OrganizationStatuses
                   select new CodeValueItem
                   {
                       Id = c.Id,
                       Code = c.Code,
                       Value = c.Description
                   };
        }

        public IEnumerable<CodeValueItem> OrganizationStatusCodes0()
        {
            return OrganizationStatusCodes().AddNotSpecified();
        }

        public IEnumerable<CodeValueItem> OrganizationTypes()
        {
            return from ms in Db.OrganizationTypes
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> OrganizationTypes0()
        {
            return OrganizationTypes().AddNotSpecified();
        }

        public IEnumerable<CodeValueItem> OrgDivTags()
        {
            return from t in Db.Programs
                   orderby t.Name
                   select new CodeValueItem
                   {
                       Id = t.Id,
                       Value = t.Name
                   };
        }

        public IEnumerable<CodeValueItem> OrgSubDivTags(int ProgId)
        {
            var q = from div in Db.Divisions
                    where div.ProgId == ProgId
                    orderby div.Name
                    select new CodeValueItem
                    {
                        Id = div.Id,
                        Value = div.Name
                    };
            return top.Union(q);
        }

        public IEnumerable<string> OrgSubDivTags2(int ProgId)
        {
            return from program in Db.Programs
                   from div in program.Divisions
                   where (program.Id == ProgId) || (ProgId == 0)
                   orderby program.Name, div.Name
                   select (ProgId > 0 ? program.Name + "." : "") + div.Name;
        }

        public static IEnumerable<SelectListItem> OrgTypes()
        {
            var q = from t in DbUtil.Db.OrganizationTypes
                    orderby t.Code
                    select new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Description
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "Suspended Checkin", Value = OrgType.SuspendedCheckin.ToString() });
            list.Insert(0, new SelectListItem { Text = "Main Fellowship", Value = OrgType.MainFellowship.ToString() });
            list.Insert(0,
                new SelectListItem { Text = "Not Main Fellowship", Value = OrgType.NotMainFellowship.ToString() });
            list.Insert(0, new SelectListItem { Text = "Parent Org", Value = OrgType.ParentOrg.ToString() });
            list.Insert(0, new SelectListItem { Text = "Child Org", Value = OrgType.ChildOrg.ToString() });
            list.Insert(0, new SelectListItem { Text = "Orgs Without Type", Value = OrgType.NoOrgType.ToString() });
            list.Insert(0, new SelectListItem { Text = "Orgs With Fees", Value = OrgType.Fees.ToString() });
            list.Insert(0, new SelectListItem { Text = "Orgs Without Fees", Value = OrgType.NoFees.ToString() });
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }

        public IEnumerable<CodeValueItem> Origins()
        {
            return from ms in Db.Origins
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public List<CodeValueItem> PeopleToEmailFor()
        {
            var p = Db.LoadPersonById(Util.UserPeopleId ?? 0);

            var q = from cf in Db.PeopleCanEmailFors
                    where cf.CanEmail == p.PeopleId
                    select new CodeValueItem
                    {
                        Id = cf.OnBehalfOf,
                        Code = cf.OnBehalfOfPerson.EmailAddress,
                        Value = cf.OnBehalfOfPerson.Name
                    };
            var list = q.ToList();
            list.Insert(0, new CodeValueItem
            {
                Id = p.PeopleId,
                Code = p.EmailAddress,
                Value = p.Name
            });
            return list;
        }

        public static List<CodeValueItem> PmmLabels()
        {
            var list = (from lab in DbUtil.Db.BackgroundCheckLabels
                        select new CodeValueItem
                        {
                            Id = lab.Id,
                            Code = lab.Code,
                            Value = lab.Description
                        }).ToList();
            list.Insert(0, new CodeValueItem { Id = 0, Code = "LB", Value = "(not specified)" });
            return list;
        }

        public List<string> QueryBuilderCategories()
        {
            return (from f in CategoryClass.Categories
                    select f.Title).ToList();
        }

        public IEnumerable<CodeValueItem> QueryBuilderFields(string category)
        {
            var n = 1;
            return from f in FieldClass.Fields.Values
                   where f.Category == category
                   select new CodeValueItem
                   {
                       Id = n++,
                       Value = f.Title,
                       Code = f.Name
                   };
        }

        public static IEnumerable<SelectListItem> RegistrationTypeIds()
        {
            var q = from o in RegistrationTypeCode.GetCodePairs()
                    select new SelectListItem
                    {
                        Value = o.Key.ToString(),
                        Text = o.Value
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem
            {
                Value = RegistrationClassification.AnyOnlineRegOnApp94.ToString(),
                Text = "(any reg on app)"
            });
            list.Insert(0, new SelectListItem
            {
                Value = RegistrationClassification.AnyOnlineRegNotOnApp95.ToString(),
                Text = "(active reg not on app)"
            });
            list.Insert(0, new SelectListItem
            {
                Value = RegistrationClassification.AnyOnlineRegActive96.ToString(),
                Text = "(active registration)"
            });
            list.Insert(0, new SelectListItem
            {
                Value = RegistrationClassification.AnyOnlineRegNonPicklist97.ToString(),
                Text = "(registration, no picklist)"
            });
            list.Insert(0, new SelectListItem
            {
                Value = RegistrationClassification.AnyOnlineReg99.ToString(),
                Text = "(any registration)"
            });
            list.Insert(0, new SelectListItem
            {
                Value = "-1",
                Text = "(not specified)"
            });
            list.Add(new SelectListItem
            {
                Value = RegistrationClassification.AnyOnlineRegMissionTrip98.ToString(),
                Text = "Mission Trip"
            });
            return list;
        }

        public IEnumerable<CodeValueItem> RegistrationTypes()
        {
            var q = RegistrationTypeCode.GetCodePairs();
            if (!HttpContextFactory.Current.User.IsInRole("Developer"))
                q = q.Where(pp => pp.Key != RegistrationTypeCode.RegisterLinkMaster);
            return from i in q
                   select new CodeValueItem
                   {
                       Id = i.Key,
                       Code = i.Key.ToString(),
                       Value = i.Value
                   };
        }

        public IEnumerable<CodeValueItem> RegistrationTypes99()
        {
            var list = (from i in RegistrationTypeCode.GetCodePairs()
                        select new CodeValueItem
                        {
                            Id = i.Key,
                            Code = i.Key.ToString(),
                            Value = i.Value
                        }).ToList();
            list.Insert(0, new CodeValueItem { Id = 99, Code = "99", Value = "(not specified)" });
            return list;
        }

        public static IEnumerable<CodeValueItem> ResidentCodes()
        {
            return from c in DbUtil.Db.ResidentCodes
                   select new CodeValueItem
                   {
                       Id = c.Id,
                       Code = c.Code,
                       Value = c.Description
                   };
        }

        public static List<CodeValueItem> ResidentCodesWithZero()
        {
            var list = ResidentCodes().ToList();
            list.Insert(0, top[0]);
            return list;
        }

        public IEnumerable<CodeValueItem> Schedules()
        {
            return from o in Db.Organizations
                   let sc = o.OrgSchedules.FirstOrDefault()
                   // SCHED
                   where sc != null
                   group o by new { sc.ScheduleId, sc.MeetingTime }
                into g
                   orderby g.Key.ScheduleId
                   where g.Key.ScheduleId != null
                   select new CodeValueItem
                   {
                       Id = g.Key.ScheduleId.Value,
                       Code = g.Key.ScheduleId.ToString(),
                       Value = Db.GetScheduleDesc(g.Key.MeetingTime)
                   };
        }

        public IEnumerable<CodeValueItem> Schedules0()
        {
            return Schedules().AddNotSpecified();
        }

        public IEnumerable<CodeValueItem> Schools()
        {
            return from p in Db.People
                   group p by p.SchoolOther
                into g
                   orderby g.Key
                   select new CodeValueItem
                   {
                       Value = g.Key,
                       Code = g.Key
                   };
        }

        public List<CodeValueItem> SecurityTypeCodes()
        {
            return new List<CodeValueItem>
            {
                new CodeValueItem {Id = 0, Value = "None", Code = "N"},
                new CodeValueItem {Id = 2, Value = "LeadersOnly", Code = "U"},
                new CodeValueItem {Id = 3, Value = "UnShared", Code = "U"}
            };
        }

        public static IEnumerable<CodeValueItem> StatusFlags()
        {
            var sf = from ms in DbUtil.Db.ViewStatusFlagLists.ToList()
                     where (ms.RoleName == null) || HttpContextFactory.Current.User.IsInRole(ms.RoleName)
                     select new CodeValueItem
                     {
                         Code = ms.Flag,
                         Value = ms.Name
                     };
            return sf.OrderBy(ss => ss.Value);
        }
        public static IEnumerable<CodeValueItem> QueryTags()
        {
            var sf = from t in DbUtil.Db.Tags
                     where t.TypeId == DbUtil.TagTypeId_QueryTags
                     select new CodeValueItem
                     {
                         Id = t.Id,
                         Code = t.Id.ToString(),
                         Value = t.Name
                     };
            return sf.OrderBy(ss => ss.Value);
        }

        public static IEnumerable<SelectListItem> StatusIds()
        {
            var q = from s in DbUtil.Db.OrganizationStatuses
                    select new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Description
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            return list;
        }

        public static IEnumerable<SelectListItem> Tags()
        {
            var cv = new CodeValueModel();
            var tg = ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Id").ToList();
            if (HttpContextFactory.Current.User.IsInRole("Edit"))
                tg.Insert(0, new SelectListItem { Value = "-1", Text = "(last query)" });
            tg.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            return tg;
        }

        public IEnumerable<CodeValueItem> TitleCodes()
        {
            var q = from ms in Db.People.Where(mm => mm.TitleCode.Length > 0).Select(tt => tt.TitleCode).Distinct()
                    select new CodeValueItem
                    {
                        Code = ms,
                        Value = ms
                    };
            var list = q.ToList();
            list.Insert(0, new CodeValueItem { Code = "", Value = "(not specified)" });
            return list;
        }


        public IEnumerable<CodeValueItem> UserRoles()
        {
            var q = from s in Db.Roles
                    orderby s.RoleId
                    select new CodeValueItem
                    {
                        Id = s.RoleId,
                        Code = s.RoleName,
                        Value = s.RoleName
                    };
            var list = q.ToList();
            list.Insert(0, new CodeValueItem { Value = "(not specified)", Id = 0 });
            return list;
        }

        public IEnumerable<CodeValueItem> UserRolesMyData()
        {
            var q = from s in Db.Roles
                    orderby s.RoleId
                    select new CodeValueItem
                    {
                        Id = s.RoleId,
                        Code = s.RoleName,
                        Value = s.RoleName
                    };
            var list = q.ToList();
            list.Insert(0, new CodeValueItem { Value = "(mydata, no role)", Id = 0 });
            return list;
        }

        public static List<CodeValueItem> UserTags()
        {
            var tid = DbUtil.Db.TagCurrent().Id;
            return new CodeValueModel().UserTags(Util.UserPeopleId).Where(tt => tt.Id != tid).ToList();
        }

        public List<CodeValueItem> UserTags(int? UserPeopleId)
        {
            if (UserPeopleId == Util.UserPeopleId)
                Db.TagCurrent(); // make sure the current tag exists

            var q1 = from t in Db.Tags
                     where t.PeopleId == UserPeopleId
                     where t.TypeId == DbUtil.TagTypeId_Personal
                     orderby t.Name.StartsWith(".") ? "z" : "", t.Name
                     select new CodeValueItem
                     {
                         Id = t.Id,
                         Code = $"{t.Id},{t.PeopleId}!{t.Name}",
                         Value = t.Name
                     };
            var q2 = from t in Db.Tags
                     where t.PeopleId != UserPeopleId
                     where t.TagShares.Any(ts => ts.PeopleId == UserPeopleId)
                     where t.TypeId == DbUtil.TagTypeId_Personal
                     orderby t.PersonOwner.Name2, t.Name.StartsWith(".") ? "z" : "", t.Name
                     let op = Db.People.SingleOrDefault(p => p.PeopleId == t.PeopleId)
                     select new CodeValueItem
                     {
                         Id = t.Id,
                         Code = $"{t.Id},{t.PeopleId}!{t.Name}",
                         Value = op.Name + "!" + t.Name
                     };
            var list = q1.ToList();
            list.AddRange(q2);
            return list;
        }

        public static List<CodeValueItem> UserTagsAll()
        {
            return new CodeValueModel().UserTags(Util.UserPeopleId).ToList();
        }

        public IEnumerable<CodeValueItem> UserTagsWithUnspecified()
        {
            var list = UserTags(Util.UserPeopleId).ToList();
            list.Insert(0, top[0]);
            return list;
        }

        public IEnumerable<CodeValueItem> VolApplicationStatusCodes()
        {
            var q = from sc in Db.VolApplicationStatuses
                    orderby sc.Description
                    select new CodeValueItem
                    {
                        Id = sc.Id,
                        Code = sc.Code,
                        Value = sc.Description
                    };
            return q.AddNotSpecified();
        }

        public IEnumerable<CodeValueItem> VolunteerCodes()
        {
            return from vc in Db.VolunteerCodes
                   select new CodeValueItem
                   {
                       Id = vc.Id,
                       Code = vc.Code,
                       Value = vc.Description
                   };
        }

        public static IEnumerable<string> VolunteerOpportunities()
        {
            return from c in DbUtil.Db.Contents
                   where c.Name.StartsWith("Volunteer-")
                   where c.Name.EndsWith(".view")
                   orderby c.Name
                   select c.Name.Substring(10, c.Name.Length - 15);
        }

        public static IEnumerable<CodeValueItem> YesNoAll()
        {
            return new List<CodeValueItem>
            {
                new CodeValueItem {Value = "All"},
                new CodeValueItem {Value = "Yes"},
                new CodeValueItem {Value = "No"}
            };
        }

        public class DropDownItem
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }

        public class OrgType
        {
            public const int NoFees = -8;
            public const int Fees = -7;
            public const int ChildOrg = -6;
            public const int ParentOrg = -5;
            public const int SuspendedCheckin = -4;
            public const int MainFellowship = -3;
            public const int NotMainFellowship = -2;
            public const int NoOrgType = -1;
        }

        public class RegistrationClassification
        {
            public const int NotSpecified = -1;
            public const int AnyOnlineReg99 = 99;
            public const int AnyOnlineRegMissionTrip98 = 98;
            public const int AnyOnlineRegNonPicklist97 = 97;
            public const int AnyOnlineRegActive96 = 96;
            public const int AnyOnlineRegNotOnApp95 = 95;
            public const int AnyOnlineRegOnApp94 = 94;
        }
    }
}
