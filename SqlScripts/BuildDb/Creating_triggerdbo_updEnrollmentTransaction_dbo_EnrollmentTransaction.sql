CREATE TRIGGER [dbo].[updEnrollmentTransaction]
   ON  [dbo].[EnrollmentTransaction]
   AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @Cinfo VARBINARY(128) = Context_Info()  
	IF @Cinfo = 0x55555  
		RETURN  
	
	DECLARE @oid INT, @pid INT, @tid INT, @tdt DATETIME, @thistid INT

	DECLARE @newtdt DATETIME
	
	DECLARE upd2 CURSOR FORWARD_ONLY FOR
	SELECT OrganizationId, PeopleId, TransactionDate, TransactionId
	FROM deleted 

	OPEN upd2
	FETCH NEXT FROM upd2 INTO @oid, @pid, @tdt, @thistid
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SELECT @newtdt = TransactionDate FROM INSERTED WHERE TransactionId = @thistid
		
		UPDATE dbo.EnrollmentTransaction
		SET NextTranChangeDate = @newtdt
		WHERE OrganizationId = @oid AND PeopleId = @pid AND NextTranChangeDate = @tdt

		FETCH NEXT FROM upd2 INTO @oid, @pid, @tdt, @thistid
	END
	CLOSE upd2
	DEALLOCATE upd2

	UPDATE dbo.Organizations
	SET PrevMemberCount = dbo.OrganizationPrevCount(o.OrganizationId)
	FROM dbo.Organizations o
	JOIN deleted i ON i.OrganizationId = o.OrganizationId

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
