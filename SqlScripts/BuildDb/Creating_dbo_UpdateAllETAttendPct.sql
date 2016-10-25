-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateAllETAttendPct]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		DECLARE cur CURSOR FOR SELECT TransactionId FROM dbo.EnrollmentTransaction WHERE TransactionTypeId > 3
		OPEN cur
		DECLARE @tid INT, @n INT
		SET @n = 0
		FETCH NEXT FROM cur INTO @tid
		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXECUTE dbo.UpdateETAttendPct @tid
			SET @n = @n + 1
			IF (@n % 1000) = 0
				RAISERROR ('%d', 0, 1, @n) WITH NOWAIT
			FETCH NEXT FROM cur INTO @tid
		END
		CLOSE cur
		DEALLOCATE cur
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
