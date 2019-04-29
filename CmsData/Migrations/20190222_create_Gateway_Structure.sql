--Droping Function--
IF EXISTS (
	SELECT type_desc, type
	FROM SYS.OBJECTS WITH(NOLOCK)
	WHERE object_id = OBJECT_ID(N'[dbo].[ImportGatewatSettings]')
		AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	BEGIN
		DROP FUNCTION [dbo].[ImportGatewatSettings]
	END
GO

--Droping SP--
IF EXISTS (
	SELECT type_desc, type
	FROM SYS.PROCEDURES WITH(NOLOCK)
	WHERE NAME = 'AddGatewayDetail'
		AND type = 'P')
	BEGIN
		DROP PROCEDURE [dbo].[AddGatewayDetail]
	END
GO

IF EXISTS (
       SELECT type_desc, type
       FROM SYS.PROCEDURES WITH(NOLOCK)
       WHERE NAME = 'AddGatewaySettings'
       AND type = 'P')
	BEGIN
		DROP PROCEDURE [dbo].[AddGatewaySettings]
	END
GO

--Droping Views--
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS where 
	TABLE_NAME = 'MyGatewaySettings' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN
		DROP VIEW [dbo].[MyGatewaySettings]
	END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS where 
	TABLE_NAME = 'AvailableProcesses' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN
		DROP VIEW [dbo].AvailableProcesses
	END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS where 
	TABLE_NAME = 'GatewayDetailsInformation' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN
		DROP VIEW [dbo].[GatewayDetailsInformation]
	END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS where 
	TABLE_NAME = 'PaymentProcessDetails' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN
		DROP VIEW [dbo].[PaymentProcessDetails]
	END
GO

--Droping Tables--
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'GatewayDetails' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN
		DROP TABLE [dbo].[GatewayDetails]
	END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'GatewaySettings' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN
		DROP TABLE [dbo].[GatewaySettings]
	END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'PaymentProcess' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN
		DROP TABLE [dbo].[PaymentProcess]
	END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'GatewayAccount' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN
		DROP TABLE [dbo].[GatewayAccount]
	END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'GatewayConfigurationTemplate' AND 
	TABLE_SCHEMA = 'lookup')
	BEGIN
		DROP TABLE [lookup].[GatewayConfigurationTemplate]
	END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'ProcessType' AND 
	TABLE_SCHEMA = 'lookup')
	BEGIN
		DROP TABLE [lookup].[ProcessType]
	END
GO


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'Gateways' AND 
	TABLE_SCHEMA = 'lookup')
	BEGIN
		DROP TABLE [lookup].[Gateways]
	END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'GatewayServiceType' AND 
	TABLE_SCHEMA = 'lookup')
	BEGIN
		DROP TABLE [lookup].[GatewayServiceType]
	END
GO
--End Droping--

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
		('Acceptiva', 3),
		('AuthorizeNet', 2)
	END
GO

CREATE FUNCTION [ImportGatewatSettings]
(
@Key[nvarchar](125)
)
RETURNS [nvarchar](125)
AS
	BEGIN
		DECLARE @Value[nvarchar](125);
		SELECT @Value = (SELECT TOP 1 [Setting] FROM [Setting]
						WHERE [Id] LIKE @Key);
		IF @Value IS NULL
		BEGIN
			SELECT @Value = '';
		END

		RETURN @Value;
	END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'GatewayConfigurationTemplate' AND 
	TABLE_SCHEMA = 'lookup')
	BEGIN
		CREATE TABLE [lookup].[GatewayConfigurationTemplate](
			[GatewayDetailId][int] IDENTITY(1,1) PRIMARY KEY,
			[GatewayId][int] FOREIGN KEY REFERENCES [lookup].[Gateways]([GatewayId]),
			[GatewayDetailName][nvarchar](125) NOT NULL,
			[GatewayDetailValue][nvarchar](max) NOT NULL,
			[IsBoolean][bit] NOT NULL
		);
		INSERT INTO [lookup].[GatewayConfigurationTemplate]
		([GatewayId]
		,[GatewayDetailName]
		,[GatewayDetailValue]
		,[IsBoolean])
		VALUES
		(1, 'GatewayTesting', 'true', 1),
		(1, 'PushpayMerchant', (SELECT [dbo].[ImportGatewatSettings]('PushpayMerchant')), 0),
		(2, 'GatewayTesting', 'true', 1),
		(2, 'M_ID', (SELECT [dbo].[ImportGatewatSettings]('M_ID')), 0),
		(2, 'M_KEY', (SELECT [dbo].[ImportGatewatSettings]('M_KEY')), 0),
		(3, 'GatewayTesting', 'true', 1),
		(3, 'TNBUsername', (SELECT [dbo].[ImportGatewatSettings]('TNBUsername')), 0),
		(3, 'TNBPassword', (SELECT [dbo].[ImportGatewatSettings]('TNBPassword')), 0),
		(4, 'GatewayTesting', 'true', 1),
		(4, 'AcceptivaApiKey', (SELECT [dbo].[ImportGatewatSettings]('AcceptivaApiKey')), 0),
		(4, 'AcceptivaAchId', (SELECT [dbo].[ImportGatewatSettings]('AcceptivaAchId')), 0),
		(4, 'AcceptivaCCId', (SELECT [dbo].[ImportGatewatSettings]('AcceptivaCCId')), 0),
		(4, 'UseSavingAccounts', 'true', 1),
		(5, 'GatewayTesting', 'true', 1),
		(5, 'x_login', (SELECT [dbo].[ImportGatewatSettings]('x_login')),0),
		(5, 'x_tran_key', (SELECT [dbo].[ImportGatewatSettings]('x_tran_key')),0);
	DROP FUNCTION [ImportGatewatSettings]
	END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'GatewayAccount' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN				
		CREATE TABLE [dbo].[GatewayAccount](
			[GatewayAccountId][int] IDENTITY(1,1) PRIMARY KEY,
			[GatewayAccountName][nvarchar](30) NOT NULL,
			[GatewayId][int] FOREIGN KEY REFERENCES [lookup].[Gateways]([GatewayId]) NOT NULL
		);

		INSERT INTO [dbo].[GatewayAccount]
		([GatewayAccountName]
		,[GatewayId])
		VALUES
		('PushpayAcc', 1),
		('SageAcc', 2),
		('TransnationalAcc', 3),
		('AcceptivaAcc', 4),
		('AuthorizeNetAcc', 5)
	END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'GatewayDetails' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN		
		CREATE TABLE [dbo].[GatewayDetails](
			[GatewayDetailId][int] IDENTITY(1,1) PRIMARY KEY,
			[GatewayAccountId][int] FOREIGN KEY REFERENCES [dbo].[GatewayAccount]([GatewayAccountId]),
			[GatewayDetailName][nvarchar](125) NOT NULL,
			[GatewayDetailValue][nvarchar](max) NOT NULL,
			[IsBoolean][bit] NOT NULL,
			[IsDefault][bit] NOT NULL
		);

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
	END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'PaymentProcess' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN
		CREATE TABLE [dbo].[PaymentProcess](
	        [ProcessId][int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	        [ProcessName][nvarchar](30) NOT NULL,
			[GatewayAccountId][int] FOREIGN KEY REFERENCES [dbo].[GatewayAccount]([GatewayAccountId]) NULL,
        );
        INSERT INTO [dbo].[PaymentProcess]
        ([ProcessName])
        VALUES
        ('One-Time Giving'),
        ('Recurring Giving'),
        ('Online Registration');
	END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS where 
	TABLE_NAME = 'PaymentProcessDetails' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN		
		EXEC dbo.sp_executesql @statement = N'
		CREATE VIEW [dbo].[PaymentProcessDetails]
		AS
		SELECT [dbo].[PaymentProcess].[ProcessId]
			,[dbo].[PaymentProcess].[ProcessName]
			,[dbo].[GatewayAccount].[GatewayAccountId]
			,[dbo].[GatewayAccount].[GatewayAccountName]
			,[dbo].[GatewayAccount].[GatewayId]
			,[dbo].[GatewayDetails].[GatewayDetailName]
			,[dbo].[GatewayDetails].[GatewayDetailValue]
			,[dbo].[GatewayDetails].[IsDefault]
			,[dbo].[GatewayDetails].[IsBoolean]
		FROM [dbo].[PaymentProcess]
		LEFT JOIN [dbo].[GatewayAccount]
		ON [dbo].[PaymentProcess].[GatewayAccountId] = [dbo].[GatewayAccount].[GatewayAccountId]
		LEFT JOIN [dbo].[GatewayDetails]
		ON [dbo].[GatewayAccount].[GatewayAccountId] = [dbo].[GatewayDetails].[GatewayAccountId]
		'
	END	
GO

/*
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
*/