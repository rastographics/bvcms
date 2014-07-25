CREATE TABLE [dbo].[VolInterestCodes]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Description] [nvarchar] (180) NULL,
[Code] [nvarchar] (100) NULL,
[Org] [nvarchar] (150) NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
