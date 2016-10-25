CREATE FUNCTION [dbo].[PrimaryState] ( @pid int )
RETURNS nvarchar(5)
AS
	BEGIN
declare @st nvarchar(5)
select @st =
	case AddressTypeId
			when 10 then f.StateCode
			when 30 then p.StateCode
	end
from dbo.People p join dbo.Families f on f.FamilyId = p.FamilyId
where PeopleId = @pid

	RETURN @st
	END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
