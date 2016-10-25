CREATE FUNCTION [dbo].[PrimaryCity] ( @pid int )
RETURNS nvarchar(50)
AS
	BEGIN
declare @city nvarchar(50)
select @city =
	case AddressTypeId
			when 10 then f.CityName
			when 30 then p.CityName
	end
from dbo.People p join dbo.Families f on f.FamilyId = p.FamilyId
where PeopleId = @pid

	RETURN @city
	END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
