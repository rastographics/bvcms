using System;
using System.Diagnostics.CodeAnalysis;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	[SuppressMessage( "ReSharper", "ConvertToConstant.Global" )]
	[SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Global" )]
	[SuppressMessage( "ReSharper", "ClassNeverInstantiated.Global" )]
	public class NumberSearch
	{
		public string search = "";

		public DateTime date = DateTime.Now;

		public int campus = 0;
	}
}