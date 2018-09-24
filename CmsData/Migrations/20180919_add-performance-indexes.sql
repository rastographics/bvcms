-- further optimize these queries to either not execute so often or do it in a better way
-- SELECT [t0].[Name], [t0].[Type], [t0].[Role], [t0].[ShowOnOrgId], [t0].[class] AS [ClassX], [t0].[Url]  FROM [CustomScriptRoles] AS [t0]
-- UpdateAttendStr functions (SELECT TOP 52 ...)

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_People_DeceasedDate' AND object_id = OBJECT_ID('dbo.People'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_People_DeceasedDate]
    ON [dbo].[People] ([DeceasedDate])
    INCLUDE ([PeopleId],[CreatedBy],[CreatedDate],[DropCodeId],[GenderId],[DoNotMailFlag],[DoNotCallFlag],[DoNotVisitFlag],[AddressTypeId],[PhonePrefId],[MaritalStatusId],[PositionInFamilyId],[MemberStatusId],[FamilyId],[BirthMonth],[BirthDay],[BirthYear],[OriginId],[EntryPointId],[InterestPointId],[BaptismTypeId],[BaptismStatusId],[DecisionTypeId],[NewMemberClassStatusId],[LetterStatusId],[JoinCodeId],[EnvelopeOptionsId],[BadAddressFlag],[ResCodeId],[AddressFromDate],[AddressToDate],[WeddingDate],[OriginDate],[BaptismSchedDate],[BaptismDate],[DecisionDate],[LetterDateRequested],[LetterDateReceived],[JoinDate],[DropDate],[TitleCode],[FirstName],[MiddleName],[MaidenName],[LastName],[SuffixCode],[NickName],[AddressLineOne],[AddressLineTwo],[CityName],[StateCode],[ZipCode],[CountryName],[StreetName],[CellPhone],[WorkPhone],[EmailAddress],[OtherPreviousChurch],[OtherNewChurch],[SchoolOther],[EmployerOther],[OccupationOther],[HobbyOther],[SkillOther],[InterestOther],[LetterStatusNotes],[Comments],[ChristAsSavior],[MemberAnyChurch],[InterestedInJoining],[PleaseVisit],[InfoBecomeAChristian],[ContributionsStatement],[ModifiedBy],[ModifiedDate],[PictureId],[ContributionOptionsId],[PrimaryCity],[PrimaryZip],[PrimaryAddress],[PrimaryState],[HomePhone],[SpouseId],[PrimaryAddress2],[PrimaryResCode],[PrimaryBadAddrFlag],[LastContact],[Grade],[CellPhoneLU],[WorkPhoneLU],[BibleFellowshipClassId],[CampusId],[CellPhoneAC],[CheckInNotes],[AltName],[CustodyIssue],[OkTransport],[BDate],[HasDuplicates],[FirstName2],[EmailAddress2],[SendEmailAddress1],[SendEmailAddress2],[NewMemberClassDate],[PrimaryCountry],[ReceiveSMS],[DoNotPublishPhones],[IsDeceased],[SSN],[DLN],[DLStateID],[HashNum],[PreferredName],[Name2],[Name],[ElectronicStatement])
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Tag_PeopleId' AND object_id = OBJECT_ID('dbo.Tag'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Tag_PeopleId]
    ON [dbo].[Tag] ([PeopleId])
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Contribution_FundId_StatusId_TypeId_Date' AND object_id = OBJECT_ID('dbo.Contribution'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Contribution_FundId_StatusId_TypeId_Date]
    ON [dbo].[Contribution] ([FundId],[ContributionStatusId],[ContributionTypeId],[ContributionDate])
    INCLUDE ([PeopleId],[ContributionAmount])
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Users_Username' AND object_id = OBJECT_ID('dbo.Users'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Users_Username]
	ON [dbo].[Users] ([Username])
END
GO