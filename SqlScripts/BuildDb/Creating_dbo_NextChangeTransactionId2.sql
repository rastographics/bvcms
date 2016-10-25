CREATE FUNCTION [dbo].[NextChangeTransactionId2]
(
  @pid int
 ,@oid int
 ,@tid int
 ,@typeid int
)
RETURNS int
AS
	BEGIN
	  DECLARE @rtid int 
		  select top 1 @rtid = TransactionId
			from dbo.EnrollmentTransaction
		   where TransactionTypeId >= 3
		     and @typeid <= 3
			 and PeopleId = @pid
			 and OrganizationId = @oid
			 and TransactionId > @tid
			 AND TransactionStatus = 0
	   order by TransactionId
	RETURN @rtid
	END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
