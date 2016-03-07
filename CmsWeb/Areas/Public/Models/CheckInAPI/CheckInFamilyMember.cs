using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.CheckInAPI
{
    public class CheckInFamilyMember
    {
        public int id = 0;
        public int age = 0;
        public int position = 0;
        public int genderID = 0;

        public string name = "";
        public string picture = "";

        public int pictureX = 0;
        public int pictureY = 0;

        public List<CheckInOrganization> orgs = new List<CheckInOrganization>();

        public CheckInFamilyMember(CmsData.View.CheckinFamilyMember member, int day, int tzOffset)
        {
            id = member.Id.Value;
            age = member.Age.Value;
            position = member.Position ?? 100;
            genderID = member.Genderid ?? 0;

            name = member.Name;

            Person p = DbUtil.Db.LoadPersonById(id);

            if (p.Picture != null)
            {
                var image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == p.Picture.SmallId);

                if (image != null)
                {
                    picture = Convert.ToBase64String(image.Bits);
                    pictureX = p.Picture.X ?? 0;
                    pictureY = p.Picture.Y ?? 0;
                }
            }

            addOrg(member, day, tzOffset);
        }

        public void addOrg(CmsData.View.CheckinFamilyMember member, int day, int tzOffset)
        {
            CheckInOrganization org = new CheckInOrganization(member, day, tzOffset);

            orgs.Add(org);
        }
    }
}
