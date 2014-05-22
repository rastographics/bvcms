-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ShowTableSizes]
AS
BEGIN
CREATE TABLE #temp (
       table_name sysname ,
       row_count int,
       reserved_size nvarchar(50),
       data_size nvarchar(50),
       index_size nvarchar(50),
       unused_size nvarchar(50))
SET NOCOUNT ON
INSERT     #temp
EXEC       sp_msforeachtable 'sp_spaceused ''?'''
SELECT     b.table_schema as owner,
		   a.table_name,
           a.row_count,
           count(*) as col_count,
           a.data_size
FROM       #temp a
INNER JOIN information_schema.columns b
           ON a.table_name collate database_default
                = b.table_name collate database_default
GROUP BY   b.table_schema, a.table_name, a.row_count, a.data_size
ORDER BY   a.row_count desc
DROP TABLE #temp
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding constraints to [dbo].[Meetings]'
GO
ALTER TABLE [dbo].[Meetings] ADD CONSTRAINT [MeetingDateOrgId] UNIQUE NONCLUSTERED  ([MeetingDate], [OrganizationId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Attend]'
GO
ALTER TABLE [dbo].[Attend] WITH NOCHECK  ADD CONSTRAINT [FK_AttendWithAbsents_TBL_ORGANIZATIONS_TBL] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[Attend] WITH NOCHECK  ADD CONSTRAINT [FK_AttendWithAbsents_TBL_AttendType] FOREIGN KEY ([AttendanceTypeId]) REFERENCES [lookup].[AttendType] ([Id])
ALTER TABLE [dbo].[Attend] WITH NOCHECK  ADD CONSTRAINT [FK_Attend_MemberType] FOREIGN KEY ([MemberTypeId]) REFERENCES [lookup].[MemberType] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[BundleDetail]'
GO
ALTER TABLE [dbo].[BundleDetail] WITH NOCHECK  ADD CONSTRAINT [BUNDLE_DETAIL_BUNDLE_FK] FOREIGN KEY ([BundleHeaderId]) REFERENCES [dbo].[BundleHeader] ([BundleHeaderId])
ALTER TABLE [dbo].[BundleDetail] WITH NOCHECK  ADD CONSTRAINT [BUNDLE_DETAIL_CONTR_FK] FOREIGN KEY ([ContributionId]) REFERENCES [dbo].[Contribution] ([ContributionId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[BundleHeader]'
GO
ALTER TABLE [dbo].[BundleHeader] WITH NOCHECK  ADD CONSTRAINT [FK_BUNDLE_HEADER_TBL_BundleStatusTypes] FOREIGN KEY ([BundleStatusId]) REFERENCES [lookup].[BundleStatusTypes] ([Id])
ALTER TABLE [dbo].[BundleHeader] WITH NOCHECK  ADD CONSTRAINT [FK_BUNDLE_HEADER_TBL_BundleHeaderTypes] FOREIGN KEY ([BundleHeaderTypeId]) REFERENCES [lookup].[BundleHeaderTypes] ([Id])
ALTER TABLE [dbo].[BundleHeader] WITH NOCHECK  ADD CONSTRAINT [BundleHeaders__Fund] FOREIGN KEY ([FundId]) REFERENCES [dbo].[ContributionFund] ([FundId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[ChangeDetails]'
GO
ALTER TABLE [dbo].[ChangeDetails] WITH NOCHECK  ADD CONSTRAINT [FK_ChangeDetails_ChangeLog] FOREIGN KEY ([Id]) REFERENCES [dbo].[ChangeLog] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Contact]'
GO
ALTER TABLE [dbo].[Contact] WITH NOCHECK  ADD CONSTRAINT [FK_Contacts_ContactTypes] FOREIGN KEY ([ContactTypeId]) REFERENCES [lookup].[ContactType] ([Id])
ALTER TABLE [dbo].[Contact] WITH NOCHECK  ADD CONSTRAINT [FK_NewContacts_ContactReasons] FOREIGN KEY ([ContactReasonId]) REFERENCES [lookup].[ContactReason] ([Id])
ALTER TABLE [dbo].[Contact] WITH NOCHECK  ADD CONSTRAINT [FK_Contacts_Ministries] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministries] ([MinistryId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Contribution]'
GO
ALTER TABLE [dbo].[Contribution] WITH NOCHECK  ADD CONSTRAINT [FK_Contribution_ContributionFund] FOREIGN KEY ([FundId]) REFERENCES [dbo].[ContributionFund] ([FundId])
ALTER TABLE [dbo].[Contribution] WITH NOCHECK  ADD CONSTRAINT [FK_Contribution_ContributionType] FOREIGN KEY ([ContributionTypeId]) REFERENCES [lookup].[ContributionType] ([Id])
ALTER TABLE [dbo].[Contribution] WITH NOCHECK  ADD CONSTRAINT [FK_Contribution_ContributionStatus] FOREIGN KEY ([ContributionStatusId]) REFERENCES [lookup].[ContributionStatus] ([Id])
ALTER TABLE [dbo].[Contribution] WITH NOCHECK  ADD CONSTRAINT [FK_Contribution_ExtraData] FOREIGN KEY ([ExtraDataId]) REFERENCES [dbo].[ExtraData] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[RecurringAmounts]'
GO
ALTER TABLE [dbo].[RecurringAmounts] WITH NOCHECK  ADD CONSTRAINT [FK_RecurringAmounts_ContributionFund] FOREIGN KEY ([FundId]) REFERENCES [dbo].[ContributionFund] ([FundId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Coupons]'
GO
ALTER TABLE [dbo].[Coupons] WITH NOCHECK  ADD CONSTRAINT [FK_Coupons_Division] FOREIGN KEY ([DivId]) REFERENCES [dbo].[Division] ([Id])
ALTER TABLE [dbo].[Coupons] WITH NOCHECK  ADD CONSTRAINT [FK_Coupons_Organizations] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[ContentKeyWords]'
GO
ALTER TABLE [dbo].[ContentKeyWords] WITH NOCHECK  ADD CONSTRAINT [FK_ContentKeyWords_Content] FOREIGN KEY ([Id]) REFERENCES [dbo].[Content] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Promotion]'
GO
ALTER TABLE [dbo].[Promotion] WITH NOCHECK  ADD CONSTRAINT [FromPromotions__FromDivision] FOREIGN KEY ([FromDivId]) REFERENCES [dbo].[Division] ([Id])
ALTER TABLE [dbo].[Promotion] WITH NOCHECK  ADD CONSTRAINT [ToPromotions__ToDivision] FOREIGN KEY ([ToDivId]) REFERENCES [dbo].[Division] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[RelatedFamilies]'
GO
ALTER TABLE [dbo].[RelatedFamilies] WITH NOCHECK  ADD CONSTRAINT [RelatedFamilies1__RelatedFamily1] FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families] ([FamilyId])
ALTER TABLE [dbo].[RelatedFamilies] WITH NOCHECK  ADD CONSTRAINT [RelatedFamilies2__RelatedFamily2] FOREIGN KEY ([RelatedFamilyId]) REFERENCES [dbo].[Families] ([FamilyId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[EnrollmentTransaction]'
GO
ALTER TABLE [dbo].[EnrollmentTransaction] WITH NOCHECK  ADD CONSTRAINT [ENROLLMENT_TRANSACTION_ORG_FK] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[EnrollmentTransaction] WITH NOCHECK  ADD CONSTRAINT [ENROLLMENT_TRANSACTION_PPL_FK] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[EnrollmentTransaction] WITH NOCHECK  ADD CONSTRAINT [FK_ENROLLMENT_TRANSACTION_TBL_MemberType] FOREIGN KEY ([MemberTypeId]) REFERENCES [lookup].[MemberType] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Meetings]'
GO
ALTER TABLE [dbo].[Meetings] WITH NOCHECK  ADD CONSTRAINT [FK_MEETINGS_TBL_ORGANIZATIONS_TBL] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[Meetings] WITH NOCHECK  ADD CONSTRAINT [FK_Meetings_AttendCredit] FOREIGN KEY ([AttendCreditId]) REFERENCES [lookup].[AttendCredit] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[MemberTags]'
GO
ALTER TABLE [dbo].[MemberTags] WITH NOCHECK  ADD CONSTRAINT [FK_MemberTags_Organizations] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[OrganizationMembers]'
GO
ALTER TABLE [dbo].[OrganizationMembers] WITH NOCHECK  ADD CONSTRAINT [ORGANIZATION_MEMBERS_PPL_FK] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId]) ON DELETE CASCADE
ALTER TABLE [dbo].[OrganizationMembers] WITH NOCHECK  ADD CONSTRAINT [ORGANIZATION_MEMBERS_ORG_FK] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[OrganizationMembers] WITH NOCHECK  ADD CONSTRAINT [FK_ORGANIZATION_MEMBERS_TBL_MemberType] FOREIGN KEY ([MemberTypeId]) REFERENCES [lookup].[MemberType] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[TagPerson]'
GO
ALTER TABLE [dbo].[TagPerson] WITH NOCHECK  ADD CONSTRAINT [Tags__Person] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[TagPerson] WITH NOCHECK  ADD CONSTRAINT [PersonTags__Tag] FOREIGN KEY ([Id]) REFERENCES [dbo].[Tag] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[OrganizationExtra]'
GO
ALTER TABLE [dbo].[OrganizationExtra] WITH NOCHECK  ADD CONSTRAINT [FK_OrganizationExtra_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Organizations]'
GO
ALTER TABLE [dbo].[Organizations] WITH NOCHECK  ADD CONSTRAINT [ChildOrgs__ParentOrg] FOREIGN KEY ([ParentOrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Task]'
GO
ALTER TABLE [dbo].[Task] WITH NOCHECK  ADD CONSTRAINT [Tasks__TaskList] FOREIGN KEY ([ListId]) REFERENCES [dbo].[TaskList] ([Id])
ALTER TABLE [dbo].[Task] WITH NOCHECK  ADD CONSTRAINT [CoTasks__CoTaskList] FOREIGN KEY ([CoListId]) REFERENCES [dbo].[TaskList] ([Id])
ALTER TABLE [dbo].[Task] WITH NOCHECK  ADD CONSTRAINT [FK_Task_TaskStatus] FOREIGN KEY ([StatusId]) REFERENCES [lookup].[TaskStatus] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[TaskListOwners]'
GO
ALTER TABLE [dbo].[TaskListOwners] WITH NOCHECK  ADD CONSTRAINT [FK_TaskListOwners_TaskList] FOREIGN KEY ([TaskListId]) REFERENCES [dbo].[TaskList] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Volunteer]'
GO
ALTER TABLE [dbo].[Volunteer] WITH NOCHECK  ADD CONSTRAINT [FK_Volunteer_VolApplicationStatus] FOREIGN KEY ([StatusId]) REFERENCES [lookup].[VolApplicationStatus] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Zips]'
GO
ALTER TABLE [dbo].[Zips] WITH NOCHECK  ADD CONSTRAINT [FK_Zips_ResidentCode] FOREIGN KEY ([MetroMarginalCode]) REFERENCES [lookup].[ResidentCode] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[VoluteerApprovalIds]'
GO
ALTER TABLE [dbo].[VoluteerApprovalIds] WITH NOCHECK  ADD CONSTRAINT [FK_VoluteerApprovalIds_VolunteerCodes] FOREIGN KEY ([ApprovalId]) REFERENCES [lookup].[VolunteerCodes] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Attend]'
GO
ALTER TABLE [dbo].[Attend] ADD CONSTRAINT [FK_AttendWithAbsents_TBL_PEOPLE_TBL] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[Attend] ADD CONSTRAINT [FK_AttendWithAbsents_TBL_MEETINGS_TBL] FOREIGN KEY ([MeetingId]) REFERENCES [dbo].[Meetings] ([MeetingId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[SubRequest]'
GO
ALTER TABLE [dbo].[SubRequest] ADD CONSTRAINT [SubRequests__Attend] FOREIGN KEY ([AttendId]) REFERENCES [dbo].[Attend] ([AttendId])
ALTER TABLE [dbo].[SubRequest] ADD CONSTRAINT [SubRequests__Requestor] FOREIGN KEY ([RequestorId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[SubRequest] ADD CONSTRAINT [SubResponses__Substitute] FOREIGN KEY ([SubstituteId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[ActivityLog]'
GO
ALTER TABLE [dbo].[ActivityLog] ADD CONSTRAINT [FK_ActivityLog_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
ALTER TABLE [dbo].[ActivityLog] ADD CONSTRAINT [FK_ActivityLog_Organizations] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[ActivityLog] ADD CONSTRAINT [FK_ActivityLog_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[BackgroundChecks]'
GO
ALTER TABLE [dbo].[BackgroundChecks] ADD CONSTRAINT [People__User] FOREIGN KEY ([UserID]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[BackgroundChecks] ADD CONSTRAINT [FK_BackgroundChecks_People] FOREIGN KEY ([PeopleID]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[CardIdentifiers]'
GO
ALTER TABLE [dbo].[CardIdentifiers] ADD CONSTRAINT [FK_CardIdentifiers_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[CheckInActivity]'
GO
ALTER TABLE [dbo].[CheckInActivity] ADD CONSTRAINT [FK_CheckInActivity_CheckInTimes] FOREIGN KEY ([Id]) REFERENCES [dbo].[CheckInTimes] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[CheckInTimes]'
GO
ALTER TABLE [dbo].[CheckInTimes] ADD CONSTRAINT [Guests__GuestOf] FOREIGN KEY ([GuestOfId]) REFERENCES [dbo].[CheckInTimes] ([Id])
ALTER TABLE [dbo].[CheckInTimes] ADD CONSTRAINT [FK_CheckInTimes_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Contactees]'
GO
ALTER TABLE [dbo].[Contactees] ADD CONSTRAINT [contactees__contact] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Contact] ([ContactId])
ALTER TABLE [dbo].[Contactees] ADD CONSTRAINT [contactsHad__person] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Contactors]'
GO
ALTER TABLE [dbo].[Contactors] ADD CONSTRAINT [contactsMakers__contact] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Contact] ([ContactId])
ALTER TABLE [dbo].[Contactors] ADD CONSTRAINT [contactsMade__person] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Task]'
GO
ALTER TABLE [dbo].[Task] ADD CONSTRAINT [TasksAssigned__SourceContact] FOREIGN KEY ([SourceContactId]) REFERENCES [dbo].[Contact] ([ContactId])
ALTER TABLE [dbo].[Task] ADD CONSTRAINT [TasksCompleted__CompletedContact] FOREIGN KEY ([CompletedContactId]) REFERENCES [dbo].[Contact] ([ContactId])
ALTER TABLE [dbo].[Task] ADD CONSTRAINT [Tasks__Owner] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[Task] ADD CONSTRAINT [TasksAboutPerson__AboutWho] FOREIGN KEY ([WhoId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[Task] ADD CONSTRAINT [TasksCoOwned__CoOwner] FOREIGN KEY ([CoOwnerId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Coupons]'
GO
ALTER TABLE [dbo].[Coupons] ADD CONSTRAINT [FK_Coupons_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[Coupons] ADD CONSTRAINT [FK_Coupons_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[AuditValues]'
GO
ALTER TABLE [dbo].[AuditValues] ADD CONSTRAINT [FK_AuditValues_Audits] FOREIGN KEY ([AuditId]) REFERENCES [dbo].[Audits] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[DivOrg]'
GO
ALTER TABLE [dbo].[DivOrg] ADD CONSTRAINT [FK_DivOrg_Division] FOREIGN KEY ([DivId]) REFERENCES [dbo].[Division] ([Id])
ALTER TABLE [dbo].[DivOrg] ADD CONSTRAINT [FK_DivOrg_Organizations] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Contribution]'
GO
ALTER TABLE [dbo].[Contribution] ADD CONSTRAINT [FK_Contribution_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Organizations]'
GO
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_Organizations_Division] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Division] ([Id])
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_ORGANIZATIONS_TBL_OrganizationStatus] FOREIGN KEY ([OrganizationStatusId]) REFERENCES [lookup].[OrganizationStatus] ([Id])
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_ORGANIZATIONS_TBL_EntryPoint] FOREIGN KEY ([EntryPointId]) REFERENCES [lookup].[EntryPoint] ([Id])
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_Organizations_Campus] FOREIGN KEY ([CampusId]) REFERENCES [lookup].[Campus] ([Id])
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_Organizations_Gender] FOREIGN KEY ([GenderId]) REFERENCES [lookup].[Gender] ([Id])
ALTER TABLE [dbo].[Organizations] ADD CONSTRAINT [FK_Organizations_OrganizationType] FOREIGN KEY ([OrganizationTypeId]) REFERENCES [lookup].[OrganizationType] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[ProgDiv]'
GO
ALTER TABLE [dbo].[ProgDiv] ADD CONSTRAINT [FK_ProgDiv_Division] FOREIGN KEY ([DivId]) REFERENCES [dbo].[Division] ([Id])
ALTER TABLE [dbo].[ProgDiv] ADD CONSTRAINT [FK_ProgDiv_Program] FOREIGN KEY ([ProgId]) REFERENCES [dbo].[Program] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Division]'
GO
ALTER TABLE [dbo].[Division] ADD CONSTRAINT [FK_Division_Program] FOREIGN KEY ([ProgId]) REFERENCES [dbo].[Program] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[EmailQueueTo]'
GO
ALTER TABLE [dbo].[EmailQueueTo] ADD CONSTRAINT [FK_EmailQueueTo_EmailQueue] FOREIGN KEY ([Id]) REFERENCES [dbo].[EmailQueue] ([Id])
ALTER TABLE [dbo].[EmailQueueTo] ADD CONSTRAINT [FK_EmailQueueTo_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[EmailLinks]'
GO
ALTER TABLE [dbo].[EmailLinks] ADD CONSTRAINT [FK_EmailLinks_EmailQueue] FOREIGN KEY ([EmailID]) REFERENCES [dbo].[EmailQueue] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[EmailResponses]'
GO
ALTER TABLE [dbo].[EmailResponses] ADD CONSTRAINT [FK_EmailResponses_EmailQueue] FOREIGN KEY ([EmailQueueId]) REFERENCES [dbo].[EmailQueue] ([Id])
ALTER TABLE [dbo].[EmailResponses] ADD CONSTRAINT [FK_EmailResponses_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[EmailOptOut]'
GO
ALTER TABLE [dbo].[EmailOptOut] ADD CONSTRAINT [FK_EmailOptOut_People] FOREIGN KEY ([ToPeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[EmailQueue]'
GO
ALTER TABLE [dbo].[EmailQueue] ADD CONSTRAINT [FK_EmailQueue_People] FOREIGN KEY ([QueuedBy]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[FamilyCheckinLock]'
GO
ALTER TABLE [dbo].[FamilyCheckinLock] ADD CONSTRAINT [FK_FamilyCheckinLock_FamilyCheckinLock1] FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families] ([FamilyId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[FamilyExtra]'
GO
ALTER TABLE [dbo].[FamilyExtra] ADD CONSTRAINT [FK_FamilyExtra_Family] FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families] ([FamilyId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[People]'
GO
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_Families] FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families] ([FamilyId])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_DropType] FOREIGN KEY ([DropCodeId]) REFERENCES [lookup].[DropType] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_Gender] FOREIGN KEY ([GenderId]) REFERENCES [lookup].[Gender] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_MaritalStatus] FOREIGN KEY ([MaritalStatusId]) REFERENCES [lookup].[MaritalStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_FamilyPosition] FOREIGN KEY ([PositionInFamilyId]) REFERENCES [lookup].[FamilyPosition] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_MemberStatus] FOREIGN KEY ([MemberStatusId]) REFERENCES [lookup].[MemberStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_Origin] FOREIGN KEY ([OriginId]) REFERENCES [lookup].[Origin] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_EntryPoint] FOREIGN KEY ([EntryPointId]) REFERENCES [lookup].[EntryPoint] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_InterestPoint] FOREIGN KEY ([InterestPointId]) REFERENCES [lookup].[InterestPoint] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_BaptismType] FOREIGN KEY ([BaptismTypeId]) REFERENCES [lookup].[BaptismType] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_BaptismStatus] FOREIGN KEY ([BaptismStatusId]) REFERENCES [lookup].[BaptismStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_DecisionType] FOREIGN KEY ([DecisionTypeId]) REFERENCES [lookup].[DecisionType] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_DiscoveryClassStatus] FOREIGN KEY ([NewMemberClassStatusId]) REFERENCES [lookup].[NewMemberClassStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_MemberLetterStatus] FOREIGN KEY ([LetterStatusId]) REFERENCES [lookup].[MemberLetterStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_JoinType] FOREIGN KEY ([JoinCodeId]) REFERENCES [lookup].[JoinType] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [EnvPeople__EnvelopeOption] FOREIGN KEY ([EnvelopeOptionsId]) REFERENCES [lookup].[EnvelopeOption] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [ResCodePeople__ResidentCode] FOREIGN KEY ([ResCodeId]) REFERENCES [lookup].[ResidentCode] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_PEOPLE_TBL_Picture] FOREIGN KEY ([PictureId]) REFERENCES [dbo].[Picture] ([PictureId])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [StmtPeople__ContributionStatementOption] FOREIGN KEY ([ContributionOptionsId]) REFERENCES [lookup].[EnvelopeOption] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [BFMembers__BFClass] FOREIGN KEY ([BibleFellowshipClassId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_Campus] FOREIGN KEY ([CampusId]) REFERENCES [lookup].[Campus] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Families]'
GO
ALTER TABLE [dbo].[Families] ADD CONSTRAINT [ResCodeFamilies__ResidentCode] FOREIGN KEY ([ResCodeId]) REFERENCES [lookup].[ResidentCode] ([Id])
ALTER TABLE [dbo].[Families] ADD CONSTRAINT [FamiliesHeaded__HeadOfHousehold] FOREIGN KEY ([HeadOfHouseholdId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[Families] ADD CONSTRAINT [FamiliesHeaded2__HeadOfHouseholdSpouse] FOREIGN KEY ([HeadOfHouseholdSpouseId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[Families] ADD CONSTRAINT [FK_Families_Picture] FOREIGN KEY ([PictureId]) REFERENCES [dbo].[Picture] ([PictureId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[EnrollmentTransaction]'
GO
ALTER TABLE [dbo].[EnrollmentTransaction] ADD CONSTRAINT [DescTransactions__FirstTransaction] FOREIGN KEY ([EnrollmentTransactionId]) REFERENCES [dbo].[EnrollmentTransaction] ([TransactionId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[MeetingExtra]'
GO
ALTER TABLE [dbo].[MeetingExtra] ADD CONSTRAINT [FK_MeetingExtra_Meetings] FOREIGN KEY ([MeetingId]) REFERENCES [dbo].[Meetings] ([MeetingId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[VolRequest]'
GO
ALTER TABLE [dbo].[VolRequest] ADD CONSTRAINT [VolRequests__Meeting] FOREIGN KEY ([MeetingId]) REFERENCES [dbo].[Meetings] ([MeetingId])
ALTER TABLE [dbo].[VolRequest] ADD CONSTRAINT [VolRequests__Requestor] FOREIGN KEY ([RequestorId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[VolRequest] ADD CONSTRAINT [VolResponses__Volunteer] FOREIGN KEY ([VolunteerId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[GoerSupporter]'
GO
ALTER TABLE [dbo].[GoerSupporter] ADD CONSTRAINT [FK_Supporters__Goer] FOREIGN KEY ([GoerId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[GoerSupporter] ADD CONSTRAINT [FK_Goers__Supporter] FOREIGN KEY ([SupporterId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[MemberDocForm]'
GO
ALTER TABLE [dbo].[MemberDocForm] ADD CONSTRAINT [FK_MemberDocForm_PEOPLE_TBL] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[ManagedGiving]'
GO
ALTER TABLE [dbo].[ManagedGiving] ADD CONSTRAINT [FK_ManagedGiving_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[OrgMemMemTags]'
GO
ALTER TABLE [dbo].[OrgMemMemTags] ADD CONSTRAINT [FK_OrgMemMemTags_MemberTags] FOREIGN KEY ([MemberTagId]) REFERENCES [dbo].[MemberTags] ([Id])
ALTER TABLE [dbo].[OrgMemMemTags] ADD CONSTRAINT [FK_OrgMemMemTags_OrganizationMembers] FOREIGN KEY ([OrgId], [PeopleId]) REFERENCES [dbo].[OrganizationMembers] ([OrganizationId], [PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[PaymentInfo]'
GO
ALTER TABLE [dbo].[PaymentInfo] ADD CONSTRAINT [FK_PaymentInfo_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[PeopleExtra]'
GO
ALTER TABLE [dbo].[PeopleExtra] ADD CONSTRAINT [FK_PeopleExtra_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[RecReg]'
GO
ALTER TABLE [dbo].[RecReg] ADD CONSTRAINT [FK_RecReg_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[RecurringAmounts]'
GO
ALTER TABLE [dbo].[RecurringAmounts] ADD CONSTRAINT [FK_RecurringAmounts_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[SMSItems]'
GO
ALTER TABLE [dbo].[SMSItems] ADD CONSTRAINT [FK_SMSItems_People] FOREIGN KEY ([PeopleID]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[SMSItems] ADD CONSTRAINT [FK_SMSItems_SMSList] FOREIGN KEY ([ListID]) REFERENCES [dbo].[SMSList] ([ID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[SMSList]'
GO
ALTER TABLE [dbo].[SMSList] ADD CONSTRAINT [FK_SMSList_People] FOREIGN KEY ([SenderID]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[SMSList] ADD CONSTRAINT [FK_SMSList_SMSGroups] FOREIGN KEY ([SendGroupID]) REFERENCES [dbo].[SMSGroups] ([ID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[TagShare]'
GO
ALTER TABLE [dbo].[TagShare] ADD CONSTRAINT [FK_TagShare_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[TagShare] ADD CONSTRAINT [FK_TagShare_Tag] FOREIGN KEY ([TagId]) REFERENCES [dbo].[Tag] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[TaskListOwners]'
GO
ALTER TABLE [dbo].[TaskListOwners] ADD CONSTRAINT [FK_TaskListOwners_PEOPLE_TBL] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Transaction]'
GO
ALTER TABLE [dbo].[Transaction] ADD CONSTRAINT [FK_Transaction_People] FOREIGN KEY ([LoginPeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[Transaction] ADD CONSTRAINT [Transactions__OriginalTransaction] FOREIGN KEY ([OriginalId]) REFERENCES [dbo].[Transaction] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[TransactionPeople]'
GO
ALTER TABLE [dbo].[TransactionPeople] ADD CONSTRAINT [FK_TransactionPeople_Person] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[TransactionPeople] ADD CONSTRAINT [FK_TransactionPeople_Transaction] FOREIGN KEY ([Id]) REFERENCES [dbo].[Transaction] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Users]'
GO
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [FK_Users_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[VolInterestInterestCodes]'
GO
ALTER TABLE [dbo].[VolInterestInterestCodes] ADD CONSTRAINT [FK_VolInterestInterestCodes_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[VolInterestInterestCodes] ADD CONSTRAINT [FK_VolInterestInterestCodes_VolInterestCodes] FOREIGN KEY ([InterestCodeId]) REFERENCES [dbo].[VolInterestCodes] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Volunteer]'
GO
ALTER TABLE [dbo].[Volunteer] ADD CONSTRAINT [FK_Volunteer_PEOPLE_TBL] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[VolunteerForm]'
GO
ALTER TABLE [dbo].[VolunteerForm] ADD CONSTRAINT [FK_VolunteerForm_PEOPLE_TBL] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[VolunteerForm] ADD CONSTRAINT [VolunteerFormsUploaded__Uploader] FOREIGN KEY ([UploaderId]) REFERENCES [dbo].[Users] ([UserId])
ALTER TABLE [dbo].[VolunteerForm] ADD CONSTRAINT [FK_VolunteerForm_Volunteer1] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[Volunteer] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[VoluteerApprovalIds]'
GO
ALTER TABLE [dbo].[VoluteerApprovalIds] ADD CONSTRAINT [FK_VoluteerApprovalIds_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[VoluteerApprovalIds] ADD CONSTRAINT [FK_VoluteerApprovalIds_Volunteer] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[Volunteer] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[PeopleCanEmailFor]'
GO
ALTER TABLE [dbo].[PeopleCanEmailFor] ADD CONSTRAINT [OnBehalfOfPeople__PersonCanEmail] FOREIGN KEY ([CanEmail]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[PeopleCanEmailFor] ADD CONSTRAINT [PersonsCanEmail__OnBehalfOfPerson] FOREIGN KEY ([OnBehalfOf]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Tag]'
GO
ALTER TABLE [dbo].[Tag] ADD CONSTRAINT [TagsOwned__PersonOwner] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[Preferences]'
GO
ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [FK_UserPreferences_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[OrganizationMembers]'
GO
ALTER TABLE [dbo].[OrganizationMembers] ADD CONSTRAINT [FK_OrganizationMembers_Transaction] FOREIGN KEY ([TranId]) REFERENCES [dbo].[Transaction] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[OrgSchedule]'
GO
ALTER TABLE [dbo].[OrgSchedule] ADD CONSTRAINT [FK_OrgSchedule_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[SMSGroupMembers]'
GO
ALTER TABLE [dbo].[SMSGroupMembers] ADD CONSTRAINT [FK_SMSGroupMembers_SMSGroups] FOREIGN KEY ([GroupID]) REFERENCES [dbo].[SMSGroups] ([ID])
ALTER TABLE [dbo].[SMSGroupMembers] ADD CONSTRAINT [FK_SMSGroupMembers_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[UserRole]'
GO
ALTER TABLE [dbo].[UserRole] ADD CONSTRAINT [FK_UserRole_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId])
ALTER TABLE [dbo].[UserRole] ADD CONSTRAINT [FK_UserRole_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [lookup].[MemberType]'
GO
ALTER TABLE [lookup].[MemberType] ADD CONSTRAINT [FK_MemberType_AttendType] FOREIGN KEY ([AttendanceTypeId]) REFERENCES [lookup].[AttendType] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
