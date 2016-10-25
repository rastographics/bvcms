CREATE FUNCTION [dbo].[PrimaryResCode]( @pid int )
RETURNS int
AS
	BEGIN
declare @rescodeid int
select @rescodeid =
	case AddressTypeId
		when 10 then f.ResCodeId
		when 30 then p.ResCodeId
	end
from dbo.People p join dbo.Families f on f.FamilyId = p.FamilyId
where PeopleId = @pid

if @rescodeid is null
	select @rescodeid = 40

	RETURN @rescodeid
	END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
