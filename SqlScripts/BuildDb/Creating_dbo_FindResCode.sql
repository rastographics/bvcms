CREATE FUNCTION [dbo].[FindResCode]
(
	@zipcode nvarchar(20), @country NVARCHAR(50)
)
RETURNS int
AS
BEGIN
	DECLARE @Result int
	DECLARE @z5 nvarchar(10)

	IF (@zipcode IS NOT NULL AND LEN(@zipcode) >= 5)
	BEGIN
		SELECT @z5 = SUBSTRING(@zipcode, 1, 5)

		SELECT @Result = MetroMarginalCode 
		FROM dbo.Zips 
		WHERE ZipCode = @z5
		AND (@country IS NULL OR @country = '' OR @country in ('US', 'USA', 'United States', 'USA, Not Validated'))

		IF @Result IS NULL
			SELECT @Result = 30
	END
	-- Return the result of the function
	RETURN @Result

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
