using System.Diagnostics.CodeAnalysis;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2.Results
{
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	public class AddEditPersonResults
	{
		public int familyID = 0;
		public int peopleID = 0;
		public int positionID = 0;
	}
}