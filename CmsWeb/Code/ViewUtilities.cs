using CmsData;

namespace CmsWeb.Code
{
    public class ViewUtilities
    {
        /// <summary>
        /// Church has enabled the override and the current person is not currently the head of household assigned to a house.
        /// </summary>
        /// <remarks>This allows a user to circumvent the head of household calculation completely, maybe some rules about who can serve in the role could be added</remarks>
        /// <param name="peopleId">The id of the person you wish to check to see if they can serve as head of household for their family</param>
        /// <returns>boolean: false if not enabled or the person is the current head of household, otherwise true</returns>
        public static bool CanBePromotedToHeadOfHousehold(int peopleId)
        {
            var churchHasEnabledFeature = DbUtil.Db.GetSetting("CanOverrideHeadOfHousehold", "false") == "true";

            if (!churchHasEnabledFeature)
            {
                return false;
            }

            var family = DbUtil.Db.LoadFamilyByPersonId(peopleId);

            return family.HeadOfHouseholdId != peopleId;
        }
    }
}
