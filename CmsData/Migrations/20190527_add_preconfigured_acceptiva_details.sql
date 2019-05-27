IF NOT EXISTS (SELECT * FROM [dbo].[GatewayDetails] t
    JOIN [dbo].[GatewayAccount] g  on t.GatewayAccountId = g.GatewayAccountId
    WHERE g.GatewayAccountName = 'Acceptiva')
	BEGIN
		INSERT INTO [dbo].[GatewayDetails]
		([GatewayAccountId]
		,[GatewayDetailName]
		,[GatewayDetailValue]
		,[IsBoolean]
		,[IsDefault])
		SELECT [dbo].[GatewayAccount].[GatewayAccountId]
		,[lookup].[GatewayConfigurationTemplate].[GatewayDetailName]
		,[lookup].[GatewayConfigurationTemplate].[GatewayDetailValue]
		,[lookup].[GatewayConfigurationTemplate].[IsBoolean]
		,1
		FROM [dbo].[GatewayAccount]
		INNER JOIN [lookup].[Gateways]
		ON [dbo].[GatewayAccount].[GatewayId] = [lookup].[Gateways].[GatewayId]
		INNER JOIN [lookup].[GatewayConfigurationTemplate]
		ON [lookup].[Gateways].[GatewayId] = [lookup].[GatewayConfigurationTemplate].[GatewayId]
		WHERE [dbo].[GatewayAccount].[GatewayAccountName] = 'Acceptiva'
	END
GO
