CREATE FUNCTION [dbo].[FindPerson0](@first nvarchar(25), @last nvarchar(50), @dob DATETIME, @email nvarchar(60), @phone nvarchar(15))
RETURNS INT 
AS
BEGIN

	DECLARE @n INT

	IF (SELECT COUNT(*) FROM dbo.FindPerson(@first, @last, @dob, @email, @phone)) = 1
		SELECT @n = PeopleId FROM dbo.FindPerson(@first, @last, @dob, @email, @phone)

	RETURN @n
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
