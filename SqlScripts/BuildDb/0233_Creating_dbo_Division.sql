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
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
