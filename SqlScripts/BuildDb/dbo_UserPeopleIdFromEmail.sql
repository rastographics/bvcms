-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[UserPeopleIdFromEmail]
(
@email nvarchar(50)
)
RETURNS INT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @pid INT

	-- Add the T-SQL statements to compute the return value here
	SELECT TOP(1) @pid = PeopleId FROM dbo.Users
	WHERE EmailAddress = @email
	

	-- Return the result of the function
	RETURN @pid

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
