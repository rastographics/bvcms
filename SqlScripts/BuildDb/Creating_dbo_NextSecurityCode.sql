CREATE PROCEDURE [dbo].[NextSecurityCode]
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION

	DECLARE @midnight DATE = GETDATE()
	DECLARE @4code BIT = (CASE WHEN EXISTS(SELECT NULL FROM dbo.Setting WHERE Id = 'UseFourDigitCodeForCheckin' AND Setting = 'true') THEN 1 ELSE 0 END)
	DECLARE @hascodes BIT = (CASE WHEN EXISTS(SELECT NULL FROM dbo.SecurityCodes) THEN 1 ELSE 0 END)
	DECLARE @hasalpha BIT = (CASE WHEN EXISTS(SELECT NULL FROM dbo.SecurityCodes WHERE Code LIKE '%[A-Z]%') THEN 1 ELSE 0 END)
	DECLARE @needcodes BIT = (CASE WHEN @hascodes = 0 THEN 1 
								   WHEN @4code = 1 AND @hasalpha = 1 THEN 1 
								   WHEN @4code = 0 AND @hasalpha = 0 THEN 1
								   ELSE 0 END)
	IF (@needcodes = 1)
	BEGIN
		EXEC dbo.SetupNumbers
		DELETE dbo.SecurityCodes WHERE Id IS NOT NULL
		IF(@4code = 1)
			INSERT dbo.SecurityCodes ( Id, Code, DateUsed )
			SELECT Number, Number, '1/1/80' 
			FROM dbo.Numbers
			WHERE Number >= 1000 AND Number < 10000
        ELSE
			WITH codes AS ( SELECT n = Number, code = dbo.DecToBase(Number, 36) FROM dbo.Numbers )
			INSERT dbo.SecurityCodes ( Id, Code, DateUsed )
			SELECT n, code, '1/1/80' FROM codes
			WHERE LEN(code) = 3
					AND code LIKE '%[1-9]%' -- must have some digits
					AND code LIKE '%[A-Z]%' -- must have some letters
					AND code NOT LIKE '%I%' -- no char I 
					AND code NOT LIKE '%O%' -- no char O
					AND code NOT LIKE '%0%' -- no digit 0
	END
    
	DECLARE @id INT
	SELECT TOP 1 @id = id FROM SecurityCodes WHERE DateUsed < @midnight ORDER BY NEWID()
	UPDATE SecurityCodes SET DateUsed = @midnight WHERE id = ISNULL(@id, 0)
	IF NOT EXISTS(SELECT NULL FROM dbo.SecurityCodes WHERE DateUsed < @midnight)
		UPDATE dbo.SecurityCodes SET DateUsed = '1/1/80' WHERE Id <> ISNULL(@id, 0)

	COMMIT
	SELECT TOP 1 * FROM dbo.SecurityCodes WHERE id = @id
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
