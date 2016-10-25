SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO
CREATE FUNCTION [dbo].[AllRegexMatchs] (@subject [nvarchar] (max), @pattern [nvarchar] (200))
RETURNS [nvarchar] (max)
WITH EXECUTE AS CALLER
EXTERNAL NAME [CmsDataSqlClr].[CmsDataSqlClr.UserDefinedFunctions].[AllRegexMatchs]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
