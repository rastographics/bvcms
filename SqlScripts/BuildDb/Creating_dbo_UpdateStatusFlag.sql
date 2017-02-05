CREATE PROC [dbo].[UpdateStatusFlag](@name NVARCHAR(200), @tagid INT)
AS
BEGIN
	DECLARE @sflagid INT
	SELECT @sflagid = Id FROM Tag WHERE Name = @name AND TypeId = 100
	
	IF @sflagid IS NULL
	BEGIN
		INSERT dbo.Tag ( Name , TypeId , Created )
		VALUES  ( @name, 100, GETDATE() )
		SET @sflagid = SCOPE_IDENTITY()
    END
    
	-- insert new where not in statusflag but is in tag
	INSERT INTO dbo.TagPerson ( Id, PeopleId )
	SELECT @sflagid, tp.PeopleId
	FROM dbo.TagPerson tp
	LEFT OUTER JOIN dbo.TagPerson ed ON ed.PeopleId = tp.PeopleId AND ed.Id = @sflagid 
	WHERE tp.Id = @tagid
	AND ed.PeopleId IS NULL


	-- delete statusflag where not in tag
	DELETE dbo.TagPerson
	FROM dbo.TagPerson ed
	WHERE ed.Id = @sflagid
	AND NOT EXISTS(
			SELECT NULL 
			FROM dbo.TagPerson tp
			WHERE Id = @tagid AND tp.PeopleId = ed.PeopleId
	)

	DELETE dbo.TagPerson
	WHERE Id = @tagid
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
