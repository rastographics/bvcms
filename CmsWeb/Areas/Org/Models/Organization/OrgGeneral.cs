using System.ComponentModel.DataAnnotations;
using CmsData;
using CmsWeb.Code;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgGeneral
    {
        public Organization Org;
        public int Id 
        {
            get { return Org != null ? Org.OrganizationId : 0; }
            set
            {
                if (Org == null)
                    Org = DbUtil.Db.LoadOrganizationById(value);
            }
        }
        public void Update()
        {
            this.CopyPropertiesTo(Org);
            DbUtil.Db.SubmitChanges();
        }

        public OrgGeneral(int id)
        {
            Id = id;
            this.CopyPropertiesFrom(Org);
        }

        public OrgGeneral()
        {
            
        }


        [Display(Description = @"
Typically something short like a room number, not enough room for an address.
")]
        public string Location { get; set; }

        [Display(Name = "Pending Location", Description = @"
Used for promotion, setup classes that will be moved see wiki re annual class promotion.
")]
        public string PendingLoc { get; set; }

        [Display(Description = @"
A longer text description of the organization, typically displayed in a flyer or on a website.
")]
        public string Description { get; set; }

        [Display(Description = @"
When a new person visits a meeting for this organization, they will be coded with this to indicate their first entrance to the church.
")]
        public CodeInfo EntryPoint { get; set; }

        [Display(Description = @"
This makes available an accessible directory for any member of this organization.
")]
        public CodeInfo PublishDirectory { get; set; }

        [Display(Name = "Grade", Description = @"
Used during promotion to assign a grade to a student who joins this class.

Must be a integer number, not a range, 
**Do Not Use** something like 7-10.
Only effective for graded classes.

This facility keeps you from having to maintain grades once a year.
")]
        public int? GradeAgeStart { get; set; }

        [Display(Description = @"
For the 'Compute Org by Birthday/Gender' Registration Type.

Commonly used for recreation sports leagues 
where each league is a Division (in TouchPoint vernacular) 
and each division within the league is an Organization.
")]
        public CodeInfo Gender { get; set; }

        [Display(Description = @"
Used to display a number to call in a confirmation email with {phone}.

If you doing a registration with more than one org, 
allows you to specify a different number for each org 
and have only one confirmation #.
")]
        public string PhoneNumber { get; set; }

        [Display(Description = @"
User to designate an orginazation as a recreation team.

This enables special handling for team handling and creation.
")]
        public bool IsRecreationTeam { get; set; }

        [Display(Description = @"
Allows others to donate on behalf of a mission trip participant.
")]
        public bool IsMissionTrip { get; set; }

        [Display(Description = @"
Disallow Credit Cards on this org.
")]
        public bool NoCreditCards { get; set; }

        [Display(Name = "Security Role", Description = @"
This organization will be visible to only users in this role.

**Warning**, if no one has this role, 
you will never be able to get back here 
unless you create the role and assign it to somebody.
")]
        public string LimitToRole { get; set; }

    }
}