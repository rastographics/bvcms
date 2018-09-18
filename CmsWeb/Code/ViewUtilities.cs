using CmsData;

namespace CmsWeb.Code
{
    public class ViewUtilities
    {
        public static bool CanOverrideHeadOfHousehold(int peopleId)
        {
            var overrideEnabled = DbUtil.Db.GetSetting("CanOverrideHeadOfHousehold", "false") == "true";

            if (!overrideEnabled)
            {
                return overrideEnabled;
            }

            var family = DbUtil.Db.LoadFamilyByPersonId(peopleId);

            return peopleId != family.HeadOfHouseholdId;
        }
    }
}
