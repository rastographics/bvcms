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
IF @@ERROR <> 0 SET NOEXEC ON
GO
