CREATE PROCEDURE [dbo].[AddExtraValueData]
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
	        ,1 
	        ,@instance
	        )
	COMMIT TRAN

	RETURN @instance
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
