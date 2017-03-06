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
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
