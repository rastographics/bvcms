CREATE TABLE [dbo].[EmailQueueTo]
(
[Id] [int] NOT NULL,
[PeopleId] [int] NOT NULL,
[OrgId] [int] NULL,
[Sent] [datetime] NULL,
[AddEmail] [nvarchar] (max) NULL,
[guid] [uniqueidentifier] NULL,
[messageid] [nvarchar] (100) NULL,
[GoerSupportId] [int] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
