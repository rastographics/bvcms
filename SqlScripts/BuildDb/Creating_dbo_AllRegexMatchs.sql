CREATE FUNCTION [dbo].[AllRegexMatchs] (@subject [nvarchar] (max), @pattern [nvarchar] (max))
RETURNS [nvarchar] (max)
WITH EXECUTE AS CALLER
EXTERNAL NAME [CmsSqlClr].[UserDefinedFunctions].[AllRegexMatchs]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
