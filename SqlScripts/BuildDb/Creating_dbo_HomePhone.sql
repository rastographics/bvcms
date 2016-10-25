CREATE FUNCTION [dbo].[HomePhone](@pid int)
RETURNS nvarchar(11)
AS
	BEGIN
	declare @homephone nvarchar(11)
	select @homephone = f.HomePhone from dbo.People p join dbo.Families f on f.FamilyId = p.FamilyId
where PeopleId = @pid

	RETURN @homephone
	END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
