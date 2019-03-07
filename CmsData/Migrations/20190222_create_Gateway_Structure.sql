IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'GatewayServiceType' AND 
	TABLE_SCHEMA = 'lookup')
	BEGIN		
		CREATE TABLE [lookup].[GatewayServiceType](
	        [GatewayServiceTypeId][int] IDENTITY(1,1) PRIMARY KEY,
	        [GatewayServiceTypeName][nvarchar](25) NOT NULL
        );
        INSERT INTO [lookup].[GatewayServiceType]
        ([GatewayServiceTypeName])
        VALUES
        ('Redirect Link'),
        ('SOAP'),
        ('API')        
	END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'ProcessType' AND 
	TABLE_SCHEMA = 'lookup')
	BEGIN		
		CREATE TABLE [lookup].[ProcessType](
	        [ProcessTypeId][int] IDENTITY(1,1) PRIMARY KEY,
	        [ProcessTypeName][nvarchar](75) NOT NULL
        );
        INSERT INTO [lookup].[ProcessType]
        ([ProcessTypeName])
        VALUES
        ('Payment'),
        ('No Payment Required')
    
	END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'Gateways' AND 
	TABLE_SCHEMA = 'lookup')
	BEGIN				
		CREATE TABLE [lookup].[Gateways](
			[GatewayId][int] IDENTITY(1,1) PRIMARY KEY,
			[GatewayName][nvarchar](30) NOT NULL,
			[GatewayServiceTypeId][int] FOREIGN KEY REFERENCES [lookup].[GatewayServiceType]([GatewayServiceTypeId]) NOT NULL
		);
		INSERT INTO [lookup].[Gateways]
		([GatewayName],
		[GatewayServiceTypeId])
		VALUES
		('Pushpay', 1),
		('Sage', 2),
		('Transnational', 2),
		('Acceptival', 3)
	END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'Process' AND 
	TABLE_SCHEMA = 'lookup')
	BEGIN
		CREATE TABLE [lookup].[Process](
	        [ProcessId][int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	        [ProcessName][nvarchar](30) NOT NULL,
	        [ProcessTypeId][int] FOREIGN KEY REFERENCES [lookup].[ProcessType]([ProcessTypeId]) NOT NULL
        );
        INSERT INTO [lookup].[Process]
        ([ProcessName]
        ,[ProcessTypeId])
        VALUES
        ('One-Time Giving', 1),
        ('Recurring Giving', 1),
        ('Online Registration', 1)
        
	END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'GatewaySettings' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN		
		CREATE TABLE [dbo].[GatewaySettings](
		[GatewaySettingId][int] IDENTITY(1,1) PRIMARY KEY,
		[GatewayId][int] FOREIGN KEY REFERENCES [lookup].[Gateways]([GatewayId]) NOT NULL,
		[ProcessId][int] UNIQUE FOREIGN KEY REFERENCES [lookup].[Process]([ProcessId]) NOT NULL);	
	END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'GatewayDetails' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN		
		CREATE TABLE [dbo].[GatewayDetails](
			[GatewayDetailId][int] IDENTITY(1,1) PRIMARY KEY,
			[GatewayId][int] FOREIGN KEY REFERENCES [lookup].[Gateways]([GatewayId]),
			[GatewayDetailName][nvarchar](125) NOT NULL,
			[GatewayDetailValue][nvarchar](max) NOT NULL,
			[IsDefault][bit] NOT NULL
		);
		INSERT INTO [dbo].[GatewayDetails]
		([GatewayId]
		,[GatewayDetailName]
		,[GatewayDetailValue]
		,[IsDefault])
		VALUES
		(1, 'PushpayAPIBaseUrl', 'https://sandbox-api.pushpay.io/v1/', 1),
		(1, 'PushpayClientID', 'pursuant-touchpoint-dev', 1),
		(1, 'PushpayClientSecret', '', 1),
		(1, 'OAuth2AuthorizeEndpoint', 'https://auth.pushpay.com/pushpay-sandbox/oauth/authorize', 1),
		(1, 'OAuth2TokenEndpoint', 'https://auth.pushpay.com/pushpay-sandbox/oauth/token', 1),
		(1, 'PushpayScope', 'list_my_merchants merchant:manage_community_members merchant:manage_webhooks merchant:view_community_members merchant:view_payments merchant:view_recurring_payments read', 1),
		(1, 'TenantHostDev', 'localhost:44301', 1),
		(1, 'OrgBaseDomain', 'tpsdb.com', 1),
		(1, 'TouchpointAuthServer', 'https://123ec8c6.ngrok.io/pushpay/complete', 1),
		(1, 'IsDeveloperMode', 'true', 1)
	END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS where 
	TABLE_NAME = 'GatewayDetailsInformation' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN		
		EXEC dbo.sp_executesql @statement = N'
		CREATE VIEW [dbo].[GatewayDetailsInformation] AS
		SELECT [GtS].[GatewayId]
			,[Gt].[GatewayName]
			,[GtD].[GatewayDetailId]
			,[GtD].[GatewayDetailName]
			,[GtD].[GatewayDetailValue]
			,[GtD].[IsDefault]
		FROM [dbo].[GatewaySettings] [GtS]
		INNER JOIN [lookup].[Gateways] [Gt]
		ON [GtS].[GatewayId] = [Gt].[GatewayId]
		LEFT JOIN [dbo].[GatewayDetails] [GtD]
		ON [GtD].[GatewayId] = [Gt].[GatewayId]'
	END	
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS where 
	TABLE_NAME = 'AvailableProcess' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN		
		EXEC dbo.sp_executesql @statement = N'
		CREATE VIEW [dbo].[AvailableProcess] AS
		SELECT [ProcessId], [ProcessName]
		FROM [lookup].[Process]
		WHERE [ProcessId] NOT IN(SELECT [ProcessId] FROM [GatewaySettings])
		AND [ProcessTypeId] = 1
		'
	END	
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS where 
	TABLE_NAME = 'MyGatewaySettings' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN		
		EXEC dbo.sp_executesql @statement = N'
		CREATE VIEW [dbo].[MyGatewaySettings]
		AS
			SELECT [GatewaySettings].[GatewaySettingId], 
				[lookup].[Process].[ProcessName],
				[dbo].[GatewaySettings].[ProcessId],
				[lookup].[Gateways].[GatewayName],
				[lookup].[Gateways].[GatewayId]
			FROM [GatewaySettings]
			INNER JOIN [lookup].[Process]
			ON [GatewaySettings].[ProcessId] = [lookup].[Process].[ProcessId]
			INNER JOIN [lookup].[Gateways]
			ON [GatewaySettings].[GatewayId] = [lookup].[Gateways].[GatewayId]
		'
	END	
GO

IF NOT EXISTS (
       SELECT type_desc, type
       FROM SYS.PROCEDURES WITH(NOLOCK)
       WHERE NAME = 'AddGatewaySettings'
       AND type = 'P')
	BEGIN
		EXEC dbo.sp_executesql @statement = N'
		CREATE PROCEDURE [dbo].[AddGatewaySettings]
		(@GatewaySettingId[int] NULL,
		@GatewayId[int],
		@ProcessId[int],
		@Operation[int]) --0 INSERT, 1 UPDATE, 2 DELETE
		AS
			IF(@Operation = 0)
				BEGIN
					IF(SELECT COUNT(*) FROM [dbo].[GatewaySettings] WHERE [ProcessId] = @ProcessId) > 0
						SELECT CONVERT([nvarchar](max), ''Error process already exists'') AS ''Status''
					ELSE
						BEGIN
							INSERT INTO [dbo].[GatewaySettings]
							([GatewayId]
							,[ProcessId])
							VALUES
							(@GatewayId
							,@ProcessId);
							IF(@@ROWCOUNT) > 0
								SELECT CONVERT([nvarchar](8), ''Success'') AS ''Status''
							ELSE
								SELECT CONVERT([nvarchar](max), ''Error inserting data'') AS ''Status''
						END
				END
			ELSE IF(@Operation = 1)
				BEGIN
					IF(SELECT COUNT(*) FROM [dbo].[GatewaySettings] WHERE [GatewaySettingId] = @GatewaySettingId) > 0
						BEGIN
							UPDATE [dbo].[GatewaySettings]
							SET [GatewayId] = @GatewayId
							WHERE [GatewaySettingId] = @GatewaySettingId
							IF(@@ROWCOUNT) > 0
								SELECT CONVERT([nvarchar](8), ''Success'') AS ''Status''
							ELSE
								SELECT CONVERT([nvarchar](max), ''Error updating data'') AS ''Status''
						END
					ELSE
						SELECT CONVERT([nvarchar](max), ''Error register doesn´t exists'') AS ''Status''
				END
			ELSE IF (@Operation = 2)
				BEGIN
					IF(SELECT COUNT(*) FROM [dbo].[GatewaySettings] WHERE [GatewaySettingId] = @GatewaySettingId) > 0
						BEGIN
							DELETE FROM [dbo].[GatewaySettings]
							WHERE [GatewaySettingId] = @GatewaySettingId
							IF(@@ROWCOUNT) > 0
								SELECT CONVERT([nvarchar](8), ''Success'') AS ''Status''
							ELSE
								SELECT CONVERT([nvarchar](max), ''Error updating data'') AS ''Status''
						END
					ELSE
						SELECT CONVERT([nvarchar](max), ''Error register doesn´t exists'') AS ''Status''
				END'
	END
GO

IF NOT EXISTS (
       SELECT type_desc, type
       FROM SYS.PROCEDURES WITH(NOLOCK)
       WHERE NAME = 'AddGatewayDetail'
            AND type = 'P'
     )
	BEGIN
		EXEC dbo.sp_executesql @statement = N'
		CREATE PROCEDURE [dbo].[AddGatewayDetail]
		(@GatewayDetailId[int] NULL,
		@GatewayId[int],
		@GatewayDetailName[nvarchar](255),
		@GatewayDetailValue[nvarchar](max),
		@IsDefault[bit],
		@Operation[int]) --0 INSERT, 1 UPDATE, 2 DELETE
		AS
			IF(@Operation = 0)
				BEGIN
					IF(SELECT COUNT(*) FROM [dbo].[GatewayDetails] WHERE [GatewayDetailName] LIKE @GatewayDetailName) > 0
						SELECT CONVERT([nvarchar](max), ''Error configuration already exists'') AS ''Status''
					ELSE
						BEGIN
							INSERT INTO [dbo].[GatewayDetails]
							([GatewayId]
							,[GatewayDetailName]
							,[GatewayDetailValue]
							,[IsDefault])
							VALUES
							(@GatewayId
							,@GatewayDetailName
							,@GatewayDetailValue
							,@IsDefault);
							IF(@@ROWCOUNT) > 0
								SELECT CONVERT([nvarchar](8), ''Success'') AS ''Status''
							ELSE
								SELECT CONVERT([nvarchar](max), ''Error inserting data'') AS ''Status''
						END
				END
			ELSE IF(@Operation = 1)
				BEGIN
					IF(SELECT COUNT(*) FROM [dbo].[GatewayDetails] WHERE @GatewayDetailId = @GatewayDetailId) > 0
						BEGIN
							UPDATE [dbo].[GatewayDetails]
							SET [GatewayDetailName] = @GatewayDetailName,
								[GatewayDetailValue] = @GatewayDetailValue
							WHERE [GatewayDetailId] = @GatewayDetailId
							IF(@@ROWCOUNT) > 0
								SELECT CONVERT([nvarchar](8), ''Success'') AS ''Status''
							ELSE
								SELECT CONVERT([nvarchar](max), ''Error updateing data'') AS ''Status''
						END
					ELSE
						SELECT CONVERT([nvarchar](max), ''Error register doesn´t exists'') AS ''Status''
				END
			ELSE IF(@Operation = 2)
				BEGIN
					IF(SELECT COUNT(*) FROM [dbo].[GatewayDetails] WHERE [GatewayDetailId] = @GatewayDetailId) > 0
						BEGIN
							DELETE FROM [dbo].[GatewayDetails]
							WHERE [GatewayDetailId] = @GatewayDetailId
							IF(@@ROWCOUNT) > 0
								SELECT CONVERT([nvarchar](8), ''Success'') AS ''Status''
							ELSE
								SELECT CONVERT([nvarchar](max), ''Error updateing data'') AS ''Status''
						END
					ELSE
						SELECT CONVERT([nvarchar](max), ''Error register doesn´t exists'') AS ''Status''
				END'
	END
GO