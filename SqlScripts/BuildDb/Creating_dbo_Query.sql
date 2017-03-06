CREATE TABLE [dbo].[Query]
(
[QueryId] [uniqueidentifier] NOT NULL,
[text] [nvarchar] (max) NULL,
[owner] [nvarchar] (50) NULL,
[created] [datetime] NULL,
[lastRun] [datetime] NULL,
[name] [nvarchar] (100) NULL,
[ispublic] [bit] NOT NULL CONSTRAINT [DF_Query_ispublic] DEFAULT ((0)),
[runCount] [int] NOT NULL CONSTRAINT [DF_Query_runCount] DEFAULT ((0)),
[CopiedFrom] [uniqueidentifier] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
