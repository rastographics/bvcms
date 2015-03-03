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
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
