
CREATE PROCEDURE [dbo].[AddEditExtraDate](@pid INT, @name VARCHAR(50), @dt DATETIME)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @ev INT
	SELECT @ev = PeopleId FROM dbo.PeopleExtra WHERE PeopleId = @pid AND Field = @name
	IF @ev IS NULL
		INSERT dbo.PeopleExtra (PeopleId, Field, DateValue, TransactionTime)
		VALUES  ( @pid, @name, @dt, GETDATE())
	ELSE
		UPDATE dbo.PeopleExtra
		SET DateValue = @dt
		WHERE PeopleId = @pid AND Field = @name
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
