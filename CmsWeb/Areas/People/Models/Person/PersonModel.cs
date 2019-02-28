using CmsData;
using CmsWeb.Areas.Manage.Models;
using CmsWeb.Code;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class PersonModel
    {
        private FamilyModel familyModel;
        private Picture familypicture;
        private AddressInfo otherAddr;
        private Picture picture;
        private AddressInfo primaryAddr;
        private IEnumerable<User> users;
        public PersonModel() { }
        public PersonModel(int id)
        {
            var flags = DbUtil.Db.Setting("StatusFlags", "F04,F01,F02,F03");
            var isvalid = Regex.IsMatch(flags, @"\A(F\d\d,{0,1})(,F\d\d,{0,1})*\z", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

            var i = (from pp in DbUtil.Db.People
                     let spouse = (from sp in pp.Family.People where sp.PeopleId == pp.SpouseId select sp.Name).SingleOrDefault()
                     where pp.PeopleId == id
                     select new
                     {
                         pp,
                         pp.Picture,
                         f = pp.Family,
                         FamilyPicture = pp.Family.Picture,
                         memberStatus = pp.MemberStatus.Description
                     }).FirstOrDefault();
            if (i == null)
            {
                return;
            }

            string statusflags;
            if (isvalid)
            {
                statusflags = string.Join(",", from s in DbUtil.Db.StatusFlagsPerson(id).ToList()
                                               where s.RoleName == null || HttpContextFactory.Current.User.IsInRole(s.RoleName)
                                               orderby s.TokenID
                                               select s.Name);
            }
            else
            {
                statusflags = "invalid setting in status flags";
            }

            Person = i.pp;
            var p = Person;
            var fam = i.f;
            HttpContextFactory.Current.Items["FamilyFromMyDataPage"] = fam; // for when Family is needed deep down in the call stack from this page

            MemberStatus = i.memberStatus;
            PeopleId = p.PeopleId;
            AddressTypeId = p.AddressTypeId;
            FamilyId = p.FamilyId;
            Name = p.Name;
            Picture = i.Picture;
            FamilyPicture = i.FamilyPicture;
            StatusFlags = (statusflags ?? "").Split(',');

            basic = new BasicPersonInfo(p.PeopleId);

            FamilyAddr = new AddressInfo
            {
                Name = "FamilyAddr",
                PeopleId = p.PeopleId,
                person = p,
                AddressLineOne = fam.AddressLineOne,
                AddressLineTwo = fam.AddressLineTwo,
                CityName = fam.CityName,
                ZipCode = fam.ZipCode,
                BadAddress = fam.BadAddressFlag ?? false,
                StateCode = new CodeInfo(fam.StateCode, "State"),
                Country = new CodeInfo(fam.CountryName, "Country"),
                ResCode = new CodeInfo(fam.ResCodeId, "ResCode"),
                FromDt = fam.AddressFromDate,
                ToDt = fam.AddressToDate,
                Preferred = p.AddressTypeId == 10
            };
            PersonalAddr = new AddressInfo
            {
                Name = "PersonalAddr",
                PeopleId = p.PeopleId,
                person = p,
                AddressLineOne = p.AddressLineOne,
                AddressLineTwo = p.AddressLineTwo,
                CityName = p.CityName,
                StateCode = new CodeInfo(p.StateCode, "State"),
                Country = new CodeInfo(p.CountryName, "Country"),
                ResCode = new CodeInfo(p.ResCodeId, "ResCode"),
                ZipCode = p.ZipCode,
                BadAddress = p.BadAddressFlag ?? false,
                FromDt = p.AddressFromDate,
                ToDt = p.AddressToDate,
                Preferred = p.AddressTypeId == 30
            };
        }

        public BasicPersonInfo basic { get; set; }
        public int PeopleId { get; set; }
        public int FamilyId { get; set; }
        public string[] StatusFlags { get; set; }
        public string Name { get; set; }
        public string MemberStatus { get; set; }

        public FamilyModel FamilyModel => familyModel ?? (familyModel = new FamilyModel(PeopleId));

        public IEnumerable<User> Users => users ?? (users = Person.Users);

        public Picture Picture
        {
            get { return picture ?? new Picture(); }
            set { picture = value; }
        }

        public Picture FamilyPicture
        {
            get { return familypicture ?? new Picture(); }
            set { familypicture = value; }
        }

        public int AddressTypeId { get; set; }
        public Person Person { get; set; }

        public AddressInfo PrimaryAddr
        {
            get
            {
                if (primaryAddr == null)
                {
                    if (FamilyAddr.Preferred)
                    {
                        primaryAddr = FamilyAddr;
                    }
                    else if (PersonalAddr.Preferred)
                    {
                        primaryAddr = PersonalAddr;
                    }
                }

                return primaryAddr;
            }
        }

        public AddressInfo OtherAddr
        {
            get
            {
                if (otherAddr == null)
                {
                    if (FamilyAddr.Preferred)
                    {
                        otherAddr = PersonalAddr;
                    }
                    else if (PersonalAddr.Preferred)
                    {
                        otherAddr = FamilyAddr;
                    }
                }

                return otherAddr;
            }
        }

        public AddressInfo FamilyAddr { get; set; }
        public AddressInfo PersonalAddr { get; set; }

        public string Email
        {
            get
            {
                if (Person.EmailAddress.HasValue())
                {
                    return $"<a href='mailto:{Person.EmailAddress}' target='_blank'>{Person.EmailAddress}</a>";
                }

                return null;
            }
        }

        public string Cell
        {
            get
            {
                if (Person.CellPhone.HasValue())
                {
                    return Person.CellPhone.FmtFone("C").Replace(" ", "&nbsp;");
                }

                return null;
            }
        }

        public string HomePhone
        {
            get
            {
                if (Person.HomePhone.HasValue())
                {
                    return Person.HomePhone.FmtFone("H").Replace(" ", "&nbsp;");
                }

                return null;
            }
        }

        public string CheckView()
        {
            if (Person == null)
            {
                return "person not found";
            }

            if (!HttpContextFactory.Current.User.IsInRole("Access"))
            {
                if (Person == null || !Person.CanUserSee)
                {
                    return "no access";
                }
            }

            if (Util2.OrgLeadersOnly)
            {
                var olotag = DbUtil.Db.OrgLeadersOnlyTag2();
                if (!DbUtil.Db.TagPeople.Any(pt => pt.PeopleId == PeopleId && pt.Id == olotag.Id))
                {
                    DbUtil.LogActivity($"Trying to view person: {Person.Name}");
                    return $"<h3 style='color:red'>{"You must be a leader of one of this person's organizations to have access to this page"}</h3>\n<a href='{"javascript: history.go(-1)"}'>{"Go Back"}</a>";
                }
            }
            return null;
        }

        private List<ResourceTypeModel> _resourceTypes;
        public List<ResourceTypeModel> ResourceTypes
        {
            get
            {
                if (_resourceTypes != null)
                {
                    return _resourceTypes;
                }

                _resourceTypes = new List<ResourceTypeModel>();

                var memberOrgs = DbUtil.Db.OrganizationMembers.Where(x => x.PeopleId == PeopleId).ToList();

                var orgLevels = memberOrgs.ToDictionary(x => x.OrganizationId, x => x.MemberType.Id);

                var orgIds = orgLevels.Keys.ToList();

                // Ensure child level orgs also show up, too
                foreach (var org in memberOrgs.Select(o => o.Organization))
                {
                    if (!org.ChildOrgs.Any())
                    {
                        continue;
                    }

                    orgIds.AddRange(org.ChildOrgs.Select(o => o.OrganizationId));
                }

                var divisionIds = DbUtil.Db.OrganizationMembers.Where(x => x.PeopleId == PeopleId)
                    .Select(x => x.Organization.DivisionId).Distinct().ToList();

                var orgTypeIds = DbUtil.Db.Organizations
                    .Where(x => orgIds.Contains(x.OrganizationId) && x.OrganizationType != null)
                    .Select(x => x.OrganizationType.Id)
                    .Distinct()
                    .ToList();

                // Filter out any resources that have a division the user isn't in
                // same for campus
                var resources =
                    DbUtil.Db.Resources
                        .Where(x => !x.DivisionId.HasValue || divisionIds.Contains(x.DivisionId))
                        .Where(x => !x.CampusId.HasValue || Person.CampusId == x.CampusId.Value);

                // Filter out resources that have an org the user isn't in
                resources = resources.Where(
                    x => !x.ResourceOrganizations.Any() ||
                         x.ResourceOrganizations.Select(ro => ro.OrganizationId).Any(o => orgIds.Contains(o)));

                // Filter out resources that have an org type the user isn't associated with (thru orgs)
                resources = resources.Where(
                    x => !x.ResourceOrganizationTypes.Any() ||
                         x.ResourceOrganizationTypes.Select(ro => ro.OrganizationTypeId).Any(ot => orgTypeIds.Contains(ot)));

                var resourceModels = new List<Resource>();

                foreach (var resource in resources)
                {
                    if (!ShouldShowResource(resource))
                    {
                        continue;
                    }

                    var noMemberTypesSet = string.IsNullOrEmpty(resource.MemberTypeIds);

                    var noOrgRestrictionsSet = !resource.ResourceOrganizations.Any();
                    var orgRestrictionsSet = !noOrgRestrictionsSet;

                    var noOrgTypeRestrictionsSet = !resource.ResourceOrganizationTypes.Any();
                    var orgTypeRestrictionsSet = !noOrgTypeRestrictionsSet;

                    if (noMemberTypesSet)
                    {
                        resourceModels.Add(resource);
                    }
                    else if (orgRestrictionsSet && orgTypeRestrictionsSet)
                    {
                        // only okay to add if the user is of member type in one of the restricted orgs and the restricted org is in the restricted org types list
                        var okayToAdd = false;
                        foreach (var ro in resource.ResourceOrganizations)
                        {
                            if (!resource.MemberTypeIds.Split(',').Contains(GetDictionaryValueOrDefault(orgLevels, ro.OrganizationId)))
                            {
                                continue;
                            }

                            if (resource.ResourceOrganizationTypes
                                .Select(rot => rot.OrganizationTypeId)
                                .Contains(ro.Organization.OrganizationTypeId.GetValueOrDefault()))
                            {
                                okayToAdd = true;
                            }
                        }
                        if (okayToAdd)
                        {
                            resourceModels.Add(resource);
                        }
                    }
                    else if (orgRestrictionsSet && noOrgTypeRestrictionsSet)
                    {
                        // only okay to add if the user is of member type in one of the restricted orgs
                        var okayToAdd = false;
                        foreach (var ro in resource.ResourceOrganizations)
                        {
                            okayToAdd = resource.MemberTypeIds.Split(',').Contains(GetDictionaryValueOrDefault(orgLevels, ro.OrganizationId));
                        }

                        if (okayToAdd)
                        {
                            resourceModels.Add(resource);
                        }
                    }
                    else if (noOrgRestrictionsSet && orgTypeRestrictionsSet)
                    {
                        // only okay to add if the user is of member type in one of the restricted org types
                        var okayToAdd = false;
                        foreach (var rot in resource.ResourceOrganizationTypes)
                        {
                            if (memberOrgs.Any(mo =>
                                    mo.Organization.OrganizationTypeId == rot.OrganizationTypeId &&
                                    resource.MemberTypeIds.Split(',').Contains(GetDictionaryValueOrDefault(orgLevels, mo.OrganizationId))
                            ))
                            {
                                okayToAdd = true;
                            }
                        }
                        if (okayToAdd)
                        {
                            resourceModels.Add(resource);
                        }
                    }
                    else if (noOrgRestrictionsSet && noOrgTypeRestrictionsSet)
                    {
                        var restrictedMemberTypes = resource.MemberTypeIds.Split(',').ToList();
                        var assignedMemberTypes = orgLevels.Values.Select(x => x.ToString());

                        // only okay to add if the user has any of the restricted member types
                        if (restrictedMemberTypes.Intersect(assignedMemberTypes).Any())
                        {
                            resourceModels.Add(resource);
                        }
                    }
                }

                // after filtering to the individual resources, make the list by type for easy rendering
                _resourceTypes = resourceModels
                    .GroupBy(x => x.ResourceTypeId)
                    .Select(x => new ResourceTypeModel(x.First().ResourceType, x.OrderBy(y => y.DisplayOrder)))
                    .OrderBy(x => x.ResourceType.DisplayOrder)
                    .ToList();


                return _resourceTypes;
            }
        }

        private string GetDictionaryValueOrDefault(Dictionary<int, int> dictionary, int key, int defaultValue = 0)
        {
            int value = defaultValue;

            dictionary.TryGetValue(key, out value);

            return value.ToString();
        }

        private bool ShouldShowResource(Resource resource)
        {
            var resourceStatusFlags = resource.StatusFlagIds?.Split(',') ?? new string[] { };
            var personStatusFlags = DbUtil.Db.ViewAllStatusFlags.Where(sf => sf.PeopleId == PeopleId).Select(x => x.Flag).ToList();

            var shouldShow = true;
            foreach (var flag in resourceStatusFlags.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                if (!personStatusFlags.Contains(flag))
                {
                    shouldShow = false;
                }
            }

            return shouldShow;
        }
    }
}
