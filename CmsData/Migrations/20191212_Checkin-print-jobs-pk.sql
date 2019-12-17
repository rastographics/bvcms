IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'PrintJobId'
          AND Object_ID = Object_ID(N'dbo.PrintJob'))
BEGIN
    ALTER TABLE dbo.PrintJob DROP CONSTRAINT PK_TempData;

    ALTER TABLE dbo.PrintJob ADD
	    PrintJobId uniqueidentifier NOT NULL CONSTRAINT DF_PrintJob_PrintJobId DEFAULT NEWID();

    ALTER TABLE dbo.PrintJob ADD CONSTRAINT
	    PK_PrintJob PRIMARY KEY CLUSTERED 
	    (
	    PrintJobId
	    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

    ALTER TABLE dbo.PrintJob SET (LOCK_ESCALATION = TABLE);
END
