CREATE TABLE [lookup].[StateLookup]
(
[StateCode] [nvarchar] (10) NOT NULL,
[StateName] [nvarchar] (30) NULL,
[Hardwired] [bit] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
