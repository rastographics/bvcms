CREATE TABLE [dbo].[Program]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (50) NULL,
[RptGroup] [nvarchar] (200) NULL,
[StartHoursOffset] [real] NULL,
[EndHoursOffset] [real] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
