CREATE FUNCTION [dbo].[NextTranChangeDate]
(
  @pid int
 ,@oid int
 ,@tdt datetime
 ,@typeid int
)
RETURNS datetime
AS
	BEGIN
	  DECLARE @dt datetime 
		  select top 1 @dt = TransactionDate
			from dbo.EnrollmentTransaction
		   where TransactionTypeId >= 3
		     and @typeid <= 3
			 and PeopleId = @pid
			 and OrganizationId = @oid
			 and TransactionDate > @tdt
	   order by TransactionDate
	RETURN @dt
	END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
