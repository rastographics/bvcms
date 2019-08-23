using CmsData;
using ImageData;
using System;
using System.Linq;

namespace CmsWeb.MobileAPI
{
    public class MobileFamilyMember
    {
        public string id = "";
        public string name = "";
        public string age = "";
        public string gender = "";
        public string position = "";
        public string memberStatus = "";

        public bool deceased;

        public string pictureData = "";

        public int pictureX;
        public int pictureY;

        public MobileFamilyMember() { }

        public MobileFamilyMember(Person person, CMSImageDataContext cmsidb)
        {
            id = person.PeopleId.ToString();
            name = person.Name ?? "";
            age = Person.AgeDisplay(person.Age, person.PeopleId).ToString();
            gender = person.Gender.Description ?? "";
            position = person.FamilyPosition.Description ?? "";
            memberStatus = person.MemberStatus.Description;
            deceased = person.Deceased;

            if (person.Picture != null)
            {
                var image = cmsidb.Images.SingleOrDefault(i => i.Id == person.Picture.SmallId);

                if (image != null)
                {
                    pictureData = Convert.ToBase64String(image.Bits);
                    pictureX = person.Picture.X ?? 0;
                    pictureY = person.Picture.Y ?? 0;
                }
            }
        }
    }
}
