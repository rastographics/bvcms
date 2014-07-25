CREATE TABLE [dbo].[RegistrationData]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Data] [xml] NULL,
[Stamp] [datetime] NULL,
[completed] [bit] NULL,
[OrganizationId] [int] NULL,
[UserPeopleId] [int] NULL,
[abandoned] [bit] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
