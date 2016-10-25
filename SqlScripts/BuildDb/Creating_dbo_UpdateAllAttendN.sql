CREATE PROCEDURE [dbo].[UpdateAllAttendN] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	UPDATE dbo.Attend SET SeqNo = NULL
	
    -- Insert statements for procedure here
		DECLARE cur CURSOR FOR 
		SELECT PeopleId 
		FROM dbo.People p WHERE EXISTS(SELECT NULL FROM Attend WHERE PeopleId = p.PeopleId AND AttendanceFlag = 1)
		OPEN cur
		DECLARE @pid INT
		FETCH NEXT FROM cur INTO @pid
		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXECUTE dbo.AttendUpdateN @pid, 100000
			FETCH NEXT FROM cur INTO @pid
		END
		CLOSE cur
		DEALLOCATE cur
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
