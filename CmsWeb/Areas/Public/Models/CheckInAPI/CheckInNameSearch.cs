using UtilityExtensions;

namespace CmsWeb.CheckInAPI
{
    public class CheckInNameSearch
    {
        public string name = "";

        public string first = "";
        public string last = "";

        public void splitName()
        {
            Util.NameSplit(name, out first, out last);
        }
    }
}