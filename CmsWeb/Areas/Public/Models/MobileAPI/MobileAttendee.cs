using CmsData;
using CmsWeb.Areas.Reports.Models;
using System;
using System.Linq;

namespace CmsWeb.MobileAPI
{
    public class MobileAttendee
    {
        public int id = 0;

        public string name = "";
        public string memberType = "";

        public bool orgMember = false;
        public bool member = false;
        public bool attended = false;

        public string picture = "";
        public int pictureX = 0;
        public int pictureY = 0;

        public MobileAttendee populate(RollsheetModel.AttendInfo p)
        {
            id = p.PeopleId;
            name = p.Name;
            memberType = p.CurrMemberType;
            orgMember = p.CurrMember;
            member = p.Member;
            attended = p.Attended;

            Person person = DbUtil.Db.People.SingleOrDefault(e => e.PeopleId == id);

            if (person.Picture != null)
            {
                var image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.SmallId);

                if (image != null)
                {
                    picture = Convert.ToBase64String(image.Bits);
                    pictureX = person.Picture.X ?? 0;
                    pictureY = person.Picture.Y ?? 0;
                }
            }

            return this;
        }
    }
}
