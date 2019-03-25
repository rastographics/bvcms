using CmsData;
using ImageData;
using System.Linq;
using DbUtil = ImageData.DbUtil;

namespace CmsWeb.Models
{
    public class ExcelPic
    {
        public int PeopleId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string BirthDate => Person.FormatBirthday(BYear, BMon, BDay, PeopleId);
        public int? BMon;
        public int? BYear;
        public int? BDay;
        public string BirthDay { get; set; }
        public string Anniversary { get; set; }
        public string JoinDate { get; set; }
        public string JoinType { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string WorkPhone { get; set; }
        public string MemberStatus { get; set; }
        public string FellowshipLeader { get; set; }
        public string Spouse { get; set; }
        public string Children { get; set; }
        public string Age { get; set; }
        public string School { get; set; }
        public string Grade { get; set; }
        public decimal AttendPctBF { get; set; }
        public string Married { get; set; }
        public int FamilyId { get; set; }
        public int? ImageId { get; set; }

        public Image GetImage()
        {
            Image i = null;
            try
            {
                i = DbUtil.Db.Images.SingleOrDefault(ii => ii.Id == ImageId);
            }
            catch
            {
            }
            return i;
        }

        public string ImageUrl()
        {
            return ImageId.HasValue
                ? CmsData.DbUtil.Db.ServerLink($"/Portrait/{ImageId}/160/200")
                : "";
        }
    }
}
