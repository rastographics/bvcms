
IF NOT EXISTS(SELECT 1 FROM information_schema.columns WHERE table_name = 'PaymentInfo' AND COLUMN_NAME = 'GatewayAccountId')
BEGIN
    ALTER TABLE dbo.PaymentInfo ADD GatewayAccountId int NOT NULL
    CONSTRAINT DF_PaymentInfo_GatewayAccountId DEFAULT (0)
END
GO

UPDATE PaymentInfo SET GatewayAccountId = (SELECT ISNULL(GatewayAccountId, 0) FROM PaymentProcess WHERE ProcessId = 2)
GO

IF NOT EXISTS (SELECT * FROM sys.objects o WHERE o.object_id = object_id(N'[dbo].[FK_PaymentInfo_GatewayAccount]') AND OBJECTPROPERTY(o.object_id, N'IsForeignKey') = 1)
BEGIN
    ALTER TABLE dbo.PaymentInfo ADD CONSTRAINT
	FK_PaymentInfo_GatewayAccount FOREIGN KEY
	(GatewayAccountId) REFERENCES dbo.GatewayAccount (GatewayAccountId)
END
GO
