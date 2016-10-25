CREATE FUNCTION [dbo].[OrgFee](@oid INT)
RETURNS MONEY
AS
BEGIN
	DECLARE @start BIGINT, @end INT, @s NVARCHAR(MAX), @ret MONEY
	SELECT @s = RegSetting FROM dbo.Organizations WHERE OrganizationId = @oid
	SELECT @start = PATINDEX('%' + CHAR(10) + 'Fee:%', @s)
	IF @start = 0
		RETURN NULL
	SELECT @end = CHARINDEX(CHAR(10), SUBSTRING(@s, @start + 6, 20))
	SELECT @ret = CONVERT(MONEY, REPLACE(SUBSTRING(@s, @start + 6, @end -1),',',''))
	RETURN @ret
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
