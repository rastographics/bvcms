CREATE TABLE [dbo].[CustomColumns]
(
[Ord] [int] NOT NULL,
[Column] [varchar] (50) NOT NULL,
[Select] [varchar] (300) NULL,
[JoinTable] [varchar] (200) NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
