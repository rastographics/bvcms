SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS, NOCOUNT ON
GO
SET DATEFORMAT YMD
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
-- Pointer used for text / image updates. This might not be needed, but is declared here just in case
DECLARE @pv binary(16)
ALTER TABLE [dbo].[Volunteer] DROP CONSTRAINT [StatusMvrId__StatusMvr]
ALTER TABLE [dbo].[Volunteer] DROP CONSTRAINT [FK_Volunteer_PEOPLE_TBL]
ALTER TABLE [dbo].[Volunteer] DROP CONSTRAINT [FK_Volunteer_VolApplicationStatus]
ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_Users_People]
ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [FK_UserRole_Roles]
ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [FK_UserRole_Users]
ALTER TABLE [dbo].[RecReg] DROP CONSTRAINT [FK_RecReg_People]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_BaptismStatus]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_BaptismType]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [BFMembers__BFClass]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_Campus]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [StmtPeople__ContributionStatementOption]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_DecisionType]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_DropType]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_EntryPoint]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [EnvPeople__EnvelopeOption]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_Families]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_Gender]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_InterestPoint]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_JoinType]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_MemberLetterStatus]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_MaritalStatus]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_MemberStatus]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_DiscoveryClassStatus]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_Origin]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_PEOPLE_TBL_Picture]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_FamilyPosition]
ALTER TABLE [dbo].[People] DROP CONSTRAINT [ResCodePeople__ResidentCode]
ALTER TABLE [dbo].[Families] DROP CONSTRAINT [FamiliesHeaded__HeadOfHousehold]
ALTER TABLE [dbo].[Families] DROP CONSTRAINT [FamiliesHeaded2__HeadOfHouseholdSpouse]
ALTER TABLE [dbo].[Families] DROP CONSTRAINT [FK_Families_Picture]
ALTER TABLE [dbo].[Families] DROP CONSTRAINT [ResCodeFamilies__ResidentCode]
ALTER TABLE [dbo].[OrgSchedule] DROP CONSTRAINT [FK_OrgSchedule_Organizations]
ALTER TABLE [dbo].[DivOrg] DROP CONSTRAINT [FK_DivOrg_Division]
ALTER TABLE [dbo].[DivOrg] DROP CONSTRAINT [FK_DivOrg_Organizations]
ALTER TABLE [dbo].[ProgDiv] DROP CONSTRAINT [FK_ProgDiv_Division]
ALTER TABLE [dbo].[ProgDiv] DROP CONSTRAINT [FK_ProgDiv_Program]
ALTER TABLE [dbo].[Organizations] DROP CONSTRAINT [FK_Organizations_Campus]
ALTER TABLE [dbo].[Organizations] DROP CONSTRAINT [FK_Organizations_Division]
ALTER TABLE [dbo].[Organizations] DROP CONSTRAINT [FK_ORGANIZATIONS_TBL_EntryPoint]
ALTER TABLE [dbo].[Organizations] DROP CONSTRAINT [FK_Organizations_Gender]
ALTER TABLE [dbo].[Organizations] DROP CONSTRAINT [FK_ORGANIZATIONS_TBL_OrganizationStatus]
ALTER TABLE [dbo].[Organizations] DROP CONSTRAINT [FK_Organizations_OrganizationType]
ALTER TABLE [dbo].[Organizations] DROP CONSTRAINT [ChildOrgs__ParentOrg]
ALTER TABLE [dbo].[Attend] DROP CONSTRAINT [FK_AttendWithAbsents_TBL_ORGANIZATIONS_TBL]
ALTER TABLE [dbo].[Coupons] DROP CONSTRAINT [FK_Coupons_Organizations]
ALTER TABLE [dbo].[EnrollmentTransaction] DROP CONSTRAINT [ENROLLMENT_TRANSACTION_ORG_FK]
ALTER TABLE [dbo].[GoerSenderAmounts] DROP CONSTRAINT [FK_GoerSenderAmounts_Organizations]
ALTER TABLE [dbo].[Meetings] DROP CONSTRAINT [FK_MEETINGS_TBL_ORGANIZATIONS_TBL]
ALTER TABLE [dbo].[MemberTags] DROP CONSTRAINT [FK_MemberTags_Organizations]
ALTER TABLE [dbo].[OrganizationExtra] DROP CONSTRAINT [FK_OrganizationExtra_Organizations]
ALTER TABLE [dbo].[OrganizationMembers] DROP CONSTRAINT [ORGANIZATION_MEMBERS_ORG_FK]
ALTER TABLE [lookup].[MemberType] DROP CONSTRAINT [FK_MemberType_AttendType]
ALTER TABLE [dbo].[Attend] DROP CONSTRAINT [FK_Attend_MemberType]
ALTER TABLE [dbo].[EnrollmentTransaction] DROP CONSTRAINT [FK_ENROLLMENT_TRANSACTION_TBL_MemberType]
ALTER TABLE [dbo].[OrganizationMembers] DROP CONSTRAINT [FK_ORGANIZATION_MEMBERS_TBL_MemberType]
ALTER TABLE [dbo].[Division] DROP CONSTRAINT [FK_Division_Program]
ALTER TABLE [dbo].[Coupons] DROP CONSTRAINT [FK_Coupons_Division]
ALTER TABLE [dbo].[Promotion] DROP CONSTRAINT [FromPromotions__FromDivision]
ALTER TABLE [dbo].[Promotion] DROP CONSTRAINT [ToPromotions__ToDivision]
ALTER TABLE [dbo].[ChangeDetails] DROP CONSTRAINT [FK_ChangeDetails_ChangeLog]
ALTER TABLE [dbo].[VoluteerApprovalIds] DROP CONSTRAINT [FK_VoluteerApprovalIds_VolunteerCodes]
ALTER TABLE [dbo].[Task] DROP CONSTRAINT [FK_Task_TaskStatus]
ALTER TABLE [dbo].[Zips] DROP CONSTRAINT [FK_Zips_ResidentCode]
ALTER TABLE [dbo].[Contribution] DROP CONSTRAINT [FK_Contribution_ContributionType]
ALTER TABLE [dbo].[Contribution] DROP CONSTRAINT [FK_Contribution_ContributionStatus]
ALTER TABLE [dbo].[Contact] DROP CONSTRAINT [FK_Contacts_ContactTypes]
ALTER TABLE [dbo].[Contact] DROP CONSTRAINT [FK_NewContacts_ContactReasons]
ALTER TABLE [dbo].[BundleHeader] DROP CONSTRAINT [FK_BUNDLE_HEADER_TBL_BundleStatusTypes]
ALTER TABLE [dbo].[BundleHeader] DROP CONSTRAINT [FK_BUNDLE_HEADER_TBL_BundleHeaderTypes]
ALTER TABLE [dbo].[Attend] DROP CONSTRAINT [FK_AttendWithAbsents_TBL_AttendType]
ALTER TABLE [dbo].[Meetings] DROP CONSTRAINT [FK_Meetings_AttendCredit]
ALTER TABLE [dbo].[OrganizationMembers] DROP CONSTRAINT [FK_OrganizationMembers_RegistrationData]
ALTER TABLE [dbo].[Contact] DROP CONSTRAINT [FK_Contacts_Ministries]
ALTER TABLE [dbo].[Contribution] DROP CONSTRAINT [FK_Contribution_ExtraData]
ALTER TABLE [dbo].[BundleHeader] DROP CONSTRAINT [BundleHeaders__Fund]
ALTER TABLE [dbo].[Contribution] DROP CONSTRAINT [FK_Contribution_ContributionFund]
ALTER TABLE [dbo].[RecurringAmounts] DROP CONSTRAINT [FK_RecurringAmounts_ContributionFund]
ALTER TABLE [dbo].[ContentKeyWords] DROP CONSTRAINT [FK_ContentKeyWords_Content]
SET IDENTITY_INSERT [dbo].[BackgroundCheckMVRCodes] ON
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (2, N'AL', N'Alabama', N'AL')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (3, N'AK', N'Alaska', N'AK')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (7, N'AZ39M', N'Arizona (39 month)', N'AZ')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (8, N'ARD', N'Arkansas (Driver Check)', N'AR')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (9, N'CA', N'California', N'CA')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (11, N'CO', N'Colorado', N'CO')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (12, N'CT', N'Connecticut', N'CT')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (13, N'DE', N'Delaware', N'DE')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (14, N'DC', N'District of Columbia', N'DC')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (15, N'FL3', N'Florida (3 Year)', N'FL')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (17, N'GA3Y', N'Georgia (3 Year)', N'GA')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (18, N'HI', N'Hawaii', N'HI')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (19, N'ID', N'Idaho', N'ID')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (20, N'IL', N'Illinois', N'IL')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (21, N'IN', N'Indiana', N'IN')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (22, N'IA', N'Iowa', N'IA')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (23, N'KS', N'Kansas', N'KS')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (24, N'KY', N'Kentucky', N'KY')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (25, N'LA', N'Louisiana', N'LA')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (26, N'ME', N'Maine', N'ME')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (27, N'MD', N'Maryland', N'MD')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (29, N'MA', N'Massachusetts', N'MA')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (30, N'MI', N'Michigan', N'MI')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (31, N'MNC', N'Minnesota (Complete)', N'MN')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (32, N'MS', N'Mississippi', N'MS')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (33, N'MOC', N'Missouri (Complete)', N'MO')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (34, N'MT', N'Montana', N'MT')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (35, N'NE', N'Nebraska', N'NE')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (36, N'NV', N'Nevada', N'NV')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (37, N'NH', N'New Hampshire', N'NH')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (38, N'NJ', N'New Jersey', N'NJ')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (39, N'NM', N'New Mexico', N'NM')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (40, N'NY', N'New York', N'NY')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (41, N'NC3Y', N'North Carolina (3 Year)', N'NC')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (42, N'ND', N'North Dakota', N'ND')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (43, N'OH', N'Ohio', N'OH')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (45, N'OK', N'Oklahoma', N'OK')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (46, N'OR', N'Oregon', N'OR')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (47, N'PA3Y', N'Pennsylvania (3 Year Insurance)', N'PA')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (48, N'RI', N'Rhode Island', N'RI')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (50, N'SC3Y', N'South Carolina (3 Year)', N'SC')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (51, N'SD', N'South Dakota', N'SD')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (52, N'TN3Y', N'Tennessee (3 Year)', N'TN')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (53, N'TX3', N'Texas (3 Year)', N'TX')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (54, N'UT', N'Utah', N'UT')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (55, N'VT', N'Vermont', N'VT')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (56, N'VA', N'Virginia', N'VA')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (57, N'WA', N'Washington', N'WA')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (58, N'WV', N'West Virginia', N'WV')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (59, N'WI', N'Wisconsin', N'WI')
INSERT INTO [dbo].[BackgroundCheckMVRCodes] ([ID], [Code], [Description], [StateAbbr]) VALUES (60, N'WY', N'Wyoming', N'WY')
SET IDENTITY_INSERT [dbo].[BackgroundCheckMVRCodes] OFF
SET IDENTITY_INSERT [dbo].[ChangeLog] ON
INSERT INTO [dbo].[ChangeLog] ([Id], [PeopleId], [FamilyId], [UserPeopleId], [Created], [Field], [Data], [Before], [After]) VALUES (88, 3, NULL, 3, '2015-10-13 07:48:28.763', N'Basic Info', NULL, NULL, NULL)
INSERT INTO [dbo].[ChangeLog] ([Id], [PeopleId], [FamilyId], [UserPeopleId], [Created], [Field], [Data], [Before], [After]) VALUES (89, 2, NULL, 3, '2015-10-13 07:49:02.467', N'Basic Info', NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[ChangeLog] OFF
SET IDENTITY_INSERT [dbo].[Content] ON
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (1, N'Header', N'Header', N'<h1>
	{churchname}</h1>
<h2>
	<em>{byline}</em></h2>
', '2009-04-17 20:42:09.213', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (2, N'ShellDefault', N'Default Online Reg Shell', N'<html>
<head>
	<title>Online Registration</title>
<!--Do not delete the following comment-->
    <!--FORM CSS-->
</head>
<body>
<!--Do not delete the following comment-->
<!--FORM START-->
<p>Form goes here</p>
<!--Do not delete the following comment-->
<!--FORM END-->
</body>
</html>
', '2013-09-09 23:32:14.270', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (3, N'ShellIFrame', N'Plain Online Reg Shell for iFrame use', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"> 
<html> 
<head> 
<title>Online Registration</title> 
 
<meta http-equiv=''Content-Type'' content=''text/html; charset=iso-8859-1'' /> 
<link href=''/Content/Site2.css?v=4'' rel=''stylesheet'' type=''text/css'' /> 
 
</head> 
<body> 
<!--FORM START-->
    <h2>Registration Form</h2> 
    
    <form class="DisplayEdit" action="/OnlineReg/CompleteRegistration/" method="post"> 
    <input id="m_divid" name="m.divid" type="hidden" value="" /> 
<input id="m_orgid" name="m.orgid" type="hidden" value="114" /> 
<input id="m_testing" name="m.testing" type="hidden" value="" /> 
<table cellpadding="0" cellspacing="2" width="100%"> 
 
<tr><td class="alt0"> 
<input id="m_List_index" name="m.List.index" type="hidden" value="0" /> 
<input name="m.List[0].orgid" type="hidden" value="114"></input> 
<input name="m.List[0].divid" type="hidden" value=""></input> 
<input name="m.List[0].ShowAddress" type="hidden" value="False"></input> 
 
<table cellspacing="6"> 
 
    <tr> 
        <td><label for="first">First Name</label></td> 
        <td><input type="text" name="m.List[0].first" value="" /> 
        </td> 
        <td> </td> 
    </tr> 
    <tr> 
        <td><label for="last">Last Name</label></td> 
        <td><input type="text" name="m.List[0].last" value="" /></td> 
        <td>suffix:<input type="text" name="m.List[0].suffix" class="short" value="" /> 
        </td> 
    </tr> 
     <tr> 
        <td><label for="dob">Date of Birth</label></td> 
        <td><input type="text" name="m.List[0].dob" value="" class="dob" title="m/d/y, mmddyy, mmddyyyy" /></td> 
        <td><span id="age">2009</span> (m/d/y) </td> 
    </tr> 
    <tr> 
        <td><label for="phone">Phone</label></td> 
        <td><input type="text" name="m.List[0].phone" value="" /></td> 
        <td><input type="radio" name="m.List[0].homecell" value="h"  /> Home<br /> 
        <input type="radio" name="m.List[0].homecell" value="c"  /> Cell
        </td> 
    </tr> 
    <tr> 
        <td><label for="email">Contact Email</label></td> 
        <td><input type="text" name="m.List[0].email" value="" /></td> 
        <td></td> 
    </tr> 
    
    <tr><td></td> 
        <td> 
        
            <a href="/OnlineReg/PersonFind/0" class="submitbutton">Find Record</a> 
        
        </td> 
    </tr> 
    
</table> 
 
</td></tr> 
 
</table> 
    </form> 
<!--FORM END-->
 
</body> 
</html>', '2013-09-09 23:32:14.270', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (4, N'StatementHeader', N'Contribution Statement Header', N'<h1>Sample Church</h1>
<h2>105 Highway 151 | Ventura, TN 34773 | 615.232.3432</h2>', '2013-09-09 23:32:14.270', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (5, N'StatementNotice', N'Contribution Statement Notice', N'<p><i>NOTE: No goods or services were provided to you by the church in connection with any contribution; any value received consisted entirely of intangible religious benefits.&nbsp;</i></p>

<p><i>Thank you for your faithfulness in the giving of your time, talents, and resources. Together we can share the love of Jesus with our city </i></p>
', '2013-09-09 23:32:14.270', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (6, N'TermsOfUse', N'Terms Of Use', N'
<div style="width: 300px">
<p><span style="font-size: medium">Access to this site is given by special pemission only.</span></p>
<p>This web site has a starter database.</p>
<p>The source code is licensed under the GPL (see <a href="http://bvcms.codeplex.com/license">license</a>)</p>
</div>
', '2013-09-09 23:32:14.270', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
EXEC(N'INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (11, N''MemberProfileAutomation'', N''MemberProfileAutomation'', N'' 
# this is an IronPython script for MembershipAutomation in BVCMS
# the variable p has been passed in and is the person that we are saving Member Profile information for

#import useful constants (defined in constants.py)
from constants import *


# define all functions first, codes starts running below functions

# do not allow empty join date
def SetJoinDate(p, name, dt):
    if dt == None:
        p.errorReturn = "need a " + name + " date"
    p.JoinDate = dt

# this controls the membership status, makes them a member if they have completed the two requirements
def CheckJoinStatus(p):

    if p.DecisionTypeId == DecisionCode.ProfessionForMembership:
        if p.DiscClassStatusCompletedCodes.Contains(p.NewMemberClassStatusId) and         p.BaptismStatusId == BaptismStatusCode.Completed:
            if p.NewMemberClassDate != None and p.BaptismDate != None:
                if p.NewMemberClassDate > p.BaptismDate:
                    SetJoinDate(p, "NewMemberClass", p.NewMemberClassDate)
                else:
                    SetJoinDate(p, "Baptism", p.BaptismDate)
            p.MemberStatusId = MemberStatusCode.Member
            if p.BaptismTypeId == BaptismTypeCode.Biological:
                p.JoinCodeId = JoinTypeCode.BaptismBIO
            else:
                p.JoinCodeId = JoinTypeCode.BaptismPOF

    elif p.DecisionTypeId == DecisionCode.Letter:
        if p.NewMemberClassStatusIdChanged:
            if p.DiscClassStatusCompletedCodes.Contains(p.NewMemberClassStatusId)             or p.NewMemberClassStatusId == NewMemberClassStatusCode.AdminApproval:
                p.MemberStatusId = MemberStatusCode.Member
                p.JoinCodeId = JoinTypeCode.Letter
                if p.NewMemberClassDate != None:
                    SetJoinDate(p, "NewMember", p.NewMemberClassDate)
                else:
                    SetJoinDate(p, "Decision", p.DecisionDate)

    elif p.DecisionTypeId == DecisionCode.Statement:
        if p.NewMemberClassStatusIdChanged:
            if p.DiscClassStatusCompletedCodes.Contains(p.NewMemberClassStatusId):
                p.MemberStatusId = MemberStatusCode.Member
                p.JoinCodeId = JoinTypeCode.Statement
                if p.NewMemberClassDate != None:
                    SetJoinDate(p, "NewMember", p.NewMemberClassDate)
                else:
                    SetJoinDate(p, "Decision", p.DecisionDate)

    elif p.DecisionTypeId == DecisionCode.StatementReqBaptism:
        if p.DiscClassStatusCompletedCodes.Contains(p.NewMemberClassStatusId)         and p.BaptismStatusId == BaptismStatusCode.Completed:
            p.MemberStatusId = MemberStatusCode.Member
            p.JoinCodeId = JoinTypeCode.BaptismSRB
            if p.NewMemberClassDate != None:
                 if p.NewMemberClassDate > p.BaptismDate:
                    SetJoinDate(p, "NewMember", p.NewMemberClassDate)
                 else: 
                    SetJoinDate(p, "Baptism", p.BaptismDate)
            else:
                 SetJoinDate(p, "Baptism", p.BaptismDate)

# this moves the membership process along, setting various codes based on decision
def CheckDecisionStatus(p):

    if p.DecisionTypeId == DecisionCode.ProfessionForMembership:
        p.MemberStatusId = MemberStatusCode.Pending
        if p.NewMemberClassStatusId != NewMemberClassStatusCode.Attended:
            p.NewMemberClassStatusId = NewMemberClassStatusCode.Pending
        if p.Age <= 12 and p.FamilyHasPrimaryMemberForMoreThanDays(365):
            p.BaptismTypeId = BaptismTypeCode.Biological
        else:
            p.BaptismTypeId = BaptismTypeCode.Original
        BaptismStatusId = BaptismStatusCode.NotScheduled

    elif p.DecisionTypeId == DecisionCode.ProfessionNotForMembership:
        p.MemberStatusId = MemberStatusCode.NotMember
        if p.NewMemberClassStatusId != NewMemberClassStatusCode.Attended:
            NewMemberClassStatusId = New'', ''2013-09-09 23:32:14.270'', 1, 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)')
EXEC(N'UPDATE [dbo].[Content] SET [Body].WRITE(N''MemberClassStatusCode.NotSpecified
        if p.BaptismStatusId != BaptismStatusCode.Completed:
            p.BaptismTypeId = BaptismTypeCode.NonMember
            p.BaptismStatusId = BaptismStatusCode.NotScheduled

    elif p.DecisionTypeId == DecisionCode.Letter:
        p.MemberStatusId = MemberStatusCode.Pending
        if p.NewMemberClassStatusId != NewMemberClassStatusCode.Attended:
            p.NewMemberClassStatusId = NewMemberClassStatusCode.Pending
            if p.BaptismStatusId != BaptismStatusCode.Completed:
                p.BaptismTypeId = BaptismTypeCode.NotSpecified
                p.BaptismStatusId = BaptismStatusCode.NotSpecified

    elif p.DecisionTypeId == DecisionCode.Statement:
        p.MemberStatusId = MemberStatusCode.Pending
        if p.NewMemberClassStatusId != NewMemberClassStatusCode.Attended:
            p.NewMemberClassStatusId = NewMemberClassStatusCode.Pending
            if p.BaptismStatusId != BaptismStatusCode.Completed:
                p.BaptismTypeId = BaptismTypeCode.NotSpecified
                p.BaptismStatusId = BaptismStatusCode.NotSpecified

    elif p.DecisionTypeId == DecisionCode.StatementReqBaptism:
        p.MemberStatusId = MemberStatusCode.Pending
        if p.NewMemberClassStatusId != NewMemberClassStatusCode.Attended:
            p.NewMemberClassStatusId = NewMemberClassStatusCode.Pending
        if p.BaptismStatusId != BaptismStatusCode.Completed:
            p.BaptismTypeId = BaptismTypeCode.Required
            p.BaptismStatusId = BaptismStatusCode.NotScheduled

    elif p.DecisionTypeId == DecisionCode.Cancelled:
        p.MemberStatusId = MemberStatusCode.NotMember
        if p.NewMemberClassStatusId != NewMemberClassStatusCode.Attended:
            NewMemberClassStatusId = NewMemberClassStatusCode.NotSpecified
        if p.BaptismStatusId != BaptismStatusCode.Completed:
            if p.BaptismStatusId != BaptismStatusCode.Completed:
                p.BaptismTypeId = BaptismTypeCode.NotSpecified
                p.BaptismStatusId = BaptismStatusCode.Canceled
        p.EnvelopeOptionsId = EnvelopeOptionCode.NoEnvelope

# cleanup for deceased and for dropped memberships
def DropMembership(p, Db):

    if p.MemberStatusId == MemberStatusCode.Member:
        if p.Deceased:
            p.DropCodeId = DropTypeCode.Deceased
        p.MemberStatusId = MemberStatusCode.Previous
        p.DropDate = p.Now().Date

    if p.Deceased:
        p.EmailAddress = None
        p.DoNotCallFlag = True
        p.DoNotMailFlag = True
        p.DoNotVisitFlag = True

    if p.SpouseId != None:
        spouse = Db.LoadPersonById(p.SpouseId)

        if p.Deceased:
            spouse.MaritalStatusId = MaritalStatusCode.Widowed
            if spouse.EnvelopeOptionsId != None: # not null
                if spouse.EnvelopeOptionsId != EnvelopeOptionCode.NoEnvelope:
                    spouse.EnvelopeOptionsId = EnvelopeOptionCode.Individual
            spouse.ContributionOptionsId = EnvelopeOptionCode.Individual

        if spouse.MemberStatusId == MemberStatusCode.Member:
            if spouse.EnvelopeOptionsId == EnvelopeOptionCode.Joint:
                spouse.EnvelopeOptionsId = EnvelopeOptionCode.Individual

    p.EnvelopeOptionsId = EnvelopeOptionCode.NoEnvelope
    p.DropAllMemberships(Db)

#-------------------------------------
# Main Function
class MembershipAutomation(object):
    def __init__(self):
        pass
    def Run(self, Db, p):
        p.errorReturn = "ok"
        if p.DecisionTypeIdChanged:
            CheckDecisionStatus(p)

        if (p.NewMemberClassStatusId == NewMemberClassStatusCode.AdminApproval         or p.NewMemberClassStatusId == NewMemberClassStatusCode.Attended         or p.NewMemberClassStatusId == NewMemberClassStatusCode.GrandFathered         or p.NewMemberClassStatusId == NewMemberClassStatusCode.ExemptedChild)         and p.NewMemberClassDate == None:
            p.errorR'',NULL,NULL) WHERE [Id]=11
UPDATE [dbo].[Content] SET [Body].WRITE(N''eturn = "need a NewMemberClass date"

        if (p.DecisionTypeId == DecisionCode.Letter         or p.DecisionTypeId == DecisionCode.Statement         or p.DecisionTypeId == DecisionCode.ProfessionForMembership         or p.DecisionTypeId == DecisionCode.ProfessionNotForMembership         or p.DecisionTypeId == DecisionCode.StatementReqBaptism)         and p.DecisionDate == None:
            p.errorReturn = "need a Decision date"

        if p.NewMemberClassStatusIdChanged or p.BaptismStatusIdChanged:
            CheckJoinStatus(p)

        if p.DeceasedDateChanged:
            if p.DeceasedDate != None:
                DropMembership(p, Db)

        # when people leave the church, lots of cleanup to do
        if p.DropCodeIdChanged:
            if p.DropCodesThatDrop.Contains(p.DropCodeId):
                DropMembership(p, Db)

        # this does new member class completed
        if p.NewMemberClassStatusIdChanged         and p.NewMemberClassStatusId == NewMemberClassStatusCode.Attended:
            om = Db.LoadOrgMember(p.PeopleId, "Step 1", False)
            if om != None:
                om.Drop(True) # drops and records drop in history
'',NULL,NULL) WHERE [Id]=11
')
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (63, N'OneTimeConfirmation', N'Your link to manage your subscriptions', N'<html>
<head>
	<title></title>
</head>
<body>
<p>Hi {name},</p>

<p>Here is your <a href="{url}">link</a> to manage your subscriptions.</p>

<p>Note: it will only work once for security reasons.</p>

<p>Thank you.</p>
</body>
</html>
', '2011-10-18 20:12:42.000', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (64, N'OneTimeConfirmationPledge', N'Your link to make a Pledge', N'<html>
<head>
	<title></title>
</head>
<body>
<p>Hi {name},</p>

<p>Here is your <a href="{url}">link</a> to manage your pledge.</p>

<p>Note: it will only work once for security reasons.</p>

<p>Thank you.</p>
</body>
</html>
', '2011-10-18 20:12:42.000', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (65, N'DiffEmailMessage', N'Different Email Message', N'<title></title>
<p>Hi {name},</p>

<p>You registered for {org} using a different email address than the one we have on record. It is important that you call the church <strong>{phone}</strong> to update our records so that you will receive future important notices regarding this registration.</p>
', '2011-10-18 20:12:42.000', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (66, N'NoEmailMessage', N'NoEmailMessage', N'<p>
	Hi {name},</p>
<p>
	You registered for {org}, and we found your record, but there was no email address on your existing record in our database. If you would like for us to update your record with this email address or another, Please contact the church at <strong>{phone}</strong> to let us know. It is important that we have your email address so that you will receive future important notices regarding this registration. But we won&#39;t add that to your record without your permission. Thank you</p>
', '2011-10-18 20:12:42.000', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (67, N'NewUserWelcome', N'New User Welcome', N'<html>
<head>
	<title></title>
</head>
<body>&nbsp;</body>
</html>
<title></title>
<p>Hi {name},</p>

<p>You now have a new user account on our church&#39;s Church Management System (TouchPoint Software).</p>

<p>Your username is {username}</p>

<p>Click this link to create your password: {link}</p>

<p>NOTE: If these do not appear as links, copy and paste them into your browser.</p>

<p>You can access the site anytime using this link: {cmshost}</p>

<p>Welcome,</p>

<p>The TouchPoint Team<br />
<br />
&nbsp;</p>
', '2013-09-09 23:32:14.270', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (69, N'Empty Template', N'Empty Template', N'<html>
<body>
<div bvedit style="max-width:600px;">Click here to edit content</div>
</body>
</html>', '2012-06-14 19:18:11.000', 1, 2, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
EXEC(N'INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (70, N''Basic Newsletter Template'', N''Basic Newsletter Template'', N''<html>
	<head>
		<meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
		<title>Our Newsletter</title>
		<style type="text/css">
#outlook a{padding:0;}
			body{width:100% !important;}
			body{-webkit-text-size-adjust:none;}
			body{margin:0; padding:0;}
			img{border:none; font-size:14px; font-weight:bold; height:auto; line-height:100%; outline:none; text-decoration:none; text-transform:capitalize;}
			#backgroundTable{height:100% !important; margin:0; padding:0; width:100% !important;}
			body, #backgroundTable{
				background-color:#FAFAFA;
			}
			h1, .h1{
				color:#202020;
				display:block;
				font-family:Arial;
				font-size:34px;
				font-weight:bold;
				line-height:100%;
				margin-top:0 !important;
				margin-right:0 !important;
				margin-bottom:10px !important;
				margin-left:0 !important;
				text-align:left;
			}
			h2, .h2{
				color:#202020;
				display:block;
				font-family:Arial;
				font-size:30px;
				font-weight:bold;
				line-height:100%;
				margin-top:0 !important;
				margin-right:0 !important;
				margin-bottom:10px !important;
				margin-left:0 !important;
				text-align:left;
			}
			h3, .h3{
				color:#202020;
				display:block;
				font-family:Arial;
				font-size:26px;
				font-weight:bold;
				line-height:100%;
				margin-top:0 !important;
				margin-right:0 !important;
				margin-bottom:10px !important;
				margin-left:0 !important;
				text-align:left;
			}
			h4, .h4{
				color:#202020;
				display:block;
				font-family:Arial;
				font-size:22px;
				font-weight:bold;
				line-height:100%;
				margin-top:0 !important;
				margin-right:0 !important;
				margin-bottom:10px !important;
				margin-left:0 !important;
				text-align:left;
			}
			#templatePreheader{
				background-color:#FAFAFA;
			}
			.preheaderContent div{
				color:#505050;
				font-family:Arial;
				font-size:10px;
				line-height:100%;
				text-align:left;
			}
			.preheaderContent div a:link, .preheaderContent div a:visited{
				color:#336699;
				font-weight:normal;
				text-decoration:underline;
			}
			#templateHeader{
				background-color:#D8E2EA;
				border-bottom:0;
			}
			.headerContent{
				color:#202020;
				font-family:Arial;
				font-size:34px;
				font-weight:bold;
				line-height:100%;
				padding:0;
				text-align:center;
				vertical-align:middle;
			}

			.headerContent a:link, .headerContent a:visited{
				color:#336699;
				font-weight:normal;
				text-decoration:underline;
			}

			#headerImage{
				height:auto;
				max-width:600px !important;
			}
			#templateContainer, .bodyContent{
				background-color:#FDFDFD;
			}
			.bodyContent div{
				color:#505050;
				font-family:Arial;
				font-size:14px;
				line-height:150%;
				text-align:left;
			}
			.bodyContent div a:link, .bodyContent div a:visited{
				color:#336699;
				font-weight:normal;
				text-decoration:underline;
			}
			.bodyContent img, .fullWidthBandContent img{
				display:inline;
				height:auto;
			}
			#templateSidebar{
				background-color:#FFFFFF;
			}
			.sidebarContent div{
				color:#505050;
				font-family:Arial;
				font-size:12px;
				line-height:150%;
				text-align:left;
			}
			.sidebarContent div a:link, .sidebarContent div a:visited{
				color:#336699;
				font-weight:normal;
				text-decoration:underline;
			}
			.sidebarContent img{
				display:inline;
				height:auto;
			}
			.leftColumnContent{
				background-color:#FFFFFF;
			}
			.leftColumnContent div{
				color:#505050;
				font-family:Arial;
				font-size:14px;
				line-height:150%;
				text-align:left;
			}
			.leftColumnContent div a:link, .leftColumnContent div a:visited{
				color:#336699;
				font-weight:normal;
				text-decoration:underline;
			}
			.leftColumnContent img{
				display:inline;
				height:auto;
			}
			.rightColumnContent{
				background-color:#FFFFFF;
			}
			'', ''2013-09-09 23:32:14.270'', NULL, 2, 14, 20, 0, NULL, NULL, NULL, NULL, NULL)')
EXEC(N'UPDATE [dbo].[Content] SET [Body].WRITE(N''.rightColumnContent div{
				color:#505050;
				font-family:Arial;
				font-size:14px;
				line-height:150%;
				text-align:left;
			}
			.rightColumnContent div a:link, .rightColumnContent div a:visited{
				color:#336699;
				font-weight:normal;
				text-decoration:underline;
			}
			.rightColumnContent img{
				display:inline;
				height:auto;
			}
			#templateFooter{
				background-color:#FDFDFD;
				border-top:0;
			}
			.footerContent div{
				color:#707070;
				font-family:Arial;
				font-size:12px;
				line-height:125%;
				text-align:left;
			}
			.footerContent div a:link, .footerContent div a:visited{
				color:#336699;
				font-weight:normal;
				text-decoration:underline;
			}
			.footerContent img{
				display:inline;
			}
			#social{
				background-color:#FAFAFA;
				border:0;
			}
			#social div{
				text-align:center;
			}
			#utility{
				background-color:#FDFDFD;
				border:0;
			}
			#utility div{
				text-align:center;
			}		</style>
	</head>
	<body leftmargin="0" marginheight="0" marginwidth="0" offset="0" topmargin="0">
		<center>
			<table border="0" cellpadding="0" cellspacing="0" height="100%" id="backgroundTable" width="100%">
				<tbody>
					<tr>
						<td align="center" valign="top">
							<table border="0" cellpadding="10" cellspacing="0" id="templatePreheader" width="600">
								<tbody>
									<tr>
										<td class="preheaderContent" valign="top">
											<table border="0" cellpadding="10" cellspacing="0" width="100%">
												<tbody>
													<tr>
														<td valign="top">
															<div bvedit="">
																Use this area to offer a short teaser of your email&#39;&#39;s content. Text here will show in the preview area of some email clients and in Facebook news feed posts.</div>
														</td>
														<td valign="top" width="190">
															<div>
																Is this email not displaying correctly?<br />
																<a href="{emailhref}" target="_blank">View it in your browser</a>.</div>
														</td>
													</tr>
												</tbody>
											</table>
										</td>
									</tr>
								</tbody>
							</table>
							<table border="0" cellpadding="0" cellspacing="0" id="templateContainer" width="600">
								<tbody>
									<tr>
										<td align="center" valign="top">
											<table border="0" cellpadding="0" cellspacing="0" id="templateHeader" width="600">
												<tbody>
													<tr>
														<td class="headerContent">
															<img src="http://www.bvcms.com/content/images/placeholder_600.gif" style="max-width:160px;" /></td>
													</tr>
												</tbody>
											</table>
										</td>
									</tr>
									<tr>
										<td align="center" valign="top">
											<table border="0" cellpadding="0" cellspacing="0" id="templateBody" width="600">
												<tbody>
													<tr>
														<td id="templateSidebar" valign="top" width="200">
															<table border="0" cellpadding="0" cellspacing="0" width="200">
																<tbody>
																	<tr>
																		<td class="sidebarContent" valign="top">
																			<table border="0" cellpadding="0" cellspacing="0" style="padding-top:10px; padding-left:20px;" width="100%">
																				<tbody>
																					<tr>
																						<td valign="top">
																							<table border="0" cellpadding="0" cellspacing="4">
																								<tbody>
																									<tr>
																										<td align="left" valign="middle" width="16">
																											<img src="http://www.bvcms.com/content/images/sfs_icon_facebook.png" /></td>
																										<td align="left" valign="top">
																											<div>
																												<a href="http://facebook.com/yourpage">Friend on Facebook</a></div>
																										</td'',NULL,NULL) WHERE [Id]=70
UPDATE [dbo].[Content] SET [Body].WRITE(N''>
																									</tr>
																									<tr>
																										<td align="left" valign="middle" width="16">
																											<img src="http://www.bvcms.com/content/images/sfs_icon_twitter.png" style="margin:0 !important;" /></td>
																										<td align="left" valign="top">
																											<div>
																												<a href="http://www.twitter.com/yourname">Follow on Twitter</a></div>
																										</td>
																									</tr>
																								</tbody>
																							</table>
																						</td>
																					</tr>
																				</tbody>
																			</table>
																			<table border="0" cellpadding="20" cellspacing="0" width="100%">
																				<tbody>
																					<tr bvrepeat="">
																						<td valign="top">
																							<img src="http://www.bvcms.com/content/images/placeholder_160.gif" style="max-width:160px;" />
																							<div bvedit="">
																								<h4>
																									Heading 4</h4>
																								Sections in the side bar are shown here.</div>
																						</td>
																					</tr>
																				</tbody>
																			</table>
																		</td>
																	</tr>
																</tbody>
															</table>
														</td>
														<td valign="top" width="400">
															<table border="0" cellpadding="0" cellspacing="0" width="400">
																<tbody>
																	<tr>
																		<td valign="top">
																			<table border="0" cellpadding="0" cellspacing="0" width="400">
																				<tbody>
																					<tr>
																						<td class="bodyContent" valign="top">
																							<table border="0" cellpadding="20" cellspacing="0" width="100%">
																								<tbody>
																									<tr>
																										<td valign="top">
																											<div bvedit="">
																												<h1>
																													Heading 1</h1>
																												<h2>
																													Heading 2</h2>
																												<h3>
																													Heading 3</h3>
																												<h4>
																													Heading 4</h4>
																												<strong>Getting started:</strong> Customize your template by clicking on the style editor tabs up above. Set your fonts, colors, and styles. After setting your styling is all done you can click here in this area, delete the text, and start adding your own awesome content!<br />
																												<br />
																												After you enter your content, highlight the text you want to style and select the options you set in the style editor in the &quot;styles&quot; drop down box.</div>
																										</td>
																									</tr>
																								</tbody>
																							</table>
																						</td>
																					</tr>
																				</tbody>
																			</table>
																		</td>
																	</tr>
																	<tr>
																		<td valign="top">
																			<table border="0" cellpadding="0" cellspacing="0" width="400">
																				<tbody>
																					<tr>
																						<td class="leftColumnContent" valign="top" width="180">
																							<table border="0" cellpadding="20" cellspacing="0" width="100%">
																								<tbody>
																									<tr bvrepeat="">
																										<td valign="top">
																											<img src="http://www.bvcms.com/content/images/placeholder_160.gif" style="max-width:160px;" />
																											<div bvedit="">
																												<h4>
																													Heading 4</h4>
																					'',NULL,NULL) WHERE [Id]=70
UPDATE [dbo].[Content] SET [Body].WRITE(N''							<strong>Content blocks:</strong> Put all the great things you want to say here and format it like you want it.</div>
																										</td>
																									</tr>
																								</tbody>
																							</table>
																						</td>
																						<td class="rightColumnContent" valign="top" width="180">
																							<table border="0" cellpadding="20" cellspacing="0" width="100%">
																								<tbody>
																									<tr bvrepeat="">
																										<td valign="top">
																											<img src="http://www.bvcms.com/content/images/placeholder_160.gif" style="max-width:160px;" />
																											<div bvedit="">
																												<h4>
																													Heading 4</h4>
																												<strong>Content blocks:</strong> Put all the great things you want to say here and format it like you want it.</div>
																										</td>
																									</tr>
																								</tbody>
																							</table>
																						</td>
																					</tr>
																				</tbody>
																			</table>
																		</td>
																	</tr>
																</tbody>
															</table>
														</td>
													</tr>
												</tbody>
											</table>
										</td>
									</tr>
									<tr>
										<td align="center" valign="top">
											<table border="0" cellpadding="10" cellspacing="0" id="templateFooter" width="600">
												<tbody>
													<tr>
														<td class="footerContent" valign="top">
															<table border="0" cellpadding="10" cellspacing="0" width="100%">
																<tbody>
																	<tr>
																		<td colspan="2" id="social" valign="middle">
																			<div>
																				&nbsp;<a href="http://www.twitter.com/yourname">follow on Twitter</a> | <a href="http://www.facebook.com/yourname">friend on Facebook</a></div>
																		</td>
																	</tr>
																	<tr>
																		<td valign="top" width="350">
																			<br />
																			<div>
																				<strong>Our mailing address is:</strong><br />
																				2000 Appling Rd.<br />
																				Cordova, TN 38018</div>
																		</td>
																		<td valign="top" width="190">
																			<br />
																			<div bvedit="">
																				say some final words here</div>
																		</td>
																	</tr>
																	<tr>
																		<td colspan="2" id="utility" valign="middle">
																			<div mc:edit="std_utility">
																				&nbsp;{unsubscribe} | <a href="{emailhref}" target="_blank">view email in browser</a></div>
																		</td>
																	</tr>
																</tbody>
															</table>
														</td>
													</tr>
												</tbody>
											</table>
										</td>
									</tr>
								</tbody>
							</table>
							{track}</td>
					</tr>
				</tbody>
			</table>
		</center>
		<p>
			&nbsp;</p>
	</body>
</html>
'',NULL,NULL) WHERE [Id]=70
')
EXEC(N'INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (71, N''Basic Template'', N''Basic Template With Header'', N''<html>
	<head>
		<title></title>
		<style type="text/css">
body{margin:0; padding:0;}
			img{border:none; font-size:14px; font-weight:bold; height:auto; line-height:100%; outline:none; text-decoration:none; text-transform:capitalize;}
			#backgroundTable{height:100% !important; margin:0; padding:0; width:100% !important;}
			body, #backgroundTable{
				background-color:#FAFAFA;
			}
			#templateContainer{
				border: 1px solid #DDDDDD;
			}
			h1, .h1{
				color:#202020;
				display:block;
				font-family:Arial;
				font-size:34px;
				font-weight:bold;
				line-height:100%;
				margin-top:0;
				margin-right:0;
				margin-bottom:10px;
				margin-left:0;
				text-align:left;
			}
			h2, .h2{
				color:#202020;
				display:block;
				font-family:Arial;
				font-size:30px;
				font-weight:bold;
				line-height:100%;
				margin-top:0;
				margin-right:0;
				margin-bottom:10px;
				margin-left:0;
				text-align:left;
			}
			h3, .h3{
				color:#202020;
				display:block;
				font-family:Arial;
				font-size:26px;
				font-weight:bold;
				line-height:100%;
				margin-top:0;
				margin-right:0;
				margin-bottom:10px;
				margin-left:0;
				text-align:left;
			}
			h4, .h4{
				color:#202020;
				display:block;
				font-family:Arial;
				font-size:22px;
				font-weight:bold;
				line-height:100%;
				margin-top:0;
				margin-right:0;
				margin-bottom:10px;
				margin-left:0;
				text-align:left;
			}
			#templatePreheader{
				background-color:#FAFAFA;
			}
			.preheaderContent div{
				color:#505050;
				font-family:Arial;
				font-size:10px;
				line-height:100%;
				text-align:left;
			}
			.preheaderContent div a:link, .preheaderContent div a:visited{
				color:#336699;
				font-weight:normal;
				text-decoration:underline;
			}
			#templateHeader{
				background-color:#D8E2EA;
				border-bottom:0;
			}
			.headerContent{
				color:#202020;
				font-family:Arial;
				font-size:34px;
				font-weight:bold;
				line-height:100%;
				padding:0;
				text-align:center;
				vertical-align:middle;
			}
			.headerContent a:link, .headerContent a:visited{
				color:#336699;
				font-weight:normal;
				text-decoration:underline;
			}
			#headerImage{
				height:auto;
				max-width:600px !important;
			}
			#templateContainer, .bodyContent{
				background-color:#FDFDFD;
			}
			.bodyContent div{
				color:#505050;
				font-family:Arial;
				font-size:14px;
				line-height:150%;
				text-align:left;
			}
			.bodyContent div a:link, .bodyContent div a:visited{
				color:#336699;
				font-weight:normal;
				text-decoration:underline;
			}
			.bodyContent img{
				display:inline;
				height:auto;
			}
			#templateFooter{
				background-color:#FDFDFD;
				border-top:0;
			}
			.footerContent div{
				color:#707070;
				font-family:Arial;
				font-size:12px;
				line-height:125%;
				text-align:left;
			}
			.footerContent div a:link, .footerContent div a:visited{
				color:#336699;
				font-weight:normal;
				text-decoration:underline;
			}

			.footerContent img{
				display:inline;
			}

			#social{
				background-color:#FAFAFA;
				border:0;
			}
			#social div{
				text-align:center;
			}

			#utility{
				background-color:#FDFDFD;
				border:0;
			}
			#utility div{
				text-align:center;
			}		</style>
	</head>
	<body leftmargin="0" marginheight="0" marginwidth="0" offset="0" topmargin="0">
		<center>
			<table border="0" cellpadding="0" cellspacing="0" height="100%" id="backgroundTable" width="100%">
				<tbody>
					<tr>
						<td align="center" valign="top">
							<table border="0" cellpadding="10" cellspacing="0" id="templatePreheader" width="600">
								<tbody>
									<tr>
										<td class="preheaderContent" valign="top">
											<table border="0" cellpadding="10" cellspacing="0" width="100%">
												<tbody>
													<tr>
														<td val'', ''2013-09-09 23:32:14.270'', NULL, 2, 13, 0, 0, NULL, NULL, NULL, NULL, NULL)')
UPDATE [dbo].[Content] SET [Body].WRITE(N'ign="top">
															<div bvedit="">
																Use this area to offer a short teaser of your email&#39;&#39;s content. Text here will show in the preview area of some email clients.</div>
														</td>
														<td align="right" valign="top" width="190">
															<div bvedit="">
																put a date here if you want</div>
														</td>
													</tr>
												</tbody>
											</table>
										</td>
									</tr>
								</tbody>
							</table>
							<table border="0" cellpadding="0" cellspacing="0" id="templateContainer" width="600">
								<tbody>
									<tr>
										<td align="center" valign="top">
											<table border="0" cellpadding="0" cellspacing="0" id="templateHeader" width="600">
												<tbody>
													<tr>
														<td class="headerContent">
															<div bvedit="">
																<img id="headerImage" src="http://www.bvcms.com/content/images/placeholder_600.gif" style="max-width:600px;" /></div>
														</td>
													</tr>
												</tbody>
											</table>
										</td>
									</tr>
									<tr>
										<td align="center" valign="top">
											<table border="0" cellpadding="0" cellspacing="0" id="templateBody" width="600">
												<tbody>
													<tr>
														<td class="bodyContent" valign="top">
															<table border="0" cellpadding="20" cellspacing="0" width="100%">
																<tbody>
																	<tr>
																		<td valign="top">
																			<div bvedit="">
																				<h3>
																					Heading 3</h3>
																				<h4>
																					Heading 4</h4>
																				<strong>Getting started:</strong> Customize your &nbsp;template by clicking on the style editor tabs up above. Set your fonts, colors, and styles. After setting your styling is all done you can click here in this area, delete the text, and start adding your own awesome content!<br />
																				<br />
																				After you enter your content, highlight the text you want to style and select the options you set in the style editor in the &quot;styles&quot; drop down box.</div>
																		</td>
																	</tr>
																</tbody>
															</table>
														</td>
													</tr>
												</tbody>
											</table>
										</td>
									</tr>
									<tr>
										<td align="center" valign="top">
											<table border="0" cellpadding="10" cellspacing="0" id="templateFooter" width="600">
												<tbody>
													<tr>
														<td class="footerContent" valign="top">
															<table border="0" cellpadding="10" cellspacing="0" width="100%">
																<tbody>
																	<tr>
																		<td id="utility" valign="middle">
																			<div>
																				&nbsp;Click {unsubscribe}&nbsp;{track}</div>
																		</td>
																	</tr>
																</tbody>
															</table>
														</td>
													</tr>
												</tbody>
											</table>
										</td>
									</tr>
								</tbody>
							</table>
						</td>
					</tr>
				</tbody>
			</table>
		</center>
		<p>
			&nbsp;</p>
	</body>
</html>
',NULL,NULL) WHERE [Id]=71
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (74, N'ForgotPasswordReset2', N'ForgotPasswordReset2', N'<html>
<head>
	<title></title>
</head>
<body>
<p>Someone recently requested a new password for {email}. To set your password, click your username below:</p>

<blockquote>{resetlink}</blockquote>

<p>If this is a mistake, please disregard this message, your password will not be changed.</p>

<p>Thanks,<br />
The TouchPoint Team</p>
</body>
</html>
', '2013-09-09 23:32:14.270', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (75, N'ExistingUserConfirmation', N'ExistingUserConfirmation', N'<html>
<head>
	<title></title>
</head>
<body>
<p>Hi {name},</p>

<p>We noticed you already have an account in our church&#39;s TouchPoint database.</p>

<p>You can login at <a href="{host}">{host}</a>. And if you can&#39;t remember your password or username, click the <strong>Forgot Password / Create Account link</strong> when you get there. Enter either your username or the email address that is on your people record in the database. This will send you a link you can use to reset your password. Just follow the directions in that email to set your password.</p>

<p>If you do not receive an email and you entered an email address instead of a username, perhaps we have a different email address on your record. You can try the Forgot Password link again using another email address or call the church for assistance.</p>

<p>You can use your account to help us maintain your correct contact and personal information. Just login to <a href="{host}">{host}</a> and you will be taken to your record where you can make corrections or make additions if needed, look at your giving record and even print a contribution statement.</p>

<p><span style="line-height: 1.6em;">Thank You</span></p>

<p>&nbsp;</p>
</body>
</html>
', '2014-03-01 18:22:36.030', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (76, N'NoGivingSetupMessage', N'NoGivingSetupMessage', N'<html>
<head>
	<title></title>
</head>
<body>
<p class="alert alert-block alert-info">Sorry, it appears that your church has not set up online giving through TouchPoint.</p>
</body>
</html>
', NULL, NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (77, N'StandardExtraValues2', N'StandardExtraValues2', N'<?xml version="1.0" encoding="utf-16"?>
<Views>
  <View Table="People" Location="Entry">
    <Value Name="IC:InfoCard" Type="Bits">
      <Code>IC:PleaseVisit</Code>
      <Code>IC:InterestedInJoining</Code>
      <Code>IC:SendInfoReBecomingAChristian</Code>
    </Value>
  </View>
</Views>', NULL, NULL, 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (79, N'OrgMembersModel_SendMovedNotices', N'SendMovedNotices', N'<p><span style="font-size: 13px; line-height: 1.6em;">This is to notify you that {name} has</span><span style="font-size: 13px; line-height: 1.6em;">&nbsp;the following room assignment:</span></p>

<p style="margin-left: 40px;"><span style="font-size: 13px; line-height: 1.6em;">The <strong>class</strong> is&nbsp;{org}.</span></p>

<p style="margin-left: 40px;"><span style="font-size: 13px; line-height: 1.6em;">The <strong>location</strong> is {room}.</span></p>

<p style="margin-left: 40px;"><span style="font-size: 13px; line-height: 1.6em;">The <strong>leader&#39;s name</strong> is {leader}.</span></p>

<p>Please call the church at <strong>{phone}</strong>&nbsp;if you have any questions.</p>

<p>Please print this and bring it with you as a reminder of your room number.</p>

<p>Thank you!<br />
{church}</p>
', '2014-08-04 06:30:45.873', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (80, N'ForgotPasswordBadEmail', N'ForgotPasswordBadEmail', N'<html>
<head>
	<title></title>
</head>
<body>
<p>Someone recently requested a new password for this email address {email}. However, we could not find an account associated with this email address. You may try a different email address, or contact the church.</p>

<p>If this is a mistake, please disregard this message, your password will not be changed.</p>

<p>Thanks,<br />
The TouchPoint Team</p>
</body>
</html>
', NULL, NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (81, N'CustomReports', N'CustomReports', N'<CustomReports>
  <Report name="Email">
    <Column name="Email" />
  </Report>  
  <Report name="Student and Parents">
    <Column name="PeopleId" />
    <Column name="GoesBy" />
    <Column name="Last" />
    <Column name="BirthDate" />
    <Column name="Age" />
    <Column name="Home" />
    <Column name="Cell" />
    <Column name="Work" />
    <Column name="Email" />
    <Column name="Father" />
    <Column name="FatherEmail" />
    <Column name="FatherCell" />
    <Column name="Mother" />
    <Column name="MotherEmail" />
    <Column name="MotherCell" />
    <Column name="FamilyId" />
  </Report>

</CustomReports>
', NULL, NULL, 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (82, N'CustomReportsMenu', N'CustomReportsMenu', N'<?xml version="1.0" encoding="utf-8"?>
<ReportsMenu>
  <Column1>
<!--
    <Header>Statistics</Header>
    <Report link="/Reports/VitalStats">Vital Stats</Report>
-->
  </Column1>
  <Column2>
  </Column2>
</ReportsMenu>
', NULL, NULL, 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (83, N'ContinueRegistrationLink', N'ContinueRegistrationLink', N'<html>
<head>
	<title></title>
</head>
<body>
<p>Hi {first},</p>

<p>Here is the link to continue your registration:</p>

<p>Resume [registration for {orgname}]</p>

<p>You can save your progress as many times as necessary. Just be sure to click Save Progress.</p>

<p>If you want to start over, you will have the opportunity to do so when you click the above link. You can select Continue with Existing Registration or Start a New Registration.</p>

<p>Please let us know if you have any difficulty or have any questions.</p>
</body>
</html>
', '2015-06-25 13:16:59.687', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (84, N'OneTimeManageGiving', N'Manage your recurring giving', N'<html>
<head>
	<title></title>
</head>
<body>
<p>Hi {name},</p>

<p>Here is your <a href="%7Burl%7D">link</a> to manage your recurring giving. (note: it will only work once for security reasons)</p>
</body>
</html>
', '2015-06-27 12:17:22.793', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (85, N'OneTimeConfirmationVolunteer', N'Manage your Volunteer Commitments', N'<html>
<head>
	<title></title>
</head>
<body>
<p>Hi {name},</p>

<p>Here is your <a href="url">link</a> to manage your volunteer commitments.</p>

<p>Note: it will only work once for security reasons.</p>

<p>&nbsp;</p>

<p>Thank you!</p>
</body>
</html>
', '2015-06-27 13:25:03.937', NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0)
INSERT INTO [dbo].[Content] ([Id], [Name], [Title], [Body], [DateCreated], [TextOnly], [TypeID], [ThumbID], [RoleID], [OwnerID], [CreatedBy], [Archived], [ArchivedFromId], [UseTimes], [Snippet]) VALUES (86, N'DownlineCategories', N'Downline Category XML Config', N'<root>
	<!--Note the category id="1" line, that specifies a single category which is the default.
		mainfellowship="true" will include any organization which is a main fellowship in the downline
		The other two lines are commented out 
		and show examples using programs and divisions to select the participating orgs.
	 -->
	<category id="1" name="Main Fellowships" mainfellowship="true" />
	<!--
	<category id="2" name="LifeGroups and DiscipleLife" programs="101,103" />
	<category id="3" name="Discipleship" divisions="6366" />
	-->
</root>', '2015-07-11 09:33:42.840', NULL, 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Content] OFF
INSERT INTO [dbo].[ContributionFund] ([FundId], [CreatedBy], [CreatedDate], [FundName], [FundDescription], [FundStatusId], [FundTypeId], [FundPledgeFlag], [FundAccountCode], [FundIncomeDept], [FundIncomeAccount], [FundIncomeFund], [FundCashDept], [FundCashAccount], [FundCashFund], [OnlineSort], [NonTaxDeductible], [QBIncomeAccount], [QBAssetAccount]) VALUES (1, 1, '2010-10-30 15:36:12.533', N'General Operation', N'General Operation', 1, 1, 0, NULL, N'0', N'0', N'0', N'0', N'0', N'0', 1, NULL, 0, 0)
INSERT INTO [dbo].[ContributionFund] ([FundId], [CreatedBy], [CreatedDate], [FundName], [FundDescription], [FundStatusId], [FundTypeId], [FundPledgeFlag], [FundAccountCode], [FundIncomeDept], [FundIncomeAccount], [FundIncomeFund], [FundCashDept], [FundCashAccount], [FundCashFund], [OnlineSort], [NonTaxDeductible], [QBIncomeAccount], [QBAssetAccount]) VALUES (2, 3, '2011-09-27 15:37:43.453', N'Pledge', N'Pledge', 1, 1, 1, NULL, N'0', N'0', N'0', N'0', N'0', N'0', 2, 0, 0, 0)
SET IDENTITY_INSERT [dbo].[ContributionsRun] ON
INSERT INTO [dbo].[ContributionsRun] ([id], [started], [count], [processed], [completed], [error], [LastSet], [CurrSet], [Sets]) VALUES (1, '2012-02-13 15:50:01.000', 0, 0, '2012-02-13 15:50:01.000', N'', NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[ContributionsRun] OFF
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('ActiveOtherChurch', 94, 'rr.ActiveInAnotherChurch', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Addr', 68, 'p.PrimaryAddress', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Addr2', 69, 'p.PrimaryAddress2', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Advil', 78, 'rr.Advil', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Age', 15, 'p.Age', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Allergy', 91, 'rr.MedicalDescription', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('AltName', 9, 'p.AltName', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('BadAddr', 74, 'p.PrimaryBadAddrFlag', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('BaptiseDt', 41, 'p.BaptismDate', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('BaptismScheduled', 40, 'p.BaptismSchedDate', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('BaptismStatus', 38, 'bs.Description', 'LEFT JOIN lookup.BaptismStatus bs ON bs.Id = p.BaptismStatusId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('BaptismType', 39, 'bt.Description', 'LEFT JOIN lookup.BaptismType bt ON bt.Id = p.BaptismTypeId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('BirthDate', 14, 'dbo.DOB(p.BirthMonth, p.BirthDay, p.BirthYear)', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Campus', 46, 'cp.Description', 'LEFT JOIN lookup.Campus cp ON cp.Id = p.CampusId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Cell', 23, 'dbo.FmtPhone(p.CellPhone)', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('City', 70, 'p.PrimaryCity', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Country', 73, 'p.PrimaryCountry', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('CreatedBy', 77, '(SELECT Name FROM Users u WHERE u.UserId = p.CreatedBy)', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('CreatedDt', 76, 'p.CreatedDate', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('CustodyIssue', 35, 'p.CustodyIssue', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Deceased', 18, 'p.DeceasedDate', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Decision', 42, 'dt.Description', 'LEFT JOIN lookup.DecisionType dt ON dt.Id = p.DecisionTypeId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('DecisionDt', 43, 'p.DecisionDate', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('DocPhone', 83, 'rr.docphone', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Doctor', 82, 'rr.doctor', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('DoNotCall', 30, 'p.DoNotCallFlag', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('DoNotMail', 31, 'p.DoNotMailFlag', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('DoNotPublishPhones', 33, 'p.DoNotPublishPhones', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('DoNotVisit', 32, 'p.DoNotVisitFlag', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('DropDt', 48, 'p.DropDate', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('DropType', 47, 'drt.Description', 'LEFT JOIN lookup.DropType drt ON drt.Id = p.DropCodeId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('ElectronicStatement', 51, 'p.ElectronicStatement', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Email', 25, 'p.EmailAddress', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Email2', 26, 'p.EmailAddress2', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Email2Active', 28, 'p.SendEmailAddress2', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('EmailActive', 27, 'p.SendEmailAddress1', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('EmergContact', 85, 'rr.emcontact', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('EmergPhone', 86, 'rr.emphone', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Employer', 66, 'p.EmployerOther', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('EntryPoint', 61, 'ep.Description', 'LEFT JOIN lookup.EntryPoint ep ON ep.Id = p.EntryPointId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('EnvelopeOption', 49, 'eopt.Description', 'LEFT JOIN lookup.EnvelopeOption eopt ON eopt.Id = p.EnvelopeOptionsId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('ExtraValueBit', 205, '(SELECT BitValue FROM dbo.PeopleExtra pe WHERE pe.PeopleId = p.PeopleId AND Field = ''{field}'')', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('ExtraValueCode', 201, '(SELECT StrValue FROM dbo.PeopleExtra pe WHERE pe.PeopleId = p.PeopleId AND Field = ''{field}'')', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('ExtraValueDate', 202, '(SELECT DateValue FROM dbo.PeopleExtra pe WHERE pe.PeopleId = p.PeopleId AND Field = ''{field}'')', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('ExtraValueInt', 204, '(SELECT IntValue FROM dbo.PeopleExtra pe WHERE pe.PeopleId = p.PeopleId AND Field = ''{field}'')', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('ExtraValueText', 203, '(SELECT Data FROM dbo.PeopleExtra pe WHERE pe.PeopleId = p.PeopleId AND Field = ''{field}'')', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('FamilyId', 2, 'p.FamilyId', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('FamilyPosition', 21, 'fp.Description', 'LEFT JOIN lookup.FamilyPosition fp ON fp.Id = p.PositionInFamilyId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Father', 98, 'fa.Name', 'LEFT JOIN dbo.People fa ON fa.FamilyId = p.FamilyId AND fa.GenderId = 1 AND fa.PositionInFamilyId = 10 AND p.PeopleId <> fa.PeopleId AND p.PositionInFamilyId not in (10,20)')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('FatherCell', 100, 'dbo.FmtPhone(fa.CellPhone)', 'LEFT JOIN dbo.People fa ON fa.FamilyId = p.FamilyId AND fa.GenderId = 1 AND fa.PositionInFamilyId = 10 AND p.PeopleId <> fa.PeopleId AND p.PositionInFamilyId not in (10,20)')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('FatherEmail', 99, 'fa.EmailAddress', 'LEFT JOIN dbo.People fa ON fa.FamilyId = p.FamilyId AND fa.GenderId = 1 AND fa.PositionInFamilyId = 10 AND p.PeopleId <> fa.PeopleId AND p.PositionInFamilyId not in (10,20)')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('First', 3, 'p.FirstName', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('FormerName', 8, 'p.MaidenName', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Gender', 19, 'g.Description', 'LEFT JOIN lookup.Gender g ON g.Id = p.GenderId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('GeneralComments', 53, 'p.Comments', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('GoesBy', 5, 'p.PreferredName', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Grade', 36, 'p.Grade', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Home', 22, 'dbo.FmtPhone(p.HomePhone)', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Insurance', 89, 'rr.insurance', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('InterestPoint', 62, 'ip.Description', 'LEFT JOIN lookup.InterestPoint ip ON ip.Id = p.InterestPointId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('JoinDt', 45, 'p.JoinDate', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('JoinType', 44, 'jt.Description', 'LEFT JOIN lookup.JoinType jt ON jt.Id = p.JoinCodeId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Last', 7, 'p.LastName', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('LeaderName', 95, 'dbo.OrganizationLeaderName(p.BibleFellowshipClassId)', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('LetterReceivedDt', 58, 'p.LetterDateReceived', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('LetterRequestDt', 57, 'p.LetterDateRequested', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('LetterStatus', 56, 'lts.Description', 'LEFT JOIN lookup.MemberLetterStatus lts ON lts.Id = p.LetterStatusId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Maalox', 81, 'rr.Maalox', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('MaritalStatus', 20, 'mars.Description', 'LEFT JOIN lookup.MaritalStatus mars ON mars.Id = p.MaritalStatusId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('MemberAny', 37, 'p.MemberAnyChurch', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('MembershipNotes', 59, 'p.LetterStatusNotes', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('MemberStatus', 52, 'ms.Description', 'LEFT JOIN lookup.MemberStatus ms ON ms.Id = p.MemberStatusId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Middle', 6, 'p.MiddleName', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Mother', 101, 'mo.Name', 'LEFT JOIN dbo.People mo ON mo.FamilyId = p.FamilyId AND mo.GenderId = 2 AND mo.PositionInFamilyId = 10 AND p.PeopleId <> mo.PeopleId AND p.PositionInFamilyId not in (10,20)')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('MotherCell', 103, 'dbo.FmtPhone(mo.Cellphone)', 'LEFT JOIN dbo.People mo ON mo.FamilyId = p.FamilyId AND mo.GenderId = 2 AND mo.PositionInFamilyId = 10 AND p.PeopleId <> mo.PeopleId AND p.PositionInFamilyId not in (10,20)')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('MotherEmail', 102, 'mo.EmailAddress', 'LEFT JOIN dbo.People mo ON mo.FamilyId = p.FamilyId AND mo.GenderId = 2 AND mo.PositionInFamilyId = 10 AND p.PeopleId <> mo.PeopleId AND p.PositionInFamilyId not in (10,20)')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Name', 11, 'p.Name', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Name2', 12, 'p.Name2', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('NewChurch', 64, 'p.OtherNewChurch', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('NewMemberClassDt', 55, 'p.NewMemberClassDate', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('NewMemberStatus', 54, 'nmcs.Description', 'LEFT JOIN lookup.NewMemberClassStatus nmcs ON nmcs.Id = p.NewMemberClassStatusId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Nick', 4, 'p.NickName', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Occupation', 65, 'p.OccupationOther', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('OriginPoint', 60, 'ori.Description', 'LEFT JOIN lookup.Origin ori ON ori.Id = p.OriginId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('PeopleId', 1, 'p.PeopleId', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Policy', 90, 'rr.policy', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('PrevChurch', 63, 'p.OtherPreviousChurch', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('RegCoaching', 93, 'rr.coaching', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('RegEmail', 84, 'rr.email', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('RegFather', 87, 'rr.fname', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('RegMother', 88, 'rr.mname', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('ResidentCode', 75, 'rc.Description', 'LEFT JOIN lookup.ResidentCode rc ON rc.Id = p.PrimaryResCode')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Robitussin', 80, 'rr.robitussin', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('School', 67, 'p.SchoolOther', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('ShirtSize', 92, 'rr.shirtsize', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('SpouseId', 17, 'p.SpouseId', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('State', 71, 'p.PrimaryState', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('StatementOption', 50, 'copt.Description', 'LEFT JOIN lookup.EnvelopeOption copt ON copt.Id = p.ContributionOptionsId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('StatusFlag', 200, 'CAST(CASE WHEN EXISTS(SELECT NULL FROM dbo.Tag t JOIN dbo.TagPerson tp ON tp.Id = t.Id WHERE tp.PeopleId = p.PeopleId AND t.Name = ''{flag}'' AND t.TypeId = 100) THEN 1 ELSE NULL END AS BIT)', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Suffix', 13, 'p.SuffixCode', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('TextingOK', 29, 'p.ReceiveSMS', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Title', 12, 'p.TitleCode', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('TransportOK', 33, 'p.OKTransport', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Tylenol', 79, 'rr.tylenol', 'LEFT JOIN dbo.RecReg rr ON rr.PeopleId = p.PeopleId')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('UserId', 96, '(SELECT TOP 1 UserId FROM dbo.Users u WHERE u.PeopleId = p.PeopleId ORDER BY UserId)', '')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('UserName', 97, '(SELECT TOP 1 Username FROM dbo.Users u WHERE u.PeopleId = p.PeopleId ORDER BY UserId)', '')
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Wedding', 16, 'p.WeddingDate', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Work', 24, 'dbo.FmtPhone(p.WorkPhone)', NULL)
INSERT INTO [dbo].[CustomColumns] ([Column], [Ord], [Select], [JoinTable]) VALUES ('Zip', 72, 'p.PrimaryZip', NULL)
SET IDENTITY_INSERT [dbo].[EmailQueueToFail] ON
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (1, 659, 418, '2015-01-21 15:37:33.403', N'dropped', N'Spam Reporting Address', NULL, N'zekeingram@hotmail.com', 1421876262)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (2, 656, 418, '2015-01-22 11:00:20.817', N'dropped', N'Spam Reporting Address', NULL, N'zekeingram@hotmail.com', 1421946035)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (3, 657, 895, '2015-01-22 11:00:28.300', N'dropped', N'Spam Reporting Address', NULL, N'darla.tyree@yahoo.com', 1421946042)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (4, 665, 444, '2015-01-26 14:58:11.970', N'bounce', N'550 Requested action not taken: mailbox unavailable ', N'bounce', N'yost2075@msn.com', 1422305896)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (5, 665, 660, '2015-01-26 14:58:13.760', N'bounce', N'550 5.1.1 <tro@lanl.gov>... User unknown ', N'bounce', N'tro@lanl.gov', 1422305898)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (6, 668, 418, '2015-01-26 16:14:05.807', N'dropped', N'Spam Reporting Address', NULL, N'zekeingram@hotmail.com', 1422310450)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (7, 668, 895, '2015-01-26 16:14:12.820', N'dropped', N'Spam Reporting Address', NULL, N'darla.tyree@yahoo.com', 1422310453)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (8, 669, 895, '2015-01-26 17:32:44.400', N'dropped', N'Spam Reporting Address', NULL, N'darla.tyree@yahoo.com', 1422315161)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (9, 670, 418, '2015-01-27 13:53:58.093', N'dropped', N'Spam Reporting Address', NULL, N'zekeingram@hotmail.com', 1422388435)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (10, 670, 895, '2015-01-27 13:54:02.607', N'dropped', N'Spam Reporting Address', NULL, N'darla.tyree@yahoo.com', 1422388437)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (11, 672, 418, '2015-01-31 12:07:30.890', N'dropped', N'Spam Reporting Address', NULL, N'zekeingram@hotmail.com', 1422727661)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (12, 672, 895, '2015-01-31 12:07:33.153', N'dropped', N'Spam Reporting Address', NULL, N'darla.tyree@yahoo.com', 1422727663)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (13, 673, 459, '2015-02-03 10:23:55.177', N'bounce', N'554 delivery error: dd This user doesn''t have a yahoo.com account (xshen2002@yahoo.com) [0] - mta1528.mail.ne1.yahoo.com ', N'bounce', N'xshen2002@yahoo.com', 1422980638)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (14, 675, 418, '2015-02-03 15:39:04.133', N'dropped', N'Spam Reporting Address', NULL, N'zekeingram@hotmail.com', 1422999549)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (15, 676, 418, '2015-02-03 15:52:41.323', N'dropped', N'Spam Reporting Address', NULL, N'zekeingram@hotmail.com', 1423000366)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (16, 676, 895, '2015-02-03 15:52:43.427', N'dropped', N'Spam Reporting Address', NULL, N'darla.tyree@yahoo.com', 1423000368)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (17, 674, 418, '2015-02-04 05:30:18.133', N'dropped', N'Spam Reporting Address', NULL, N'zekeingram@hotmail.com', 1423049426)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (18, 674, 895, '2015-02-04 05:30:20.420', N'dropped', N'Spam Reporting Address', NULL, N'darla.tyree@yahoo.com', 1423049429)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (19, 680, 418, '2015-02-10 12:09:16.963', N'dropped', N'Spam Reporting Address', NULL, N'zekeingram@hotmail.com', 1423591764)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (20, 680, 895, '2015-02-10 12:09:21.180', N'dropped', N'Spam Reporting Address', NULL, N'darla.tyree@yahoo.com', 1423591766)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (21, 683, 418, '2015-02-11 09:38:59.133', N'dropped', N'Spam Reporting Address', NULL, N'zekeingram@hotmail.com', 1423669150)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (22, 683, 895, '2015-02-11 09:39:03.207', N'dropped', N'Spam Reporting Address', NULL, N'darla.tyree@yahoo.com', 1423669153)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (23, 690, 418, '2015-02-15 09:36:49.680', N'dropped', N'Spam Reporting Address', NULL, N'zekeingram@hotmail.com', 1424014608)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (24, 690, 895, '2015-02-15 09:36:53.683', N'dropped', N'Spam Reporting Address', NULL, N'darla.tyree@yahoo.com', 1424014610)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (25, 691, 895, '2015-02-17 12:24:47.723', N'dropped', N'Spam Reporting Address', NULL, N'darla.tyree@yahoo.com', 1424197487)
INSERT INTO [dbo].[EmailQueueToFail] ([pkey], [Id], [PeopleId], [time], [event], [reason], [bouncetype], [email], [timestamp]) VALUES (26, 861, 3641, '2015-02-17 12:40:24.457', N'bounce', N'550 5.1.1 The email account that you tried to reach does not exist. Please try double-checking the recipient''s email address for typos or unnecessary spaces. Learn more at http://support.google.com/mail/bin/answer.py?answer=6596 gy6si10689881igb.18 - gsmtp ', N'bounce', N'bebjaneglide52@gmail.com', 1424198426)
SET IDENTITY_INSERT [dbo].[EmailQueueToFail] OFF
SET IDENTITY_INSERT [dbo].[ExtraData] ON
EXEC(N'INSERT INTO [dbo].[ExtraData] ([Id], [Data], [Stamp], [completed], [OrganizationId], [UserPeopleId], [abandoned]) VALUES (1, N''<OnlineRegModel xmlns="http://schemas.datacontract.org/2004/07/CmsWeb.Models" xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><_Classid i:nil="true"/><_Nologin>true</_Nologin><_Password i:nil="true"/><_TranId>1</_TranId><_UserPeopleId i:nil="true"/><_Username i:nil="true"/><_donation i:nil="true"/><_donor i:nil="true"/><_meeting i:nil="true" xmlns:a="http://schemas.datacontract.org/2004/07/CmsData"/><_x003C_URL_x003E_k__BackingField>https://starterdb.bvcms.com:443/onlinereg/Index/30?testing=true</_x003C_URL_x003E_k__BackingField><_x003C_divid_x003E_k__BackingField i:nil="true"/><_x003C_orgid_x003E_k__BackingField>30</_x003C_orgid_x003E_k__BackingField><_x003C_testing_x003E_k__BackingField>true</_x003C_testing_x003E_k__BackingField><list><OnlineRegPersonModel><CannotCreateAccount>false</CannotCreateAccount><CreatedAccount>false</CreatedAccount><NotFoundText i:nil="true"/><SawExistingAccount>false</SawExistingAccount><_Checkbox i:nil="true" xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays"/><_ExtraQuestion xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays"/><_Homephone i:nil="true"/><_IsFamily>false</_IsFamily><_LoggedIn>false</_LoggedIn><_MenuItem xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays"/><_Middle i:nil="true"/><_Option2 i:nil="true"/><_Whatfamily i:nil="true"/><_YesNoQuestion xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays"/><_notreq i:nil="true"/><_x003C_CreatingAccount_x003E_k__BackingField i:nil="true"/><_x003C_Found_x003E_k__BackingField>true</_x003C_Found_x003E_k__BackingField><_x003C_IsFilled_x003E_k__BackingField>false</_x003C_IsFilled_x003E_k__BackingField><_x003C_IsNew_x003E_k__BackingField>false</_x003C_IsNew_x003E_k__BackingField><_x003C_IsValidForContinue_x003E_k__BackingField>false</_x003C_IsValidForContinue_x003E_k__BackingField><_x003C_IsValidForExisting_x003E_k__BackingField>true</_x003C_IsValidForExisting_x003E_k__BackingField><_x003C_IsValidForNew_x003E_k__BackingField>false</_x003C_IsValidForNew_x003E_k__BackingField><_x003C_LastItem_x003E_k__BackingField>false</_x003C_LastItem_x003E_k__BackingField><_x003C_OtherOK_x003E_k__BackingField>true</_x003C_OtherOK_x003E_k__BackingField><_x003C_PeopleId_x003E_k__BackingField>64</_x003C_PeopleId_x003E_k__BackingField><_x003C_ShowAddress_x003E_k__BackingField>false</_x003C_ShowAddress_x003E_k__BackingField><_x003C_address_x003E_k__BackingField>9486 Mountain Spring Way</_x003C_address_x003E_k__BackingField><_x003C_advil_x003E_k__BackingField>true</_x003C_advil_x003E_k__BackingField><_x003C_city_x003E_k__BackingField>Germantown</_x003C_city_x003E_k__BackingField><_x003C_classid_x003E_k__BackingField i:nil="true"/><_x003C_coaching_x003E_k__BackingField i:nil="true"/><_x003C_divid_x003E_k__BackingField i:nil="true"/><_x003C_dob_x003E_k__BackingField>10/20/00</_x003C_dob_x003E_k__BackingField><_x003C_docphone_x003E_k__BackingField>901-555-6688</_x003C_docphone_x003E_k__BackingField><_x003C_doctor_x003E_k__BackingField>Dr. Smith</_x003C_doctor_x003E_k__BackingField><_x003C_email_x003E_k__BackingField>karen@bvcms.com</_x003C_email_x003E_k__BackingField><_x003C_emcontact_x003E_k__BackingField>Karen Worrell</_x003C_emcontact_x003E_k__BackingField><_x003C_emphone_x003E_k__BackingField>901-555-7799</_x003C_emphone_x003E_k__BackingField><_x003C_first_x003E_k__BackingField>Sharon </_x003C_first_x003E_k__BackingField><_x003C_fname_x003E_k__BackingField>George Eaton</_x003C_fname_x003E_k__BackingField><_x003C_gender_x003E_k__BackingField>2</_x003C_gender_x003E_k__BackingField><_x003C_grade_x003E_k__BackingField i:nil="true"/><_x003C_gradeoption_x003E_k__BackingField>4</_x003C_gradeoption_x003E_k__BackingField><_x003C_index_x003E_k__BackingField>0</_x003C_index_x003E_k__BackingField><_x003C_insurance_x003E_k__BackingField>Blue Cross</_x003C_insurance_x003E_k__BackingField><_x003C_last_x003E_k__BackingField>Eaton</_x003C_last_x003E_k__BackingField><_x003C_maalox_x003E_k__BackingField>true</_x003C_maal'', ''2011-05-29 20:19:35.977'', NULL, NULL, NULL, NULL)')
UPDATE [dbo].[ExtraData] SET [Data].WRITE(N'ox_x003E_k__BackingField><_x003C_married_x003E_k__BackingField>1</_x003C_married_x003E_k__BackingField><_x003C_medical_x003E_k__BackingField>peanuts</_x003C_medical_x003E_k__BackingField><_x003C_memberus_x003E_k__BackingField>false</_x003C_memberus_x003E_k__BackingField><_x003C_mname_x003E_k__BackingField>Cheryl Eaton</_x003C_mname_x003E_k__BackingField><_x003C_ntickets_x003E_k__BackingField i:nil="true"/><_x003C_option_x003E_k__BackingField i:nil="true"/><_x003C_orgid_x003E_k__BackingField>30</_x003C_orgid_x003E_k__BackingField><_x003C_otherchurch_x003E_k__BackingField>true</_x003C_otherchurch_x003E_k__BackingField><_x003C_paydeposit_x003E_k__BackingField>true</_x003C_paydeposit_x003E_k__BackingField><_x003C_phone_x003E_k__BackingField>9017565372</_x003C_phone_x003E_k__BackingField><_x003C_policy_x003E_k__BackingField>123</_x003C_policy_x003E_k__BackingField><_x003C_request_x003E_k__BackingField>Betsy Williams, Joan Ralston</_x003C_request_x003E_k__BackingField><_x003C_robitussin_x003E_k__BackingField>true</_x003C_robitussin_x003E_k__BackingField><_x003C_shirtsize_x003E_k__BackingField>Y M</_x003C_shirtsize_x003E_k__BackingField><_x003C_state_x003E_k__BackingField>TN</_x003C_state_x003E_k__BackingField><_x003C_suffix_x003E_k__BackingField i:nil="true"/><_x003C_tylenol_x003E_k__BackingField>true</_x003C_tylenol_x003E_k__BackingField><_x003C_zip_x003E_k__BackingField>38139</_x003C_zip_x003E_k__BackingField><count>1</count></OnlineRegPersonModel></list></OnlineRegModel>',NULL,NULL) WHERE [Id]=1
SET IDENTITY_INSERT [dbo].[ExtraData] OFF
SET IDENTITY_INSERT [dbo].[LabelFormats] ON
INSERT INTO [dbo].[LabelFormats] ([Id], [Name], [Size], [Format]) VALUES (1, N'Security', 100, '1,1,0,Arial,32,securitycode,0.25,0.45,2,2~1,1,0,Arial,32,securitycode,0.75,0.45,2,2~1,1,0,Arial,16,date,0.25,0.70,2,2~1,1,0,Arial,16,date,0.75,0.70,2,2~2,1,0,4,0.5,0.1,0.5,0.9')
INSERT INTO [dbo].[LabelFormats] ([Id], [Name], [Size], [Format]) VALUES (2, N'Security', 200, '1,1,0,Arial,32,securitycode,0.25,0.45,2,2~1,1,0,Arial,32,securitycode,0.75,0.45,2,2~1,1,0,Arial,16,date,0.25,0.60,2,2~1,1,0,Arial,16,date,0.75,0.60,2,2~2,1,0,4,0.5,0.1,0.5,0.9')
INSERT INTO [dbo].[LabelFormats] ([Id], [Name], [Size], [Format]) VALUES (7, N'Main', 200, '1,1,0,Arial,20,first,0.01,0.01,1,1~1,1,0,Arial,16,last,0.016,0.14,1,1~1,1,0,Arial,10,info,0.98,0.3075,3,1~4,1,0,Arial,10,WEAR THIS LABEL,0.0375,0.3075,1,1~2,1,0,1,0.035,0.31,0.035,0.38~2,1,0,1,0.47,0.31,0.47,0.38~2,1,0,1,0.035,0.31,0.47,0.31~2,1,0,1,0.035,0.38,0.47,0.38~1,3,-0.19,Arial,12,location,0.026,0.91,1,3~1,3,-0.19,Arial,12,time,0.43,0.91,1,3~1,3,-0.19,Arial,10,org,0.056,0.99,1,3~1,3,-0.19,Arial,12,date,0.98,0.91,3,3~1,1,0,Arial,16,securitycode,0.98,0.14,3,1')
INSERT INTO [dbo].[LabelFormats] ([Id], [Name], [Size], [Format]) VALUES (8, N'Guest', 200, '1,1,0,Arial,20,first,0.010,0.015,1,1~1,1,0,Arial,16,last,0.016,0.145,1,1~1,1,0,Arial,12,guest,0.021,0.295,1,1~1,1,0,Arial,12,allergies,0.98,0.295,3,1~1,1,0,Arial,16,securitycode,0.98,0.145,3,1~1,1,0,Arial,12,location,0.026,0.425,1,1~1,1,0,Arial,12,time,0.43,0.425,1,1~1,1,0,Arial,12,date,0.98,0.425,3,1~1,1,0,Arial,10,org,0.11,0.515,1,1~3,1,0,pid,0.5,0.96,200,25,2,3')
INSERT INTO [dbo].[LabelFormats] ([Id], [Name], [Size], [Format]) VALUES (9, N'Main', 100, '1,1,0,Arial,20,first,0.004,0.02,1,1~1,1,0,Arial,16,last,0.010,0.28,1,1~1,1,0,Arial,10,info,0.97,0.505,3,1~4,1,0,Arial,10,WEAR THIS LABEL,0.032,0.505,1,1~2,1,0,1,0.03,0.51,0.03,0.65~2,1,0,1,0.465,0.51,0.465,0.65~2,1,0,1,0.03,0.51,0.465,0.51~2,1,0,1,0.03,0.65,0.465,0.65~1,1,0,Arial,10,location,0.026,0.82,1,3~1,1,0,Arial,10,time,0.43,0.82,1,3~1,1,0,Arial,10,date,0.97,0.82,3,3~1,1,0,Arial,10,org,0.05,0.97,1,3~1,1,0,Arial,16,securitycode,0.97,0.28,3,1')
INSERT INTO [dbo].[LabelFormats] ([Id], [Name], [Size], [Format]) VALUES (10, N'Guest', 100, '1,1,0,Arial,20,first,0.004,0.02,1,1~1,1,0,Arial,16,last,0.010,0.28,1,1~1,1,0,Arial,10,guest,0.015,0.5,1,1~1,1,0,Arial,10,allergies,0.97,0.5,3,1~1,1,0,Arial,10,location,0.026,0.82,1,3~1,1,0,Arial,10,time,0.43,0.82,1,3~1,1,0,Arial,10,date,0.97,0.82,3,3~1,1,0,Arial,10,org,0.05,0.97,1,3~1,1,0,Arial,16,securitycode,0.97,0.28,3,1')
INSERT INTO [dbo].[LabelFormats] ([Id], [Name], [Size], [Format]) VALUES (11, N'Location', 100, '1,2,0.47,Arial,14,first,0.01,0.02,1,1~1,2,0.47,Arial,10,location,0.02,0.22,1,1~1,2,0.47,Arial,10,time,0.45,0.22,1,1~1,2,0.47,Arial,10,org,0.05,0.36,1,1')
INSERT INTO [dbo].[LabelFormats] ([Id], [Name], [Size], [Format]) VALUES (12, N'Location', 200, '1,4,0.24,Arial,16,first,0.01,0.01,1,1~1,4,0.24,Arial,10,location,0.026,0.11,1,1~1,4,0.24,Arial,10,time,0.45,0.11,1,1~1,4,0.24,Arial,10,org,0.056,0.175,1,1')
INSERT INTO [dbo].[LabelFormats] ([Id], [Name], [Size], [Format]) VALUES (13, N'NameTag', 100, '1,1,0,Arial,32,first,0.50,0.56,2,3~1,1,0,Arial,24,last,0.50,0.54,2,1')
INSERT INTO [dbo].[LabelFormats] ([Id], [Name], [Size], [Format]) VALUES (14, N'NameTag', 200, '1,1,0,Arial,32,first,0.5,0.52,2,3~1,1,0,Arial,24,last,0.5,0.52,2,1')
INSERT INTO [dbo].[LabelFormats] ([Id], [Name], [Size], [Format]) VALUES (15, N'Extra', 100, '1,1,0,Arial,20,first,0.004,0.02,1,1~1,1,0,Arial,16,last,0.010,0.28,1,1~1,1,0,Arial,10,extra,0.015,0.5,1,1~1,1,0,Arial,10,allergies,0.97,0.5,3,1~1,1,0,Arial,10,location,0.026,0.82,1,3~1,1,0,Arial,10,time,0.43,0.82,1,3~1,1,0,Arial,10,date,0.97,0.82,3,3~1,1,0,Arial,10,org,0.05,0.97,1,3~1,1,0,Arial,16,securitycode,0.97,0.28,3,1')
INSERT INTO [dbo].[LabelFormats] ([Id], [Name], [Size], [Format]) VALUES (16, N'Extra', 200, '1,1,0,Arial,20,first,0.010,0.01,1,1~1,1,0,Arial,16,last,0.016,0.14,1,1~1,1,0,Arial,12,extra,0.021,0.29,1,1~1,1,0,Arial,12,allergies,0.98,0.29,3,1~1,1,0,Arial,16,securitycode,0.98,0.14,3,1~1,1,0,Arial,12,location,0.026,0.42,1,1~1,1,0,Arial,12,time,0.43,0.42,1,1~1,1,0,Arial,12,date,0.98,0.42,3,1~1,1,0,Arial,10,org,0.056,.51,1,1~3,1,0,pid,0.5,0.96,200,25,2,3')
SET IDENTITY_INSERT [dbo].[LabelFormats] OFF
SET IDENTITY_INSERT [dbo].[Ministries] ON
INSERT INTO [dbo].[Ministries] ([MinistryId], [MinistryName], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [RecordStatus], [DepartmentId], [MinistryDescription], [MinistryContactId], [ChurchId]) VALUES (333, N'Outreach', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Ministries] ([MinistryId], [MinistryName], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [RecordStatus], [DepartmentId], [MinistryDescription], [MinistryContactId], [ChurchId]) VALUES (334, N'Homebound', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Ministries] ([MinistryId], [MinistryName], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [RecordStatus], [DepartmentId], [MinistryDescription], [MinistryContactId], [ChurchId]) VALUES (335, N'Life Groups', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Ministries] OFF
SET IDENTITY_INSERT [dbo].[MobileAppActions] ON
INSERT INTO [dbo].[MobileAppActions] ([id], [type], [title], [option], [data], [order], [loginType], [enabled], [roles], [api]) VALUES (1, 6, N'People Search', 0, N'', 1, 1, 1, N'Access', 1)
INSERT INTO [dbo].[MobileAppActions] ([id], [type], [title], [option], [data], [order], [loginType], [enabled], [roles], [api]) VALUES (2, 7, N'Attendance', 0, N'', 2, 1, 1, N'Access, Attendance', 1)
INSERT INTO [dbo].[MobileAppActions] ([id], [type], [title], [option], [data], [order], [loginType], [enabled], [roles], [api]) VALUES (3, 10, N'Tasks', 0, N'', 3, 1, 1, N'Access', 3)
SET IDENTITY_INSERT [dbo].[MobileAppActions] OFF
SET IDENTITY_INSERT [dbo].[MobileAppActionTypes] ON
INSERT INTO [dbo].[MobileAppActionTypes] ([id], [name], [loginType]) VALUES (1, N'Giving', 0)
INSERT INTO [dbo].[MobileAppActionTypes] ([id], [name], [loginType]) VALUES (2, N'Events', 0)
INSERT INTO [dbo].[MobileAppActionTypes] ([id], [name], [loginType]) VALUES (3, N'Audio', 0)
INSERT INTO [dbo].[MobileAppActionTypes] ([id], [name], [loginType]) VALUES (4, N'Video', 0)
INSERT INTO [dbo].[MobileAppActionTypes] ([id], [name], [loginType]) VALUES (5, N'Maps', 0)
INSERT INTO [dbo].[MobileAppActionTypes] ([id], [name], [loginType]) VALUES (6, N'People Search', 1)
INSERT INTO [dbo].[MobileAppActionTypes] ([id], [name], [loginType]) VALUES (7, N'Attendance', 1)
INSERT INTO [dbo].[MobileAppActionTypes] ([id], [name], [loginType]) VALUES (8, N'Custom', 0)
INSERT INTO [dbo].[MobileAppActionTypes] ([id], [name], [loginType]) VALUES (9, N'Registrations', 1)
INSERT INTO [dbo].[MobileAppActionTypes] ([id], [name], [loginType]) VALUES (10, N'Tasks', 1)
SET IDENTITY_INSERT [dbo].[MobileAppActionTypes] OFF
SET IDENTITY_INSERT [dbo].[MobileAppAudioTypes] ON
INSERT INTO [dbo].[MobileAppAudioTypes] ([id], [name]) VALUES (1, N'SoundCloud')
SET IDENTITY_INSERT [dbo].[MobileAppAudioTypes] OFF
SET IDENTITY_INSERT [dbo].[MobileAppIcons] ON
INSERT INTO [dbo].[MobileAppIcons] ([id], [setID], [actionID], [url]) VALUES (1, 1, 1, N'http://files.bvcms.com/touchpoint/search.png')
INSERT INTO [dbo].[MobileAppIcons] ([id], [setID], [actionID], [url]) VALUES (2, 1, 2, N'http://files.bvcms.com/touchpoint/attendance.png')
INSERT INTO [dbo].[MobileAppIcons] ([id], [setID], [actionID], [url]) VALUES (3, 1, 3, N'http://files.bvcms.com/touchpoint/tasks.png')
SET IDENTITY_INSERT [dbo].[MobileAppIcons] OFF
SET IDENTITY_INSERT [dbo].[MobileAppIconSets] ON
INSERT INTO [dbo].[MobileAppIconSets] ([id], [name], [active]) VALUES (1, N'Standard', 1)
SET IDENTITY_INSERT [dbo].[MobileAppIconSets] OFF
SET IDENTITY_INSERT [dbo].[MobileAppVideoTypes] ON
INSERT INTO [dbo].[MobileAppVideoTypes] ([id], [name]) VALUES (1, N'YouTube')
INSERT INTO [dbo].[MobileAppVideoTypes] ([id], [name]) VALUES (2, N'Vimeo')
SET IDENTITY_INSERT [dbo].[MobileAppVideoTypes] OFF
SET IDENTITY_INSERT [dbo].[Picture] ON
INSERT INTO [dbo].[Picture] ([PictureId], [CreatedDate], [CreatedBy], [LargeId], [MediumId], [SmallId], [ThumbId], [X], [Y]) VALUES (1, '2012-01-20 15:08:45.873', N'karenw', 12, 11, 10, 15, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Picture] OFF
SET IDENTITY_INSERT [dbo].[Program] ON
INSERT INTO [dbo].[Program] ([Id], [Name], [RptGroup], [StartHoursOffset], [EndHoursOffset]) VALUES (1, N'First Program', N'1', 1, 24)
SET IDENTITY_INSERT [dbo].[Program] OFF
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('83369c20-ae88-40c2-a00d-0c1e218fdefe', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="83369c20-ae88-40c2-a00d-0c1e218fdefe" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad">
  <Condition Id="710e70d3-30c7-4d9c-8a77-a5fe4d924fc3" Order="2" Field="MatchAnything" Comparison="Equal" />
</Condition>', 'Admin', '2014-12-16 16:28:22.673', '2014-12-16 16:30:12.660', 'scratchpad', 0, 3, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('7f56d1d0-c576-41d1-b907-0d1bb68f43a8', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="7f56d1d0-c576-41d1-b907-0d1bb68f43a8" Order="0" Field="Group" Comparison="AllTrue">
  <Condition Id="37da2282-e59c-4f17-aa7b-b400f99633a9" Order="2" Field="IsCurrentPerson" Comparison="Equal" CodeIdValue="1,T" />
</Condition>', 'System', '2014-05-05 00:45:25.073', '2014-05-05 00:45:25.073', 'IsCurrentPerson', 0, 0, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('eee06dd1-35d2-40e3-9c77-15e9d0113edd', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="977d8eb1-4fd1-4fdd-b953-9534fbf28ff5" Order="0" Field="Group">
  <Condition Id="d13ea092-cca0-49db-83e8-9caddf1885ff" Order="2" Field="RecentContactType" Comparison="OneOf" CodeIdValue="4,Card Sent;5,EMail Sent;6,Info Pack Sent;3,Letter Sent;7,Other;1,Personal Visit;2,Phone Call;99,Unknown" Days="7" />
</Condition>', 'Admin', '2014-05-18 08:51:03.250', '2016-01-07 13:55:26.020', 'Stats:Contacts', 0, 2, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('efd9c925-7798-47cc-9f38-2141af6d615f', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="efd9c925-7798-47cc-9f38-2141af6d615f" Order="0" Field="Group">
  <Condition Id="195a8ab5-8eae-41b9-ac3c-8308e8bf1715" Order="2" Field="RecentRegistrationType" Comparison="OneOf" CodeIdValue="1,Join Organization;10,User Selects Organization;11,Compute Org By Birthday;15,Manage Subscriptions;14,Manage Recurring Giving;8,Online Giving;9,Online Pledge;16,Special Script" Program="0" Division="0" Organization="0" OrgType="0" Days="7" />
</Condition>', 'david', '2016-01-07 13:51:16.347', '2016-01-07 13:52:17.503', 'scratchpad', 0, 2, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('8f397843-b99b-491c-b0a4-2ba16098f8ee', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="8f397843-b99b-491c-b0a4-2ba16098f8ee" Order="0" Field="Group">
  <Condition Id="2574fc31-439d-47db-9da3-86a05c408aa5" Order="2" Field="RecentRegistrationType" Comparison="OneOf" CodeIdValue="1,Join Organization;10,User Selects Organization;11,Compute Org By Birthday;15,Manage Subscriptions;14,Manage Recurring Giving;8,Online Giving;9,Online Pledge;16,Special Script" Program="0" Division="0" Organization="0" OrgType="0" Days="7" />
</Condition>', 'david', '2016-01-07 13:51:22.603', '2016-01-07 13:53:05.870', 'scratchpad', 0, 2, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('61dadb45-326b-4d6e-b364-34568b1b1453', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="61dadb45-326b-4d6e-b364-34568b1b1453" Order="0" Field="Group">
  <Condition Id="1ae9160e-7fb3-458d-b3f9-16ddb29c8a07" Order="2" Field="RecentDecisionType" Comparison="OneOf" CodeIdValue="0,Unknown;10,POF for Membership;20,POF NOT for Membership;30,Letter;40,Statement;50,Stmt requiring Baptism" Days="7" />
</Condition>', 'david', '2016-01-07 13:51:27.620', '2016-01-07 13:53:57.357', 'scratchpad', 0, 2, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('a4bfbfa7-91be-414f-b00e-3bca9619a989', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="61dadb45-326b-4d6e-b364-34568b1b1453" Order="0" Field="Group">
  <Condition Id="1ae9160e-7fb3-458d-b3f9-16ddb29c8a07" Order="2" Field="RecentDecisionType" Comparison="OneOf" CodeIdValue="0,Unknown;10,POF for Membership;20,POF NOT for Membership;30,Letter;40,Statement;50,Stmt requiring Baptism" Days="7" />
</Condition>', 'Admin', '2014-05-18 08:46:03.340', '2016-01-07 13:55:42.883', 'Stats:Decisions', 0, 4, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('3f1dca9e-814f-49b7-a0bb-508d8769420a', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="3f1dca9e-814f-49b7-a0bb-508d8769420a" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad" PreviousName="scratchpad">
  <Condition Id="fc1c98db-eca8-40e2-bf4b-bc4a1738461f" Order="2" Field="InBFClass" Comparison="Equal" CodeIdValue="1,T" />
</Condition>', 'Admin', '2014-05-18 08:50:22.187', '2014-09-17 21:25:26.620', 'F03:In Fellowship', 0, 3, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('e46c046d-c13a-428d-b117-509c8cd52cce', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="e46c046d-c13a-428d-b117-509c8cd52cce" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad" PreviousName="scratchpad">
  <Condition Id="63a12a3f-696f-4332-a010-270b56bfb637" Order="2" Field="RecentAttendCount" Comparison="GreaterEqual" TextValue="10" Days="140" />
</Condition>', 'Admin', '2014-05-18 08:49:19.043', '2014-09-17 21:26:39.870', 'F02:Active Attender', 0, 3, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('4898a84d-bda0-4420-99c7-57294d4bbb1e', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="4898a84d-bda0-4420-99c7-57294d4bbb1e" Order="0" Field="Group" Comparison="AllTrue">
  <Condition Id="e2fe4fed-2f8f-47e1-9d74-f66496c90a4f" Order="2" Field="InactiveCurrentOrg" Comparison="Equal" CodeIdValue="1,T" />
</Condition>', 'System', '2014-08-20 09:23:21.693', '2014-08-20 09:23:21.693', 'InactiveCurrentOrg', 0, 0, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('b8e212d3-6a0e-4de7-988f-691117df1780', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="b8e212d3-6a0e-4de7-988f-691117df1780" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad">
  <Condition Id="36f6ce2c-bc91-4511-9b7a-b793c71bf026" Order="2" Field="RecentAttendCount" Comparison="Greater" TextValue="0" Days="7" />
</Condition>', 'Admin', '2014-05-18 08:49:24.667', '2016-01-07 13:55:32.930', 'Stats:Attends', 0, 7, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('1f730f25-2186-4335-b1b2-6fff82d70f38', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="1f730f25-2186-4335-b1b2-6fff82d70f38" Order="0" Field="Group" Comparison="AllTrue">
  <Condition Id="61b35095-b7e3-4955-b87f-8272b0578374" Order="2" Field="PreviousCurrentOrg" Comparison="Equal" CodeIdValue="1,T" />
</Condition>', 'System', '2014-08-20 09:23:21.677', '2014-08-20 09:23:21.677', 'PreviousCurrentOrg', 0, 0, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('48e6b5ac-7fa8-46a0-a87f-709732590a6e', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="48e6b5ac-7fa8-46a0-a87f-709732590a6e" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad">
  <Condition Id="1b4f808b-3245-4cb7-bc0e-8667693f94fc" Order="2" Field="MemberStatusId" Comparison="Equal" CodeIdValue="10,Member" />
</Condition>', 'Admin', '2014-05-18 08:43:53.543', '2016-01-07 13:55:29.917', 'Stats:Members', 0, 6, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('822c0c06-2283-481b-be16-78677fdf87f8', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="822c0c06-2283-481b-be16-78677fdf87f8" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad" PreviousName="scratchpad" OnlineReg="0" OrgStatus="0" OrgType2="0">
  <Condition Id="3617ed70-9a41-4ea2-bdab-faac3831c114" Order="2" Field="RecentAttendCount" Comparison="Greater" TextValue="0" Days="7" OnlineReg="0" OrgStatus="0" OrgType2="0" />
</Condition>', 'Admin', '2014-12-16 16:36:18.330', '2014-12-16 16:36:21.080', 'scratchpad', 0, 1, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('e0ec1005-427d-4f6b-97f8-84b51d57f5fd', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="e0ec1005-427d-4f6b-97f8-84b51d57f5fd" Order="0" Field="Group" Comparison="AllTrue">
  <Condition Id="58342b10-da1e-4db3-a09e-bcfdd0130e40" Order="2" Field="ProspectCurrentOrg" Comparison="Equal" CodeIdValue="1,T" />
</Condition>', 'System', '2014-08-20 09:23:21.693', '2014-08-20 09:23:21.693', 'ProspectCurrentOrg', 0, 0, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('8887f5f6-8bd2-4612-9c51-88f9943fc91c', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="8887f5f6-8bd2-4612-9c51-88f9943fc91c" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad" PreviousName="scratchpad" OnlineReg="0" OrgStatus="0" OrgType2="0">
  <Condition Id="777e0d07-f596-45a0-9afd-604568c622ae" Order="2" Field="PeopleExtra" Comparison="Equal" />
</Condition>', 'karenw', '2014-05-18 09:02:11.793', '2014-10-02 08:54:46.890', 'scratchpad', 0, 7, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('977d8eb1-4fd1-4fdd-b953-9534fbf28ff5', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="977d8eb1-4fd1-4fdd-b953-9534fbf28ff5" Order="0" Field="Group">
  <Condition Id="d13ea092-cca0-49db-83e8-9caddf1885ff" Order="2" Field="RecentContactType" Comparison="OneOf" CodeIdValue="4,Card Sent;5,EMail Sent;6,Info Pack Sent;3,Letter Sent;7,Other;1,Personal Visit;2,Phone Call;99,Unknown" Days="7" />
</Condition>', 'david', '2016-01-07 13:51:33.977', '2016-01-07 13:54:56.747', 'scratchpad', 0, 2, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('3a906bf7-69be-4282-9115-9e9c0dcf64f4', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="3a906bf7-69be-4282-9115-9e9c0dcf64f4" Order="0" Field="Group">
  <Condition Id="e4dfd823-a86f-41bb-96b1-003bf5390b99" Order="2" Field="RecentDecisionType" Comparison="OneOf" CodeIdValue="0,Unknown;10,POF for Membership;20,POF NOT for Membership;30,Letter;40,Statement;50,Stmt requiring Baptism" Days="7" />
</Condition>', 'david', '2016-01-07 13:51:37.963', '2016-01-07 13:51:37.963', 'scratchpad', 0, 0, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('b6f066bb-840c-4c46-9d9f-acbbe6128289', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="c18bdd01-ab0e-4003-a8c1-fe0ea8063395" Order="0" Field="Group">
  <Condition Id="5653c3e2-fff6-4ff8-8c0e-29b77ebf9970" Order="0" Field="HasRecentNewAttend" Comparison="Equal" CodeIdValue="1,True" Program="0" Division="0" Organization="0" OrgType="0" Days="7" Quarters="365" />
</Condition>
', 'Admin', '2014-05-18 08:48:02.683', '2016-01-07 13:55:36.223', 'Stats:New Attends', 0, 6, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('3d864d5c-0b6a-4253-984b-ae989171f881', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="3d864d5c-0b6a-4253-984b-ae989171f881" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad" PreviousName="scratchpad" OnlineReg="0" OrgStatus="0" OrgType2="0">
  <Condition Id="c8c617c7-669e-41a6-aab7-64dce27f59f7" Order="2" Field="RecentAttendCount" Comparison="GreaterEqual" TextValue="10" Days="140" OnlineReg="0" OrgStatus="0" OrgType2="0" />
</Condition>', 'karenw', '2014-05-18 08:52:08.437', '2014-09-17 21:26:39.870', 'scratchpad', 0, 1, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('2c145e9d-5c97-493b-af22-b40883a6095f', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="8f397843-b99b-491c-b0a4-2ba16098f8ee" Order="0" Field="Group">
  <Condition Id="2574fc31-439d-47db-9da3-86a05c408aa5" Order="2" Field="RecentRegistrationType" Comparison="OneOf" CodeIdValue="1,Join Organization;10,User Selects Organization;11,Compute Org By Birthday;15,Manage Subscriptions;14,Manage Recurring Giving;8,Online Giving;9,Online Pledge;16,Special Script" Program="0" Division="0" Organization="0" OrgType="0" Days="7" />
</Condition>', 'Admin', '2014-05-18 08:45:39.370', '2016-01-07 13:55:39.643', 'Stats:Registrations', 0, 4, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('785ac91d-8328-4f09-b1b1-bbf1aaa25cee', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="785ac91d-8328-4f09-b1b1-bbf1aaa25cee" Order="0" Field="Group" Comparison="AllTrue">
  <Condition Id="dea0500a-50a1-4301-8d64-ee84ffa90c69" Order="2" Field="PendingCurrentOrg" Comparison="Equal" CodeIdValue="1,T" />
</Condition>', 'System', '2014-08-20 09:23:21.693', '2014-08-20 09:23:21.693', 'PendingCurrentOrg', 0, 0, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('ec40da34-ae69-4a2b-96fc-bf103905bc7b', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="ec40da34-ae69-4a2b-96fc-bf103905bc7b" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad" PreviousName="scratchpad" OnlineReg="0" OrgStatus="0" OrgType2="0">
  <Condition Id="1ee1e50a-1fba-4500-940e-c3a765375815" Order="2" Field="RecentAttendCount" Comparison="Greater" TextValue="0" Days="7" OnlineReg="0" OrgStatus="0" OrgType2="0" />
</Condition>', 'Admin', '2014-12-16 16:30:28.673', '2014-12-16 16:30:30.300', 'scratchpad', 0, 1, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('4db89ecb-bea1-4e6f-b53f-c8bbc9cdd8e7', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="4db89ecb-bea1-4e6f-b53f-c8bbc9cdd8e7" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad">
  <Condition Id="220da674-d5c6-4eb2-bc42-cd61849add9d" Order="2" Field="MatchAnything" Comparison="Equal" />
</Condition>', 'Admin', '2014-12-16 16:27:57.220', '2014-12-16 16:27:57.283', 'scratchpad', 0, 2, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('acdfcf75-6ca0-4e54-87ca-daf9acac5714', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="acdfcf75-6ca0-4e54-87ca-daf9acac5714" Order="0" Field="Group" Comparison="AllTrue">
  <Condition Id="8e6eb857-28f3-49e1-95cd-98aa60a14146" Order="2" Field="VisitedCurrentOrg" Comparison="Equal" CodeIdValue="1,T" />
</Condition>', 'System', '2014-08-20 09:23:21.677', '2014-08-20 09:23:21.677', 'VisitedCurrentOrg', 0, 0, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('3facca1e-a661-4bbd-92b0-dd3b8c1f3cda', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="3facca1e-a661-4bbd-92b0-dd3b8c1f3cda" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad" PreviousName="scratchpad" OnlineReg="0" OrgStatus="0" OrgType2="0">
  <Condition Id="b8e6a757-5c77-4d5c-9522-57bc42b6c4b7" Order="2" Field="HasPeopleExtraField" Comparison="Equal" />
</Condition>', 'karenw', '2014-05-18 09:01:03.390', '2014-10-02 08:53:11.410', 'scratchpad', 0, 6, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('4ae2791c-c52f-41fb-b762-dfd74efda9ac', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="4ae2791c-c52f-41fb-b762-dfd74efda9ac" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad" PreviousName="scratchpad">
  <Condition Id="5f9da255-70b1-4a63-b522-2b115080bc09" Order="2" Field="RecentVisitNumber" Comparison="Equal" CodeIdValue="1,T" Days="30" Quarters="1" />
</Condition>', 'Admin', '2014-05-18 08:47:56.840', '2014-09-17 21:23:50.230', 'F01:Recent New Guest', 0, 3, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('552ba5ed-4d5d-410b-b0a7-e103f075fc48', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="552ba5ed-4d5d-410b-b0a7-e103f075fc48" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad" PreviousName="scratchpad" OnlineReg="0" OrgStatus="0" OrgType2="0">
  <Condition Id="b9be4eb2-e9cc-44ba-8be0-0a03ada488c0" Order="2" Field="RecentDecisionType" Comparison="OneOf" CodeIdValue="0,UNK;10,POF-MEM;20,POF-NON;30,LETTER;40,STATEMENT;50,BAP-REQD" Days="7" OnlineReg="0" OrgStatus="0" OrgType2="0" />
</Condition>', 'karenw', '2014-05-18 08:54:00.890', '2014-09-17 21:28:59.373', 'scratchpad', 0, 1, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('be81fde4-c38c-4c1c-9a3f-eafc30bc35ac', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="be81fde4-c38c-4c1c-9a3f-eafc30bc35ac" Order="0" Field="Group" Comparison="AllTrue">
  <Condition Id="e1ee23cd-c36a-4019-9d7f-9b4119693b4e" Order="2" Field="IsCurrentUser" Comparison="Equal" CodeIdValue="1,T" />
</Condition>', 'System', '2015-06-03 10:37:49.797', '2015-06-03 10:37:49.797', 'IsCurrentUser', 0, 0, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('fc17d28b-b338-4bcd-9004-ec7cbf1a30e0', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="fc17d28b-b338-4bcd-9004-ec7cbf1a30e0" Order="0" Field="Group" Comparison="AllTrue" Description="scratchpad" PreviousName="scratchpad" OnlineReg="0" OrgStatus="0" OrgType2="0">
  <Condition Id="edbaf55a-d4c2-40ab-8f23-f3a7fd1ca890" Order="2" Field="MemberStatusId" Comparison="Equal" CodeIdValue="10,Member" OnlineReg="0" OrgStatus="0" OrgType2="0" />
</Condition>', 'karenw', '2014-05-18 09:03:17.747', '2015-01-05 15:22:54.187', 'scratchpad', 0, 0, NULL)
INSERT INTO [dbo].[Query] ([QueryId], [text], [owner], [created], [lastRun], [name], [ispublic], [runCount], [CopiedFrom]) VALUES ('06a58409-b98f-468a-a7d0-ecbcceb8ec77', '<?xml version="1.0" encoding="utf-16"?>
<Condition Id="06a58409-b98f-468a-a7d0-ecbcceb8ec77" Order="0" Field="Group" Comparison="AllTrue">
  <Condition Id="c2d01577-d6f1-4442-bc3f-a549c326d8c9" Order="2" Field="InCurrentOrg" Comparison="Equal" CodeIdValue="1,T" />
</Condition>', 'System', '2014-05-05 00:44:39.297', '2014-05-05 00:44:39.297', 'InCurrentOrg', 0, 0, NULL)
SET IDENTITY_INSERT [dbo].[RegistrationData] ON
INSERT INTO [dbo].[RegistrationData] ([Id], [Data], [Stamp], [completed], [OrganizationId], [UserPeopleId], [abandoned]) VALUES (1, CONVERT(xml,N'<OnlineRegModel><!--11/3/2015 9:00:56 AM--><Orgid>33</Orgid><Completed>False</Completed><DatumId>1</DatumId><Datum>CmsData.RegistrationDatum</Datum><URL>http://starterdb.tpsdb.com:80/OnlineReg/33</URL><nologin>True</nologin><List><OnlineRegPersonModel><IsValidForContinue>False</IsValidForContinue><IsValidForNew>False</IsValidForNew><orgid>33</orgid><IsNew>False</IsNew><QuestionsOK>False</QuestionsOK><LoggedIn>False</LoggedIn><IsValidForExisting>False</IsValidForExisting><ShowAddress>False</ShowAddress><ShowCountry>False</ShowCountry><IsFamily>False</IsFamily><Index>0</Index></OnlineRegPersonModel></List><History><item>index 11/3/2015 9:00 AM (c-ip=12.168.188.238)</item></History></OnlineRegModel>',1), '2015-11-03 09:00:56.187', NULL, 33, NULL, NULL)
SET IDENTITY_INSERT [dbo].[RegistrationData] OFF
SET IDENTITY_INSERT [dbo].[Roles] ON
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (1, N'Admin', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (2, N'Access', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (3, N'Attendance', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (4, N'Edit', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (5, N'Membership', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (6, N'OrgTagger', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (8, N'Finance', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (9, N'Developer', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (11, N'ApplicationReview', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (12, N'Coupon', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (13, N'Manager', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (14, N'OrgLeadersOnly', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (15, N'ManageEmails', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (17, N'Manager2', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (18, N'Checkin', NULL)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (19, N'ManageTransactions', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (20, N'ScheduleEmails', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (22, N'Coupon2', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (23, N'ContentEdit', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (24, N'Design', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (25, N'ManageGroups', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (26, N'MissionGiving', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (27, N'Delete', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (29, N'Tasks', 1)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (30, N'BackgroundCheck', NULL)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (31, N'FinanceAdmin', NULL)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (32, N'ManageEvents', NULL)
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName], [hardwired]) VALUES (33, N'Send SMS', NULL)
SET IDENTITY_INSERT [dbo].[Roles] OFF
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'AdminCoupon', N'YourPasswordGoesHere')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'AdminMail', N'info@touchpointsoftware.com')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'BlogAppUrl', N'http://blog.touchpointsoftware.com')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'BlogFeedUrl', N'http://feeds.feedburner.com/BvcmsBlog')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'ChurchPhone', N'')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'ChurchWebSite', NULL)
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'DbConvertedDate', N'5/5/2009')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'DefaultCampusId', NULL)
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'DefaultHost', N'https://StarterDb.tpsdb.com')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'DisplayNonTaxOnStatement', N'false')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'DisplayNotesOnStatement', N'false')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'EnableBackgroundChecks', N'false')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'EnableBackgroundLabels', N'false')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'MaxExcelRows', N'10000')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'MinContributionAmount', N'25')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'MinimumUserAge', N'16')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'MonthFirst', N'true')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'MorningBatch', N'false')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'NameOfChurch', NULL)
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'NewPeopleManagerIds', N'1')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'NoCreditCardGiving', N'false')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'NotifyCheckinChanges', N'true')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'PMMPassword', NULL)
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'PMMUser', NULL)
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'RegularMeetingHeadCount', N'true')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'ResetPasswordExpiresHours', N'24')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'RunMorningBatch', N'false')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'SendRecurringGiftFailureNoticesToFinanceUsers', N'false')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'ShowPledgeIfMet', N'false')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'StartAddress', NULL)
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'StatusFlags', N'F01,F02,F03')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'TransactionGateway', NULL)
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'TwilioSID', NULL)
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'TwilioToken', NULL)
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'TZOffset', N'0')
INSERT INTO [dbo].[Setting] ([Id], [Setting]) VALUES (N'UseMemberProfileAutomation', N'true')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ALLEE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ALLEY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ALLY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ALY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ANEX')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ANNEX')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ANNX')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ANX')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ARC')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ARCADE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'AV')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'AVE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'AVEN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'AVENU')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'AVENUE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'AVN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'AVNUE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BAYOO')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BCH')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BEACH')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BEND')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BGS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BLF')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BLFS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BLUF')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BLUFF')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BLUFFS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BLVD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BND')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BOT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BOTTM')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BOTTOM')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BOUL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BOULEVARD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BOULV')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BRANCH')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BRDGE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BRG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BRIDGE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BRK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BRKS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BRNCH')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BROOK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BROOKS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BTM')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BURG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BURGS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BYP')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BYPA')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BYPAS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BYPASS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BYPS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'BYU')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CAMP')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CANYN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CANYON')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CAPE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CAUSEWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CAUSWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CEN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CENT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CENTER')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CENTERS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CENTR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CENTRE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CIR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CIRC')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CIRCL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CIRCLE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CIRCLES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CIRS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CLB')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CLF')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CLFS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CLIFF')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CLIFFS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CLUB')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CMN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CMP')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CNTER')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CNTR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CNYN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'COMMON')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'COR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CORNER')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CORNERS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CORS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'COURSE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'COURT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'COURTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'COVE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'COVES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CP')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CPE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRCL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRCLE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRECENT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CREEK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRESCENT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRESENT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CREST')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CROSSING')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CROSSROAD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRSCNT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRSE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRSENT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRSNT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRSSING')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRSSNG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRST')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CRT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CSWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CTR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CTRS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CURV')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CURVE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CV')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CVS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'CYN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DALE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DAM')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DIV')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DIVIDE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DM')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DRIV')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DRIVE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DRIVES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DRS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DRV')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DV')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'DVD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'EST')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ESTATE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ESTATES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ESTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'EXP')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'EXPR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'EXPRESS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'EXPRESSWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'EXPW')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'EXPY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'EXT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'EXTENSION')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'EXTENSIONS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'EXTN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'EXTNSN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'EXTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FALL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FALLS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FERRY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FIELD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FIELDS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FLAT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FLATS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FLD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FLDS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FLS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FLT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FLTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FORD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FORDS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FOREST')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FORESTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FORG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FORGE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FORGES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FORK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FORKS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FORT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FRD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FRDS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FREEWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FREEWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FRG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FRGS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FRK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FRKS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FRRY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FRST')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FRT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FRWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FRWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FRY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'FWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GARDEN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GARDENS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GARDN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GATEWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GATEWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GDN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GDNS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GLEN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GLENS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GLN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GLNS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GRDEN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GRDN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GRDNS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GREEN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GREENS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GRN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GRNS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GROV')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GROVE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GROVES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GRV')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GRVS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GTWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'GTWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HARB')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HARBOR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HARBORS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HARBR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HAVEN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HAVN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HBR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HBRS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HEIGHT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HEIGHTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HGTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HIGHWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HIGHWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HILL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HILLS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HIWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HIWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HLLW')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HLS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HOLLOW')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HOLLOWS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HOLW')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HOLWS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HRBOR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HVN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'HWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'INLET')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'INLT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'IS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ISLAND')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ISLANDS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ISLE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ISLES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ISLND')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ISLNDS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ISS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'JCT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'JCTION')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'JCTN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'JCTNS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'JCTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'JUNCTION')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'JUNCTIONS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'JUNCTN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'JUNCTON')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'KEY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'KEYS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'KNL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'KNLS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'KNOL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'KNOLL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'KNOLLS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'KY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'KYS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LA')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LAKE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LAKES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LAND')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LANDING')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LANE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LANES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LCK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LCKS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LDG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LDGE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LF')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LGT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LGTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LIGHT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LIGHTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LKS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LNDG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LNDNG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LOAF')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LOCK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LOCKS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LODG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LODGE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LOOP')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'LOOPS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MALL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MANOR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MANORS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MDW')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MDWS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MEADOW')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MEADOWS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MEDOWS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MEWS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MILL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MILLS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MISSION')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MISSN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ML')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MLS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MNR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MNRS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MNT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MNTAIN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MNTN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MNTNS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MOTORWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MOUNT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MOUNTAIN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MOUNTAINS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MOUNTIN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MSN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MSSN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MTIN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MTN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MTNS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'MTWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'NCK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'NECK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'OPAS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ORCH')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ORCHARD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ORCHRD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'OVAL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'OVERPASS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'OVL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PARK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PARKS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PARKWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PARKWAYS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PARKWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PASS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PASSAGE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PATH')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PATHS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PIKE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PIKES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PINE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PINES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PKWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PKWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PKWYS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PKY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PLACE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PLAIN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PLAINES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PLAINS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PLAZA')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PLN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PLNS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PLZ')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PLZA')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PNE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PNES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'POINT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'POINTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PORT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PORTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PRAIRIE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PRARIE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PRK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PRR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PRT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PRTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PSGE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'PTS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RAD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RADIAL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RADIEL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RADL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RAMP')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RANCH')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RANCHES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RAPID')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RAPIDS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RDG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RDGE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RDGS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RDS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'REST')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RIDGE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RIDGES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RIV')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RIVER')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RIVR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RNCH')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RNCHS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ROAD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ROADS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ROUTE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ROW')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RPD')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RPDS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RST')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RTE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RUN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'RVR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SHL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SHLS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SHOAL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SHOALS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SHOAR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SHOARS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SHORE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SHORES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SHR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SHRS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SKWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SKYWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SMT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SPG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SPGS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SPNG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SPNGS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SPRING')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SPRINGS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SPRNG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SPRNGS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SPUR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SPURS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SQ')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SQR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SQRE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SQRS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SQS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SQU')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SQUARE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SQUARES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'ST')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STA')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STATION')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STATN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STRA')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STRAV')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STRAVE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STRAVEN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STRAVENUE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STRAVN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STREAM')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STREET')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STREETS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STREME')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STRM')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STRT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STRVN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STRVNUE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'STS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SUMIT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'SUMITT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TER')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TERR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TERRACE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'THROUGHWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TPK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TPKE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TR')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRACE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRACES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRACK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRACKS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRAFFICWAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRAIL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRAILS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRAK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRCE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRFY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRKS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRLS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRNPK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRPK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TRWY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TUNEL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TUNL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TUNLS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TUNNEL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TUNNELS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TUNNL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TURNPIKE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'TURNPK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'UN')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'UNDERPASS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'UNION')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'UNIONS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'UNS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'UPAS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VALLEY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VALLEYS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VALLY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VDCT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VIA')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VIADCT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VIADUCT')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VIEW')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VIEWS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VILL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VILLAG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VILLAGE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VILLAGES')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VILLE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VILLG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VILLIAGE')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VIS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VIST')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VISTA')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VLG')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VLGS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VLLY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VLY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VLYS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VST')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VSTA')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VW')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'VWS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'WALK')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'WALKS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'WALL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'WAY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'WAYS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'WELL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'WELLS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'WL')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'WLS')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'WY')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'XING')
INSERT INTO [dbo].[StreetTypes] ([Type]) VALUES (N'XRD')
INSERT INTO [dbo].[TagType] ([Id], [Name]) VALUES (1, N'Personal')
INSERT INTO [dbo].[TagType] ([Id], [Name]) VALUES (3, N'CouplesHelper')
INSERT INTO [dbo].[TagType] ([Id], [Name]) VALUES (4, N'AddSelected')
INSERT INTO [dbo].[TagType] ([Id], [Name]) VALUES (5, N'OrgMembersOnly')
INSERT INTO [dbo].[TagType] ([Id], [Name]) VALUES (6, N'OrgLeadersOnly')
INSERT INTO [lookup].[AddressType] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'F1', N'Family', 1)
INSERT INTO [lookup].[AddressType] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'P1', N'Personal', 1)
INSERT INTO [lookup].[AttendCredit] ([Id], [Code], [Description], [Hardwired]) VALUES (1, N'E', N'Every Meeting', 1)
INSERT INTO [lookup].[AttendCredit] ([Id], [Code], [Description], [Hardwired]) VALUES (2, N'W', N'Once Per Week Group 1', 1)
INSERT INTO [lookup].[AttendCredit] ([Id], [Code], [Description], [Hardwired]) VALUES (3, N'W', N'Once Per Week Group 2', 1)
INSERT INTO [lookup].[AttendType] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'L', N'Leader', 1)
INSERT INTO [lookup].[AttendType] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'VO', N'Volunteer', 1)
INSERT INTO [lookup].[AttendType] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'M', N'Member', 1)
INSERT INTO [lookup].[AttendType] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'VM', N'Visiting Member', 1)
INSERT INTO [lookup].[AttendType] ([Id], [Code], [Description], [Hardwired]) VALUES (50, N'RG', N'Recent Guest', 1)
INSERT INTO [lookup].[AttendType] ([Id], [Code], [Description], [Hardwired]) VALUES (60, N'NG', N'New Guest', 1)
INSERT INTO [lookup].[AttendType] ([Id], [Code], [Description], [Hardwired]) VALUES (70, N'ISM', N'In-Service', 1)
INSERT INTO [lookup].[AttendType] ([Id], [Code], [Description], [Hardwired]) VALUES (80, N'OFS', N'Offsite', 1)
INSERT INTO [lookup].[AttendType] ([Id], [Code], [Description], [Hardwired]) VALUES (90, N'GRP', N'Group', 1)
INSERT INTO [lookup].[AttendType] ([Id], [Code], [Description], [Hardwired]) VALUES (100, N'HMB', N'Homebound', 1)
INSERT INTO [lookup].[AttendType] ([Id], [Code], [Description], [Hardwired]) VALUES (110, N'OC', N'Other Class', 1)
INSERT INTO [lookup].[AttendType] ([Id], [Code], [Description], [Hardwired]) VALUES (190, N'PR', N'Prospect', 1)
INSERT INTO [lookup].[BaptismStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'NSP', N'(not specified)', 1)
INSERT INTO [lookup].[BaptismStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'SCH', N'Scheduled', 1)
INSERT INTO [lookup].[BaptismStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'NSC', N'Not Scheduled', 1)
INSERT INTO [lookup].[BaptismStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'CMP', N'Completed', 1)
INSERT INTO [lookup].[BaptismStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'CAN', N'Cancelled', 1)
INSERT INTO [lookup].[BaptismType] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'NSP', N'(not specified)', 1)
INSERT INTO [lookup].[BaptismType] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'ORI', N'Original', 1)
INSERT INTO [lookup].[BaptismType] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'SUB', N'Subsequent', 1)
INSERT INTO [lookup].[BaptismType] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'BIO', N'Biological', 1)
INSERT INTO [lookup].[BaptismType] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'NON', N'Non-Member', 1)
INSERT INTO [lookup].[BaptismType] ([Id], [Code], [Description], [Hardwired]) VALUES (50, N'RFM', N'Required', 1)
INSERT INTO [lookup].[BundleHeaderTypes] ([Id], [Code], [Description], [Hardwired]) VALUES (1, N'G', N'Generic Envelopes', NULL)
INSERT INTO [lookup].[BundleHeaderTypes] ([Id], [Code], [Description], [Hardwired]) VALUES (2, N'LC', N'Loose Checks and Cash', 1)
INSERT INTO [lookup].[BundleHeaderTypes] ([Id], [Code], [Description], [Hardwired]) VALUES (3, N'PE', N'Preprinted Envelopes', 1)
INSERT INTO [lookup].[BundleHeaderTypes] ([Id], [Code], [Description], [Hardwired]) VALUES (4, N'OL', N'Online', 1)
INSERT INTO [lookup].[BundleHeaderTypes] ([Id], [Code], [Description], [Hardwired]) VALUES (5, N'OLP', N'Online Pledge', 1)
INSERT INTO [lookup].[BundleHeaderTypes] ([Id], [Code], [Description], [Hardwired]) VALUES (6, N'PL', N'Pledge', 1)
INSERT INTO [lookup].[BundleHeaderTypes] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'MTT', N'Mission Trip Transaction', 1)
INSERT INTO [lookup].[BundleHeaderTypes] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'GK', N'Gifts in Kind', 1)
INSERT INTO [lookup].[BundleHeaderTypes] ([Id], [Code], [Description], [Hardwired]) VALUES (32, N'SK', N'Stock', 1)
INSERT INTO [lookup].[BundleStatusTypes] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'C', N'Closed', 1)
INSERT INTO [lookup].[BundleStatusTypes] ([Id], [Code], [Description], [Hardwired]) VALUES (1, N'O', N'Open', 1)
INSERT INTO [lookup].[ContactPreference] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'NO', N'Do Not Contact', NULL)
INSERT INTO [lookup].[ContactPreference] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'PST', N'Mail', NULL)
INSERT INTO [lookup].[ContactPreference] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'PHN', N'Phone', NULL)
INSERT INTO [lookup].[ContactPreference] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'EML', N'Email', NULL)
INSERT INTO [lookup].[ContactPreference] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'VST', N'Visit', NULL)
INSERT INTO [lookup].[ContactReason] ([Id], [Code], [Description], [Hardwired]) VALUES (99, N'U', N'Unknown', NULL)
INSERT INTO [lookup].[ContactReason] ([Id], [Code], [Description], [Hardwired]) VALUES (100, N'B', N'Bereavement', NULL)
INSERT INTO [lookup].[ContactReason] ([Id], [Code], [Description], [Hardwired]) VALUES (110, N'H', N'Health', NULL)
INSERT INTO [lookup].[ContactReason] ([Id], [Code], [Description], [Hardwired]) VALUES (120, N'P', N'Personal', NULL)
INSERT INTO [lookup].[ContactReason] ([Id], [Code], [Description], [Hardwired]) VALUES (130, N'OR', N'Out-Reach', NULL)
INSERT INTO [lookup].[ContactReason] ([Id], [Code], [Description], [Hardwired]) VALUES (140, N'IR', N'In-Reach', NULL)
INSERT INTO [lookup].[ContactReason] ([Id], [Code], [Description], [Hardwired]) VALUES (150, N'I', N'Information', NULL)
INSERT INTO [lookup].[ContactReason] ([Id], [Code], [Description], [Hardwired]) VALUES (160, N'O', N'Other', 1)
INSERT INTO [lookup].[ContactType] ([Id], [Code], [Description], [Hardwired]) VALUES (1, N'PV', N'Personal Visit', NULL)
INSERT INTO [lookup].[ContactType] ([Id], [Code], [Description], [Hardwired]) VALUES (2, N'PC', N'Phone Call', NULL)
INSERT INTO [lookup].[ContactType] ([Id], [Code], [Description], [Hardwired]) VALUES (3, N'L', N'Letter Sent', NULL)
INSERT INTO [lookup].[ContactType] ([Id], [Code], [Description], [Hardwired]) VALUES (4, N'C', N'Card Sent', NULL)
INSERT INTO [lookup].[ContactType] ([Id], [Code], [Description], [Hardwired]) VALUES (5, N'E', N'EMail Sent', NULL)
INSERT INTO [lookup].[ContactType] ([Id], [Code], [Description], [Hardwired]) VALUES (6, N'I', N'Info Pack Sent', NULL)
INSERT INTO [lookup].[ContactType] ([Id], [Code], [Description], [Hardwired]) VALUES (7, N'O', N'Other', 1)
INSERT INTO [lookup].[ContactType] ([Id], [Code], [Description], [Hardwired]) VALUES (99, N'U', N'Unknown', NULL)
INSERT INTO [lookup].[ContributionStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'C', N'Recorded', 1)
INSERT INTO [lookup].[ContributionStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (1, N'V', N'Reversed', 1)
INSERT INTO [lookup].[ContributionStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (2, N'R', N'Returned', 1)
INSERT INTO [lookup].[ContributionType] ([Id], [Code], [Description], [Hardwired]) VALUES (1, N'CC', N'Check/Cash', 1)
INSERT INTO [lookup].[ContributionType] ([Id], [Code], [Description], [Hardwired]) VALUES (5, N'OL', N'Online', 1)
INSERT INTO [lookup].[ContributionType] ([Id], [Code], [Description], [Hardwired]) VALUES (6, N'RC', N'Returned Check', 1)
INSERT INTO [lookup].[ContributionType] ([Id], [Code], [Description], [Hardwired]) VALUES (7, N'RV', N'Reversed', 1)
INSERT INTO [lookup].[ContributionType] ([Id], [Code], [Description], [Hardwired]) VALUES (8, N'PL', N'Pledge', 1)
INSERT INTO [lookup].[ContributionType] ([Id], [Code], [Description], [Hardwired]) VALUES (9, N'NT', N'Non Tax-Deductible', 1)
INSERT INTO [lookup].[ContributionType] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'GK', N'Gift in Kind', 1)
INSERT INTO [lookup].[ContributionType] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'SK', N'Stock', 1)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (1, N'US', N'United States', 1)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (2, N'NA', N'USA, Not Validated', 1)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (3, N'AF', N'Afghanistan', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (4, N'AX', N'Aland Islands', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (5, N'AL', N'Albania', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (6, N'DZ', N'Algeria', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (7, N'AS', N'American Samoa', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (8, N'AD', N'Andorra', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (9, N'AO', N'Angola', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'AI', N'Anguilla', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (11, N'AQ', N'Antarctica', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (12, N'AG', N'Antigua and Barbuda', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (13, N'AR', N'Argentina', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (14, N'AM', N'Armenia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (15, N'AW', N'Aruba', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (16, N'AU', N'Australia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (17, N'AT', N'Austria', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (18, N'AZ', N'Azerbaijan', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (19, N'BS', N'Bahamas, The', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'BH', N'Bahrain', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (21, N'BD', N'Bangladesh', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (22, N'BB', N'Barbados', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (23, N'BY', N'Belarus', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (24, N'BE', N'Belgium', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (25, N'BZ', N'Belize', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (26, N'BJ', N'Benin', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (27, N'BM', N'Bermuda', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (28, N'BT', N'Bhutan', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (29, N'BO', N'Bolivia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'BQ', N'Bonaire, Saint Eustatius and Saba', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (31, N'BA', N'Bosnia and Herzegovina', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (32, N'BW', N'Botswana', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (33, N'BV', N'Bouvet Island', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (34, N'BR', N'Brazil', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (35, N'IO', N'British Indian Ocean Territory', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (36, N'BN', N'Brunei Darussalam', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (37, N'BG', N'Bulgaria', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (38, N'BF', N'Burkina Faso', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (39, N'BI', N'Burundi', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'KH', N'Cambodia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (41, N'CM', N'Cameroon', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (42, N'CA', N'Canada', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (43, N'CV', N'Cape Verde', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (44, N'KY', N'Cayman Islands', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (45, N'CF', N'Central African Republic', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (46, N'TD', N'Chad', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (47, N'CL', N'Chile', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (48, N'CN', N'China', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (49, N'CX', N'Christmas Island', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (50, N'CC', N'Cocos (Keeling) Islands', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (51, N'CO', N'Colombia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (52, N'KM', N'Comoros', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (53, N'CG', N'Congo', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (54, N'CD', N'Congo, The Democratic Republic Of The', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (55, N'CK', N'Cook Islands', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (56, N'CR', N'Costa Rica', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (57, N'CI', N'Cote D''ivoire', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (58, N'HR', N'Croatia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (59, N'CW', N'Curaçao', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (60, N'CY', N'Cyprus', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (61, N'CZ', N'Czech Republic', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (62, N'DK', N'Denmark', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (63, N'DJ', N'Djibouti', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (64, N'DM', N'Dominica', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (65, N'DO', N'Dominican Republic', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (66, N'EC', N'Ecuador', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (67, N'EG', N'Egypt', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (68, N'SV', N'El Salvador', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (69, N'GQ', N'Equatorial Guinea', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (70, N'ER', N'Eritrea', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (71, N'EE', N'Estonia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (72, N'ET', N'Ethiopia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (73, N'FK', N'Falkland Islands (Malvinas)', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (74, N'FO', N'Faroe Islands', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (75, N'FJ', N'Fiji', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (76, N'FI', N'Finland', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (77, N'FR', N'France', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (78, N'GF', N'French Guiana', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (79, N'PF', N'French Polynesia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (80, N'TF', N'French Southern Territories', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (81, N'GA', N'Gabon', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (82, N'GM', N'Gambia, The', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (83, N'GE', N'Georgia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (84, N'DE', N'Germany', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (85, N'GH', N'Ghana', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (86, N'GI', N'Gibraltar', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (87, N'GR', N'Greece', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (88, N'GL', N'Greenland', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (89, N'GD', N'Grenada', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (90, N'GP', N'Guadeloupe', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (91, N'GU', N'Guam', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (92, N'GT', N'Guatemala', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (93, N'GG', N'Guernsey', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (94, N'GN', N'Guinea', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (95, N'GW', N'Guinea-Bissau', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (96, N'GY', N'Guyana', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (97, N'HT', N'Haiti', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (98, N'HM', N'Heard Island and the McDonald Islands', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (99, N'VA', N'Holy See', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (100, N'HN', N'Honduras', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (101, N'HK', N'Hong Kong', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (102, N'HU', N'Hungary', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (103, N'IS', N'Iceland', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (104, N'IN', N'India', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (105, N'ID', N'Indonesia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (106, N'IQ', N'Iraq', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (107, N'IE', N'Ireland', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (108, N'IM', N'Isle Of Man', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (109, N'IL', N'Israel', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (110, N'IT', N'Italy', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (111, N'JM', N'Jamaica', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (112, N'JP', N'Japan', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (113, N'JE', N'Jersey', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (114, N'JO', N'Jordan', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (115, N'KZ', N'Kazakhstan', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (116, N'KE', N'Kenya', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (117, N'KI', N'Kiribati', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (118, N'KR', N'Korea, Republic Of', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (119, N'KW', N'Kuwait', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (120, N'KG', N'Kyrgyzstan', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (121, N'LA', N'Lao People''s Democratic Republic', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (122, N'LV', N'Latvia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (123, N'LB', N'Lebanon', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (124, N'LS', N'Lesotho', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (125, N'LR', N'Liberia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (126, N'LY', N'Libya', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (127, N'LI', N'Liechtenstein', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (128, N'LT', N'Lithuania', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (129, N'LU', N'Luxembourg', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (130, N'MO', N'Macao', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (131, N'MK', N'Macedonia, The Former Yugoslav Republic Of', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (132, N'MG', N'Madagascar', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (133, N'MW', N'Malawi', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (134, N'MY', N'Malaysia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (135, N'MV', N'Maldives', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (136, N'ML', N'Mali', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (137, N'MT', N'Malta', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (138, N'MH', N'Marshall Islands', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (139, N'MQ', N'Martinique', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (140, N'MR', N'Mauritania', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (141, N'MU', N'Mauritius', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (142, N'YT', N'Mayotte', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (143, N'MX', N'Mexico', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (144, N'FM', N'Micronesia, Federated States Of', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (145, N'MD', N'Moldova, Republic Of', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (146, N'MC', N'Monaco', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (147, N'MN', N'Mongolia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (148, N'ME', N'Montenegro', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (149, N'MS', N'Montserrat', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (150, N'MA', N'Morocco', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (151, N'MZ', N'Mozambique', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (152, N'MM', N'Myanmar', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (153, N'NA', N'Namibia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (154, N'NR', N'Nauru', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (155, N'NP', N'Nepal', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (156, N'NL', N'Netherlands', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (157, N'AN', N'Netherlands Antilles', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (158, N'NC', N'New Caledonia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (159, N'NZ', N'New Zealand', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (160, N'NI', N'Nicaragua', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (161, N'NE', N'Niger', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (162, N'NG', N'Nigeria', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (163, N'NU', N'Niue', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (164, N'NF', N'Norfolk Island', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (165, N'MP', N'Northern Mariana Islands', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (166, N'NO', N'Norway', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (167, N'OM', N'Oman', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (168, N'PK', N'Pakistan', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (169, N'PW', N'Palau', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (170, N'PS', N'Palestinian Territories', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (171, N'PA', N'Panama', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (172, N'PG', N'Papua New Guinea', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (173, N'PY', N'Paraguay', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (174, N'PE', N'Peru', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (175, N'PH', N'Philippines', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (176, N'PN', N'Pitcairn', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (177, N'PL', N'Poland', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (178, N'PT', N'Portugal', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (179, N'PR', N'Puerto Rico', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (180, N'QA', N'Qatar', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (181, N'RE', N'Reunion', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (182, N'RO', N'Romania', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (183, N'RU', N'Russian Federation', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (184, N'RW', N'Rwanda', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (185, N'BL', N'Saint Barthelemy', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (186, N'SH', N'Saint Helena', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (187, N'KN', N'Saint Kitts and Nevis', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (188, N'LC', N'Saint Lucia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (189, N'MF', N'Saint Martin', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (190, N'PM', N'Saint Pierre and Miquelon', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (191, N'VC', N'Saint Vincent and The Grenadines', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (192, N'WS', N'Samoa', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (193, N'SM', N'San Marino', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (194, N'ST', N'Sao Tome and Principe', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (195, N'SA', N'Saudi Arabia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (196, N'SN', N'Senegal', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (197, N'RS', N'Serbia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (198, N'SC', N'Seychelles', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (199, N'SL', N'Sierra Leone', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (200, N'SG', N'Singapore', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (201, N'SX', N'Sint Maarten', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (202, N'SK', N'Slovakia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (203, N'SI', N'Slovenia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (204, N'SB', N'Solomon Islands', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (205, N'SO', N'Somalia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (206, N'ZA', N'South Africa', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (207, N'GS', N'South Georgia and the South Sandwich Islands', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (208, N'ES', N'Spain', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (209, N'LK', N'Sri Lanka', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (210, N'SR', N'Suriname', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (211, N'SJ', N'Svalbard and Jan Mayen', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (212, N'SZ', N'Swaziland', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (213, N'SE', N'Sweden', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (214, N'CH', N'Switzerland', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (215, N'TW', N'Taiwan', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (216, N'TJ', N'Tajikistan', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (217, N'TZ', N'Tanzania, United Republic Of', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (218, N'TH', N'Thailand', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (219, N'TL', N'Timor-leste', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (220, N'TG', N'Togo', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (221, N'TK', N'Tokelau', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (222, N'TO', N'Tonga', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (223, N'TT', N'Trinidad and Tobago', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (224, N'TN', N'Tunisia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (225, N'TR', N'Turkey', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (226, N'TM', N'Turkmenistan', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (227, N'TC', N'Turks and Caicos Islands', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (228, N'TV', N'Tuvalu', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (229, N'UG', N'Uganda', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (230, N'UA', N'Ukraine', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (231, N'AE', N'United Arab Emirates', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (232, N'GB', N'United Kingdom', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (234, N'UM', N'United States Minor Outlying Islands', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (235, N'UY', N'Uruguay', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (236, N'UZ', N'Uzbekistan', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (237, N'VU', N'Vanuatu', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (238, N'VE', N'Venezuela', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (239, N'VN', N'Vietnam', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (240, N'VG', N'Virgin Islands, British', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (241, N'VI', N'Virgin Islands, U.S.', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (242, N'WF', N'Wallis and Futuna', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (243, N'EH', N'Western Sahara', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (244, N'YE', N'Yemen', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (245, N'ZM', N'Zambia', NULL)
INSERT INTO [lookup].[Country] ([Id], [Code], [Description], [Hardwired]) VALUES (246, N'ZW', N'Zimbabwe', NULL)
INSERT INTO [lookup].[DecisionType] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'UNK', N'Unknown', 1)
INSERT INTO [lookup].[DecisionType] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'POF-MEM', N'POF for Membership', 1)
INSERT INTO [lookup].[DecisionType] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'POF-NON', N'POF NOT for Membership', 1)
INSERT INTO [lookup].[DecisionType] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'LETTER', N'Letter', 1)
INSERT INTO [lookup].[DecisionType] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'STATEMENT', N'Statement', 1)
INSERT INTO [lookup].[DecisionType] ([Id], [Code], [Description], [Hardwired]) VALUES (50, N'BAP-REQD', N'Stmt requiring Baptism', 1)
INSERT INTO [lookup].[DecisionType] ([Id], [Code], [Description], [Hardwired]) VALUES (60, N'CANCELLED', N'Cancelled', 1)
INSERT INTO [lookup].[DropType] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'NON', N'Non-Dropped', 1)
INSERT INTO [lookup].[DropType] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'ADM', N'Administrative', 1)
INSERT INTO [lookup].[DropType] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'DEC', N'Deceased', 1)
INSERT INTO [lookup].[DropType] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'LET', N'Lettered Out', 1)
INSERT INTO [lookup].[DropType] ([Id], [Code], [Description], [Hardwired]) VALUES (50, N'REQ', N'Requested Drop', 1)
INSERT INTO [lookup].[DropType] ([Id], [Code], [Description], [Hardwired]) VALUES (60, N'AND', N'Another Denomination', 1)
INSERT INTO [lookup].[DropType] ([Id], [Code], [Description], [Hardwired]) VALUES (98, N'OTH', N'Other', 1)
INSERT INTO [lookup].[EntryPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'NSP', N'(not specified)', NULL)
INSERT INTO [lookup].[EntryPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'MFC', N'Main Fellowship', NULL)
INSERT INTO [lookup].[EntryPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (15, N'ENROLL', N'Other Enrollments', NULL)
INSERT INTO [lookup].[EntryPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (17, N'ONLINE', N'Online Registration', NULL)
INSERT INTO [lookup].[EntryPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'WORSHIP', N'Worship', NULL)
INSERT INTO [lookup].[EntryPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'SPEC', N'Special Events', NULL)
INSERT INTO [lookup].[EntryPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (98, N'OTHER', N'Other', NULL)
INSERT INTO [lookup].[EntryPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (99, N'UNKNOWN', N'Unknown', NULL)
INSERT INTO [lookup].[EnvelopeOption] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'Null', N'(not specified)', 1)
INSERT INTO [lookup].[EnvelopeOption] ([Id], [Code], [Description], [Hardwired]) VALUES (1, N'I', N'Individual', 1)
INSERT INTO [lookup].[EnvelopeOption] ([Id], [Code], [Description], [Hardwired]) VALUES (2, N'J', N'Joint', 1)
INSERT INTO [lookup].[EnvelopeOption] ([Id], [Code], [Description], [Hardwired]) VALUES (9, N'N', N'None', 1)
INSERT INTO [lookup].[FamilyMemberType] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'ADU', N'Adult', NULL)
INSERT INTO [lookup].[FamilyMemberType] ([Id], [Code], [Description], [Hardwired]) VALUES (1, N'CHI', N'Child', NULL)
INSERT INTO [lookup].[FamilyPosition] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'Primary', N'Primary Adult', 1)
INSERT INTO [lookup].[FamilyPosition] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'Other', N'Secondary Adult', 1)
INSERT INTO [lookup].[FamilyPosition] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'Child', N'Child', 1)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (100, N'HOH', N'Head of Household', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (110, N'SPS', N'Spouse', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (120, N'SEC', N'Secondary Adult', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (130, N'AUN', N'Aunt', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (135, N'UNC', N'Uncle', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (140, N'GRM', N'Grand Mother', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (145, N'GRF', N'Grand Father', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (200, N'CHI', N'Child', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (210, N'DTR', N'Daughter', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (215, N'SON', N'Son', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (220, N'NCE', N'Niece', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (225, N'NPH', N'Nephew', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (230, N'GRD', N'Grand Daughter', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (235, N'GRS', N'Grand Son', NULL)
INSERT INTO [lookup].[FamilyRelationship] ([Id], [Code], [Description], [Hardwired]) VALUES (980, N'OTH', N'Other', NULL)
INSERT INTO [lookup].[Gender] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'U', N'Unknown', NULL)
INSERT INTO [lookup].[Gender] ([Id], [Code], [Description], [Hardwired]) VALUES (1, N'M', N'Male', NULL)
INSERT INTO [lookup].[Gender] ([Id], [Code], [Description], [Hardwired]) VALUES (2, N'F', N'Female', NULL)
INSERT INTO [lookup].[InterestPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'NSP', N'(not specified)', NULL)
INSERT INTO [lookup].[InterestPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'TV', N'TV', NULL)
INSERT INTO [lookup].[InterestPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'Radio', N'Radio', NULL)
INSERT INTO [lookup].[InterestPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'Newspaper', N'Newspaper', NULL)
INSERT INTO [lookup].[InterestPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (35, N'Mail', N'Mail', NULL)
INSERT INTO [lookup].[InterestPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'Friend', N'Friend', NULL)
INSERT INTO [lookup].[InterestPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (50, N'Relative', N'Relative', NULL)
INSERT INTO [lookup].[InterestPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (60, N'BillBoard', N'Billboard', NULL)
INSERT INTO [lookup].[InterestPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (70, N'Website', N'Website', NULL)
INSERT INTO [lookup].[InterestPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (98, N'Other', N'Other', NULL)
INSERT INTO [lookup].[InterestPoint] ([Id], [Code], [Description], [Hardwired]) VALUES (99, N'UNKNOWN', N'Unknown', NULL)
INSERT INTO [lookup].[JoinType] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'UNK', N'Unknown', 1)
INSERT INTO [lookup].[JoinType] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'BPP', N'Baptism POF', 1)
INSERT INTO [lookup].[JoinType] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'BPS', N'Baptism SRB', 1)
INSERT INTO [lookup].[JoinType] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'BPB', N'Baptism BIO', 1)
INSERT INTO [lookup].[JoinType] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'STM', N'Statement', 1)
INSERT INTO [lookup].[JoinType] ([Id], [Code], [Description], [Hardwired]) VALUES (50, N'LET', N'Letter', 1)
INSERT INTO [lookup].[MaritalStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'UNK', N'Unknown', 1)
INSERT INTO [lookup].[MaritalStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'SNG', N'Single', 1)
INSERT INTO [lookup].[MaritalStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'MAR', N'Married', 1)
INSERT INTO [lookup].[MaritalStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'SEP', N'Separated', NULL)
INSERT INTO [lookup].[MaritalStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'DIV', N'Divorced', 1)
INSERT INTO [lookup].[MaritalStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (50, N'WID', N'Widowed', 1)
INSERT INTO [lookup].[MeetingType] ([Id], [Code], [Description], [Hardwired]) VALUES (0, 'G', N'Group', NULL)
INSERT INTO [lookup].[MeetingType] ([Id], [Code], [Description], [Hardwired]) VALUES (1, 'R', N'Roster', NULL)
INSERT INTO [lookup].[MemberLetterStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'NSP', N'(not specified)', NULL)
INSERT INTO [lookup].[MemberLetterStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'1stReq', N'1st Request', NULL)
INSERT INTO [lookup].[MemberLetterStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'2ndReq', N'2nd Request', NULL)
INSERT INTO [lookup].[MemberLetterStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'Non-Resp', N'Non-Responsive', NULL)
INSERT INTO [lookup].[MemberLetterStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'Complete', N'Complete', NULL)
INSERT INTO [lookup].[MemberStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'Member', N'Member', 1)
INSERT INTO [lookup].[MemberStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'NonMember', N'Not Member', NULL)
INSERT INTO [lookup].[MemberStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'Pending', N'Pending Member', NULL)
INSERT INTO [lookup].[MemberStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'Previous', N'Previous Member', 1)
INSERT INTO [lookup].[MemberStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (50, N'Added', N'Just Added', 1)
INSERT INTO [lookup].[NewMemberClassStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'NSP', N'(not specified)', 1)
INSERT INTO [lookup].[NewMemberClassStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'PN', N'Pending', 1)
INSERT INTO [lookup].[NewMemberClassStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'AT', N'Attended', 1)
INSERT INTO [lookup].[NewMemberClassStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'AA', N'Admin Approval', 1)
INSERT INTO [lookup].[NewMemberClassStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'GF', N'Grandfathered', 1)
INSERT INTO [lookup].[NewMemberClassStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (50, N'EX', N'Exempted Child (thru Grade 8)', 1)
INSERT INTO [lookup].[NewMemberClassStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (99, N'UNK', N'Unknown', NULL)
INSERT INTO [lookup].[OrganizationStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'A', N'Active', 1)
INSERT INTO [lookup].[OrganizationStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'I', N'Inactive', 1)
INSERT INTO [lookup].[Origin] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'NSP', N'Not Specified', NULL)
INSERT INTO [lookup].[Origin] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'VISIT', N'Visit', 1)
INSERT INTO [lookup].[Origin] ([Id], [Code], [Description], [Hardwired]) VALUES (70, N'ENROLL', N'Enrollment', 1)
INSERT INTO [lookup].[Origin] ([Id], [Code], [Description], [Hardwired]) VALUES (90, N'CONTRIB', N'Contribution', 1)
INSERT INTO [lookup].[Origin] ([Id], [Code], [Description], [Hardwired]) VALUES (97, N'MENU', N'Main Menu', 1)
INSERT INTO [lookup].[Origin] ([Id], [Code], [Description], [Hardwired]) VALUES (100, N'FAM', N'New Family Member', 1)
INSERT INTO [lookup].[ResidentCode] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'M', N'Metro', NULL)
INSERT INTO [lookup].[ResidentCode] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'G', N'Marginal', NULL)
INSERT INTO [lookup].[ResidentCode] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'N', N'Non-Resident', NULL)
INSERT INTO [lookup].[ResidentCode] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'U', N'Unable to Locate', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'AA', N'Armed Forces America', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'AB', N'Alberta', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'AE', N'Armed Forces East', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'AK', N'Alaska', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'AL', N'Alabama', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'AP', N'Armed Forces Pacific', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'AR', N'Arkansas', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'AZ', N'Arizona', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'BC', N'British Columbia', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'CA', N'California', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'CO', N'Colorado', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'CT', N'Connecticut', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'CZ', N'Canal Zone', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'DC', N'District Of Columbia', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'DE', N'Delaware', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'FL', N'Florida', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'FR', N'Foreign Address', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'GA', N'Georgia', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'GU', N'Guam', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'HI', N'Hawaii', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'IA', N'Iowa', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'ID', N'Idaho', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'IL', N'Illinois', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'IN', N'Indiana', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'KS', N'Kansas', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'KY', N'Kentucky', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'LA', N'Louisiana', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'MA', N'Massachusetts', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'MB', N'Manatoba', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'MD', N'Maryland', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'ME', N'Maine', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'MI', N'Michigan', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'MN', N'Minnesota', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'MO', N'Missouri', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'MS', N'Mississippi', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'MT', N'Montana', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'NB', N'New Brunswick', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'NC', N'North Carolina', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'ND', N'North Dakota', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'NE', N'Nebraska', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'NH', N'New Hampshire', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'NJ', N'New Jersey', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'NL', N'Newfoundland and Labrador', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'NM', N'New Mexico', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'NS', N'Nova Scotia', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'NT', N'Northwest Territories', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'NU', N'Nunavut', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'NV', N'Nevada', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'NY', N'New York', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'OH', N'Ohio', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'OK', N'Oklahoma', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'ON', N'Ontario', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'OR', N'Oregon', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'PA', N'Pennsylvania', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'PE', N'Prince Edward Island', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'PR', N'Puerto Rico', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'QC', N'Quebec', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'RI', N'Rhode Island', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'SC', N'South Carolina', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'SD', N'South Dakota', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'SK', N'Saskatchewan', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'TN', N'Tennessee', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'TX', N'Texas', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'UT', N'Utah', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'VA', N'Virginia', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'VI', N'Virgin Islands', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'VT', N'Vermont', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'WA', N'Washington', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'WI', N'Wisconsin', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'WV', N'West Virginia', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'WY', N'Wyoming', NULL)
INSERT INTO [lookup].[StateLookup] ([StateCode], [StateName], [Hardwired]) VALUES (N'YT', N'Yukon', NULL)
INSERT INTO [lookup].[TaskStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'A', N'Active', 1)
INSERT INTO [lookup].[TaskStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'W', N'Waiting For', 1)
INSERT INTO [lookup].[TaskStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'S', N'Someday', 1)
INSERT INTO [lookup].[TaskStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'C', N'Completed', 1)
INSERT INTO [lookup].[TaskStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (50, N'P', N'Pending Acceptance', 1)
INSERT INTO [lookup].[TaskStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (60, N'R', N'ReDelegated', 1)
INSERT INTO [lookup].[TaskStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (70, N'D', N'Declined', 1)
INSERT INTO [lookup].[VolApplicationStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'UK', N'(not specified)', NULL)
INSERT INTO [lookup].[VolApplicationStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'Appr', N'Approved', 1)
INSERT INTO [lookup].[VolApplicationStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (20, N'WD', N'Withdrawn', NULL)
INSERT INTO [lookup].[VolApplicationStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'Not', N'Not Approved', 1)
INSERT INTO [lookup].[VolApplicationStatus] ([Id], [Code], [Description], [Hardwired]) VALUES (40, N'Pend', N'Pending', 1)
INSERT INTO [lookup].[VolunteerCodes] ([Id], [Code], [Description], [Hardwired]) VALUES (0, N'NA', N'None', NULL)
INSERT INTO [lookup].[VolunteerCodes] ([Id], [Code], [Description], [Hardwired]) VALUES (10, N'S', N'Standard', NULL)
INSERT INTO [lookup].[VolunteerCodes] ([Id], [Code], [Description], [Hardwired]) VALUES (30, N'L', N'Leader', NULL)
INSERT INTO [dbo].[ChangeDetails] ([Id], [Field], [Before], [After]) VALUES (88, N'EmailAddress', N'karen@bvcms.com', N'karen@touchpointsoftware.com')
INSERT INTO [dbo].[ChangeDetails] ([Id], [Field], [Before], [After]) VALUES (89, N'DoNotPublishPhones', N'(null)', N'False')
INSERT INTO [dbo].[ChangeDetails] ([Id], [Field], [Before], [After]) VALUES (89, N'EmailAddress', N'david@bvcms.com', N'david@touchpointsoftware.com')
INSERT INTO [dbo].[ChangeDetails] ([Id], [Field], [Before], [After]) VALUES (89, N'HomePhone', N'9017580791', N'901-758-0791')
INSERT INTO [dbo].[ChangeDetails] ([Id], [Field], [Before], [After]) VALUES (89, N'SendEmailAddress1', N'(null)', N'True')
INSERT INTO [dbo].[ChangeDetails] ([Id], [Field], [Before], [After]) VALUES (89, N'SendEmailAddress2', N'(null)', N'False')
SET IDENTITY_INSERT [dbo].[Division] ON
INSERT INTO [dbo].[Division] ([Id], [Name], [ProgId], [SortOrder], [EmailMessage], [EmailSubject], [Instructions], [Terms], [ReportLine], [NoDisplayZero]) VALUES (1, N'First Division', 1, NULL, NULL, NULL, NULL, NULL, 1, NULL)
SET IDENTITY_INSERT [dbo].[Division] OFF
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (103, N'DR', N'Director', 10, NULL)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (130, N'CH', N'Chairman', 30, NULL)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (136, N'CC', N'Coach', 30, NULL)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (140, N'L', N'Leader', 10, NULL)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (160, N'T', N'Teacher', 10, 1)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (161, N'AT', N'Assistant Teacher', 30, NULL)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (162, N'SC', N'Secretary', 30, NULL)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (170, N'IR', N'In Reach Leader', 10, NULL)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (172, N'OR', N'Outreach Leader', 10, NULL)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (220, N'M', N'Member', 30, 1)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (230, N'IA', N'InActive', 40, 1)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (300, N'VM', N'Visiting Member', 30, 1)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (310, N'G', N'Guest', 60, 1)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (311, N'PR', N'Prospect', 190, 1)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (415, N'HB', N'Homebound', 100, NULL)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (500, N'IM', N'In-Service Member', 70, 1)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (700, N'VI', N'VIP', 20, 1)
INSERT INTO [lookup].[MemberType] ([Id], [Code], [Description], [AttendanceTypeId], [Hardwired]) VALUES (710, N'VL', N'Volunteer', 20, NULL)
SET IDENTITY_INSERT [dbo].[Organizations] ON
INSERT INTO [dbo].[Organizations] ([OrganizationId], [CreatedBy], [CreatedDate], [OrganizationStatusId], [DivisionId], [LeaderMemberTypeId], [GradeAgeStart], [GradeAgeEnd], [RollSheetVisitorWks], [SecurityTypeId], [FirstMeetingDate], [LastMeetingDate], [OrganizationClosedDate], [Location], [OrganizationName], [ModifiedBy], [ModifiedDate], [EntryPointId], [ParentOrgId], [AllowAttendOverlap], [MemberCount], [LeaderId], [LeaderName], [ClassFilled], [OnLineCatalogSort], [PendingLoc], [CanSelfCheckin], [NumCheckInLabels], [CampusId], [AllowNonCampusCheckIn], [NumWorkerCheckInLabels], [ShowOnlyRegisteredAtCheckIn], [Limit], [GenderId], [Description], [BirthDayStart], [BirthDayEnd], [LastDayBeforeExtra], [RegistrationTypeId], [ValidateOrgs], [PhoneNumber], [RegistrationClosed], [AllowKioskRegister], [WorshipGroupCodes], [IsBibleFellowshipOrg], [NoSecurityLabel], [AlwaysSecurityLabel], [DaysToIgnoreHistory], [NotifyIds], [lat], [long], [RegSetting], [OrgPickList], [Offsite], [RegStart], [RegEnd], [LimitToRole], [OrganizationTypeId], [MemberJoinScript], [AddToSmallGroupScript], [RemoveFromSmallGroupScript], [SuspendCheckin], [NoAutoAbsents], [PublishDirectory], [ConsecutiveAbsentsThreshold], [IsRecreationTeam], [NotWeekly], [IsMissionTrip], [NoCreditCards], [GiftNotifyIds], [UseBootstrap], [PublicSortOrder], [UseRegisterLink2], [AppCategory], [RegistrationTitle], [PrevMemberCount], [ProspectCount], [RegSettingXml]) VALUES (1, 1, '2009-05-05 22:46:43.983', 40, 1, NULL, 0, 0, NULL, 0, NULL, NULL, NULL, N'', N'First Organization', NULL, NULL, 0, NULL, 0, 0, NULL, NULL, 0, NULL, NULL, 1, 0, NULL, 0, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, 1, NULL, NULL, 0, 0, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, N'Confirmation: 
	Subject: 


#ValidateOrgs:

#Shell:
#GroupToJoin:


Instructions: 
', NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
EXEC(N'INSERT INTO [dbo].[Organizations] ([OrganizationId], [CreatedBy], [CreatedDate], [OrganizationStatusId], [DivisionId], [LeaderMemberTypeId], [GradeAgeStart], [GradeAgeEnd], [RollSheetVisitorWks], [SecurityTypeId], [FirstMeetingDate], [LastMeetingDate], [OrganizationClosedDate], [Location], [OrganizationName], [ModifiedBy], [ModifiedDate], [EntryPointId], [ParentOrgId], [AllowAttendOverlap], [MemberCount], [LeaderId], [LeaderName], [ClassFilled], [OnLineCatalogSort], [PendingLoc], [CanSelfCheckin], [NumCheckInLabels], [CampusId], [AllowNonCampusCheckIn], [NumWorkerCheckInLabels], [ShowOnlyRegisteredAtCheckIn], [Limit], [GenderId], [Description], [BirthDayStart], [BirthDayEnd], [LastDayBeforeExtra], [RegistrationTypeId], [ValidateOrgs], [PhoneNumber], [RegistrationClosed], [AllowKioskRegister], [WorshipGroupCodes], [IsBibleFellowshipOrg], [NoSecurityLabel], [AlwaysSecurityLabel], [DaysToIgnoreHistory], [NotifyIds], [lat], [long], [RegSetting], [OrgPickList], [Offsite], [RegStart], [RegEnd], [LimitToRole], [OrganizationTypeId], [MemberJoinScript], [AddToSmallGroupScript], [RemoveFromSmallGroupScript], [SuspendCheckin], [NoAutoAbsents], [PublishDirectory], [ConsecutiveAbsentsThreshold], [IsRecreationTeam], [NotWeekly], [IsMissionTrip], [NoCreditCards], [GiftNotifyIds], [UseBootstrap], [PublicSortOrder], [UseRegisterLink2], [AppCategory], [RegistrationTitle], [PrevMemberCount], [ProspectCount], [RegSettingXml]) VALUES (33, 1, ''2012-07-27 10:56:55.640'', 30, 1, 0, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, N''Manage Recurring Giving'', NULL, NULL, 0, NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, 0, 0, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 14, NULL, NULL, 0, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, N''Confirmation: 
	Subject: Online Recurring Giving Management
	Body: <div style="margin: 10px; max-width: 600px"><table cellpadding="0" cellspacing="5" style="font-family:arial; font-size:13px; line-height:15px; width:100%"><tbody><tr><td><h1>{church}</h1></td><td><p>Online Recurring Giving Management<br />{date}</p></td></tr></tbody></table><p>{name},</p><p>Thank you for managing your recurring giving to {church}.&nbsp;<strong>Your recurring donations will depend on the availability of funds in your account</strong>, so&nbsp;<em>please monitor your bank activity</em>&nbsp;for the actual date the drafts occur. This email is just confirming that you initiated or made a change to your recurring giving.</p><p>You can view your current and &nbsp;previous years&#39; contributions (both online giving and check/cash) by logging on to the church database. Once you are on your record, go to the&nbsp;<strong>Giving</strong>&nbsp;tab. There are several options: a tab for&nbsp;<strong>all your contributions</strong>, one for your&nbsp;<strong>giving statements</strong>; and one for&nbsp;<strong>Online Giving -&nbsp;</strong>where you can&nbsp;<strong>Manage Your Recurring Giving&nbsp;</strong>or&nbsp;<strong>Make a One-Time Gift.</strong></p><table cellpadding="10" style="background-color:#e6efc2; border-bottom-color:#ddd; border-bottom-width:1px; border-left-color:#ddd; border-left-width:1px; border-right-color:#ddd; border-right-width:1px; border-top-color:#ddd; border-top-width:1px; width:100%"><tbody><tr><td><table cellpadding="10" style="background-color:rgb(230, 239, 194); border-color:rgb(221, 221, 221); width:598px"><tbody><tr><td>This is your confirmation for creating or managing your recurring giving. Please keep this e-mail for your records, but check your bank records for actual withdrawals.</td></tr></tbody></table></td></tr></tbody></table><div><p><br /><strong>Summary</strong></p><blockquote><p>{details}</p></blockquote><div style="border-bottom: #cccccc 1px solid; border-left: #cccccc 1px solid; color: #333333; border-top: #cccccc 1px solid; border-right: #cccccc 1px solid"><table style="border-collapse:collapse; font-family:arial; font-size:9pt; margin:5px; width:100%"><tbody><tr><td><strong>Account</strong></td><td><strong>Contact Information</strong></td></tr><tr><td>{name}<br />{email}<br />{phone}</td><td>{contact}<br />{contactphone}<br /><a href="mailto:{contactemail}">Email contact</a></td></tr></tbody></table></div></div></div>
Reminder: 
	ReminderSubject: 
SupportEmail: 
	SupportSubject: 
SenderEmail: 
	SenderSubject: 


Instructions: 
	Login: <p>Enter your&nbsp;<strong>TouchPoint Username</strong>&nbsp;(or your email address) and your&nbsp;<strong>Password</strong>.</p><p>If you have forgotten your password, click the&nbsp;<strong>Forgot Password?</strong>&nbsp;link below.</p><p>If you have any problems or questions, please call the church to speak to someone in our Finance Office.</p>
	Find: <p>Please enter the information in the required fields* below and press&nbsp;<strong>Find Profile</strong>&nbsp;in order to find your record in the database. Once you complete this process, a&nbsp;<strong>one-use link&nbsp;will be sent to the email address on your record</strong>. Use that link to finish setting up your recurring giving.</p><p>If you have a TouchPoint Username and Password, you can&nbsp;<strong>Login with an Account</strong>.</p><p>If you have any problems or questions, please call the church and ask to speak to someone in our Finance Office.</p>
	Special: <p>Enter the dollar amounts beside each fund to which you want to contribute. Then select the frequency of your contributions and the date on which you want the donations to begin (it must be after today&#39;s date).</p><p>Enter your payment information, and press Submit to complete the process.</p>
AllowOnlyOne: True
'', NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL)')
INSERT INTO [dbo].[Organizations] ([OrganizationId], [CreatedBy], [CreatedDate], [OrganizationStatusId], [DivisionId], [LeaderMemberTypeId], [GradeAgeStart], [GradeAgeEnd], [RollSheetVisitorWks], [SecurityTypeId], [FirstMeetingDate], [LastMeetingDate], [OrganizationClosedDate], [Location], [OrganizationName], [ModifiedBy], [ModifiedDate], [EntryPointId], [ParentOrgId], [AllowAttendOverlap], [MemberCount], [LeaderId], [LeaderName], [ClassFilled], [OnLineCatalogSort], [PendingLoc], [CanSelfCheckin], [NumCheckInLabels], [CampusId], [AllowNonCampusCheckIn], [NumWorkerCheckInLabels], [ShowOnlyRegisteredAtCheckIn], [Limit], [GenderId], [Description], [BirthDayStart], [BirthDayEnd], [LastDayBeforeExtra], [RegistrationTypeId], [ValidateOrgs], [PhoneNumber], [RegistrationClosed], [AllowKioskRegister], [WorshipGroupCodes], [IsBibleFellowshipOrg], [NoSecurityLabel], [AlwaysSecurityLabel], [DaysToIgnoreHistory], [NotifyIds], [lat], [long], [RegSetting], [OrgPickList], [Offsite], [RegStart], [RegEnd], [LimitToRole], [OrganizationTypeId], [MemberJoinScript], [AddToSmallGroupScript], [RemoveFromSmallGroupScript], [SuspendCheckin], [NoAutoAbsents], [PublishDirectory], [ConsecutiveAbsentsThreshold], [IsRecreationTeam], [NotWeekly], [IsMissionTrip], [NoCreditCards], [GiftNotifyIds], [UseBootstrap], [PublicSortOrder], [UseRegisterLink2], [AppCategory], [RegistrationTitle], [PrevMemberCount], [ProspectCount], [RegSettingXml]) VALUES (34, 1, '2012-07-27 11:15:41.310', 30, 1, 0, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, N'Online Giving', NULL, NULL, 0, NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, 0, 0, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 8, NULL, NULL, 0, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, N'Confirmation: 
	Subject: Confirmation for Online Giving
	Body: <<
----------
<p>Hi {first}</p>

<p>Here is your confirmation for {org}.</p>

<p>DETAILS: {details}</p>

<p>Your Ministry Team for {org}</p>
----------
Reminder: 
	ReminderSubject: 
SupportEmail: 
	SupportSubject: 
SenderEmail: 
	SenderSubject: 


Instructions: 
', NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, CONVERT(xml,N'<Settings id="34"><!--1 10/20/2015 12:04 PM--><Confirmation><Subject>Confirmation for Online Giving</Subject><Body>
&lt;p&gt;Hi {first}&lt;/p&gt;

&lt;p&gt;Here is your confirmation for {org}.&lt;/p&gt;

&lt;p&gt;DETAILS: {details}&lt;/p&gt;

&lt;p&gt;Your Ministry Team for {org}&lt;/p&gt;
</Body></Confirmation></Settings>',1))
EXEC(N'INSERT INTO [dbo].[Organizations] ([OrganizationId], [CreatedBy], [CreatedDate], [OrganizationStatusId], [DivisionId], [LeaderMemberTypeId], [GradeAgeStart], [GradeAgeEnd], [RollSheetVisitorWks], [SecurityTypeId], [FirstMeetingDate], [LastMeetingDate], [OrganizationClosedDate], [Location], [OrganizationName], [ModifiedBy], [ModifiedDate], [EntryPointId], [ParentOrgId], [AllowAttendOverlap], [MemberCount], [LeaderId], [LeaderName], [ClassFilled], [OnLineCatalogSort], [PendingLoc], [CanSelfCheckin], [NumCheckInLabels], [CampusId], [AllowNonCampusCheckIn], [NumWorkerCheckInLabels], [ShowOnlyRegisteredAtCheckIn], [Limit], [GenderId], [Description], [BirthDayStart], [BirthDayEnd], [LastDayBeforeExtra], [RegistrationTypeId], [ValidateOrgs], [PhoneNumber], [RegistrationClosed], [AllowKioskRegister], [WorshipGroupCodes], [IsBibleFellowshipOrg], [NoSecurityLabel], [AlwaysSecurityLabel], [DaysToIgnoreHistory], [NotifyIds], [lat], [long], [RegSetting], [OrgPickList], [Offsite], [RegStart], [RegEnd], [LimitToRole], [OrganizationTypeId], [MemberJoinScript], [AddToSmallGroupScript], [RemoveFromSmallGroupScript], [SuspendCheckin], [NoAutoAbsents], [PublishDirectory], [ConsecutiveAbsentsThreshold], [IsRecreationTeam], [NotWeekly], [IsMissionTrip], [NoCreditCards], [GiftNotifyIds], [UseBootstrap], [PublicSortOrder], [UseRegisterLink2], [AppCategory], [RegistrationTitle], [PrevMemberCount], [ProspectCount], [RegSettingXml]) VALUES (35, 3, ''2015-06-19 13:07:54.470'', 30, 1, 140, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, N''Sample Mission Trip'', NULL, NULL, 17, NULL, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, N''Confirmation: 
	Subject: Confirmation: Mission Project
	Body: <p>{first}</p><p>Thank you for registering online for {org}. You have paid {paid}. {paylink}. Any payments you make toward this trip are tax deductible and will appear on your giving statement.</p><p>Below are the details of your registration. Remember to <strong>keep this email if you need to use the above paylink to make subsequent payments toward your balance or your access your people record (profile), Involvement &gt; Registration tab and click Make Payment.</strong></p><p>Because you registered for a mission trip, there are several features that are available to you:</p><p><strong>TouchPoint User Account:</strong></p><ul><li>A user account will be created automatically for you and you will receive a separate email with your credentials.</li><li>If you already have a user account, you will receive an email with a link allowing you to reset your password, if necessary.&nbsp;</li></ul><p><strong>Balance Tracking:</strong></p><ul><li>When you log on to TouchPoint, go to your people record (profile) and click <strong>Involvement &gt; Registration</strong>. There will be a tracking showing the cost of the trip, how much has been paid (both by you and by your supporters), and the balance. There will also be a link to Make Payment, which functions just like your paylink.</li><li>You can return to check your balance at any point. All online donations are posted immediately.</li></ul><p><strong>Email Supporters:</strong></p><ul><li>Also, on your Registration tab is a link to <strong>Email Supporters</strong>. Here is how you can use this feature once you click that link:<ul><li>Enter either an <strong>email address</strong> or a <strong>10-digit phone number </strong>to<strong>&nbsp;</strong>find people in the TouchPoint database.</li><li>If a person is <em>not</em> in the database, you can still add them to your list by entering their email address.</li><li>Click the <strong>Edit</strong> button to edit the salutation for an individual, if you want or to remove someone from the list.</li><li><strong>Edit the body of the email </strong>so that you include your name at the bottom. Be sure to <strong>leave the {salutation}</strong> at the beginning and other code that gives them a <strong>link</strong> to donate.</li><li>Send a test to yourself and then send the email. NOTE: If you send the test, you will not see a salutation with your name, but the code {salutation}. The actual supporter will see something like Dear John.</li></ul></li></ul><p><strong>Your Supporters:</strong></p><ul><li>Those receiving your email will have a <strong>link to donate online</strong> toward your trip.</li><li>They will receive a <strong>confirmation email</strong>, thanking them for their donation.</li><li>You will receive an <strong>email notice</strong> with their name and the amount donated, unless they check the box not to send a notification.</li><li>All donations are <strong>applied to your balance</strong> and they are recorded on the donor&#39;s giving record, as the donation is <strong>tax deductible</strong>.</li></ul><p><a href="http://youtu.be/CEbMyuwbM6o"><u><strong>Watch</strong></u></a><u><strong><a href="http://youtu.be/CEbMyuwbM6o"> a short Mission Trip Registration video, demonstrating</a></strong></u><u><strong><a href="http://youtu.be/CEbMyuwbM6o">&nbsp;the above-mentioned features</a></strong></u></p><p>NOTE: This video was produced prior to the release of the new UX and prior to the name change to TouchPoint. While some screens may look a little different, the process is still the same.</p><p><br />Missions Ministry</p><p>DETAILS:</p><p>{details}</p>
Reminder: 
	ReminderSubject: 
SupportEmail: 
	SupportSubject: My Mission Trip
	SupportBody: <div class="bvedit" style="line-height: 20.7999992370605px;"><p><span style="font-family: arial,helvetica,sans-serif;">{salutation}</span></p><p><span style="font-family: arial,helvetica,sans-serif;">There is something very exciting'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)')
UPDATE [dbo].[Organizations] SET [RegSetting].WRITE(N' about to take place in my life and I want you to know about it.&nbsp; I have the opportunity to participate in a short-term mission project&nbsp;with a team from our church.</span></p><p><span style="font-family: arial,helvetica,sans-serif;">While on this mission trip, I will help with various responsibilities.&nbsp;I feel God has called us to go on this trip and trust that He has many incredible blessings in store.&nbsp;</span></p><p><span style="font-family: arial,helvetica,sans-serif;">I am writing to ask you to pray for me along with our team.&nbsp;Please pray for our travel safety and health, but most of all please pray that we will be bold in sharing the Gospel and that God will save many through our witness.&nbsp;</span></p><p><span style="font-family: arial,helvetica,sans-serif;">I am also writing to invite you to make a tax-deductible financial donation.&nbsp; Each individual is raising their own support, and the cost of the project is {fee}.&nbsp; If you would like to help,&nbsp;</span><strong><a href="https://supportlink" lang="35" rel="nofollow">please click this link to make a donation online</a>. &nbsp;</strong></p><p><span style="font-family: arial,helvetica,sans-serif;">Thank you for your support.&nbsp;If you have any questions, please reply to this email.</span></p><p><span style="font-family: arial,helvetica,sans-serif;">To God be the glory!</span></p><p>&nbsp;</p></div><p>{unsubscribe} to be removed from these types of e-mails from our church.</p>
SenderEmail: 
	SenderSubject: Missions Donation Receipt
	SenderBody: <p>{name}</p><p>Thank you for your online donation of {paid} for Short-Term Missions. If you used your bank account (and not a credit card), please be sure to monitor your account''s activity for the exact date of the draft.</p><p>God bless you.</p>
Fee: 1,500.00
Deposit: 300.00

DonationFundId: 1

Instructions: 
AllowOnlyOne: True
AllowSaveProgress: True
ExtraQuestions: 
	Passport Number
	Your name as it appears on your Passport
	Expiration Date of your Passport (Day, Month, Year)
	Date of Birth: Month, Day, Year

AskHeader: True
	Label: Medical and Emergency Information

AskEmContact: True
AskAllergies: True
AskDoctor: True
ExtraQuestions: 
	Name of Beneficiary (someone not traveling with you)
	Relationship of Beneficiary

',NULL,NULL) WHERE [OrganizationId]=35
SET IDENTITY_INSERT [dbo].[Organizations] OFF
INSERT INTO [dbo].[ProgDiv] ([ProgId], [DivId]) VALUES (1, 1)
INSERT INTO [dbo].[DivOrg] ([DivId], [OrgId], [id]) VALUES (1, 1, 1)
INSERT INTO [dbo].[DivOrg] ([DivId], [OrgId], [id]) VALUES (1, 33, NULL)
INSERT INTO [dbo].[DivOrg] ([DivId], [OrgId], [id]) VALUES (1, 34, NULL)
INSERT INTO [dbo].[DivOrg] ([DivId], [OrgId], [id]) VALUES (1, 35, NULL)
INSERT INTO [dbo].[OrgSchedule] ([OrganizationId], [Id], [ScheduleId], [SchedTime], [SchedDay], [MeetingTime], [AttendCreditId]) VALUES (1, 1, 10930, '2012-01-20 09:30:00.000', 0, '1900-01-07 09:30:00.000', 1)
SET IDENTITY_INSERT [dbo].[Families] ON
INSERT INTO [dbo].[Families] ([FamilyId], [CreatedBy], [CreatedDate], [RecordStatus], [BadAddressFlag], [AltBadAddressFlag], [ResCodeId], [AltResCodeId], [AddressFromDate], [AddressToDate], [AddressLineOne], [AddressLineTwo], [CityName], [StateCode], [ZipCode], [CountryName], [StreetName], [HomePhone], [ModifiedBy], [ModifiedDate], [HeadOfHouseholdId], [HeadOfHouseholdSpouseId], [CoupleFlag], [HomePhoneLU], [HomePhoneAC], [Comments], [PictureId]) VALUES (1, 1, '2009-05-05 22:46:43.970', 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'', NULL, NULL, NULL, NULL, NULL, 1, NULL, 0, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[Families] ([FamilyId], [CreatedBy], [CreatedDate], [RecordStatus], [BadAddressFlag], [AltBadAddressFlag], [ResCodeId], [AltResCodeId], [AddressFromDate], [AddressToDate], [AddressLineOne], [AddressLineTwo], [CityName], [StateCode], [ZipCode], [CountryName], [StreetName], [HomePhone], [ModifiedBy], [ModifiedDate], [HeadOfHouseholdId], [HeadOfHouseholdSpouseId], [CoupleFlag], [HomePhoneLU], [HomePhoneAC], [Comments], [PictureId]) VALUES (2, 0, NULL, 0, NULL, NULL, 30, NULL, NULL, NULL, N'235 Riveredge Cv.', NULL, N'Cordova', N'TN', N'38018', NULL, NULL, N'9017580791', NULL, NULL, 2, NULL, 0, '7580791', '901', NULL, NULL)
INSERT INTO [dbo].[Families] ([FamilyId], [CreatedBy], [CreatedDate], [RecordStatus], [BadAddressFlag], [AltBadAddressFlag], [ResCodeId], [AltResCodeId], [AddressFromDate], [AddressToDate], [AddressLineOne], [AddressLineTwo], [CityName], [StateCode], [ZipCode], [CountryName], [StreetName], [HomePhone], [ModifiedBy], [ModifiedDate], [HeadOfHouseholdId], [HeadOfHouseholdSpouseId], [CoupleFlag], [HomePhoneLU], [HomePhoneAC], [Comments], [PictureId]) VALUES (3, 0, NULL, 0, 0, NULL, 30, NULL, NULL, NULL, N'2000 Appling Rd', NULL, N'Cordova', N'TN', N'38016-4910', NULL, NULL, N'', NULL, NULL, 3, NULL, 0, '       ', '000', NULL, NULL)
SET IDENTITY_INSERT [dbo].[Families] OFF
SET IDENTITY_INSERT [dbo].[People] ON
INSERT INTO [dbo].[People] ([PeopleId], [CreatedBy], [CreatedDate], [DropCodeId], [GenderId], [DoNotMailFlag], [DoNotCallFlag], [DoNotVisitFlag], [AddressTypeId], [PhonePrefId], [MaritalStatusId], [PositionInFamilyId], [MemberStatusId], [FamilyId], [BirthMonth], [BirthDay], [BirthYear], [OriginId], [EntryPointId], [InterestPointId], [BaptismTypeId], [BaptismStatusId], [DecisionTypeId], [NewMemberClassStatusId], [LetterStatusId], [JoinCodeId], [EnvelopeOptionsId], [BadAddressFlag], [ResCodeId], [AddressFromDate], [AddressToDate], [WeddingDate], [OriginDate], [BaptismSchedDate], [BaptismDate], [DecisionDate], [LetterDateRequested], [LetterDateReceived], [JoinDate], [DropDate], [DeceasedDate], [TitleCode], [FirstName], [MiddleName], [MaidenName], [LastName], [SuffixCode], [NickName], [AddressLineOne], [AddressLineTwo], [CityName], [StateCode], [ZipCode], [CountryName], [StreetName], [CellPhone], [WorkPhone], [EmailAddress], [OtherPreviousChurch], [OtherNewChurch], [SchoolOther], [EmployerOther], [OccupationOther], [HobbyOther], [SkillOther], [InterestOther], [LetterStatusNotes], [Comments], [ChristAsSavior], [MemberAnyChurch], [InterestedInJoining], [PleaseVisit], [InfoBecomeAChristian], [ContributionsStatement], [ModifiedBy], [ModifiedDate], [PictureId], [ContributionOptionsId], [PrimaryCity], [PrimaryZip], [PrimaryAddress], [PrimaryState], [HomePhone], [SpouseId], [PrimaryAddress2], [PrimaryResCode], [PrimaryBadAddrFlag], [LastContact], [Grade], [CellPhoneLU], [WorkPhoneLU], [BibleFellowshipClassId], [CampusId], [CellPhoneAC], [CheckInNotes], [AltName], [CustodyIssue], [OkTransport], [HasDuplicates], [FirstName2], [EmailAddress2], [SendEmailAddress1], [SendEmailAddress2], [NewMemberClassDate], [PrimaryCountry], [ReceiveSMS], [DoNotPublishPhones], [SSN], [DLN], [DLStateID], [ElectronicStatement]) VALUES (1, 1, '2009-05-05 22:46:43.970', 0, 0, 0, 0, 0, 10, 0, 0, 10, 20, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'The', NULL, NULL, N'Admin', NULL, NULL, NULL, NULL, NULL, NULL, N'', NULL, NULL, NULL, NULL, N'info@bvcms.com', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, N'', NULL, NULL, NULL, NULL, NULL, 40, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'The', NULL, 1, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[People] ([PeopleId], [CreatedBy], [CreatedDate], [DropCodeId], [GenderId], [DoNotMailFlag], [DoNotCallFlag], [DoNotVisitFlag], [AddressTypeId], [PhonePrefId], [MaritalStatusId], [PositionInFamilyId], [MemberStatusId], [FamilyId], [BirthMonth], [BirthDay], [BirthYear], [OriginId], [EntryPointId], [InterestPointId], [BaptismTypeId], [BaptismStatusId], [DecisionTypeId], [NewMemberClassStatusId], [LetterStatusId], [JoinCodeId], [EnvelopeOptionsId], [BadAddressFlag], [ResCodeId], [AddressFromDate], [AddressToDate], [WeddingDate], [OriginDate], [BaptismSchedDate], [BaptismDate], [DecisionDate], [LetterDateRequested], [LetterDateReceived], [JoinDate], [DropDate], [DeceasedDate], [TitleCode], [FirstName], [MiddleName], [MaidenName], [LastName], [SuffixCode], [NickName], [AddressLineOne], [AddressLineTwo], [CityName], [StateCode], [ZipCode], [CountryName], [StreetName], [CellPhone], [WorkPhone], [EmailAddress], [OtherPreviousChurch], [OtherNewChurch], [SchoolOther], [EmployerOther], [OccupationOther], [HobbyOther], [SkillOther], [InterestOther], [LetterStatusNotes], [Comments], [ChristAsSavior], [MemberAnyChurch], [InterestedInJoining], [PleaseVisit], [InfoBecomeAChristian], [ContributionsStatement], [ModifiedBy], [ModifiedDate], [PictureId], [ContributionOptionsId], [PrimaryCity], [PrimaryZip], [PrimaryAddress], [PrimaryState], [HomePhone], [SpouseId], [PrimaryAddress2], [PrimaryResCode], [PrimaryBadAddrFlag], [LastContact], [Grade], [CellPhoneLU], [WorkPhoneLU], [BibleFellowshipClassId], [CampusId], [CellPhoneAC], [CheckInNotes], [AltName], [CustodyIssue], [OkTransport], [HasDuplicates], [FirstName2], [EmailAddress2], [SendEmailAddress1], [SendEmailAddress2], [NewMemberClassDate], [PrimaryCountry], [ReceiveSMS], [DoNotPublishPhones], [SSN], [DLN], [DLStateID], [ElectronicStatement]) VALUES (2, 0, '2010-10-30 15:23:10.743', 0, 1, 0, 0, 0, 10, 0, 20, 10, 20, 2, 5, 30, 1952, 70, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'Mr.', N'David', NULL, NULL, N'Carroll', NULL, NULL, NULL, NULL, NULL, NULL, N'', NULL, NULL, N'9014890611', NULL, N'david@touchpointsoftware.com', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL, N'Cordova', N'38018', N'235 Riveredge Cv.', N'TN', N'9017580791', NULL, NULL, 30, 0, NULL, NULL, '4890611', NULL, NULL, NULL, '901', NULL, NULL, NULL, NULL, NULL, N'David', NULL, 1, 0, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL)
INSERT INTO [dbo].[People] ([PeopleId], [CreatedBy], [CreatedDate], [DropCodeId], [GenderId], [DoNotMailFlag], [DoNotCallFlag], [DoNotVisitFlag], [AddressTypeId], [PhonePrefId], [MaritalStatusId], [PositionInFamilyId], [MemberStatusId], [FamilyId], [BirthMonth], [BirthDay], [BirthYear], [OriginId], [EntryPointId], [InterestPointId], [BaptismTypeId], [BaptismStatusId], [DecisionTypeId], [NewMemberClassStatusId], [LetterStatusId], [JoinCodeId], [EnvelopeOptionsId], [BadAddressFlag], [ResCodeId], [AddressFromDate], [AddressToDate], [WeddingDate], [OriginDate], [BaptismSchedDate], [BaptismDate], [DecisionDate], [LetterDateRequested], [LetterDateReceived], [JoinDate], [DropDate], [DeceasedDate], [TitleCode], [FirstName], [MiddleName], [MaidenName], [LastName], [SuffixCode], [NickName], [AddressLineOne], [AddressLineTwo], [CityName], [StateCode], [ZipCode], [CountryName], [StreetName], [CellPhone], [WorkPhone], [EmailAddress], [OtherPreviousChurch], [OtherNewChurch], [SchoolOther], [EmployerOther], [OccupationOther], [HobbyOther], [SkillOther], [InterestOther], [LetterStatusNotes], [Comments], [ChristAsSavior], [MemberAnyChurch], [InterestedInJoining], [PleaseVisit], [InfoBecomeAChristian], [ContributionsStatement], [ModifiedBy], [ModifiedDate], [PictureId], [ContributionOptionsId], [PrimaryCity], [PrimaryZip], [PrimaryAddress], [PrimaryState], [HomePhone], [SpouseId], [PrimaryAddress2], [PrimaryResCode], [PrimaryBadAddrFlag], [LastContact], [Grade], [CellPhoneLU], [WorkPhoneLU], [BibleFellowshipClassId], [CampusId], [CellPhoneAC], [CheckInNotes], [AltName], [CustodyIssue], [OkTransport], [HasDuplicates], [FirstName2], [EmailAddress2], [SendEmailAddress1], [SendEmailAddress2], [NewMemberClassDate], [PrimaryCountry], [ReceiveSMS], [DoNotPublishPhones], [SSN], [DLN], [DLStateID], [ElectronicStatement]) VALUES (3, 0, '2010-10-30 15:23:12.310', 0, 2, 0, 0, 0, 10, 0, 20, 10, 20, 3, NULL, NULL, NULL, 70, 10, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'Ms.', N'Karen', NULL, NULL, N'Worrell', NULL, NULL, NULL, NULL, NULL, NULL, N'', NULL, NULL, N'', N'', N'karen@touchpointsoftware.com', NULL, NULL, NULL, N'BVCMS/Bellevue Baptist Church', N'Systems Administrator', NULL, NULL, NULL, NULL, NULL, 0, 0, 0, 0, 0, 0, NULL, NULL, 1, NULL, N'Cordova', N'38016-4910', N'2000 Appling Rd', N'TN', N'', NULL, NULL, 30, 0, NULL, NULL, '       ', NULL, NULL, NULL, '000', NULL, NULL, NULL, NULL, NULL, N'Karen', NULL, 1, 0, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[People] OFF
SET IDENTITY_INSERT [dbo].[RecReg] ON
INSERT INTO [dbo].[RecReg] ([Id], [PeopleId], [ImgId], [IsDocument], [ActiveInAnotherChurch], [ShirtSize], [MedAllergy], [email], [MedicalDescription], [fname], [mname], [coaching], [member], [emcontact], [emphone], [doctor], [docphone], [insurance], [policy], [Comments], [Tylenol], [Advil], [Maalox], [Robitussin]) VALUES (2, 3, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[RecReg] OFF
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (1, 1)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (1, 2)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (1, 3)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (1, 4)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (1, 5)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (1, 6)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (1, 8)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (1, 9)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (1, 27)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (2, 1)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (2, 2)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (2, 3)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (2, 4)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (2, 5)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (2, 6)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (2, 8)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (2, 9)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (2, 11)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (2, 12)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (2, 25)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (3, 1)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (3, 2)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (3, 3)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (3, 4)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (3, 5)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (3, 6)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (3, 8)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (3, 9)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (3, 11)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (3, 12)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (3, 15)
INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES (3, 25)
SET IDENTITY_INSERT [dbo].[Users] ON
INSERT INTO [dbo].[Users] ([UserId], [PeopleId], [Username], [Comment], [Password], [PasswordQuestion], [PasswordAnswer], [IsApproved], [LastActivityDate], [LastLoginDate], [LastPasswordChangedDate], [CreationDate], [IsLockedOut], [LastLockedOutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [ItemsInGrid], [CurrentCart], [MustChangePassword], [Host], [TempPassword], [Name], [Name2], [ResetPasswordCode], [DefaultGroup], [ResetPasswordExpires]) VALUES (1, 1, N'Admin', NULL, N'2352354235', NULL, NULL, 1, '2015-11-09 11:12:44.827', NULL, '2015-01-14 14:36:59.747', '2009-05-05 22:46:43.890', 0, '2014-10-16 15:43:29.923', 1, '2015-11-09 11:11:29.540', 0, NULL, NULL, NULL, 1, N'starterdb.bvcms.com', N'bvcms', N'The Admin', N'Admin, The', NULL, NULL, '2014-10-17 15:42:49.050')
INSERT INTO [dbo].[Users] ([UserId], [PeopleId], [Username], [Comment], [Password], [PasswordQuestion], [PasswordAnswer], [IsApproved], [LastActivityDate], [LastLoginDate], [LastPasswordChangedDate], [CreationDate], [IsLockedOut], [LastLockedOutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [ItemsInGrid], [CurrentCart], [MustChangePassword], [Host], [TempPassword], [Name], [Name2], [ResetPasswordCode], [DefaultGroup], [ResetPasswordExpires]) VALUES (2, 2, N'david', N'', N'uNVML/ZamnY7YdE1NXvMHPIznic=', NULL, NULL, 1, '2016-01-07 13:55:54.940', '2016-01-07 12:42:32.987', '2013-12-19 00:03:08.440', '2010-10-30 15:23:25.763', 0, '2013-12-19 00:03:08.360', 0, '2013-12-18 22:54:19.783', 0, '2010-10-30 15:23:25.763', NULL, NULL, 0, N'starterdb.bvcms.com', NULL, N'David Carroll', N'Carroll, David', NULL, NULL, '2013-12-19 22:55:00.120')
INSERT INTO [dbo].[Users] ([UserId], [PeopleId], [Username], [Comment], [Password], [PasswordQuestion], [PasswordAnswer], [IsApproved], [LastActivityDate], [LastLoginDate], [LastPasswordChangedDate], [CreationDate], [IsLockedOut], [LastLockedOutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [ItemsInGrid], [CurrentCart], [MustChangePassword], [Host], [TempPassword], [Name], [Name2], [ResetPasswordCode], [DefaultGroup], [ResetPasswordExpires]) VALUES (3, 3, N'karenw', N'', N'lpSVokbyDdVaXxNGDjZT4St468A=', NULL, NULL, 1, '2016-01-07 12:55:36.257', '2016-01-07 12:54:22.597', '2013-10-14 10:43:23.743', '2010-10-30 15:29:25.757', 0, '2013-10-14 10:43:23.667', 0, '2013-10-14 10:41:24.547', 0, '2010-10-30 15:29:25.757', NULL, NULL, 0, N'starterdb.bvcms.com', NULL, N'Karen Worrell', N'Worrell, Karen', NULL, NULL, '2013-10-15 10:42:47.710')
SET IDENTITY_INSERT [dbo].[Users] OFF
INSERT INTO [dbo].[Volunteer] ([PeopleId], [StatusId], [ProcessedDate], [Standard], [Children], [Leader], [Comments], [MVRStatusId], [MVRProcessedDate]) VALUES (3, NULL, NULL, 0, 0, 0, NULL, NULL, NULL)
ALTER TABLE [dbo].[Volunteer] WITH NOCHECK ADD CONSTRAINT [StatusMvrId__StatusMvr] FOREIGN KEY ([MVRStatusId]) REFERENCES [lookup].[VolApplicationStatus] ([Id])
ALTER TABLE [dbo].[Volunteer] ADD CONSTRAINT [FK_Volunteer_PEOPLE_TBL] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[Volunteer] WITH NOCHECK ADD CONSTRAINT [FK_Volunteer_VolApplicationStatus] FOREIGN KEY ([StatusId]) REFERENCES [lookup].[VolApplicationStatus] ([Id])
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [FK_Users_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[UserRole] ADD CONSTRAINT [FK_UserRole_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId])
ALTER TABLE [dbo].[UserRole] ADD CONSTRAINT [FK_UserRole_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
ALTER TABLE [dbo].[RecReg] ADD CONSTRAINT [FK_RecReg_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_BaptismStatus] FOREIGN KEY ([BaptismStatusId]) REFERENCES [lookup].[BaptismStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_BaptismType] FOREIGN KEY ([BaptismTypeId]) REFERENCES [lookup].[BaptismType] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [BFMembers__BFClass] FOREIGN KEY ([BibleFellowshipClassId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_Campus] FOREIGN KEY ([CampusId]) REFERENCES [lookup].[Campus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [StmtPeople__ContributionStatementOption] FOREIGN KEY ([ContributionOptionsId]) REFERENCES [lookup].[EnvelopeOption] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_DecisionType] FOREIGN KEY ([DecisionTypeId]) REFERENCES [lookup].[DecisionType] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_DropType] FOREIGN KEY ([DropCodeId]) REFERENCES [lookup].[DropType] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_EntryPoint] FOREIGN KEY ([EntryPointId]) REFERENCES [lookup].[EntryPoint] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [EnvPeople__EnvelopeOption] FOREIGN KEY ([EnvelopeOptionsId]) REFERENCES [lookup].[EnvelopeOption] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_Families] FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families] ([FamilyId])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_Gender] FOREIGN KEY ([GenderId]) REFERENCES [lookup].[Gender] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_InterestPoint] FOREIGN KEY ([InterestPointId]) REFERENCES [lookup].[InterestPoint] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_JoinType] FOREIGN KEY ([JoinCodeId]) REFERENCES [lookup].[JoinType] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_MemberLetterStatus] FOREIGN KEY ([LetterStatusId]) REFERENCES [lookup].[MemberLetterStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_MaritalStatus] FOREIGN KEY ([MaritalStatusId]) REFERENCES [lookup].[MaritalStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_MemberStatus] FOREIGN KEY ([MemberStatusId]) REFERENCES [lookup].[MemberStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_DiscoveryClassStatus] FOREIGN KEY ([NewMemberClassStatusId]) REFERENCES [lookup].[NewMemberClassStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_Origin] FOREIGN KEY ([OriginId]) REFERENCES [lookup].[Origin] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_PEOPLE_TBL_Picture] FOREIGN KEY ([PictureId]) REFERENCES [dbo].[Picture] ([PictureId])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_FamilyPosition] FOREIGN KEY ([PositionInFamilyId]) REFERENCES [lookup].[FamilyPosition] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [ResCodePeople__ResidentCode] FOREIGN KEY ([ResCodeId]) REFERENCES [lookup].[ResidentCode] ([Id])
ALTER TABLE [dbo].[Families] ADD CONSTRAINT [FamiliesHeaded__HeadOfHousehold] FOREIGN KEY ([HeadOfHouseholdId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[Families] ADD CONSTRAINT [FamiliesHeaded2__HeadOfHouseholdSpouse] FOREIGN KEY ([HeadOfHouseholdSpouseId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[Families] ADD CONSTRAINT [FK_Families_Picture] FOREIGN KEY ([PictureId]) REFERENCES [dbo].[Picture] ([PictureId])
ALTER TABLE [dbo].[Families] ADD CONSTRAINT [ResCodeFamilies__ResidentCode] FOREIGN KEY ([ResCodeId]) REFERENCES [lookup].[ResidentCode] ([Id])
ALTER TABLE [dbo].[OrgSchedule] ADD CONSTRAINT [FK_OrgSchedule_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[DivOrg] ADD CONSTRAINT [FK_DivOrg_Division] FOREIGN KEY ([DivId]) REFERENCES [dbo].[Division] ([Id])
ALTER TABLE [dbo].[DivOrg] ADD CONSTRAINT [FK_DivOrg_Organizations] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[ProgDiv] ADD CONSTRAINT [FK_ProgDiv_Division] FOREIGN KEY ([DivId]) REFERENCES [dbo].[Division] ([Id])
ALTER TABLE [dbo].[ProgDiv] ADD CONSTRAINT [FK_ProgDiv_Program] FOREIGN KEY ([ProgId]) REFERENCES [dbo].[Program] ([Id])
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_Organizations_Campus] FOREIGN KEY ([CampusId]) REFERENCES [lookup].[Campus] ([Id])
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_Organizations_Division] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Division] ([Id])
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_ORGANIZATIONS_TBL_EntryPoint] FOREIGN KEY ([EntryPointId]) REFERENCES [lookup].[EntryPoint] ([Id])
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_Organizations_Gender] FOREIGN KEY ([GenderId]) REFERENCES [lookup].[Gender] ([Id])
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_ORGANIZATIONS_TBL_OrganizationStatus] FOREIGN KEY ([OrganizationStatusId]) REFERENCES [lookup].[OrganizationStatus] ([Id])
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_Organizations_OrganizationType] FOREIGN KEY ([OrganizationTypeId]) REFERENCES [lookup].[OrganizationType] ([Id])
ALTER TABLE [dbo].[Organizations] WITH NOCHECK ADD CONSTRAINT [ChildOrgs__ParentOrg] FOREIGN KEY ([ParentOrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[Attend] WITH NOCHECK ADD CONSTRAINT [FK_AttendWithAbsents_TBL_ORGANIZATIONS_TBL] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[Coupons] WITH NOCHECK ADD CONSTRAINT [FK_Coupons_Organizations] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[EnrollmentTransaction] WITH NOCHECK ADD CONSTRAINT [ENROLLMENT_TRANSACTION_ORG_FK] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[GoerSenderAmounts] WITH NOCHECK ADD CONSTRAINT [FK_GoerSenderAmounts_Organizations] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[Meetings] WITH NOCHECK ADD CONSTRAINT [FK_MEETINGS_TBL_ORGANIZATIONS_TBL] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[MemberTags] WITH NOCHECK ADD CONSTRAINT [FK_MemberTags_Organizations] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[OrganizationExtra] WITH NOCHECK ADD CONSTRAINT [FK_OrganizationExtra_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[OrganizationMembers] WITH NOCHECK ADD CONSTRAINT [ORGANIZATION_MEMBERS_ORG_FK] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [lookup].[MemberType] ADD CONSTRAINT [FK_MemberType_AttendType] FOREIGN KEY ([AttendanceTypeId]) REFERENCES [lookup].[AttendType] ([Id])
ALTER TABLE [dbo].[Attend] WITH NOCHECK ADD CONSTRAINT [FK_Attend_MemberType] FOREIGN KEY ([MemberTypeId]) REFERENCES [lookup].[MemberType] ([Id])
ALTER TABLE [dbo].[EnrollmentTransaction] WITH NOCHECK ADD CONSTRAINT [FK_ENROLLMENT_TRANSACTION_TBL_MemberType] FOREIGN KEY ([MemberTypeId]) REFERENCES [lookup].[MemberType] ([Id])
ALTER TABLE [dbo].[OrganizationMembers] WITH NOCHECK ADD CONSTRAINT [FK_ORGANIZATION_MEMBERS_TBL_MemberType] FOREIGN KEY ([MemberTypeId]) REFERENCES [lookup].[MemberType] ([Id])
ALTER TABLE [dbo].[Division] ADD CONSTRAINT [FK_Division_Program] FOREIGN KEY ([ProgId]) REFERENCES [dbo].[Program] ([Id])
ALTER TABLE [dbo].[Coupons] WITH NOCHECK ADD CONSTRAINT [FK_Coupons_Division] FOREIGN KEY ([DivId]) REFERENCES [dbo].[Division] ([Id])
ALTER TABLE [dbo].[Promotion] WITH NOCHECK ADD CONSTRAINT [FromPromotions__FromDivision] FOREIGN KEY ([FromDivId]) REFERENCES [dbo].[Division] ([Id])
ALTER TABLE [dbo].[Promotion] WITH NOCHECK ADD CONSTRAINT [ToPromotions__ToDivision] FOREIGN KEY ([ToDivId]) REFERENCES [dbo].[Division] ([Id])
ALTER TABLE [dbo].[ChangeDetails] WITH NOCHECK ADD CONSTRAINT [FK_ChangeDetails_ChangeLog] FOREIGN KEY ([Id]) REFERENCES [dbo].[ChangeLog] ([Id])
ALTER TABLE [dbo].[VoluteerApprovalIds] WITH NOCHECK ADD CONSTRAINT [FK_VoluteerApprovalIds_VolunteerCodes] FOREIGN KEY ([ApprovalId]) REFERENCES [lookup].[VolunteerCodes] ([Id])
ALTER TABLE [dbo].[Task] WITH NOCHECK ADD CONSTRAINT [FK_Task_TaskStatus] FOREIGN KEY ([StatusId]) REFERENCES [lookup].[TaskStatus] ([Id])
ALTER TABLE [dbo].[Zips] WITH NOCHECK ADD CONSTRAINT [FK_Zips_ResidentCode] FOREIGN KEY ([MetroMarginalCode]) REFERENCES [lookup].[ResidentCode] ([Id])
ALTER TABLE [dbo].[Contribution] WITH NOCHECK ADD CONSTRAINT [FK_Contribution_ContributionType] FOREIGN KEY ([ContributionTypeId]) REFERENCES [lookup].[ContributionType] ([Id])
ALTER TABLE [dbo].[Contribution] WITH NOCHECK ADD CONSTRAINT [FK_Contribution_ContributionStatus] FOREIGN KEY ([ContributionStatusId]) REFERENCES [lookup].[ContributionStatus] ([Id])
ALTER TABLE [dbo].[Contact] WITH NOCHECK ADD CONSTRAINT [FK_Contacts_ContactTypes] FOREIGN KEY ([ContactTypeId]) REFERENCES [lookup].[ContactType] ([Id])
ALTER TABLE [dbo].[Contact] WITH NOCHECK ADD CONSTRAINT [FK_NewContacts_ContactReasons] FOREIGN KEY ([ContactReasonId]) REFERENCES [lookup].[ContactReason] ([Id])
ALTER TABLE [dbo].[BundleHeader] WITH NOCHECK ADD CONSTRAINT [FK_BUNDLE_HEADER_TBL_BundleStatusTypes] FOREIGN KEY ([BundleStatusId]) REFERENCES [lookup].[BundleStatusTypes] ([Id])
ALTER TABLE [dbo].[BundleHeader] WITH NOCHECK ADD CONSTRAINT [FK_BUNDLE_HEADER_TBL_BundleHeaderTypes] FOREIGN KEY ([BundleHeaderTypeId]) REFERENCES [lookup].[BundleHeaderTypes] ([Id])
ALTER TABLE [dbo].[Attend] WITH NOCHECK ADD CONSTRAINT [FK_AttendWithAbsents_TBL_AttendType] FOREIGN KEY ([AttendanceTypeId]) REFERENCES [lookup].[AttendType] ([Id])
ALTER TABLE [dbo].[Meetings] WITH NOCHECK ADD CONSTRAINT [FK_Meetings_AttendCredit] FOREIGN KEY ([AttendCreditId]) REFERENCES [lookup].[AttendCredit] ([Id])
ALTER TABLE [dbo].[OrganizationMembers] WITH NOCHECK ADD CONSTRAINT [FK_OrganizationMembers_RegistrationData] FOREIGN KEY ([RegistrationDataId]) REFERENCES [dbo].[RegistrationData] ([Id])
ALTER TABLE [dbo].[Contact] WITH NOCHECK ADD CONSTRAINT [FK_Contacts_Ministries] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministries] ([MinistryId])
ALTER TABLE [dbo].[Contribution] WITH NOCHECK ADD CONSTRAINT [FK_Contribution_ExtraData] FOREIGN KEY ([ExtraDataId]) REFERENCES [dbo].[ExtraData] ([Id])
ALTER TABLE [dbo].[BundleHeader] WITH NOCHECK ADD CONSTRAINT [BundleHeaders__Fund] FOREIGN KEY ([FundId]) REFERENCES [dbo].[ContributionFund] ([FundId])
ALTER TABLE [dbo].[Contribution] WITH NOCHECK ADD CONSTRAINT [FK_Contribution_ContributionFund] FOREIGN KEY ([FundId]) REFERENCES [dbo].[ContributionFund] ([FundId])
ALTER TABLE [dbo].[RecurringAmounts] WITH NOCHECK ADD CONSTRAINT [FK_RecurringAmounts_ContributionFund] FOREIGN KEY ([FundId]) REFERENCES [dbo].[ContributionFund] ([FundId])
ALTER TABLE [dbo].[ContentKeyWords] WITH NOCHECK ADD CONSTRAINT [FK_ContentKeyWords_Content] FOREIGN KEY ([Id]) REFERENCES [dbo].[Content] ([Id])
COMMIT TRANSACTION
GO
