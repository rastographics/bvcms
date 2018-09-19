using System;
using System.Diagnostics.CodeAnalysis;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2.Searches
{
	[SuppressMessage( "ReSharper", "ClassNeverInstantiated.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	[SuppressMessage( "ReSharper", "ConvertToConstant.Global" )]
	public class SubgroupList
	{
		public int groupID = 0;
		public int scheduleID = 0;
		public int peopleID = 0;

		public DateTime meetingDate = DateTime.Now;
	}
}