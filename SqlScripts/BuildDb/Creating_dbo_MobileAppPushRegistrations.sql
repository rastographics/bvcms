CREATE TABLE [dbo].[MobileAppPushRegistrations]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Type] [int] NOT NULL CONSTRAINT [DF_MobileAppPushRegistrations_type] DEFAULT ((0)),
[PeopleId] [int] NOT NULL CONSTRAINT [DF_MobileAppPushRegistrations_peopleId] DEFAULT ((0)),
[RegistrationId] [varchar] (max) NOT NULL CONSTRAINT [DF_MobileAppPushRegistrations_registrationId] DEFAULT (''),
[Priority] [int] NOT NULL CONSTRAINT [DF_MobileAppPushRegistrations_Priority] DEFAULT ((0)),
[Enabled] [bit] NOT NULL CONSTRAINT [DF_MobileAppPushRegistrations_enabled] DEFAULT ((1))
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
