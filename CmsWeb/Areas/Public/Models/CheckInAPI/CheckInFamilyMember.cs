using System;
using System.Collections.Generic;

namespace CmsWeb.CheckInAPI
{
    public class CheckInFamilyMember
    {
        public int id = 0;
        public int age = 0;

        public string name = "";
        public string imageURL = "";

        public List<CheckInFamilyMemberOrg> orgs = new List<CheckInFamilyMemberOrg>();

        public CheckInFamilyMember(CmsData.View.CheckinFamilyMember member)
        {
            id = member.Id.Value;
            age = member.Age.Value;
            name = member.Name;

            addOrg(member);
        }

        public void addOrg(CmsData.View.CheckinFamilyMember member)
        {
            CheckInFamilyMemberOrg org = new CheckInFamilyMemberOrg(member);

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
