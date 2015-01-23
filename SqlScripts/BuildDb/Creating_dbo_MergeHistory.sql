CREATE TABLE [dbo].[MergeHistory]
(
[FromId] [int] NOT NULL,
[ToId] [int] NOT NULL,
[FromName] [nvarchar] (150) NULL,
[ToName] [nvarchar] (150) NULL,
[Dt] [datetime] NOT NULL CONSTRAINT [DF_MergeHistory_Dt] DEFAULT (getdate()),
[WhoName] [nvarchar] (150) NULL,
[WhoId] [int] NULL,
[Action] [varchar] (50) NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
