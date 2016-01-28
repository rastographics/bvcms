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

        public string name = "";
        public string picture = "";

        public int pictureX = 0;
        public int pictureY = 0;

        public List<CheckInFamilyMemberOrg> orgs = new List<CheckInFamilyMemberOrg>();

        public CheckInFamilyMember(CmsData.View.CheckinFamilyMember member, int day, int tzOffset)
        {
            id = member.Id.Value;
            age = member.Age.Value;
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
            CheckInFamilyMemberOrg org = new CheckInFamilyMemberOrg(member, day, tzOffset);

            orgs.Add(org);
        }
    }

    /* Returned from database
    *Id INT, 
	*,Age INT
	*[Name] VARCHAR(150),
	*,OrgId INT
	*,Class VARCHAR(100)
	*,Leader VARCHAR(100)
	*,[hour] DATETIME
	*MemberVisitor CHAR,
	*,CheckedIn BIT
	*,NumLabels INT

	[Position] INT,
	[First] VARCHAR(50),
	PreferredName VARCHAR(50),
	[Last] VARCHAR(100)
	,BYear INT
	,BMon INT
	,BDay INT
	,Location VARCHAR(200)
	,Gender VARCHAR(10)
	,goesby VARCHAR(50)
	,email VARCHAR(150)
	,addr VARCHAR(100)
	,zip VARCHAR(15)
	,home VARCHAR(20)
	,cell VARCHAR(20)
	,marital INT
	,genderid INT
	,CampusId INT
	,allergies VARCHAR(1000)
	,emfriend VARCHAR(100)
	,emphone VARCHAR(100)
	,activeother BIT
	,parent VARCHAR(100)
	,grade INT
	,HasPicture BIT
	,Custody BIT
	,Transport BIT
	,RequiresSecurityLabel BIT
	,church VARCHAR(130) */
}
