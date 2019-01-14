IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[FindPersonByPayerKey]') AND type in (N'FN', N'IF',N'TF', N'FS', N'FT'))
BEGIN	
	DROP FUNCTION [dbo].[FindPersonByPayerKey]	
END
GO

CREATE FUNCTION [dbo].[FindPersonByPayerKey](@GatewayPayerKey nvarchar(100), @GatewayPayerValue nvarchar(max))
RETURNS @t TABLE ( PeopleId INT)
AS
BEGIN
--DECLARE @t TABLE ( PeopleId INT)
--DECLARE @GatewayPayerKey nvarchar(100) = 'GatewayPayerKey', 
--		@@GatewayPayerValue nvarchar(max) = 'MzpMNU9YSTIwSmt4REN5ZWluUlhSbGNvLV9lUk0', 

	INSERT INTO @t
	SELECT PeopleId 
	FROM dbo.PeopleExtra
	WHERE Field = @GatewayPayerKey
	AND Data = @GatewayPayerValue

	RETURN
END


GO

