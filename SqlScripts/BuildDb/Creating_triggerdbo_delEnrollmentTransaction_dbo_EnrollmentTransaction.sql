-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[delEnrollmentTransaction]
   ON  [dbo].[EnrollmentTransaction]
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

	UPDATE dbo.Organizations
	SET PrevMemberCount = dbo.OrganizationPrevCount(o.OrganizationId)
	FROM dbo.Organizations o
	JOIN deleted i ON i.OrganizationId = o.OrganizationId

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
