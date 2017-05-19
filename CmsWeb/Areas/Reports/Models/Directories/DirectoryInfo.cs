using CmsData;

namespace CmsWeb.Areas.Reports.Models
{
    public class DirectoryInfo
    {
//        private Person p;
//        public int PeopleId { get; set; }
//        public Person Person => p ?? (p = DbUtil.Db.LoadPersonById(PeopleId));
        public Person Person { get; set; }
        public string SpouseFirst { get; set; }
        public string SpouseLast { get; set; }
        public string SpouseName => SpouseLast == Person.LastName ? SpouseFirst : $"{SpouseFirst} {SpouseLast}";
        public string Children { get; set; }
        public int? ImageId { get; set; }
    }
}