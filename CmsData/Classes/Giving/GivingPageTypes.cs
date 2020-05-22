using System.Collections.Generic;

namespace CmsData.Classes.Giving
{
    public class GivingPageTypes
    {
        public static List<PageType> GetGivingPageTypes()
        {
            var pageTypesList = new List<PageType>();
            PageType pledge = new PageType
            {
                Id = 1,
                Name = "Pledge"
            };
            PageType oneTime = new PageType
            {
                Id = 2,
                Name = "One Time"
            };
            PageType recurring = new PageType
            {
                Id = 4,
                Name = "Recurring"
            };
            pageTypesList.Add(pledge);
            pageTypesList.Add(oneTime);
            pageTypesList.Add(recurring);
            return pageTypesList;
        }
    }
    public class PageType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
