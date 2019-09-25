using CmsData;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class SettingsCheckInModel
    {
        private CMSDataContext _currentDatabase;

        public CMSDataContext CurrentDatabase
        {
            get => _currentDatabase;
            set
            {
                _currentDatabase = value;
                _org = null;
            }
        }

        private Organization _org;
        public Organization Org
        {
            get
            {
                if (_org == null && Id > 0 && CurrentDatabase != null)
                {
                    _org = CurrentDatabase.LoadOrganizationById(Id);
                }
                return _org;
            }
        }

        public int Id { get; set; }

        public SettingsCheckInModel()
        {
        }

        public SettingsCheckInModel(int id, CMSDataContext dataContext)
        {
            _currentDatabase = dataContext;
            Id = id;
            this.CopyPropertiesFrom(Org);
        }

        public void Update()
        {
            this.CopyPropertiesTo(Org);
            CurrentDatabase.SubmitChanges();
        }

        [Display(Name = "Allow Self Check-In",
            Description = @"
Causes this meeting to show up on the Touchscreen Checkin.
")]
        public bool CanSelfCheckin { get; set; }

        [Display(Description = @"
Causes this meeting to show up only when the magic button is pressed
")]
        public bool SuspendCheckin { get; set; }

        [Display(Description = @"
If you are using self-checkin and you have multiple campuses, 
and you have an Organization (that is assigned a campus) 
and you want that organization to display as available for checkin to anyone, 
regardless of the campus where they are checking in, 
select this box.

Also, if you have multiple campuses in your database 
and you do not assign a campus to Organizations used for self-checkin, 
they will display without this box being checked.
")]
        public bool AllowNonCampusCheckIn { get; set; }
        
        [Display(Name = "No security label required",
            Description = @"
Used for when children are old enough 
to not need a security label to be picked up.
")]
        public bool NoSecurityLabel { get; set; }

        [Display(Name = "Number of CheckIn Labels",
            Description = @"
Default is 1, use 0 if no labels needed.
")]
        public int? NumCheckInLabels { get; set; }

        [Display(Name = "Number of Worker CheckIn Labels",
            Description = @"
Allows workers to get 1 or 0 labels when checking in.
")]
        public int? NumWorkerCheckInLabels { get; set; }

        [Display(Name = "Start Birthday",
            Description = @"
Used on Touchscreen Checkin for when a guest needs to choose a class.
Also used to prevent someone joining an organization during registration if they are outside the birthday range.
")]
        public DateTime? BirthDayStart { get; set; }

        [Display(Name = "End Birthday",
            Description = @"
Used on Touchscreen Checkin for when a guest needs to choose a class.
Also used to prevent someone joining an organization during registration if they are outside the birthday range.
")]
        public DateTime? BirthDayEnd { get; set; }
    }
}
