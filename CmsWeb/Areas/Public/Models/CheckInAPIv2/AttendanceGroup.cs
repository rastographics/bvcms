using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CmsData;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	public class AttendanceGroup
	{
		public int groupID = 0;
		public int subgroupID = 0;
		public string subgroupName = "";
		public DateTime datetime = DateTime.Now;
		public bool present = false;
		public bool join = false;
	}
}