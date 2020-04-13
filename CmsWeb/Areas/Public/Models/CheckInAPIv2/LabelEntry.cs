using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.WebPages;
using CmsData;
using CmsWeb.Areas.Public.Models.CheckInAPIv2.Caching;
using UtilityExtensions;

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

        public bool invert;
        public int order;

		public Point<decimal> start = new Point<decimal>();
		public Point<int> align = new Point<int>();
		public Point<decimal> end = new Point<decimal>();
		public Point<decimal> size = new Point<decimal>();

		public LabelEntry( AttendanceCacheSet cacheSet, LabelFormatEntry formatEntry, Attendance attendance, AttendanceGroup group = null, int index = 0 )
		{
            bool conditionalRemovedEntry = false;
			typeID = formatEntry.typeID;

            if (formatEntry.orgEV.HasValue()) {
                Organization org = cacheSet.getOrganization(group.groupID);
                if (org != null)
                {
                    var ev = org.OrganizationExtras.SingleOrDefault(e => e.Field == formatEntry.orgEV);
                    if (ev == null)
                    {
                        conditionalRemovedEntry = true;
                    }
                } else
                {
                    conditionalRemovedEntry = true;
                }
            }
            if (formatEntry.personFlag.HasValue() && conditionalRemovedEntry == false)
            {
                CmsData.Person person = cacheSet.getPerson(attendance.peopleID);
                if (person != null)
                {
                    var sf = cacheSet.dataContext.ViewAllStatusFlags.SingleOrDefault(f => f.Flag == formatEntry.personFlag && f.PeopleId == person.PeopleId);
                    if (sf == null)
                    {
                        conditionalRemovedEntry = true;
                    }
                } else
                {
                    conditionalRemovedEntry = true;
                }
            }

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

                case 6:
                    // populate box data so that it is printed if data is present
                    // later we remove box entries if they are behind a blank field by querying this data prop
                    try
                    {
                        if (formatEntry.invert && formatEntry.fieldID != 0)
                        {
                            data = getField(cacheSet, (LabelField)formatEntry.fieldID, formatEntry.fieldFormat, attendance, group);
                        }
                        else
                        {
                            data = "print";
                        }
                    }
                    catch (Exception)
                    {
                        data = "Format Exception";
                    }

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

            invert = formatEntry.invert;
            order = formatEntry.order;

            if (conditionalRemovedEntry)
            {
                data = "ConditionalRemovedEntry";
            }
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
                    string firstname = person.PreferredName;
                    return string.Format( format, firstname );

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

                case LabelField.PERSON_FULL_NAME:
                    return string.Format(format, person.PreferredName, person.LastName);

                case LabelField.PERSON_DOB:
                    return string.Format(format, person.BirthDate);

                case LabelField.PERSON_EMERGENCY_NAME:
                    return string.Format(format, person.GetRecReg().Emcontact);

                case LabelField.PERSON_EMERGENCY_PHONE:
                    return string.Format(format, person.GetRecReg().Emphone.FmtFone());
                    
                case LabelField.PERSON_SCHOOL:
                    return string.Format(format, person.SchoolOther);

                case LabelField.PERSON_GRADE:
                    return string.Format(format, person.Grade);

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

                case LabelField.GROUP_LOCATION_AND_SUBGROUP:
                    org = cacheSet.getOrganization(group.groupID);
                    List<string> groupItems = new List<string>();
                    if (org != null && org.Location.HasValue())
                    {
                        groupItems.Add(org.Location);
                    }
                    if (group.subgroupName.HasValue())
                    {
                        groupItems.Add(group.subgroupName);
                    }
                    return string.Format( format, string.Join( " - ", groupItems) );

                case LabelField.GROUP_NAME_AND_TIME:
                    org = cacheSet.getOrganization(group.groupID);
                    meeting = cacheSet.getMeeting(group.groupID, group.datetime);

                    string orgName = org == null ? "" : org.OrganizationName;
                    return string.Format(format, orgName, meeting.MeetingDate);

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
