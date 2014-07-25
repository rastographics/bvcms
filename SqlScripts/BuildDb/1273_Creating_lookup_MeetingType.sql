CREATE TABLE [lookup].[MeetingType]
(
[Id] [int] NOT NULL,
[Code] [char] (1) NULL,
[Description] [nvarchar] (10) NULL,
[Hardwired] [bit] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
