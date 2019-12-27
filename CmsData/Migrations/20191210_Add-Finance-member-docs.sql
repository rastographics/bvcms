IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Finance'
          AND Object_ID = Object_ID(N'dbo.MemberDocForm'))
BEGIN
    ALTER TABLE dbo.MemberDocForm
    ADD [Finance] bit NOT NULL CONSTRAINT DF_MemberDocForm_Finance DEFAULT 0
END