CREATE FUNCTION [dbo].[PrimaryAddress2] ( @pid int )
RETURNS nvarchar(60)
AS
	BEGIN
declare @addr nvarchar(60)
select @addr =
	case AddressTypeId
			when 10 then f.AddressLineTwo
			when 30 then p.AddressLineTwo
	end
from dbo.People p join dbo.Families f on f.FamilyId = p.FamilyId
where PeopleId = @pid

	RETURN @addr
	END

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
