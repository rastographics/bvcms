USE [CMS_localhost]
GO

CREATE TABLE [lookup].[GatewayServiceType](
	[GatewayServiceTypeId][int] IDENTITY(1,1) PRIMARY KEY,
	[GatewayServiceTypeName][nvarchar](25) NOT NULL
)
GO

INSERT INTO [lookup].[GatewayServiceType]
([GatewayServiceTypeName])
VALUES
('Redirect Link'),
('SOAP'),
('API')
GO

CREATE TABLE [lookup].[Gateways](
	[GatewayId][int] IDENTITY(1,1) PRIMARY KEY,
	[GatewayName][nvarchar](30) NOT NULL,
	[GatewayServiceTypeId][int] FOREIGN KEY REFERENCES [lookup].[GatewayServiceType]([GatewayServiceTypeId]) NOT NULL
)

INSERT INTO [lookup].[Gateways]
([GatewayName],
[GatewayServiceTypeId])
VALUES
('Pushpay', 1),
('Sage', 2),
('Transnational', 2),
('Acceptival', 3)
GO

CREATE TABLE [lookup].[ProcessType](
	[ProcessTypeId][int] IDENTITY(1,1) PRIMARY KEY,
	[ProcessTypeName][nvarchar](75) NOT NULL
)

INSERT INTO [lookup].[ProcessType]
([ProcessTypeName])
VALUES
('Payment'),
('No Payment Required')
GO

CREATE TABLE [lookup].[Process](
	[ProcessId][int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[ProcessName][nvarchar](30) NOT NULL,
	[ProcessTypeId][int] FOREIGN KEY REFERENCES [lookup].[ProcessType]([ProcessTypeId]) NOT NULL
)
GO

INSERT INTO [lookup].[Process]
([ProcessName]
,[ProcessTypeId])
VALUES
('One-Time Giving', 1),
('Recurring Giving', 1),
('Online Registration', 1)
GO

CREATE TABLE [dbo].[GatewaySettings](
	[GatewaySettingId][int] IDENTITY(1,1) PRIMARY KEY,
	[GatewayId][int] FOREIGN KEY REFERENCES [lookup].[Gateways]([GatewayId]) NOT NULL,
	[ProcessId][int] UNIQUE FOREIGN KEY REFERENCES [lookup].[Process]([ProcessId]) NOT NULL,
	[Settings][nvarchar](max) NOT NULL
)
GO

CREATE PROCEDURE [dbo].[AddGatewaySettings]
@GatewayId[int],
@ProcessId[int],
@Settings[nvarchar](max)
AS
	IF(SELECT [ProcessTypeId] FROM [lookup].[Process] WHERE [ProcessId] = @ProcessId) = 1
	BEGIN
		INSERT INTO [dbo].[GatewaySettings]
		([GatewayId]
		,[ProcessId]
		,[Settings])
		VALUES
		(@GatewayId
		,@ProcessId
		,@Settings)
		IF(@@ROWCOUNT) > 0
			SELECT CONVERT([nvarchar](8), 'Success') AS 'Status'
		ELSE
			SELECT CONVERT([nvarchar](8), 'Error inserting data') AS 'Status'
	END
	ELSE
		SELECT CONVERT([nvarchar](8), 'No Payment allowed with this process') AS 'Status'
GO