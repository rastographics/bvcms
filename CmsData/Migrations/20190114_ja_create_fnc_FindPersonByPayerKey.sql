IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[FindPersonByExtraValue]') AND type in (N'FN', N'IF',N'TF', N'FS', N'FT'))
BEGIN	
	DROP FUNCTION [dbo].[FindPersonByExtraValue]	
END
GO

CREATE FUNCTION [dbo].[FindPersonByExtraValue](@key nvarchar(100), @value nvarchar(max))
RETURNS @t TABLE ( PeopleId INT)
AS
BEGIN

	INSERT INTO @t
	SELECT PeopleId 
	FROM dbo.PeopleExtra
	WHERE Field = @Key
	AND Data = @Value

	RETURN
END
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[FindPersonByEmail]') AND type in (N'FN', N'IF',N'TF', N'FS', N'FT'))
BEGIN	
	DROP FUNCTION [dbo].[FindPersonByEmail]	
END
GO

CREATE FUNCTION [dbo].[FindPersonByEmail](@email nvarchar(max))
RETURNS @t TABLE ( PeopleId INT)
AS
BEGIN
    INSERT INTO @t
	SELECT top 1 PeopleId 
	FROM dbo.People
	WHERE EmailAddress = @email OR EmailAddress2 = @email
	order by CreatedDate 

	RETURN
END


