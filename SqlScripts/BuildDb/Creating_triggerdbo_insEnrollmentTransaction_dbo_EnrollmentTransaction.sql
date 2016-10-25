-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[insEnrollmentTransaction] 
   ON  [dbo].[EnrollmentTransaction] 
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE 
		@tid INT
		,@trandt DATETIME
		,@typeid INT
		,@orgid INT
		,@pid INT
		,@skipInsertTriggerProcessing BIT

	SELECT
		@skipInsertTriggerProcessing = SkipInsertTriggerProcessing
	FROM Inserted

	IF @skipInsertTriggerProcessing = 1
		RETURN

	DECLARE cet CURSOR FORWARD_ONLY FOR
	SELECT TransactionId, TransactionDate, TransactionTypeId, OrganizationId, PeopleId 
	FROM inserted 
	WHERE TransactionTypeId > 2

	OPEN cet
	FETCH NEXT FROM cet INTO @tid, @trandt, @typeid, @orgid, @pid
	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC dbo.LinkEnrollmentTransaction @tid, @trandt, @typeid, @orgid, @pid
		
		FETCH NEXT FROM cet INTO @tid, @trandt, @typeid, @orgid, @pid
	END
	CLOSE cet
	DEALLOCATE cet

	UPDATE dbo.Organizations
	SET PrevMemberCount = dbo.OrganizationPrevCount(o.OrganizationId)
	FROM dbo.Organizations o
	JOIN Inserted i ON i.OrganizationId = o.OrganizationId

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
