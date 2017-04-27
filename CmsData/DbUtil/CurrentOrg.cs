using System;
using System.Runtime.Serialization;
using UtilityExtensions;

namespace CmsData
{
    public interface IFilterOrg
    {
        Guid QueryId { get; set; }
        int? Id { get; set; }
        string GroupSelect { get; set; }
        string NameFilter { get; set; }
        string SgFilter { get; set; }
        bool ShowHidden { get; set; }
        bool FilterIndividuals { get; set; }
        bool FilterTag { get; set; }
        string TagName { get; set; }
        int TagOwnerId { get; set; }
    }
    [Serializable]
    public class CurrentOrg : IFilterOrg
    {
        public Guid QueryId { get; set; }
        public int? Id { get; set; }
        public string GroupSelect { get; set; }
        public string NameFilter { get; set; }
        public string SgFilter { get; set; }
        public bool ShowHidden { get; set; }
        public bool FilterIndividuals { get; set; }
        public bool FilterTag { get; set; }
        public string TagName { get; set; }
        public int TagOwnerId { get; set; }
    }

//    public static class CurrentOrgExtensions
//    {
//        public static string First(this IFilterOrg c)
//        {
//            if (!c.NameFilter.HasValue())
//                return null;
//            string first, last;
//            Util.NameSplit(c.NameFilter, out first, out last);
//            return first;
//        }
//        public static string Last(this IFilterOrg c)
//        {
//            if (!c.NameFilter.HasValue())
//                return null;
//            string first, last;
//            Util.NameSplit(c.NameFilter, out first, out last);
//            return last;
//        }
//        public static void ClearCurrentOrg(this IFilterOrg c)
//        {
//            c.SgFilter = null;
//            c.NameFilter = null;
//        }
//    }
}