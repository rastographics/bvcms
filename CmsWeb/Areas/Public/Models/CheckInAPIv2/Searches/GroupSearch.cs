using System.Diagnostics.CodeAnalysis;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2.Searches
{
	[SuppressMessage( "ReSharper", "UnassignedField.Global" )]
	[SuppressMessage( "ReSharper", "ClassNeverInstantiated.Global" )]
	public class GroupSearch
	{
		public int peopleID;
		public int campusID;
		public int dayID;

		public bool showAll;
	}
}