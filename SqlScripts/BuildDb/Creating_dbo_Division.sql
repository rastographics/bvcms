CREATE TABLE [dbo].[Division]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ProgId] [int] NULL,
[SortOrder] [int] NULL,
[EmailMessage] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[EmailSubject] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Instructions] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Terms] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ReportLine] [int] NULL,
[NoDisplayZero] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
