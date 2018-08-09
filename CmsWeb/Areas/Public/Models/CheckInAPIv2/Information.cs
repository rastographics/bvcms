using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	[SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Global" )]
	public class Information
	{
		public List<SettingsEntry> settings;
		public List<State> states;
		public List<Country> countries;
		public List<Campus> campuses;
		public List<Gender> genders;
		public List<MaritalStatus> maritals;
	}
}