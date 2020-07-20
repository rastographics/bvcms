using CmsData;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Areas.Org.Models.Venues;
using SeatsioDotNet;
using SeatsioDotNet.Events;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public partial class CodeValueModel
    {
        public IEnumerable<CodeValueItem> EnvelopeOptionList()
        {
            return from ms in Db.EnvelopeOptions
                   orderby ms.Description
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> ContributionOptionsList()
        {
            return EnvelopeOptionList();
        }

        public IEnumerable<CodeValueItem> EnvelopeOptionsList()
        {
            return EnvelopeOptionList();
        }

        public IEnumerable<CodeValueItem> JoinTypeList()
        {
            return from ms in Db.JoinTypes
                   orderby ms.Description
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> DropTypeList()
        {
            return from ms in Db.DropTypes
                   orderby ms.Description
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> NewMemberClassStatusList()
        {
            return from c in Db.NewMemberClassStatuses
                   select new CodeValueItem
                   {
                       Id = c.Id,
                       Code = c.Code,
                       Value = c.Description
                   };
        }

        public IEnumerable<CodeValueItem> BaptismTypeList()
        {
            return from ms in Db.BaptismTypes
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> BaptismStatusList()
        {
            return from ms in Db.BaptismStatuses
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> DecisionTypeList()
        {
            return from ms in Db.DecisionTypes
                   select new CodeValueItem
                   {
                       Id = ms.Id,
                       Code = ms.Code,
                       Value = ms.Description
                   };
        }

        public IEnumerable<CodeValueItem> CampusNoNoCampusList()
        {
            return AllCampuses();
        }
        public IEnumerable<CodeValueItem> CampusList()
        {
            return AllCampusesNo();
        }

        public IEnumerable<CodeValueItem> FundList()
        {
            return FundsScopedByRoleMembershipWithUnspecified();
        }

        public IEnumerable<CodeValueItem> Campus0List()
        {
            return AllCampuses0();
        }

        public IEnumerable<CodeValueItem> ContactReasonList()
        {
            return ContactReasonCodes().AddNotSpecified();
        }

        public IEnumerable<CodeValueItem> ContactTypeList()
        {
            return ContactTypeCodes().AddNotSpecified();
        }

        public IEnumerable<CodeValueItem> CountryList()
        {
            return GetCountryList();
        }

        public IEnumerable<CodeValueItem> EntryPointList()
        {
            return EntryPoints();
        }

        public IEnumerable<CodeValueItem> OriginList()
        {
            return Origins();
        }

        public IEnumerable<CodeValueItem> GenderList()
        {
            return GenderCodesWithUnspecified();
        }

        public IEnumerable<CodeValueItem> MaritalStatusList()
        {
            return MaritalStatusCodes();
        }

        public IEnumerable<CodeValueItem> MemberTypeList()
        {
            return MemberTypeCodes0();
        }

        public IEnumerable<CodeValueItem> MinistryList()
        {
            return Ministries().AddNotSpecified();
        }

        public IEnumerable<CodeValueItem> PositionInFamilyList()
        {
            return FamilyPositionCodes();
        }

        public IEnumerable<CodeValueItem> ResCodeList()
        {
            return ResidentCodesWithZero();
        }

        public IEnumerable<CodeValueItem> StateList()
        {
            return GetStateList();
        }

        public IEnumerable<CodeValueItem> LetterStatusList()
        {
            return LetterStatusCodes();
        }

        public IEnumerable<CodeValueItem> OrganizationStatusList()
        {
            return OrganizationStatusCodes();
        }

        public IEnumerable<CodeValueItem> SecurityTypeList()
        {
            return SecurityTypeCodes();
        }

        public IEnumerable<CodeValueItem> OrganizationTypeList()
        {
            return OrganizationTypes0();
        }

        public IEnumerable<CodeValueItem> LeaderMemberTypeList()
        {
            return MemberTypeCodes0().Select(c => new CodeValueItem { Code = c.Code, Id = c.Id, Value = c.Value });
        }

        public IEnumerable<CodeValueItem> AttendCreditList()
        {
            return AttendCredits();
        }

        public IEnumerable<CodeValueItem> RegistrationTypeList()
        {
            return RegistrationTypes();
        }

        public SelectList SchedDayList()
        {
            return DaysOfWeek();
        }

        public SelectList PublishDirectoryList()
        {
            return new SelectList(new[]
            {
                new {Value = "0", Text = "No Directory"},
                new {Value = "1", Text = "Yes Publish Directory"},
                new {Value = "2", Text = "Yes, Publish Family Directory"}
            }, "Value", "Text");
        }

        public SelectList MinistrySelectList()
        {
            return MinistryList().ToSelect();
        }

        public SelectList ContactReasonSelectList()
        {
            return ContactReasonCodes().ToSelect();
        }

        public SelectList ContactTypeSelectList()
        {
            return ContactTypeCodes().ToSelect();
        }

        public SelectList ExtraValueTypeList()
        {
            return ExtraValueTypeCodes().ToSelect("Code");
        }

        public SelectList AdhocExtraValueTypeList()
        {
            return AdhocExtraValueTypeCodes().ToSelect("Code");
        }

        public IEnumerable<CodeValueItem> ContactResultList()
        {
            return new List<CodeValueItem>
            {
                new CodeValueItem {Value = "(not selected)"},
                new CodeValueItem {Value = "Attempted/Not Available"},
                new CodeValueItem {Value = "Left Note Card"},
                new CodeValueItem {Value = "Left Message"},
                new CodeValueItem {Value = "Contact Made"},
                new CodeValueItem {Value = "Gospel Shared"},
                new CodeValueItem {Value = "Prayer Request Received"},
                new CodeValueItem {Value = "Gift Bag Given"}
            };
        }

        public SelectList TagList()
        {
            var tg = UserTags(Db.UserPeopleId).ToList();
            if (HttpContextFactory.Current.User.IsInRole("Edit"))
            {
                tg.Insert(0, new CodeValueItem { Id = -1, Value = "(last query)" });
            }

            tg.Insert(0, new CodeValueItem { Id = 0, Value = "(not specified)" });
            return tg.ToSelect();
        }

        public SelectList PositionInFamilySelectList()
        {
            return PositionInFamilyList().ToSelect();
        }

        public IEnumerable<CodeValueItem> MemberStatusList()
        {
            return MemberStatusCodes();
        }
        public SelectList TaskSearchStatusList()
        {
            var list = (from vc in Db.TaskStatuses
                        orderby vc.Description
                        select new CodeValueItem
                        {
                            Id = vc.Id,
                            Code = vc.Code,
                            Value = vc.Description
                        }).ToList();
            list.Insert(0, new CodeValueItem { Id = 99, Value = "Active + Pending" });
            return list.AddNotSpecified().ToSelect();
        }
        public SelectList TaskStatusList()
        {
            var list = Db.TaskStatuses.ToList();
            return new SelectList(list, "Id", "Description");
        }
        public SelectList TaskLimitToRoleList()
        {
            var roles = Db.Setting("LimitToRolesForTasks",
                    Db.Setting("LimitToRolesForContacts", ""))
                .SplitStr(",").Where(rr => rr.HasValue()).ToArray();

            if (roles.Length == 0)
            {
                roles = Db.Roles.OrderBy(r => r.RoleName).Select(r => r.RoleName).ToArray();
            }

            var list = roles.Select(rolename => new
            {
                Value = rolename,
                Text = rolename,
                //Selected = !string.IsNullOrWhiteSpace(TaskLimitToRole.Value) && TaskLimitToRole.Value == rolename
            }).ToList();

            list.Insert(0, new { Value = "0", Text = @"(not specified)" });
            return new SelectList(list, "Value", "Text");
        }
        public SelectList VenueList()
        {
            var BaseUrl = "https://api.seatsio.net";
            var secretKey = Db.Setting("TicketingWorkspaceSecretKey", "");
            var workspaceKey = Db.Setting("TicketingWorkspaceKey", "");
            var client = new SeatsioClient(secretKey, workspaceKey, BaseUrl);
            var chartsFromApi = client.Charts.ListAll(expandEvents: true).ToList();

            var list = (from vc in chartsFromApi
                        orderby vc.Name
                        select new CodeValueItem()
                        {
                            Code = vc.Key,
                            Value = vc.Name
                        }).ToList();
            list.Insert(0, new CodeValueItem { Code = "0", Value = "(not specified)" });
            return new SelectList(list, "Code", "Value");
        }
    }
}
