CREATE PROCEDURE [dbo].[AddEditExtraValues]
(
	@List VARCHAR(MAX), 
	@Name VARCHAR(100), 
	@StrValue VARCHAR(100),
	@Data VARCHAR(MAX),
	@DateValue DATETIME,
	@IntValue INT,
	@BitValue BIT
)
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @t TABLE(PeopleId INT)
	INSERT INTO @t SELECT Value FROM dbo.SplitInts(@List)
	
	DECLARE @updates TABLE(PeopleId INT)
	INSERT INTO @updates
	SELECT e.PeopleId FROM dbo.PeopleExtra e
	JOIN @t t ON e.PeopleId = t.PeopleId AND e.Field = @Name
	
	DECLARE @inserts TABLE(PeopleId INT)
	INSERT INTO @inserts
	SELECT PeopleId FROM @t WHERE PeopleId NOT IN (SELECT PeopleId FROM @updates)
	
	IF LEN(@StrValue) > 0
	BEGIN
		UPDATE dbo.PeopleExtra
		SET Field = @Name,
			StrValue = @StrValue
		FROM dbo.PeopleExtra e
		JOIN @updates u ON e.PeopleId = u.PeopleId
		
		INSERT dbo.PeopleExtra ( PeopleId, Field, StrValue )
			SELECT i.PeopleId, @Name, @StrValue 
			FROM @inserts i
	END
	ELSE IF @DateValue IS NOT NULL
	BEGIN
		UPDATE dbo.PeopleExtra
		SET Field = @Name,
			DateValue = @DateValue
		FROM dbo.PeopleExtra e
		JOIN @t ON e.PeopleId = [@t].PeopleId
		
		INSERT dbo.PeopleExtra ( PeopleId, Field, DateValue )
			SELECT t.PeopleId, @Name, @DateValue FROM @t t
			LEFT JOIN dbo.PeopleExtra e ON t.PeopleId = e.PeopleId
			WHERE e.PeopleId IS NULL
	END
	ELSE IF @IntValue IS NOT NULL
	BEGIN
		UPDATE dbo.PeopleExtra
		SET Field = @Name,
			IntValue = @IntValue
		FROM dbo.PeopleExtra e
		JOIN @t ON e.PeopleId = [@t].PeopleId
		
		INSERT dbo.PeopleExtra ( PeopleId, Field, IntValue )
			SELECT t.PeopleId, @Name, @IntValue FROM @t t
			LEFT JOIN dbo.PeopleExtra e ON t.PeopleId = e.PeopleId
			WHERE e.PeopleId IS NULL
	END
	ELSE IF @BitValue IS NOT NULL
	BEGIN
		UPDATE dbo.PeopleExtra
		SET Field = @Name,
			BitValue = @BitValue
		FROM dbo.PeopleExtra e
		JOIN @t ON e.PeopleId = [@t].PeopleId
		
		INSERT dbo.PeopleExtra ( PeopleId, Field, BitValue )
			SELECT t.PeopleId, @Name, @BitValue FROM @t t
			LEFT JOIN dbo.PeopleExtra e ON t.PeopleId = e.PeopleId
			WHERE e.PeopleId IS NULL
	END
	ELSE IF LEN(@Data) > 0
	BEGIN
		UPDATE dbo.PeopleExtra
		SET Field = @Name,
			Data = @Data
		FROM dbo.PeopleExtra e
		JOIN @t ON e.PeopleId = [@t].PeopleId
		
		INSERT dbo.PeopleExtra ( PeopleId, Field, Data )
			SELECT t.PeopleId, @Name, @Data FROM @t t
			LEFT JOIN dbo.PeopleExtra e ON t.PeopleId = e.PeopleId
			WHERE e.PeopleId IS NULL
	END
	        
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
