CREATE TABLE [dbo].[QueryAnalysis]
(
[Id] [uniqueidentifier] NOT NULL,
[Seconds] [int] NULL,
[OriginalCount] [int] NULL,
[ParsedCount] [int] NULL,
[Message] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Updated] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
