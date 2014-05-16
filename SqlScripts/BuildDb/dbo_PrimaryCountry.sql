CREATE FUNCTION [dbo].[PrimaryCountry] ( @pid int )
RETURNS nvarchar(30)
AS
	BEGIN
declare @n nvarchar(30)
select @n =
	case AddressTypeId
			when 10 then f.CountryName
			when 30 then p.CountryName
	end
from dbo.People p join dbo.Families f on f.FamilyId = p.FamilyId
where PeopleId = @pid

	RETURN @n
	END

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
