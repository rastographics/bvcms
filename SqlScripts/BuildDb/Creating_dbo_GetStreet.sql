SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO
CREATE FUNCTION [dbo].[GetStreet] (@address [nvarchar] (max))
RETURNS [nvarchar] (max)
WITH EXECUTE AS CALLER
EXTERNAL NAME [CmsSqlClr].[UserDefinedFunctions].[GetStreet]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
