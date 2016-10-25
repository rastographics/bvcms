-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[ParseDate](@dtin varchar(50))
RETURNS DATETIME
AS
BEGIN
	-- Declare the return variable here
	DECLARE @dt DATETIME

	SELECT @dt = TRY_CONVERT(DATETIME, @dtin)
	IF @dt IS NOT NULL
		RETURN @dt

	DECLARE @digits INT = LEN(dbo.GetDigits(@dtin))
	IF @digits < LEN(@dtin)
		RETURN NULL

	DECLARE @s VARCHAR(10)
	IF @digits = 6
		SELECT @s = SUBSTRING(@dtin, 1, 2) + '/' + SUBSTRING(@dtin, 3, 2) + '/' + SUBSTRING(@dtin, 5, 2)
	IF @digits = 8
		SELECT @s = SUBSTRING(@dtin, 1, 2) + '/' + SUBSTRING(@dtin, 3, 2) + '/' + SUBSTRING(@dtin, 5, 4)

	SELECT @dt = TRY_CONVERT(DATETIME, @s)
	RETURN @dt

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
