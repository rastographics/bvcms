CREATE FUNCTION [dbo].[RegexMatch] (@subject [nvarchar] (max), @pattern [nvarchar] (200))
RETURNS [nvarchar] (max)
WITH EXECUTE AS CALLER
EXTERNAL NAME [CmsDataSqlClr].[CmsDataSqlClr.UserDefinedFunctions].[RegexMatch]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
