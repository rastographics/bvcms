using System;
using CmsData;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegModel
    {
        public class FamilyMember
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public int? Age { get; set; }
        }
        public IEnumerable<FamilyMember> FamilyMembers()
        {
            var family = (from p in user.Family.People
                          where p.DeceasedDate == null
                          select new { p.PeopleId, p.Name2, p.Age, p.Name }).ToList();
            var q = from m in family
                    where _list.All(vv => vv.PeopleId != m.PeopleId)
                    orderby m.PeopleId == user.Family.HeadOfHouseholdId ? 1 :
                            m.PeopleId == user.Family.HeadOfHouseholdSpouseId ? 2 :
                            3, m.Age descending, m.Name2
                    select new FamilyMember
                    {
                        PeopleId = m.PeopleId,
                        Name = m.Name,
                        Age = m.Age
                    };
            return q;
        }

        public void StartRegistrationForFamilyMember(int id, ModelStateDictionary modelState)
        {
            modelState.Clear(); // ensure we pull form fields from our model, not MVC's
            HistoryAdd("Register");
            int index = List.Count - 1;
            var p = LoadExistingPerson(id, index);
            if (p.NeedsToChooseClass())
            {
                modelState.AddModelError("fammember-" + p.PeopleId, "Please make selection above");
                return;
            }
            if (p.ComputesOrganizationByAge())
            {
                if (p.org == null)
                {
                    modelState.AddModelError("fammember-" + p.PeopleId, "No Appropriate Org");
                    return;
                }
            }
            List[index] = p;

            p.ValidateModelForFind(modelState, id, selectFromFamily: true);
            if (!modelState.IsValid)
            {
                return;
            }

            if (p.ManageSubscriptions() && p.Found == true)
            {
                return;
            }

            if (p.org != null && p.Found == true)
            {
                if (!SupportMissionTrip)
                {
                    p.IsFilled = p.org.RegLimitCount(DbUtil.Db) >= p.org.Limit;
                }

                if (p.IsFilled)
                {
                    modelState.AddModelError(this.GetNameFor(mm => mm.List[List.IndexOf(p)].Found),
                        "Sorry, but registration is filled.");
                }

                if (p.IsCommunityGroup() && DbUtil.Db.Setting("RestrictCGSignupsTo24Hrs"))
                {
                    if (!p.CanRegisterInCommunityGroup(DateTime.Today.AddDays(-1)))
                    {
                        var message = DbUtil.Db.Setting("RestrictCGSignupsTo24HrsMessage", "Sorry, but you cannot register for multiple community groups on the same day.");
                        modelState.AddModelError(this.GetNameFor(mm => mm.List[List.IndexOf(p)].Found), message);
                    }
                        
                }

                if (p.Found == true)
                {
                    p.FillPriorInfo();
                }

                return;
            }
            if (p.org == null && p.ComputesOrganizationByAge())
            {
                modelState.AddModelError(this.GetNameFor(mm => mm.List[id].Found), p.NoAppropriateOrgError);
            }
        }
    }
}
