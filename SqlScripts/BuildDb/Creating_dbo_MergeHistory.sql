CREATE TABLE [dbo].[MergeHistory]
(
[FromId] [int] NOT NULL,
[ToId] [int] NOT NULL,
[FromName] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ToName] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Dt] [datetime] NOT NULL CONSTRAINT [DF_MergeHistory_Dt] DEFAULT (getdate()),
[WhoName] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[WhoId] [int] NULL,
[Action] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
