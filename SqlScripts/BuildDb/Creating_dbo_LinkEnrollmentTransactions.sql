-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[LinkEnrollmentTransactions] (@pid INT, @orgid INT)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @tid INT, @typeid INT, @tdt DATETIME

	DECLARE c CURSOR FOR
	SELECT TransactionId, TransactionTypeId, TransactionDate
	FROM dbo.EnrollmentTransaction et
	WHERE et.TransactionStatus = 0 AND et.PeopleId = @pid AND et.OrganizationId = @orgid
	ORDER BY TransactionDate, TransactionId

	OPEN c
	FETCH NEXT FROM c INTO @tid, @typeid, @tdt
	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC dbo.LinkEnrollmentTransaction @tid, @tdt, @typeid, @orgid, @pid
		FETCH NEXT FROM c INTO @tid, @typeid, @tdt
	END
	CLOSE c
	DEALLOCATE c
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
