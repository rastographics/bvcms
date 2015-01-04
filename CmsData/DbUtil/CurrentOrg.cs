using System;
using UtilityExtensions;

namespace CmsData
{
    public interface ICurrentOrg
    {
        int? Id { get; set; }
        string GroupSelect { get; set; }
        string NameFilter { get; set; }
        string SgFilter { get; set; }
        bool ShowHidden { get; set; }
        bool FilterIndividuals { get; set; }
        bool FilterTag { get; set; }
    }
    [Serializable]
    public class CurrentOrg : ICurrentOrg
    {
        public int? Id { get; set; }
        public string GroupSelect { get; set; }
        public string NameFilter { get; set; }
        public string SgFilter { get; set; }
        public bool ShowHidden { get; set; }
        public bool FilterIndividuals { get; set; }
        public bool FilterTag { get; set; }
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
        }
    }
}