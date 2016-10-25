SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[StartsLower] (@s NVARCHAR) 
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ret bit

	SELECT @ret = 0
	-- Add the T-SQL statements to compute the return value here
	SELECT @ret = 1 WHERE @s COLLATE Latin1_General_BIN2 > 'Z'


	-- Return the result of the function
	RETURN @ret

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
