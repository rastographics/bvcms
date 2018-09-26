using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.WebPages;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	[SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Global" )]
	public class LabelEntry
	{
		public int typeID;

		public string data;

		public string font;
		public int fontSize;

		public Point<decimal> start = new Point<decimal>();
		public Point<int> align = new Point<int>();
		public Point<decimal> end = new Point<decimal>();
		public Point<int> size = new Point<int>();

		public LabelEntry( LabelFormatEntry formatEntry, Attendance attendance, AttendanceGroup group = null, int index = 0 )
		{
			typeID = formatEntry.typeID;

			switch( formatEntry.typeID ) {
				case 1:
					try {
						data = getField( (LabelField) formatEntry.fieldID, formatEntry.fieldFormat, attendance, group );
					} catch( Exception ) {
						data = "Format Exception";
					}

					break;

				case 4:
				case 5:
					data = formatEntry.fieldFormat;
					break;

				default:
					data = "";
					break;
			}

			font = formatEntry.font;
			fontSize = formatEntry.fontSize;

			start.x = formatEntry.startX;
			start.y = formatEntry.startY + (formatEntry.offset * index);

			align.x = formatEntry.alignX;
			align.y = formatEntry.alignY;

			end.x = formatEntry.endX;
			end.y = formatEntry.endY + (formatEntry.offset * index);

			size.x = formatEntry.width;
			size.y = formatEntry.height;
		}

		public string getField( LabelField field, string format, Attendance attendance, AttendanceGroup group )
		{
			switch( field.category() ) {
				case LabelFieldAttribute.CATEGORY_UNUSED:
					return "";

				case LabelFieldAttribute.CATEGORY_PERSON:
					return getPersonField( field, format, attendance );

				case LabelFieldAttribute.CATEGORY_PARENTS:
					return getParentsField( field, format, attendance );

				case LabelFieldAttribute.CATEGORY_GROUP:
					return getGroupField( field, format, group );

				default:
					return "";
			}
		}

		private string getPersonField( LabelField field, string format, Attendance attendance )
		{
			if( attendance.person == null ) return "";

			switch( field ) {
				case LabelField.PERSON_SECURITY_CODE:
					return string.Format( format, attendance.labelSecurityCode );

				case LabelField.PERSON_FIRST_NAME:
					return string.Format( format, attendance.person.FirstName );

				case LabelField.PERSON_LAST_NAME:
					return string.Format( format, attendance.person.LastName );

				case LabelField.PERSON_ALLERGIES:
					return string.Format( format, attendance.person.GetRecReg().MedicalDescription );

				case LabelField.PERSON_INFO:
					string allergies = attendance.person.GetRecReg().MedicalDescription.IsEmpty() ? "" : "A";
					string custody = attendance.person.CustodyIssue.HasValue && attendance.person.CustodyIssue.Value ? "C" : "";
					string transport = attendance.person.OkTransport.HasValue && attendance.person.OkTransport.Value ? "T" : "";

					return string.Format( format, allergies, custody, transport );

				case LabelField.PERSON_MEMBER_GUEST:
					string member = attendance.person.MemberStatus.Member ? "Member" : "";
					string guest = attendance.person.MemberStatus.Member ? "" : "Guest";

					return string.Format( format, member, guest );

				default:
					return "";
			}
		}

		private string getParentsField( LabelField field, string format, Attendance attendance )
		{
			if( attendance.head == null && attendance.spouse == null ) return "";

			switch( field ) {
				case LabelField.PARENTS_NAME:
					List<string> names = new List<string>();

					if( attendance.head != null ) names.Add( attendance.head.FirstName );
					if( attendance.spouse != null ) names.Add( attendance.spouse.FirstName );

					return string.Format( format, string.Join( ", ", names ) );

				case LabelField.PARENTS_PHONE:
					List<string> phones = new List<string>();

					if( attendance.head != null && !attendance.head.CellPhone.IsEmpty() ) phones.Add( attendance.head.FirstName + ": " + attendance.head.CellPhone );
					if( attendance.spouse != null && !attendance.spouse.CellPhone.IsEmpty() ) phones.Add( attendance.spouse.FirstName + ": " + attendance.spouse.CellPhone );

					return string.Format( format, string.Join( ", ", phones ) );

				default:
					return "";
			}
		}

		private string getGroupField( LabelField field, string format, AttendanceGroup group )
		{
			if( group == null ) return "";

			switch( field ) {
				case LabelField.GROUP_NAME:
					return string.Format( format, group.org.OrganizationName );

				case LabelField.GROUP_LOCATION:
					return string.Format( format, group.org.Location );

				case LabelField.GROUP_SUBGROUPS:
					return group.subgroupName;

				case LabelField.ATTENDANCE_DATE_TIME:
					return string.Format( format, group.meeting.MeetingDate );

				case LabelField.ATTENDANCE_PAGER:
					return "";

				case LabelField.ATTENDANCE_NOTES:
					return "";

				default:
					return "";
			}
		}

		public class Point<T>
		{
			public const int TOP = 1;
			public const int LEFT = 1;
			public const int CENTER = 2;
			public const int RIGHT = 3;
			public const int BOTTOM = 3;

			public T x;
			public T y;
		}
	}
}
