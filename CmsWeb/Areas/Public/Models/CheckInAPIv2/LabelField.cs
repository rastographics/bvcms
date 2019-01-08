using System;
using System.Reflection;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	public enum LabelField
	{
		// @formatter:off
		// Unused 0 - Used for translating INT to ENUM
		[LabelField( LabelFieldAttribute.CATEGORY_UNUSED )] UNUSED_FIELD_TYPE = 0,
		
		// Person 1-20
		[LabelField( LabelFieldAttribute.CATEGORY_PERSON )] PERSON_SECURITY_CODE = 1,
		[LabelField( LabelFieldAttribute.CATEGORY_PERSON )] PERSON_FIRST_NAME = 2,
		[LabelField( LabelFieldAttribute.CATEGORY_PERSON )] PERSON_LAST_NAME = 3,
		[LabelField( LabelFieldAttribute.CATEGORY_PERSON )] PERSON_ALLERGIES = 4,
		[LabelField( LabelFieldAttribute.CATEGORY_PERSON )] PERSON_INFO = 5,
		[LabelField( LabelFieldAttribute.CATEGORY_PERSON )] PERSON_MEMBER_GUEST = 6,

		// Parents 21-40
		[LabelField( LabelFieldAttribute.CATEGORY_PARENTS )] PARENTS_NAME = 21,
		[LabelField( LabelFieldAttribute.CATEGORY_PARENTS )] PARENTS_PHONE = 22,
		
		// Groups (Orgs) 61-80
		[LabelField( LabelFieldAttribute.CATEGORY_GROUP )] GROUP_NAME = 41,
		[LabelField( LabelFieldAttribute.CATEGORY_GROUP )] GROUP_LOCATION = 42,
		[LabelField( LabelFieldAttribute.CATEGORY_GROUP )] GROUP_SUBGROUPS = 43,
		
		// Attendance 41-60
		[LabelField( LabelFieldAttribute.CATEGORY_GROUP )] ATTENDANCE_DATE_TIME = 61,
		[LabelField( LabelFieldAttribute.CATEGORY_GROUP )] ATTENDANCE_PAGER = 62,
		[LabelField( LabelFieldAttribute.CATEGORY_GROUP )] ATTENDANCE_NOTES = 63,
	
		// @formatter:on
	}

	class LabelFieldAttribute : Attribute
	{
		public const int CATEGORY_UNUSED = 0;
		public const int CATEGORY_PERSON = 1;
		public const int CATEGORY_PARENTS = 2;
		public const int CATEGORY_GROUP = 3;

		public int category { get; }

		internal LabelFieldAttribute( int category )
		{
			this.category = category;
		}
	}

	static class LabelFieldExtension
	{
		public static int category( this LabelField field )
		{
			return GetAttr( field ).category;
		}

		private static LabelFieldAttribute GetAttr( LabelField p )
		{
			return (LabelFieldAttribute) Attribute.GetCustomAttribute( ForValue( p ), typeof( LabelFieldAttribute ) );
		}

		private static MemberInfo ForValue( LabelField p )
		{
			return typeof( LabelField ).GetField( Enum.GetName( typeof( LabelField ), p ) );
		}
	}
}