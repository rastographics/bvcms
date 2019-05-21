IF NOT EXISTS (SELECT * FROM [lookup].[Gateways]
	WHERE GatewayName = 'Acceptiva')
	BEGIN				
		INSERT INTO [lookup].[Gateways]
		([GatewayName],
		[GatewayServiceTypeId])
		VALUES
		('Acceptiva', 3)
	END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GatewayAccount]
	WHERE GatewayAccountName = 'Acceptiva')
	BEGIN
		INSERT INTO [dbo].[GatewayAccount]
		([GatewayAccountName]
		,[GatewayId])
		SELECT GatewayName GatewayAccountName, GatewayId FROM [lookup].[Gateways] WHERE GatewayName = 'Acceptiva'
	END
GO


IF NOT EXISTS (SELECT * FROM [lookup].[GatewayConfigurationTemplate] t
    JOIN [lookup].[Gateways] g  on t.GatewayId = g.GatewayId
    WHERE g.GatewayName = 'Acceptiva')
	BEGIN
		WITH acc AS (SELECT top(1) * FROM [lookup].[Gateways] WHERE GatewayName = 'Acceptiva')
		INSERT INTO [lookup].[GatewayConfigurationTemplate]
		([GatewayId]
		,[GatewayDetailName]
		,[GatewayDetailValue]
		,[IsBoolean])
		VALUES
		((SELECT top(1) GatewayId FROM acc), 'GatewayTesting', 'false', 1),
		((SELECT top(1) GatewayId FROM acc), 'AcceptivaApiKey', '', 0),
		((SELECT top(1) GatewayId FROM acc), 'AcceptivaAchId', '', 0),
		((SELECT top(1) GatewayId FROM acc), 'AcceptivaCCId', '', 0);
	END
GO
