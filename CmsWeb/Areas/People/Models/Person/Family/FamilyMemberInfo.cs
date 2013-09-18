using CmsWeb.Code;

namespace CmsWeb.Areas.People.Models.Person
{
    public class FamilyMemberInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public string Color { get; set; }
        public string PositionInFamily { get; set; }
        public string SpouseIndicator { get; set; }
        public string Email { get; set; }
        public string MemberStatus { get; set; }
        public CmsData.Picture Pictures { get; set; }
    }
}
