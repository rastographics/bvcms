using CmsData;
using CmsData.View;
using ImageData;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable MemberInitializerValueIgnored
// ReSharper disable RedundantDefaultMemberInitializer
// ReSharper disable CheckNamespace

namespace CmsWeb.CheckInAPI
{
    public class CheckInFamilyMember
    {
        public int id = 0;
        public int age = 0;
        public int position = 0;
        public int genderID = 0;

        public string name = "";
        public string altName = "";
        public string picture = "";

        public int pictureX = 0;
        public int pictureY = 0;

        public List<CheckInOrganization> orgs = new List<CheckInOrganization>();

        public CheckInFamilyMember(CMSDataContext cmsdb, CMSImageDataContext cmsidb, CheckinFamilyMember member, int day, int tzOffset)
        {
            id = member.Id ?? 0;
            age = member.Age ?? 0;
            position = member.Position ?? 100;
            genderID = member.Genderid ?? 0;

            name = member.Name;
            altName = member.AltName;

            Person p = cmsdb.LoadPersonById(id);

            if (p.Picture != null)
            {
                Image image = cmsidb.Images.SingleOrDefault(i => i.Id == p.Picture.SmallId);

                if (image != null)
                {
                    picture = Convert.ToBase64String(image.Bits);
                    pictureX = p.Picture.X ?? 0;
                    pictureY = p.Picture.Y ?? 0;
                }
            }

            addOrg(member, day, tzOffset);
        }

        public void addOrg(CheckinFamilyMember member, int day, int tzOffset)
        {
            CheckInOrganization org = new CheckInOrganization(member, day, tzOffset);

            orgs.Add(org);
        }
    }
}
