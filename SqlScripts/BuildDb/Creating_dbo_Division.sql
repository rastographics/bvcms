CREATE TABLE [dbo].[Division]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (50) NULL,
[ProgId] [int] NULL,
[SortOrder] [int] NULL,
[EmailMessage] [nvarchar] (max) NULL,
[EmailSubject] [nvarchar] (100) NULL,
[Instructions] [nvarchar] (max) NULL,
[Terms] [nvarchar] (max) NULL,
[ReportLine] [int] NULL,
[NoDisplayZero] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
