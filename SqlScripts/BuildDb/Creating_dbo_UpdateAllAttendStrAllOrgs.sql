
CREATE PROCEDURE [dbo].[UpdateAllAttendStrAllOrgs]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    -- Insert statements for procedure here
		DECLARE curorg CURSOR FOR 
		SELECT OrganizationId
		FROM dbo.Organizations
		OPEN curorg
		DECLARE @oid INT
		FETCH NEXT FROM curorg INTO @oid
		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXECUTE dbo.UpdateAllAttendStr @oid
			FETCH NEXT FROM curorg INTO @oid
		END
		CLOSE curorg
		DEALLOCATE curorg
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
