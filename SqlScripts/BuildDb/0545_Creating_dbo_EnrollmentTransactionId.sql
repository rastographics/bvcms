
CREATE FUNCTION [dbo].[EnrollmentTransactionId]
(
  @pid int
 ,@oid int
 ,@tdt datetime
 ,@ttid int
)
RETURNS int
AS
	BEGIN
	  DECLARE @TransactionId int
	  SELECT @TransactionId = NULL
	  if @ttid >= 3
		  select top 1 @TransactionId = et.TransactionId
			from  dbo.EnrollmentTransaction et
		   where et.TransactionTypeId <= 2
			 and et.PeopleId = @pid
			 and et.OrganizationId = @oid
			 and et.TransactionDate < @tdt
	   order by et.TransactionDate desc
	RETURN @TransactionId
	END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
