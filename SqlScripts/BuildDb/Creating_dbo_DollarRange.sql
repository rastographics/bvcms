-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[DollarRange](@amt DECIMAL)
RETURNS int
AS
BEGIN
DECLARE @ret INT 
	SELECT @ret =
	CASE
		WHEN @amt IS NULL THEN 1
		WHEN @amt < 101 THEN 1
		WHEN @amt < 251 THEN 2
		WHEN @amt < 501 THEN 3
		WHEN @amt < 751 THEN 4
		WHEN @amt < 1001 THEN 5
		WHEN @amt < 2001 THEN 6
		WHEN @amt < 3001 THEN 7
		WHEN @amt < 4001 THEN 8
		WHEN @amt < 5001 THEN 9
		WHEN @amt < 10001 THEN 10
		WHEN @amt < 20001 THEN 11
		WHEN @amt < 30001 THEN 12
		WHEN @amt < 40001 THEN 13
		WHEN @amt < 50001 THEN 14
		WHEN @amt < 100001 THEN 15
		ELSE 16
	END 
	RETURN @ret
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
