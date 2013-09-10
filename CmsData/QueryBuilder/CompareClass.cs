/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Caching;

namespace CmsData
{
    public class CompareClass
    {
        public FieldType FieldType { get; set; }
        public CompareType CompType { get; set; }
        public bool IsMultiple
        {
            get { return CompType == CompareType.OneOf || CompType == CompareType.NotOneOf; }
        }
        public string Display { get; set; }
        internal string ToString(QueryBuilderClause c)
        {
            string fld = c.FieldInfo.Display(c);
            if (c.Field == "MatchAnything")
                return fld;
            switch (FieldType)
            {
                case FieldType.NullBit:
                case FieldType.Bit:
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                    return Display.Fmt(fld, c.CodeValues);
                case FieldType.String:
                case FieldType.StringEqual:
                case FieldType.StringEqualOrStartsWith:
                case FieldType.Number:
                case FieldType.NumberLG:
                case FieldType.NullNumber:
                case FieldType.Integer:
                case FieldType.IntegerSimple:
                case FieldType.IntegerEqual:
                case FieldType.NullInteger:
                    return Display.Fmt(fld, c.TextValue);
                case FieldType.Date:
                case FieldType.DateSimple:
                    return Display.Fmt(fld, c.DateValue);
                case FieldType.DateField:
                    return Display.Fmt(fld, c.CodeIdValue);
                default:
                    throw new ArgumentException();
            }
        }
        internal Expression Expression(QueryBuilderClause c, ParameterExpression parm, CMSDataContext Db)
        {
            switch (c.FieldInfo.QueryType)
            {
                case QueryType.DaysAfterNthVisitDateRange:
                    return Expressions.DaysAfterNthVisitDateRange(parm, Db,
                               c.StartDate,
                               c.EndDate,
                               c.Quarters.ToInt(),
                               c.Program,
                               c.Division,
                               c.Organization,
                               CompType,
                               c.TextValue.ToInt());
                case QueryType.AttendPct:
                    return Expressions.AttendPct(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
                               CompType,
                               decimal.Parse(c.TextValue));
                case QueryType.AttendPctHistory:
                    return Expressions.AttendPctHistory(parm, Db,
                               c.Program,
                               c.Division,
                               c.Organization,
                               c.StartDate,
                               c.EndDate,
                               CompType,
                               double.Parse(c.TextValue));
                case QueryType.AttendCntHistory:
                    return Expressions.AttendCntHistory(parm, Db,
                               c.Program,
                               c.Division,
                               c.Organization,
                               c.Schedule,
                               c.StartDate,
                               c.EndDate,
                               CompType,
                               c.TextValue.ToInt());
                case QueryType.AttendTypeAsOf:
                    return Expressions.AttendanceTypeAsOf(parm,
                               c.StartDate,
                               c.EndDate,
                               c.Program,
                               c.Division,
                               c.Organization,
							   c.OrgType ?? 0,
                               CompType,
                               c.CodeIntIds);
                case QueryType.AttendMemberTypeAsOf:
                    return Expressions.AttendMemberTypeAsOf(Db,
                               parm,
                               c.StartDate,
                               c.EndDate,
                               c.Program,
                               c.Division,
                               c.Organization,
                               CompType,
                               string.Join(",", c.CodeStrIds));
                // B --------------------
                case QueryType.PmmBackgroundCheckStatus:
                    return Expressions.BackgroundCheckStatus(parm,
                               c.Tags,
                               c.Quarters,
                               CompType,
                               c.CodeIntIds);
                case QueryType.Birthday:
                    return Expressions.Birthday(parm, CompType, c.TextValue);
                // C ------------------------
                case QueryType.CampusId:
                    return Expressions.CampusId(parm,
                               CompType,
                               c.CodeIntIds);
                case QueryType.CheckInVisits:
                    return Expressions.CheckInVisits(parm, Db,
                        CompType, c.TextValue.ToInt());
                case QueryType.CreatedBy:
                    return Expressions.CreatedBy(parm, Db,
                        CompType, c.TextValue);
                case QueryType.ContributionAmount2:
                    return Expressions.ContributionAmount2(parm, Db,
                               c.StartDate,
                               c.EndDate, c.Quarters.ToInt(),
                               CompType,
                               Decimal.Parse(c.TextValue));
                case QueryType.ContributionChange:
                    return Expressions.ContributionChange(parm, Db,
                               c.StartDate,
                               c.EndDate,
                               CompType,
                               double.Parse(c.TextValue));
                // D --------------------
                case QueryType.DaysBetween12Attendance:
                    return Expressions.DaysBetween12Attendance(parm, Db,
                               c.Days,
                               c.Program,
                               c.Division,
                               c.Organization,
                               CompType,
                               c.TextValue.ToInt());
                case QueryType.DaysTillBirthday:
                    return Expressions.DaysTillBirthday(parm, Db,
                               CompType,
                               c.TextValue.ToInt());
                case QueryType.DaysTillAnniversary:
                    return Expressions.DaysTillAnniversary(parm, Db,
                               CompType,
                               c.TextValue.ToInt());
                case QueryType.DaysSinceContact:
                    return Expressions.DaysSinceContact(parm,
                               CompType,
                               c.TextValue.ToInt());
                case QueryType.DuplicateNames:
                    return Expressions.DuplicateNames(Db,
                               parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.DuplicateEmails:
                    return Expressions.DuplicateEmails(Db,
                               parm,
                               CompType,
                               c.CodeIds == "1");
                // E ----------------------------
                case QueryType.EmailRecipient:
                    return Expressions.EmailRecipient(parm,
                               CompType,
                               c.TextValue.ToInt());
                // F ----------------------------
                case QueryType.FamHasPrimAdultChurchMemb:
                    return Expressions.FamHasPrimAdultChurchMemb(parm, CompType, c.CodeIds == "1");
                case QueryType.FamilyHasChildren:
                    return Expressions.FamilyHasChildren(parm, CompType, c.CodeIds == "1");
                case QueryType.FamilyHasChildrenAged:
                    return Expressions.FamilyHasChildrenAged(parm, c.Age.ToInt(), CompType, c.CodeIds == "1");
                case QueryType.FamilyHasChildrenAged2:
                    return Expressions.FamilyHasChildrenAged2(parm, c.Quarters, CompType, c.CodeIds == "1");
                case QueryType.FamilyHasChildrenAged3:
                    return Expressions.FamilyHasChildrenAged3(parm, c.Quarters, CompType, c.CodeIntIds);
                // H --------------------
                case QueryType.HasBalanceInCurrentOrg:
                    return Expressions.HasBalanceInCurrentOrg(Db, parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.HasCurrentTag:
                    return Expressions.HasCurrentTag(Db, parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.HasMyTag:
                    return Expressions.HasMyTag(parm,
                               c.Tags,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.HasLowerName:
                    return Expressions.HasLowerName(Db,
                               parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.HasPicture:
                    return Expressions.HasPicture(parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.HasRelatedFamily:
                    return Expressions.HasRelatedFamily(parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.HasParents:
                    return Expressions.HasParents(parm,
                               CompType,
                               c.CodeIds == "1");//HasValidEmailAddress
                case QueryType.HasInvalidEmailAddress:
                    return Expressions.HasInvalidEmailAddress(Db, parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.HaveVolunteerApplications:
                    return Expressions.HasVolunteerApplications(parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.HasManagedGiving:
                    return Expressions.HasManagedGiving(parm, Db,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.HasMemberDocs:
                    return Expressions.HasMemberDocs(parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.HasContacts:
                    return Expressions.HasContacts(parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.HasTaskWithName:
                    return Expressions.HasTaskWithName(parm,
                                CompType,
                                c.TextValue);
				case QueryType.HasIncompleteTask:
					return Expressions.HasIncompleteTask(parm,
                                CompType,
                                c.TextValue);
                case QueryType.HasOptoutsForEmail:
                    return Expressions.HasEmailOptout(parm,
								c.EndDate,
                                CompType,
                                c.TextValue);
                // I -----------------
                case QueryType.IsCurrentPerson:
                    return Expressions.IsCurrentPerson(Db, parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.InCurrentOrg:
                    return Expressions.InCurrentOrg(Db, parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.InactiveCurrentOrg:
                    return Expressions.InactiveCurrentOrg(Db, parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.InOneOfMyOrgs:
                    return Expressions.InOneOfMyOrgs(parm, Db,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.IsUser:
                    return Expressions.IsUser(parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.IsMemberOf:
                    return Expressions.IsMemberOf(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.IsInactiveMemberOf:
                    return Expressions.IsInactiveMemberOf(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.IsPendingMemberOf:
                    return Expressions.IsPendingMemberOf(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.IsTopGiver:
                    return Expressions.IsTopGiver(parm, Db,
                                c.Days,
                                c.Quarters,
                                CompType,
                                c.CodeIds == "1");
                case QueryType.IsTopPledger:
                    return Expressions.IsTopPledger(parm, Db,
                                c.Days,
                                c.Quarters,
                                CompType,
                                c.CodeIds == "1");
                case QueryType.InBFClass:
                    return Expressions.InBFClass(parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.IncludeDeceased:
                    c.SetIncludeDeceased();
                    return Expressions.IncludeDeceased(parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.IsHeadOfHousehold:
                    return Expressions.IsHeadOfHousehold(parm,
                               CompType,
                               c.CodeIds == "1");
                // K -------------------
                case QueryType.KidsRecentAttendCount:
                    return Expressions.KidsRecentAttendCount(parm,
                               c.Days,
                               CompType,
                               c.TextValue.ToInt());
                // L -------------------
                case QueryType.LeadersUnderCurrentOrg:
                    return Expressions.LeadersUnderCurrentOrg(parm, Db,
                               CompType,
                               c.CodeIds == "1");
                // M -------------------
                case QueryType.MatchAnything:
                    return Expressions.MatchAnything(parm);
                case QueryType.MembersUnderCurrentOrg:
                    return Expressions.MembersUnderCurrentOrg(parm, Db,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.MadeContactTypeAsOf:
                    return Expressions.MadeContactTypeAsOf(parm,
                               c.StartDate,
                               c.EndDate,
                               c.Program,
                               CompType,
                               c.CodeIntIds);
                case QueryType.MedicalLength:
                    return Expressions.MedicalLength(parm,
                               CompType,
                               c.TextValue.ToInt());
				case QueryType.MembOfOrgWithCampus:
                    return Expressions.MembOfOrgWithCampus(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
							   c.OrgType ?? 0,
                               CompType,
                               c.CodeIntIds);
                case QueryType.MembOfOrgWithSched:
                    return Expressions.MembOfOrgWithSched(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
                               CompType,
                               c.CodeIntIds);
                case QueryType.MemberTypeCodes:
                    return Expressions.MemberTypeIds(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
							   c.OrgType ?? 0,
                               c.Schedule,
							   c.Campus ?? 0,
                               CompType,
                               c.CodeIntIds);
                case QueryType.MemberTypeAsOf:
                    return Expressions.MemberTypeAsOf(parm,
                               c.StartDate,
                               c.EndDate,
                               c.Program,
                               c.Division,
                               c.Organization,
							   c.OrgType ?? 0,
                               CompType,
                               c.CodeIntIds);
                case QueryType.MeetingId:
                    return Expressions.MeetingId(parm,
                               CompType,
                               c.TextValue.ToInt());
                // N -------------------
                case QueryType.NumberOfMemberships:
                    return Expressions.NumberOfMemberships(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
                               c.Schedule,
                               CompType,
                               c.TextValue.ToInt());
                case QueryType.NumberOfFamilyMembers:
                    return Expressions.NumberOfFamilyMembers(parm,
                               CompType,
                               c.TextValue.ToInt());
                // O --------------------------
                //case QueryType.OrgMemberCreatedDate:
                //    return Expressions.OrgMemberCreatedDate(parm,
                //               c.Program,
                //               c.Division,
                //               c.Organization,
                //               CompType,
                //               c.DateValue);
                case QueryType.OrgInactiveDate:
                    return Expressions.OrgInactiveDate(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
                               CompType,
                               c.DateValue);
                case QueryType.OrgJoinDateCompare:
                    return Expressions.OrgJoinDateCompare(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
                               CompType,
                               c.CodeIdValue);
                case QueryType.OrgJoinDateDaysAgo:
                    return Expressions.OrgJoinDateDaysAgo(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
                               CompType,
                               c.TextValue.ToInt());
                case QueryType.OrgJoinDate:
                    return Expressions.OrgJoinDate(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
                               CompType,
                               c.DateValue);
                // P ----------------
                case QueryType.ParentsOf:
                    c.SetParentsOf(CompType, c.CodeIds == "1");
                    return Expressions.ParentsOf(parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.PendingCurrentOrg:
                    return Expressions.PendingCurrentOrg(Db, parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.HasPeopleExtraField:
                    return Expressions.HasPeopleExtraField(parm,
                                CompType,
                                c.TextValue);
                case QueryType.PeopleExtra:
                    return Expressions.PeopleExtra(parm,
                                CompType,
                                c.CodeStrIds);
                case QueryType.PeopleExtraData:
                    return Expressions.PeopleExtraData(parm,
                                c.Quarters,
                                CompType,
                                c.TextValue);
                case QueryType.PeopleExtraDate:
                    return Expressions.PeopleExtraDate(parm,
                                c.Quarters,
                                CompType,
                                c.DateValue);
                case QueryType.PeopleExtraInt:
                    return Expressions.PeopleExtraInt(parm,
                                c.Quarters,
                                CompType,
                                c.TextValue.ToInt2());
                case QueryType.PeopleIds:
                    return Expressions.PeopleIds(parm,
                                CompType,
								(c.TextValue ?? "").Split(',').Select(aa => aa.ToInt()).ToArray());
                case QueryType.PreviousCurrentOrg:
                    return Expressions.PreviousCurrentOrg(Db, parm,
                               CompType,
                               c.CodeIds == "1");
                // R ----------------
                case QueryType.RecentCreated:
                    return Expressions.RecentCreated(parm,
								c.Days,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.RecentJoinChurch:
                    return Expressions.RecentJoinChurch(
                        parm,
                        c.Days,
                        CompType,
                        c.CodeIds == "1");
                case QueryType.RecentAttendType:
                    return Expressions.RecentAttendType(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
							   c.OrgType ?? 0,
                               c.Days,
                               CompType,
                               c.CodeIntIds);
                case QueryType.RecentContactMinistry:
                    return Expressions.RecentContactMinistry(parm,
                               c.Days,
                               CompType,
                               c.CodeIntIds);
                case QueryType.RecentContactType:
                    return Expressions.RecentContactType(parm,
                               c.Days,
                               CompType,
                               c.CodeIntIds);
                case QueryType.RecentContactReason:
                    return Expressions.RecentContactReason(parm,
                               c.Days,
                               CompType,
                               c.CodeIntIds);
                case QueryType.RecentDecisionType:
                    return Expressions.RecentDecisionType(parm,
                               c.Days,
                               CompType,
                               c.CodeIntIds);
                case QueryType.RecentEmailCount:
                    return Expressions.RecentEmailCount(parm,
                               c.Days,
                               CompType,
                               c.TextValue.ToInt());
				case QueryType.RecentHasIndContributions:
					return Expressions.RecentHasIndContributions(parm, Db,
							   c.Days,
							   CompType,
							   c.CodeIds == "1");
                case QueryType.RecentContributionCount:
                    return Expressions.RecentContributionCount(parm, Db,
                               c.Days, c.Quarters.ToInt(),
                               CompType,
                               c.TextValue.ToInt());
                case QueryType.RecentFirstTimeGiver:
                    return Expressions.RecentFirstTimeGiver(parm, Db,
                               c.Days, c.Quarters.ToInt(),
                               CompType,
							   c.CodeIds == "1");
                case QueryType.RecentContributionAmount:
                    return Expressions.RecentContributionAmount(parm, Db,
                               c.Days, c.Quarters.ToInt(),
                               CompType,
                               Decimal.Parse(c.TextValue));
                case QueryType.RecentGivingAsPctOfPrevious:
                    return Expressions.RecentGivingAsPctOfPrevious(parm, Db,
                               c.Quarters.ToInt2() ?? 365,
                               CompType,
                               Double.Parse(c.TextValue));
                case QueryType.RecentPledgeCount:
                    return Expressions.RecentPledgeCount(parm, Db,
                               c.Days, c.Quarters.ToInt(),
                               CompType,
                               c.TextValue.ToInt());
                case QueryType.RecentPledgeAmount:
                    return Expressions.RecentPledgeAmount(parm, Db,
                               c.Days, c.Quarters.ToInt(),
                               CompType,
                               Decimal.Parse(c.TextValue));
                case QueryType.RecentAttendCount:
                    return Expressions.RecentAttendCount(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
							   c.OrgType ?? 0,
                               c.Days,
                               CompType,
                               c.TextValue.ToInt());
                case QueryType.RecentAttendCountAttCred:
                    return Expressions.RecentAttendCountAttCred(parm, Db,
                               c.Program,
                               c.Division,
                               c.Organization,
							   c.Quarters.ToInt(),
                               c.Schedule,
                               c.Days,
                               CompType,
                               c.TextValue.ToInt());
                case QueryType.RecentNewVisitCount:
                    return Expressions.RecentNewVisitCount(parm, Db,
                               c.Program,
                               c.Division,
                               c.Organization,
							   c.OrgType ?? 0,
                               c.Quarters,
                               c.Days,
                               CompType,
                               c.TextValue.ToInt());
                case QueryType.RecentRegistrationType:
                    return Expressions.RecentRegistrationType(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
							   c.OrgType ?? 0,
                               c.Days,
                               CompType,
                               c.CodeIntIds);
                case QueryType.RecentAttendMemberType:
                    return Expressions.RecentAttendMemberType(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
                               c.Days,
                               CompType,
                               c.CodeIntIds);
				case QueryType.RecentVisitNumber:
                    return Expressions.RecentVisitNumber(parm, Db,
								c.Quarters,
                               c.Days,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.RecActiveOtherChurch:
                    return Expressions.RecActiveOtherChurch(parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.RecInterestedCoaching:
                    return Expressions.RecInterestedCoaching(parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.RegisteredForMeetingId:
                    return Expressions.RegisteredForMeetingId(parm,
                               CompType,
                               c.TextValue.ToInt());
                // S -------------------------
                case QueryType.SavedQuery:
                    return Expressions.SavedQuery(parm, Db,
                               c.SavedQueryIdDesc,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.SmallGroup:
                    return Expressions.SmallGroup(parm,
                               c.Program,
                               c.Division,
                               c.Organization,
                               CompType, c.TextValue);
                case QueryType.StatusFlag:
                    return Expressions.StatusFlag(parm,
                               CompType,
                               c.CodeValues);

                // U ----------------------
                case QueryType.UserRole:
                    return Expressions.UserRole(parm,
                               CompType,
                               c.CodeIntIds);
                // V -------------------
                case QueryType.VisitNumber:
                    return Expressions.VisitNumber(parm, Db,
                               c.Quarters,
                               CompType,
                               c.DateValue);
                case QueryType.VisitedCurrentOrg:
                    return Expressions.VisitedCurrentOrg(Db, parm,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.VolunteerApprovalCode:
                    return Expressions.VolunteerApprovalCode(parm,
                               CompType,
                               c.CodeIntIds);
                case QueryType.VolAppStatusCode:
                    return Expressions.VolAppStatusCode(parm,
                               CompType,
                               c.CodeIntIds);
                case QueryType.VolunteerProcessedDateMonthsAgo:
                    return Expressions.VolunteerProcessedDateMonthsAgo(parm,
                        CompType,
                        int.Parse(c.TextValue));
                // W ----------------------
                case QueryType.WasMemberAsOf:
                    return Expressions.WasMemberAsOf(parm, 
                               c.StartDate,
                               c.EndDate,
                               c.Program,
                               c.Division,
                               c.Organization,
							   c.OrgType ?? 0,
                               CompType,
                               c.CodeIds == "1");
                case QueryType.WeddingDate:
                    return Expressions.WeddingDate(parm, CompType, c.TextValue);
                case QueryType.WidowedDate:
                    return Expressions.WidowedDate(parm,
                               Db,
                               CompType,
                               c.DateValue);
				case QueryType.MemberStatusId:
				case QueryType.MaritalStatusId:
				case QueryType.GenderId:
				case QueryType.DropCodeId:
				case QueryType.JoinCodeId:
                    if (CompType == CompareType.IsNull || CompType == CompareType.IsNotNull)
						return Expressions.CompareConstant(parm, c.Field, CompType, -1);
            		return Expressions.CompareConstant(parm, c.Field, CompType,
            			IsMultiple ? (object)c.CodeIntIds : (object)c.CodeIds.ToInt());
				case QueryType.PrimaryAddress:
				case QueryType.PrimaryAddress2:
				case QueryType.PrimaryZip:
				case QueryType.PrimaryCountry:
				case QueryType.PrimaryCity:
				case QueryType.FirstName:
				case QueryType.MiddleName:
				case QueryType.MaidenName:
				case QueryType.NickName:
				case QueryType.CellPhone:
				case QueryType.WorkPhone:
				case QueryType.HomePhone:
				case QueryType.EmailAddress:
				case QueryType.EmailAddress2:
                    if (CompType == CompareType.IsNull || CompType == CompareType.IsNotNull)
						return Expressions.CompareConstant(parm, c.Field, CompType, null);
					return Expressions.CompareStringConstant(parm, c.Field, CompType, c.TextValue ?? "");
            	default:
                    if (CompType == CompareType.IsNull || CompType == CompareType.IsNotNull)
                        return Expressions.CompareConstant(parm,
                                   c.Field,
                                   CompType,
                                   null);
                    switch (FieldType)
                    {
                        case FieldType.NullBit:
                        case FieldType.Bit:
                            return Expressions.CompareConstant(parm,
                                       c.Field,
                                       CompType,
                                       c.CodeIds == "1");
                        case FieldType.Code:
                            return Expressions.CompareConstant(parm,
                                       c.Field,
                                       CompType,
                                       IsMultiple ? (object)c.CodeIntIds : (object)c.CodeIds.ToInt());
                        case FieldType.NullCode:
                            return Expressions.CompareCodeConstant(parm,
                                       c.Field,
                                       CompType,
                                       IsMultiple ? (object)c.CodeIntIds : (object)c.CodeIds.ToInt());
                        case FieldType.CodeStr:
                            return Expressions.CompareConstant(parm,
                                       c.Field,
                                       CompType,
                                       IsMultiple ? (object)c.CodeStrIds : (object)c.CodeIdValue);
                        case FieldType.String:
                            return Expressions.CompareConstant(parm,
                                       c.Field,
                                       CompType,
                                       c.TextValue);
                        case FieldType.Number:
                        case FieldType.NullNumber:
                            return Expressions.CompareConstant(parm,
                                       c.Field,
                                       CompType,
                                       decimal.Parse(c.TextValue));
                        case FieldType.Integer:
                        case FieldType.IntegerSimple:
                        case FieldType.NullInteger:
                            return Expressions.CompareIntConstant(parm,
                                       c.Field,
                                       CompType,
                                       c.TextValue);
                        case FieldType.Date:
                        case FieldType.DateSimple:
                            if (c.Field == "DeceasedDate")
                                c.SetIncludeDeceased();
                            return Expressions.CompareDateConstant(parm,
                                       c.Field,
                                       CompType,
                                       c.DateValue);
                        default:
                            throw new ArgumentException();
                    }
            }
        }
        public static CompareType Convert(string type)
        {
            if (type == null)
                return CompareType.Equal;
            return (CompareType)Enum.Parse(typeof(CompareType), type);
        }
        public static List<CompareClass> Comparisons
        {
            get
            {
                var _Comparisons = (List<CompareClass>)HttpRuntime.Cache["comparisons"];
                if (_Comparisons == null)
                {
                    var xdoc = XDocument.Parse(Properties.Resources.CompareMap);
                    var q = from f in xdoc.Descendants("FieldType")
                            from c in f.Elements("Comparison")
                            select new CompareClass
                            {
                                FieldType = FieldClass.Convert((string)f.Attribute("Name")),
                                CompType = Convert((string)c.Attribute("Type")),
                                Display = (string)c.Attribute("Display")
                            };
                    _Comparisons = q.ToList();
					HttpRuntime.Cache.Insert("comparisons", _Comparisons, null,
						DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
                }
                return _Comparisons;
            }
        }
    }
}
