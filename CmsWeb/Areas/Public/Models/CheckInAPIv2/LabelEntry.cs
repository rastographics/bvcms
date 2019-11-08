using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.WebPages;
using CmsData;
using CmsWeb.Areas.Public.Models.CheckInAPIv2.Caching;

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

		public LabelEntry( AttendanceCacheSet cacheSet, LabelFormatEntry formatEntry, Attendance attendance, AttendanceGroup group = null, int index = 0 )
		{
			typeID = formatEntry.typeID;

			switch( formatEntry.typeID ) {
				case 1:
					try {
						data = getField( cacheSet, (LabelField) formatEntry.fieldID, formatEntry.fieldFormat, attendance, group );
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

		public string getField( AttendanceCacheSet cacheSet, LabelField field, string format, Attendance attendance, AttendanceGroup group )
		{
			switch( field.category() ) {
				case LabelFieldAttribute.CATEGORY_UNUSED:
					return "";

				case LabelFieldAttribute.CATEGORY_PERSON:
					return getPersonField( cacheSet, field, format, attendance );

				case LabelFieldAttribute.CATEGORY_PARENTS:
					return getParentsField( cacheSet, field, format, attendance );

				case LabelFieldAttribute.CATEGORY_GROUP:
					return getGroupField( cacheSet, field, format, group );

				default:
					return "";
			}
		}

		private string getPersonField( AttendanceCacheSet cacheSet, LabelField field, string format, Attendance attendance )
		{
			CmsData.Person person = cacheSet.getPerson( attendance.peopleID );

			if( person == null ) {
				return "";
			}

			switch( field ) {
				case LabelField.PERSON_SECURITY_CODE:
					return string.Format( format, cacheSet.securityCode );

				case LabelField.PERSON_FIRST_NAME:
					return string.Format( format, person.FirstName );

				case LabelField.PERSON_LAST_NAME:
					return string.Format( format, person.LastName );

				case LabelField.PERSON_ALLERGIES:
					return string.Format( format, person.GetRecReg().MedicalDescription );

				case LabelField.PERSON_INFO:
					string allergies = person.GetRecReg().MedicalDescription.IsEmpty() ? "" : "A";
					string custody = person.CustodyIssue.HasValue && person.CustodyIssue.Value ? "C" : "";
					string transport = person.OkTransport.HasValue && person.OkTransport.Value ? "T" : "";

					return string.Format( format, allergies, custody, transport );

				case LabelField.PERSON_MEMBER_GUEST:
					string member = person.MemberStatus.Member ? "Member" : "";
					string guest = person.MemberStatus.Member ? "" : "Guest";

					return string.Format( format, member, guest );

				default:
					return "";
			}
		}

		private string getParentsField( AttendanceCacheSet cacheSet, LabelField field, string format, Attendance attendance )
		{
			CmsData.Person person = cacheSet.getPerson( attendance.peopleID );

			if( person == null ) {
				return "";
			}

			CmsData.Family family = cacheSet.getFamily( person.FamilyId );

			if( family == null ) {
				return "";
			}

			CmsData.Person head = cacheSet.getPerson( family.HeadOfHouseholdId ?? 0 );
			CmsData.Person spouse = cacheSet.getPerson( family.HeadOfHouseholdSpouseId ?? 0 );

			if( head == null && spouse == null ) {
				return "";
			}

			switch( field ) {
				case LabelField.PARENTS_NAME:
					List<string> names = new List<string>();

					if( head != null ) {
						names.Add( head.FirstName );
					}

					if( spouse != null ) {
						names.Add( spouse.FirstName );
					}

					return string.Format( format, string.Join( ", ", names ) );

				case LabelField.PARENTS_PHONE:
					List<string> phones = new List<string>();

					if( head != null && !head.CellPhone.IsEmpty() ) {
						phones.Add( head.FirstName + ": " + head.CellPhone );
					}

					if( spouse != null && !spouse.CellPhone.IsEmpty() ) {
						phones.Add( spouse.FirstName + ": " + spouse.CellPhone );
					}

					return string.Format( format, string.Join( ", ", phones ) );

				default:
					return "";
			}
		}

		private string getGroupField( AttendanceCacheSet cacheSet, LabelField field, string format, AttendanceGroup group )
		{
			if( group == null ) {
				return "";
			}

			Organization org;
			Meeting meeting;

			switch( field ) {
				case LabelField.GROUP_NAME:
					org = cacheSet.getOrganization( group.groupID );

					return org == null ? "" : string.Format( format, org.OrganizationName );

				case LabelField.GROUP_LOCATION:
					org = cacheSet.getOrganization( group.groupID );

					return org == null ? "" : string.Format( format, org.Location );

				case LabelField.GROUP_SUBGROUPS:
					return group.subgroupName;

				case LabelField.ATTENDANCE_DATE_TIME:
					meeting = cacheSet.getMeeting( group.groupID, group.datetime );

					return string.Format( format, meeting.MeetingDate );

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