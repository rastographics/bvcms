IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'AcceptACH'
          AND Object_ID = Object_ID(N'dbo.PaymentProcess'))
BEGIN
    ALTER TABLE dbo.PaymentProcess 
	    ADD AcceptACH BIT NOT NULL CONSTRAINT DF_PaymentProcessAcceptACH DEFAULT (1),
	        AcceptCredit BIT NOT NULL CONSTRAINT DF_PaymentProcessAcceptCredit DEFAULT (1),
	        AcceptDebit BIT NOT NULL CONSTRAINT DF_PaymentProcessAcceptDebit DEFAULT (1)
END
GO

WITH oldsettings AS (
	SELECT
		ISNULL((SELECT Setting FROM dbo.Setting WHERE Id = 'NoCreditCardGiving'), 'false') NoCreditCardGiving,
		ISNULL((SELECT Setting FROM dbo.Setting WHERE Id = 'NoEChecksAllowed'), 'false') NoEChecksAllowed
),
newsettings AS (
	SELECT IIF(os.NoEChecksAllowed = 'true', 0, 1) AcceptACH,
		1 AcceptDebit,
		IIF(os.NoCreditCardGiving = 'true', 0, 1) AcceptCredit
		FROM oldsettings os
),
updates AS (
SELECT pp.ProcessId, pp.ProcessName, s.AcceptDebit,
	CASE pp.ProcessId 
	WHEN 3 THEN 1 --Registrations
	ELSE s.AcceptACH
	END AcceptACH,
	CASE pp.ProcessId 
	WHEN 3 THEN 1 --Registrations
	ELSE s.AcceptCredit
	END AcceptCredit
FROM newsettings s
CROSS JOIN dbo.PaymentProcess pp
)
UPDATE process
SET AcceptACH = updates.AcceptACH, AcceptCredit = updates.AcceptCredit, AcceptDebit = updates.AcceptDebit
FROM dbo.PaymentProcess process
JOIN updates u ON u.ProcessId = process.ProcessId
GO
