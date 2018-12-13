IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Disabled'
          AND Object_ID = Object_ID(N'dbo.RecurringAmounts'))
BEGIN
	UPDATE dbo.RecurringAmounts SET Amt = 0
	WHERE Disabled = 1

	DECLARE @nameConst as nvarchar(255);
	
	SELECT @nameConst = name FROM dbo.sysobjects 
	WHERE name like 'DF__Recurring__Disab__%' and type = 'D'

	IF @nameConst is not null 
	BEGIN
		EXEC('ALTER TABLE dbo.RecurringAmounts DROP CONSTRAINT ' + @nameConst);
	END

	ALTER TABLE dbo.RecurringAmounts
	DROP COLUMN Disabled
END
GO
