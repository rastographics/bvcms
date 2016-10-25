-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[LinkEnrollmentTransaction] (@tid INT, @trandt DATETIME, @typeid INT, @orgid INT, @pid int)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @etid INT -- Find the original enrollment transaction
	SELECT TOP 1 @etid = TransactionId
	FROM dbo.EnrollmentTransaction
	WHERE TransactionTypeId <= 2
		AND PeopleId = @pid
		AND OrganizationId = @orgid
		AND TransactionId < @tid
		AND @typeid >= 3
	ORDER  BY  TransactionId DESC 

	-- point the current transction to the original enrollment
	UPDATE dbo.EnrollmentTransaction
	SET EnrollmentTransactionId = @etid
	WHERE TransactionId = @tid AND @etid IS NOT NULL

	DECLARE @previd INT -- find previous transaction
	
	SELECT TOP 1 @previd = TransactionId
	FROM dbo.EnrollmentTransaction
	WHERE TransactionTypeId <= 3
		AND @typeid >= 3
		AND PeopleId = @pid
		AND OrganizationId = @orgid
		AND TransactionId < @tid
	ORDER BY TransactionId DESC
	
	-- set the previous transaction's next tran date
	UPDATE dbo.EnrollmentTransaction
	SET NextTranChangeDate = @trandt
	WHERE TransactionId = @previd
	
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
