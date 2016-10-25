CREATE FUNCTION [dbo].[FmtPhone](@PhoneNumber nvarchar(32))
RETURNS nvarchar(32)
AS
BEGIN
	SET @PhoneNumber = dbo.GetDigits(@PhoneNumber)
	IF LEN(@PhoneNumber) >= 10
		RETURN Stuff(Stuff(@PhoneNumber,7,0,'-'),4,0,'-')
	IF LEN(@PhoneNumber) = 7
		RETURN Stuff(@PhoneNumber,4,0,'-')
	RETURN @PhoneNumber
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
