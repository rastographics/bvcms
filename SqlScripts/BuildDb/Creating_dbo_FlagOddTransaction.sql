-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FlagOddTransaction] (@pid INT, @orgid INT)
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM dbo.BadET WHERE PeopleId = @pid AND OrgId = @orgid

	DECLARE @tid INT, @typeid INT, @tdt DATETIME
	DECLARE @ptypeid INT = 0, @ptdt DATETIME

	DECLARE c CURSOR FOR
	SELECT TransactionId, TransactionTypeId, TransactionDate
	FROM dbo.EnrollmentTransaction et
	WHERE et.TransactionStatus = 0 AND et.PeopleId = @pid AND et.OrganizationId = @orgid
	ORDER BY TransactionDate, TransactionId

	OPEN c
	FETCH NEXT FROM c INTO @tid, @typeid, @tdt
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF @ptypeid > 0
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

		SELECT @ptypeid = @typeid, @ptdt = @tdt

		FETCH NEXT FROM c INTO @tid, @typeid, @tdt
	END
	CLOSE c
	DEALLOCATE c

	IF @typeid < 3 AND NOT EXISTS(SELECT NULL FROM dbo.OrganizationMembers
					WHERE OrganizationId = @orgid AND PeopleId = @pid)
		INSERT INTO dbo.BadET (PeopleId, OrgId,	TranId,	Flag) 
						VALUES ( @pid, @orgid, @tid, 10)
	
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
