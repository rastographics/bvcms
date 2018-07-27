ALTER TABLE lookup.OrganizationStatus ADD Active BIT NOT NULL DEFAULT 0
GO
UPDATE lookup.OrganizationStatus SET Active = 1 WHERE Id = 30
GO

ALTER TABLE lookup.MemberType ADD Pending BIT NOT NULL DEFAULT 0
GO
UPDATE lookup.MemberType SET Pending = 1 WHERE Id = 311
GO

ALTER TABLE lookup.MemberType ADD Inactive bit not null default 0
GO
UPDATE lookup.MemberType SET Inactive = 1 WHERE Id = 230
GO

ALTER TABLE lookup.AttendType ADD Worker BIT NOT NULL DEFAULT 0
GO
UPDATE lookup.AttendType SET Worker = 1 WHERE Id IN( 10, 20 )
GO

ALTER TABLE lookup.AttendType ADD Guest BIT NOT NULL DEFAULT 0
GO
UPDATE lookup.AttendType SET Guest = 1 WHERE Id IN( 40, 50, 60 )
GO

ALTER TABLE lookup.MemberStatus ADD Member BIT DEFAULT 0 NOT NULL
GO
UPDATE lookup.MemberStatus SET Member = 1 WHERE Id IN( 10, 40 )
GO

ALTER TABLE lookup.MemberStatus ADD Previous BIT DEFAULT 0 NOT NULL
GO
UPDATE lookup.MemberStatus SET Previous = 1 WHERE Id IN( 40 )
GO

ALTER TABLE lookup.MemberStatus ADD Pending BIT DEFAULT 0 NOT NULL
GO
UPDATE lookup.MemberStatus SET Pending = 1 WHERE Id IN( 30 )
GO

ALTER TABLE lookup.MaritalStatus ADD Single INT DEFAULT 0 NOT NULL
GO
UPDATE lookup.MaritalStatus SET Single = 1 WHERE Id IN( 10, 40, 50 )
GO

ALTER TABLE lookup.MaritalStatus ADD Married INT DEFAULT 0 NOT NULL
GO
UPDATE lookup.MaritalStatus SET Married = 1 WHERE Id IN( 20, 30 )
GO

ALTER TABLE lookup.FamilyPosition ADD PrimaryAdult INT DEFAULT 0 NOT NULL
GO
UPDATE lookup.FamilyPosition SET PrimaryAdult = 1 WHERE Id IN( 10 )
GO

ALTER TABLE lookup.FamilyPosition ADD SecondaryAdult INT DEFAULT 0 NOT NULL
GO
UPDATE lookup.FamilyPosition SET SecondaryAdult = 1 WHERE Id IN( 20 )
GO

ALTER TABLE lookup.FamilyPosition ADD Child INT DEFAULT 0 NOT NULL
GO
UPDATE lookup.FamilyPosition SET Child = 1 WHERE Id IN( 30 )
GO