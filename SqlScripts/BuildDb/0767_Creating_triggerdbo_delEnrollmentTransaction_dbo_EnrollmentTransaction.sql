-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER dbo.delEnrollmentTransaction
   ON  dbo.EnrollmentTransaction
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @oid INT, @pid INT, @tid INT

	DECLARE det CURSOR FORWARD_ONLY FOR
	SELECT OrganizationId, PeopleId 
	FROM deleted 

	OPEN det
	FETCH NEXT FROM det INTO @oid, @pid
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SELECT TOP 1 @tid = TransactionId
		FROM dbo.EnrollmentTransaction
		WHERE OrganizationId = @oid AND PeopleId = @pid
		ORDER BY TransactionId DESC
		
		UPDATE dbo.EnrollmentTransaction
		SET NextTranChangeDate = NULL
		WHERE TransactionId = @tid
		
		FETCH NEXT FROM det INTO @oid, @pid
	END
	CLOSE det
	DEALLOCATE det

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
