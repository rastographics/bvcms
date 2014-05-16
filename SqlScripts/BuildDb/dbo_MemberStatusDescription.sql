CREATE FUNCTION [dbo].[MemberStatusDescription](@pid int)
RETURNS nvarchar(50)
AS
	BEGIN
	declare @desc nvarchar(50)
	select @desc = m.description from lookup.memberstatus m
	join dbo.People p on p.MemberStatusId = m.id
	where p.PeopleId = @pid
	return @desc
	END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
