CREATE FUNCTION [dbo].[RegexMatch] (@subject [nvarchar] (4000), @pattern [nvarchar] (4000))
RETURNS [nvarchar] (4000)
WITH EXECUTE AS CALLER
EXTERNAL NAME [CmsDataSqlClr].[UserDefinedFunctions].[RegexMatch]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
