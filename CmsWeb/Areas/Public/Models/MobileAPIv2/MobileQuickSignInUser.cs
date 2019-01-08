using CmsData;

namespace CmsWeb.Areas.Public.Models.MobileAPIv2
{
	public class MobileQuickSignInUser
	{
		public int userID = 0;
		public int peopleID = 0;

		public string name = "";
		public string user = "";

		public MobileQuickSignInUser( Person person, User user )
		{
			if( user == null ) {
				this.user = "Create User";
			} else {
				this.userID = user.UserId;
				this.user = user.Username;
			}

			this.peopleID = person.PeopleId;
			this.name = person.Name;
		}
	}
}