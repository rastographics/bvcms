IF NOT EXISTS (SELECT * FROM [dbo].[GatewayAccount]
	WHERE GatewayAccountName = 'Acceptiva')
	BEGIN
		INSERT INTO [dbo].[GatewayAccount]
		([GatewayAccountName]
		,[GatewayId])
		VALUES
		('Acceptiva', 6)
	END
GO

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

IF NOT EXISTS (SELECT * FROM [lookup].[GatewayConfigurationTemplate]
	WHERE GatewayId = 6)
	BEGIN		
		INSERT INTO [lookup].[GatewayConfigurationTemplate]
		([GatewayId]
		,[GatewayDetailName]
		,[GatewayDetailValue]
		,[IsBoolean])
		VALUES
		(6, 'GatewayTesting', 'false', 1),
		(6, 'AcceptivaApiKey', NULL, 0),
		(6, 'AcceptivaAchId', NULL, 0),
		(6, 'AcceptivaCCId', NULL, 0);
	END
GO
