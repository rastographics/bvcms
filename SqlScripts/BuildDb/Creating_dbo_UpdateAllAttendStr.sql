CREATE PROCEDURE [dbo].[UpdateAllAttendStr] (@orgid INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @start DATETIME = CURRENT_TIMESTAMP;
	
    -- Insert statements for procedure here
		DECLARE cur CURSOR FOR 
		SELECT PeopleId 
		FROM dbo.OrganizationMembers
		WHERE OrganizationId = @orgid
		OPEN cur
		DECLARE @pid INT, @n INT
		FETCH NEXT FROM cur INTO @pid
		WHILE @@FETCH_STATUS = 0
		BEGIN	
			--raiserror ('%d %d', 10,1, @orgid, @pid) with nowait
			EXECUTE dbo.UpdateAttendStr @orgid, @pid
			EXECUTE dbo.AttendUpdateN @pid, 1000000
			
			FETCH NEXT FROM cur INTO @pid
		END
		CLOSE cur
		DEALLOCATE cur
		
	INSERT INTO dbo.ActivityLog
	        ( ActivityDate , UserId , Activity , Machine )
	VALUES  ( GETDATE(), NULL , 'UpdateAllAttendStr (' + CONVERT(nvarchar, @orgid) + ',' + CONVERT(nvarchar, DATEDIFF(ms, @start, CURRENT_TIMESTAMP) / 1000) + ')', 'DB' )
END

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
