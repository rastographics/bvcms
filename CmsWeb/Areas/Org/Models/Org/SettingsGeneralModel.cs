using CmsData;
using CmsWeb.Code;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Org.Models
{
    public class SettingsGeneralModel
    {
        public Organization Org;

        public int Id
        {
            get { return Org?.OrganizationId ?? 0; }
            set
            {
                if (Org == null)
                {
                    Org = DbUtil.Db.LoadOrganizationById(value);
                }
            }
        }
        public void Update(bool userIsAdmin)
        {
            if (LimitToRole == null)
            {
                LimitToRole = new CodeInfo("0", new SelectList(Roles(), "Value", "Text"));
            }
            if (LimitToRole.Value == "0")
            {
                LimitToRole.Value = null;
            }
            if (Gender.Value == "99")
            {
                Gender.Value = null;
            }
            string exclusions = null;
            if (!userIsAdmin)
            {
                exclusions = "LimitToRole";
            }
            this.CopyPropertiesTo(Org, excludefields: exclusions);
            DbUtil.Db.SubmitChanges();
        }

        public SettingsGeneralModel(int id)
            : this()
        {
            Id = id;
            this.CopyPropertiesFrom(Org);
        }

        public SettingsGeneralModel()
        {
            LimitToRole = new CodeInfo("0", new SelectList(Roles(), "Value", "Text"));
        }
        public IEnumerable<SelectListItem> Roles()
        {
            var s = LimitToRole?.Value;
            var list = DbUtil.Db.Roles.OrderBy(r => r.RoleName).ToList().Select(x => new SelectListItem
            {
                Value = x.RoleName,
                Text = x.RoleName,
                Selected = !string.IsNullOrWhiteSpace(s) && LimitToRole.Value.Contains(x.RoleName)
            }).ToList();

            var seldefault = !list.Any(vv => vv.Selected);
            list.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)", Selected = seldefault });
            return list;
        }

        [Display(Description = LocationDescription)]
        public string Location { get; set; }

        [Display(Name = "Pending Location", Description = PendingLocDescription)]
        public string PendingLoc { get; set; }

        [Display(Description = DescriptionDescription)]
        public string Description { get; set; }

        [Display(Description = EntryPointDescription)]
        public CodeInfo EntryPoint { get; set; }

        [Display(Description = PublishDirectoryDescription)]
        public CodeInfo PublishDirectory { get; set; }

        [Display(Name = "Grade", Description = GradeAgeStartDescription)]
        public int? GradeAgeStart { get; set; }

        [Display(Description = GenderDescription)]
        public CodeInfo Gender { get; set; }

        [Display(Description = PhoneNumberDescription)]
        public string PhoneNumber { get; set; }

        [Display(Description = IsRecreationTeamDescription)]
        public bool IsRecreationTeam { get; set; }

        [Display(Description = IsMissionTripDescription)]
        public bool IsMissionTrip { get; set; }

        [Display(Name = "Enable Funding Pages", Description = TripFundingPagesEnableDescription)]
        public bool TripFundingPagesEnable { get; set; }

        [Display(Name = "Enable Public Funding Pages", Description = TripFundingPagesPublicDescription)]
        public bool TripFundingPagesPublic { get; set; }

        [Display(Name = "Show Public Funding Amounts", Description = TripFundingPagesShowAmountsDescription)]
        public bool TripFundingPagesShowAmounts { get; set; }

        [Display(Description = NoCreditCardsDescription)]
        public bool NoCreditCards { get; set; }

        [Display(Name = "Limit Org to Role", Description = LimitToRoleDescription)]
        public CodeInfo LimitToRole { get; set; }

        #region Description

        private const string LocationDescription = @"
Typically something short like a room number, not enough room for an address.
";
        private const string PendingLocDescription = @"
Used for promotion, setup classes that will be moved see wiki re annual class promotion.
";
        private const string DescriptionDescription = @"
A longer text description of the organization, typically displayed in a flyer or on a website.
";
        private const string EntryPointDescription = @"
When a new person visits a meeting for this organization, they will be coded with this to indicate their first entrance to the church.
";
        private const string PublishDirectoryDescription = @"
This makes available an accessible directory for any member of this organization.
";
        private const string GradeAgeStartDescription = @"
Used during promotion to assign a grade to a student who joins this class.
Keeps you from having to maintain grades once a year.

**This field only works when the Org is a Main Fellowship type of organization.**
See *<a href=""https://docs.touchpointsoftware.com/Organizations/GeneralSettings.html"" target=""_blank"">this help article</a>*.

Must be an integer number, not a range,
**Do Not Use** something like 7-10.
Only effective for graded classes.
";
        private const string GenderDescription = @"
For the 'Compute Org by Birthday/Gender' Registration Type.

Commonly used for recreation sports leagues
where each league is a Division (in TouchPoint vernacular)
and each division within the league is an Organization.
";
        private const string PhoneNumberDescription = @"
Used to display a number to call in a confirmation email with {phone}.

If you doing a registration with more than one org,
allows you to specify a different number for each org
and have only one confirmation #.
";
        private const string IsRecreationTeamDescription = @"
Used to designate an organization as a recreation team.

This enables special handling for team handling and creation.
";
        private const string IsMissionTripDescription = @"
Allows others to donate on behalf of a mission trip participant.
";
        private const string TripFundingPagesEnableDescription = @"
Allows participants to view how much they still owe, a list of people who donated and their gifts on a single page.
";
        private const string TripFundingPagesPublicDescription = @"
Used for fundraising. Each participants funding page will be public for anyone who has the link. This also changes the donation emails to include the funding page link.
";
        private const string TripFundingPagesShowAmountsDescription = @"
Allows others to view how much each giver donated on the funding page for a participant.
";
        private const string NoCreditCardsDescription = @"
Disallow Credit Cards on this org.
";
        private const string LimitToRoleDescription = @"
This organization will be visible to only users in this role.

**Warning**, if no one has this role,
you will never be able to get back here
unless you create the role and assign it to somebody.
";

        #endregion
    }
}
