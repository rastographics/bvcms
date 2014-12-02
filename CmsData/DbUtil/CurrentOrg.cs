using UtilityExtensions;

namespace CmsData
{
    public interface ICurrentOrg
    {
        int? Id { get; set; }
        string NameFilter { get; set; }
        string SgFilter { get; set; }
        bool ShowHidden { get; set; }
        bool ClearFilter { get; set; }
    }
    public class CurrentOrg : ICurrentOrg
    {
        public int? Id { get; set; }
        public string NameFilter { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string SgFilter { get; set; }
        public bool ShowHidden { get; set; }
        public bool ClearFilter { get; set; }

        public bool IsFiltered
        {
            get { return SgFilter.HasValue() || NameFilter.HasValue(); }
        }
    }

    public static class CurrentOrgExtensions
    {
        public static string First(this ICurrentOrg c)
        {
            if (!c.NameFilter.HasValue())
                return null;
            string first, last;
            Util.NameSplit(c.NameFilter, out first, out last);
            return first;
        }
        public static string Last(this ICurrentOrg c)
        {
            if (!c.NameFilter.HasValue())
                return null;
            string first, last;
            Util.NameSplit(c.NameFilter, out first, out last);
            return last;
        }
        public static void ClearCurrentOrg(this ICurrentOrg c)
        {
            c.SgFilter = null;
            c.NameFilter = null;
            c.ClearFilter = false;
        }
    }
}