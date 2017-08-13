SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO
CREATE FUNCTION [dbo].[RegexMatch] (@subject [nvarchar] (max), @pattern [nvarchar] (max))
RETURNS [nvarchar] (max)
WITH EXECUTE AS CALLER
EXTERNAL NAME [CmsSqlClr].[UserDefinedFunctions].[RegexMatch]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
