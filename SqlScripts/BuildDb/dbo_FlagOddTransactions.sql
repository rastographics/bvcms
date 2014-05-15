CREATE PROCEDURE [dbo].[FlagOddTransactions]
AS
BEGIN
	SET NOCOUNT ON;

	TRUNCATE TABLE dbo.BadET

	DECLARE @tid INT, @typeid INT, @orgid INT, @pid INT, @tdt DATETIME
	DECLARE @ptid INT = 0, @ptypeid INT = 0, @porgid INT = 0, @ppid INT = 0, @ptdt DATETIME

	DECLARE c CURSOR FOR
	SELECT TransactionId, TransactionTypeId, OrganizationId, PeopleId, TransactionDate
	FROM dbo.EnrollmentTransaction et
	WHERE et.TransactionStatus = 0
	ORDER BY PeopleId, OrganizationId, TransactionDate, TransactionId

	OPEN c
	FETCH NEXT FROM c INTO @tid, @typeid, @orgid, @pid, @tdt
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF @ptid > 0 AND (@ppid <> @pid OR @porgid <> @orgid)
		BEGIN
			IF (@ppid <> @pid OR @porgid <> @orgid) AND 
					@ptypeid < 3 AND NOT EXISTS(SELECT NULL FROM dbo.OrganizationMembers
							WHERE OrganizationId = @porgid AND PeopleId = @ppid)
				INSERT INTO dbo.BadET (PeopleId, OrgId,	TranId,	Flag) 
								VALUES ( @ppid, @porgid, @ptid, 10)
			SELECT @ptid = 0, @ptypeid = 0, @porgid = 0, @ppid = 0
		END
		IF @ptid > 0
		BEGIN
			IF @tdt = @ptdt
				INSERT INTO dbo.BadET (PeopleId, OrgId,	TranId,	Flag) 
								VALUES ( @pid, @orgid, @tid, 15)
			ELSE IF @typeid < 3 AND @ptypeid <= 3
				INSERT INTO dbo.BadET (PeopleId, OrgId,	TranId,	Flag) 
								VALUES ( @pid, @orgid, @tid, 11)
				
			ELSE IF @typeid > 3 AND @ptypeid > 3
				INSERT INTO dbo.BadET (PeopleId, OrgId,	TranId,	Flag) 
								VALUES ( @pid, @orgid, @tid, 55)
		END

		SELECT @ptid = @tid, @ptypeid = @typeid, @porgid = @orgid, @ppid = @pid, @ptdt = @tdt

		FETCH NEXT FROM c INTO @tid, @typeid, @orgid, @pid, @tdt
	END
	CLOSE c
	DEALLOCATE c
	
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
