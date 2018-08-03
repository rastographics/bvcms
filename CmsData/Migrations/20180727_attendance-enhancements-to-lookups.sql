IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Active'
          AND Object_ID = Object_ID(N'lookup.OrganizationStatus'))
BEGIN
    ALTER TABLE lookup.OrganizationStatus ADD Active BIT NOT NULL DEFAULT 0
END
GO

UPDATE lookup.OrganizationStatus SET Active = 1 WHERE Id = 30
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Pending'
          AND Object_ID = Object_ID(N'lookup.MemberType'))
BEGIN
    ALTER TABLE lookup.MemberType ADD Pending BIT NOT NULL DEFAULT 0
END
GO

UPDATE lookup.MemberType SET Pending = 1 WHERE Id = 311
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Inactive'
          AND Object_ID = Object_ID(N'lookup.MemberType'))
BEGIN
    ALTER TABLE lookup.MemberType ADD Inactive bit not null default 0
END
GO

UPDATE lookup.MemberType SET Inactive = 1 WHERE Id = 230
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Worker'
          AND Object_ID = Object_ID(N'lookup.AttendType'))
BEGIN
    ALTER TABLE lookup.AttendType ADD Worker BIT NOT NULL DEFAULT 0
END
GO
UPDATE lookup.AttendType SET Worker = 1 WHERE Id IN( 10, 20 )
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Guest'
          AND Object_ID = Object_ID(N'lookup.AttendType'))
BEGIN    
    ALTER TABLE lookup.AttendType ADD Guest BIT NOT NULL DEFAULT 0
END
GO

UPDATE lookup.AttendType SET Guest = 1 WHERE Id IN( 40, 50, 60 )
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Member'
          AND Object_ID = Object_ID(N'lookup.MemberStatus'))
BEGIN
    ALTER TABLE lookup.MemberStatus ADD Member BIT DEFAULT 0 NOT NULL
END
GO

UPDATE lookup.MemberStatus SET Member = 1 WHERE Id IN( 10, 40 )
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Previous'
          AND Object_ID = Object_ID(N'lookup.MemberStatus'))
BEGIN
    ALTER TABLE lookup.MemberStatus ADD Previous BIT DEFAULT 0 NOT NULL
END
GO

UPDATE lookup.MemberStatus SET Previous = 1 WHERE Id IN( 40 )
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Pending'
          AND Object_ID = Object_ID(N'lookup.MemberStatus'))
BEGIN
    ALTER TABLE lookup.MemberStatus ADD Pending BIT DEFAULT 0 NOT NULL
END
GO

UPDATE lookup.MemberStatus SET Pending = 1 WHERE Id IN( 30 )
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Single'
          AND Object_ID = Object_ID(N'lookup.MaritalStatus'))
BEGIN
    ALTER TABLE lookup.MaritalStatus ADD Single INT DEFAULT 0 NOT NULL
END
GO

UPDATE lookup.MaritalStatus SET Single = 1 WHERE Id IN( 10, 40, 50 )
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Married'
          AND Object_ID = Object_ID(N'lookup.MaritalStatus'))
BEGIN
    ALTER TABLE lookup.MaritalStatus ADD Married INT DEFAULT 0 NOT NULL
END
GO

UPDATE lookup.MaritalStatus SET Married = 1 WHERE Id IN( 20, 30 )
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'PrimaryAdult'
          AND Object_ID = Object_ID(N'lookup.FamilyPosition'))
BEGIN
    ALTER TABLE lookup.FamilyPosition ADD PrimaryAdult INT DEFAULT 0 NOT NULL
END
GO

UPDATE lookup.FamilyPosition SET PrimaryAdult = 1 WHERE Id IN( 10 )
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'SecondaryAdult'
          AND Object_ID = Object_ID(N'lookup.FamilyPosition'))
BEGIN
    ALTER TABLE lookup.FamilyPosition ADD SecondaryAdult INT DEFAULT 0 NOT NULL
END
GO

UPDATE lookup.FamilyPosition SET SecondaryAdult = 1 WHERE Id IN( 20 )
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Child'
          AND Object_ID = Object_ID(N'lookup.FamilyPosition'))
BEGIN
    ALTER TABLE lookup.FamilyPosition ADD Child INT DEFAULT 0 NOT NULL
END
GO

UPDATE lookup.FamilyPosition SET Child = 1 WHERE Id IN( 30 )
GO
