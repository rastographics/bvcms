CREATE TABLE [dbo].[QueryAnalysis]
(
[Id] [uniqueidentifier] NOT NULL,
[Seconds] [int] NULL,
[OriginalCount] [int] NULL,
[ParsedCount] [int] NULL,
[Message] [varchar] (max) NULL,
[Updated] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
