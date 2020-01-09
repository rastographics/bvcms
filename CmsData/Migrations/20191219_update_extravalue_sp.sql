ALTER PROCEDURE [dbo].[AddExtraValueData]
     @pid INT
    ,@field VARCHAR(150)
    ,@strvalue NVARCHAR(200)
    ,@datevalue DATETIME
    ,@text NVARCHAR(MAX)
	,@intvalue INT
	,@bitvalue BIT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @instance INT 

	BEGIN TRAN

	SELECT @instance = ISNULL(MAX(Instance), 0) + 1 FROM dbo.PeopleExtra WHERE PeopleId = @pid AND Field = @field
	INSERT dbo.PeopleExtra
	        ( PeopleId
	        ,Field
	        ,StrValue
	        ,DateValue
	        ,TransactionTime
	        ,Data
	        ,IntValue
	        ,BitValue
	        ,UseAllValues
	        ,Instance
	        )
	VALUES  ( @pid 
	        ,@field
	        ,@strvalue
	        ,@datevalue
	        ,GETDATE()
	        ,@text
	        ,@intvalue
	        ,@bitvalue
	        ,0 
	        ,@instance
	        )
	COMMIT TRAN

	RETURN @instance
END
GO

UPDATE PeopleExtra SET UseAllValues = 0 WHERE Field = 'PushPayKey'
GO
