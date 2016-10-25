CREATE TRIGGER [dbo].[updUser] 
   ON  [dbo].[Users]
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF UPDATE(PeopleId)
	BEGIN
		UPDATE Users
		SET Name = dbo.UName(PeopleId),
		Name2 = dbo.UName2(PeopleId)
		WHERE PeopleId IN (SELECT PeopleId FROM INSERTED)
	END
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
