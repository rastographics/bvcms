

CREATE FUNCTION [dbo].[FirstLast]( @name NVARCHAR(150) )
RETURNS @t TABLE 
(
	[First] NVARCHAR(150), 
	[Last] NVARCHAR(150)
)
AS
BEGIN
	DECLARE @delim VARCHAR(1) = IIF(@name LIKE '%,%', ',', ' ')
	DECLARE @names TABLE (i INT, v NVARCHAR(150))
	INSERT INTO @names SELECT TokenID, RTRIM(LTRIM(Value)) FROM dbo.Split(@name, @delim)
	DECLARE @count INT = (SELECT COUNT(*) FROM @names)

	IF @count = 1
		INSERT @t ([Last]) SELECT v FROM @names WHERE i = 1
	ELSE IF @delim = ','
	BEGIN
		INSERT @t ([Last], [First]) SELECT
			(SELECT v FROM @names WHERE i = 1),
			(SELECT v FROM @names WHERE i = 2)
	END
	ELSE
	BEGIN
		INSERT @t ([First], [Last]) SELECT
			(SELECT v FROM @names WHERE i = 1),
			(SELECT v FROM @names WHERE i = 2)
	END
	RETURN
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
