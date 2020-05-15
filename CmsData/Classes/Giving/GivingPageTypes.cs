using System.Collections.Generic;

namespace CmsData.Classes.Giving
{
    public class GivingPageTypes
    {
        public static List<PageTypesClass> GetGivingPageTypes()
        {
            var pageTypesList = new List<PageTypesClass>();
            PageTypesClass pledge = new PageTypesClass
            {
                id = 1,
                pageTypeName = "Pledge"
            };
            PageTypesClass oneTime = new PageTypesClass
            {
                id = 2,
                pageTypeName = "One Time"
            };
            PageTypesClass recurring = new PageTypesClass
            {
                id = 4,
                pageTypeName = "Recurring"
            };
            pageTypesList.Add(pledge);
            pageTypesList.Add(oneTime);
            pageTypesList.Add(recurring);
            return pageTypesList;
        }
    }
    public class PageTypesClass
    {
        public int id { get; set; }
        public string pageTypeName { get; set; }
    }
}
