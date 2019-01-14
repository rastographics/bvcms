IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[FindPersonByExtraValue]') AND type in (N'FN', N'IF',N'TF', N'FS', N'FT'))
BEGIN	
	DROP FUNCTION [dbo].[FindPersonByExtraValue]	
END
GO

CREATE FUNCTION [dbo].[FindPersonByExtraValue](@key nvarchar(100), @value nvarchar(max))
RETURNS @t TABLE ( PeopleId INT)
AS
BEGIN
--DECLARE @t TABLE ( PeopleId INT)
--DECLARE @key nvarchar(100) = 'GatewayPayerKey', 
--		@value nvarchar(max) = 'MzpMNU9YSTIwSmt4REN5ZWluUlhSbGNvLV9lUk0', 

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
--DECLARE @t TABLE ( PeopleId INT)
--DECLARE @email nvarchar(max) = 'jenefa1912@gmail.com', 


	INSERT INTO @t
	SELECT top 1 PeopleId 
	FROM dbo.People
	WHERE EmailAddress = @email OR EmailAddress2 = @email
	order by CreatedDate 

	RETURN
END


