using CmsData;
using ImageData;
using System;
using System.Linq;
using UtilityExtensions;

// ReSharper disable MemberInitializerValueIgnored
// ReSharper disable RedundantDefaultMemberInitializer
// ReSharper disable CheckNamespace

namespace CmsWeb.CheckInAPI
{
    public class CheckInPerson
    {
        public int id = 0;
        public int familyID = 0;

        public string first = "";
        public string last = "";

        public string goesby = "";
        public string altName = "";

        public string father = "";
        public string mother = "";

        public int genderID = 0;
        public int maritalStatusID = 0;

        public DateTime? birthday;

        public string email = "";
        public string cell = "";
        public string home = "";

        public string address = "";
        public string address2 = "";
        public string city = "";
        public string state = "";
        public string zipcode = "";

        public string country = "";

        public string church = "";

        public string allergies = "";

        public string emergencyName = "";
        public string emergencyPhone = "";

        public int age = 0;

        public string picture = "";

        public int pictureX = 0;
        public int pictureY = 0;

        public CheckInPerson populate(Person p)
        {
            id = p.PeopleId;
            familyID = p.FamilyId;

            first = p.FirstName ?? "";
            last = p.LastName ?? "";

            goesby = p.NickName;
            altName = p.AltName;

            father = p.GetRecReg().Fname;
            mother = p.GetRecReg().Mname;

            genderID = p.GenderId;
            maritalStatusID = p.MaritalStatus.Id;

            birthday = p.BirthDate;

            email = p.EmailAddress;
            cell = p.CellPhone.FmtFone();
            home = p.HomePhone.FmtFone();

            address = p.Family.AddressLineOne ?? "";
            address2 = p.Family.AddressLineTwo ?? "";
            city = p.Family.CityName ?? "";
            state = p.Family.StateCode ?? "";
            zipcode = p.Family.ZipCode.FmtZip() ?? "";

            country = p.PrimaryCountry;

            church = p.OtherPreviousChurch;

            allergies = p.SetRecReg().MedicalDescription;

            emergencyName = p.SetRecReg().Emcontact;
            emergencyPhone = p.SetRecReg().Emphone;

            age = p.Age ?? 0;

            return this;
        }

        public void loadImage()
        {
            Person p = CmsData.DbUtil.Db.LoadPersonById(id);

            if (p.Picture != null)
            {
                Image image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == p.Picture.SmallId);

                if (image != null)
                {
                    picture = Convert.ToBase64String(image.Bits);
                    pictureX = p.Picture.X ?? 0;
                    pictureY = p.Picture.Y ?? 0;
                }
            }
        }
    }
}
