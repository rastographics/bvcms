CREATE FUNCTION [dbo].[PrimaryAddress] ( @pid int )
RETURNS nvarchar(60)
AS
	BEGIN
declare @addr nvarchar(60)
select @addr =
	case AddressTypeId
			when 10 then f.AddressLineOne
			when 30 then p.AddressLineOne
	end
from dbo.People p join dbo.Families f on f.FamilyId = p.FamilyId
where PeopleId = @pid

	RETURN @addr
	END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
