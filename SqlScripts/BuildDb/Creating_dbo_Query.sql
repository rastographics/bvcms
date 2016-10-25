CREATE TABLE [dbo].[Query]
(
[QueryId] [uniqueidentifier] NOT NULL,
[text] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[owner] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[created] [datetime] NULL,
[lastRun] [datetime] NULL,
[name] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ispublic] [bit] NOT NULL CONSTRAINT [DF_Query_ispublic] DEFAULT ((0)),
[runCount] [int] NOT NULL CONSTRAINT [DF_Query_runCount] DEFAULT ((0)),
[CopiedFrom] [uniqueidentifier] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
