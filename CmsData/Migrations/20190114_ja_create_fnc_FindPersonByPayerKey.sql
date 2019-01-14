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

