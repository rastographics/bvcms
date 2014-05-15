CREATE FUNCTION [dbo].[PrimaryBadAddressFlag]( @pid int )
RETURNS int
AS
	BEGIN
declare @flag bit
select @flag =
	case AddressTypeId
		when 10 then f.BadAddressFlag
		when 30 then p.BadAddressFlag
	end
	
from dbo.People p join dbo.Families f on f.FamilyId = p.FamilyId
where PeopleId = @pid

if (@flag is null)
	select @flag = 0

	RETURN @flag
	END

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
