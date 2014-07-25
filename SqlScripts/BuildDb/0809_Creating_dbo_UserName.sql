CREATE FUNCTION [dbo].[UserName] (@pid int)
RETURNS nvarchar(100)
AS
	BEGIN
	declare @name nvarchar(100)
	
SELECT  @name = [LastName]+', '+(case when [Nickname]<>'' then [nickname] else [FirstName] end)
FROM         dbo.People
WHERE     PeopleId = @pid

	return @name
	END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
