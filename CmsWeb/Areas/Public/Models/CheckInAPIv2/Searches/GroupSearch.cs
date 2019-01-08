using System.Diagnostics.CodeAnalysis;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2.Searches
{
	[SuppressMessage( "ReSharper", "UnassignedField.Global" )]
	[SuppressMessage( "ReSharper", "ClassNeverInstantiated.Global" )]
	[SuppressMessage( "ReSharper", "ConvertToConstant.Global" )]
	public class GroupSearch
	{
		public int peopleID = 0;
		public int campusID = 0;
		public int dayID = 0;

		public bool showAll = false;
	}
}