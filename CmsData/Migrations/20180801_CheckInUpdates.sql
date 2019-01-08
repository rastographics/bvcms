
IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'NextMeetingDate'
          AND Object_ID = Object_ID(N'dbo.OrgSchedule'))
BEGIN
ALTER TABLE dbo.OrgSchedule
ADD NextMeetingDate AS (CAST( DATEADD( dd, CASE WHEN SchedDay - (DATEPART( dw, GETDATE( ) ) - 1) < 0 THEN 7 + SchedDay - (DATEPART( dw, GETDATE( ) ) - 1)
								WHEN SchedDay - (DATEPART( dw, GETDATE( ) ) - 1) = 0 THEN 0
								ELSE SchedDay - (DATEPART( dw, GETDATE( ) ) - 1)
								END, CAST( GETDATE( ) AS DATE )) AS DATETIME) + CAST(CAST( MeetingTime AS TIME )AS DATETIME ))
END
GO

-- Misc
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
    WHERE Name = N'version'
    AND Object_ID = OBJECT_ID(N'dbo.CheckInSettings'))
BEGIN
ALTER TABLE dbo.CheckInSettings ADD version INT NOT NULL DEFAULT 1
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'SubGroupID'
    AND Object_ID = OBJECT_ID(N'dbo.Attend'))
BEGIN
ALTER TABLE dbo.Attend ADD SubGroupID INT NOT NULL DEFAULT 0
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'SubGroupName'
    AND Object_ID = OBJECT_ID(N'dbo.Attend'))
BEGIN
ALTER TABLE dbo.Attend ADD SubGroupName NVARCHAR(200) NOT NULL DEFAULT ''
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'Pager'
    AND Object_ID = OBJECT_ID(N'dbo.Attend'))
BEGIN
ALTER TABLE dbo.Attend ADD Pager NVARCHAR(20) NOT NULL DEFAULT ''
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'CheckIn'
    AND Object_ID = OBJECT_ID(N'dbo.MemberTags'))
BEGIN
ALTER TABLE dbo.MemberTags ADD CheckIn BIT NOT NULL DEFAULT 0
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'CheckInOpen'
    AND Object_ID = OBJECT_ID(N'dbo.MemberTags'))
BEGIN
ALTER TABLE dbo.MemberTags ADD CheckInOpen BIT NOT NULL DEFAULT 1
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'CheckInOpenDefault'
    AND Object_ID = OBJECT_ID(N'dbo.MemberTags'))
BEGIN
ALTER TABLE dbo.MemberTags ADD CheckInOpenDefault BIT NOT NULL DEFAULT 1
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'CheckInCapacity'
    AND Object_ID = OBJECT_ID(N'dbo.MemberTags'))
BEGIN
ALTER TABLE dbo.MemberTags ADD CheckInCapacity INT NOT NULL DEFAULT 0
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'CheckInCapacityDefault'
    AND Object_ID = OBJECT_ID(N'dbo.MemberTags'))
BEGIN
ALTER TABLE dbo.MemberTags ADD CheckInCapacityDefault INT NOT NULL DEFAULT 0
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'ScheduleId'
    AND Object_ID = OBJECT_ID(N'dbo.MemberTags'))
BEGIN
ALTER TABLE dbo.MemberTags ADD ScheduleId INT NULL
ALTER TABLE dbo.MemberTags ADD CONSTRAINT
	FK_MemberTags_OrgSchedule FOREIGN KEY
	(
	OrgId,
	ScheduleId
	) REFERENCES dbo.OrgSchedule
	(
	OrganizationId,
	Id
	)
END
GO

-- Person: computePositionInFamily
IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'PrimaryAdult'
    AND Object_ID = OBJECT_ID(N'lookup.FamilyPosition'))
BEGIN
ALTER TABLE lookup.FamilyPosition ADD PrimaryAdult INT DEFAULT 0 NOT NULL
END
GO

UPDATE lookup.FamilyPosition SET PrimaryAdult = 1 WHERE Id IN( 10 )
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'SecondaryAdult'
    AND Object_ID = OBJECT_ID(N'lookup.FamilyPosition'))
BEGIN
ALTER TABLE lookup.FamilyPosition ADD SecondaryAdult INT DEFAULT 0 NOT NULL
END
GO

UPDATE lookup.FamilyPosition SET SecondaryAdult = 1 WHERE Id IN( 20 )
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'Child'
    AND Object_ID = OBJECT_ID(N'lookup.FamilyPosition'))
BEGIN
ALTER TABLE lookup.FamilyPosition ADD Child INT DEFAULT 0 NOT NULL
END
GO

UPDATE lookup.FamilyPosition SET Child = 1 WHERE Id IN( 30 )
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'Single'
    AND Object_ID = OBJECT_ID(N'lookup.MaritalStatus'))
BEGIN
ALTER TABLE lookup.MaritalStatus ADD Single INT DEFAULT 0 NOT NULL
END
GO

UPDATE lookup.MaritalStatus SET Single = 1 WHERE Id IN( 10, 40, 50 )
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns
    WHERE Name = N'Married'
    AND Object_ID = OBJECT_ID(N'lookup.MaritalStatus'))
BEGIN
ALTER TABLE lookup.MaritalStatus ADD Married INT DEFAULT 0 NOT NULL
END
GO

IF OBJECT_ID(N'dbo.CheckInLabelType', N'U') IS NULL
BEGIN
CREATE TABLE dbo.CheckInLabelType
(
	id        INT IDENTITY             NOT NULL PRIMARY KEY,
	name      NVARCHAR(20) DEFAULT ''  NOT NULL,
	canRepeat BIT DEFAULT 0            NOT NULL
)
END
GO

IF NOT EXISTS(SELECT 1 FROM dbo.CheckInLabelType)
BEGIN
SET IDENTITY_INSERT dbo.CheckInLabelType ON;
INSERT INTO dbo.CheckInLabelType (id, name, canRepeat) VALUES (1, 'Main', 1);
INSERT INTO dbo.CheckInLabelType (id, name, canRepeat) VALUES (2, 'Location', 1);
INSERT INTO dbo.CheckInLabelType (id, name, canRepeat) VALUES (3, 'Security', 0);
INSERT INTO dbo.CheckInLabelType (id, name, canRepeat) VALUES (4, 'Guest', 1);
INSERT INTO dbo.CheckInLabelType (id, name, canRepeat) VALUES (5, 'Extra', 1);
INSERT INTO dbo.CheckInLabelType (id, name, canRepeat) VALUES (6, 'Name Tag', 0);
SET IDENTITY_INSERT dbo.CheckInLabelType OFF;
END
GO

IF OBJECT_ID(N'dbo.CheckInLabel', N'U') IS NULL
BEGIN
CREATE TABLE dbo.CheckInLabel
(
	id      INT IDENTITY            NOT NULL PRIMARY KEY,
	typeID  INT DEFAULT 0           NOT NULL,
	name    NVARCHAR(50) DEFAULT '' NOT NULL,
	minimum INT DEFAULT 0           NOT NULL,
	maximum INT DEFAULT 0           NOT NULL
)
END
GO

IF NOT EXISTS(SELECT 1 FROM dbo.CheckInLabel)
BEGIN
SET IDENTITY_INSERT dbo.CheckInLabel ON;
INSERT INTO dbo.CheckInLabel (id, typeID, name, minimum, maximum) VALUES (1, 1, '1 Inch - Main', 50, 149);
INSERT INTO dbo.CheckInLabel (id, typeID, name, minimum, maximum) VALUES (2, 2, '1 Inch - Location', 50, 149);
INSERT INTO dbo.CheckInLabel (id, typeID, name, minimum, maximum) VALUES (3, 3, '1 Inch - Security', 50, 149);
INSERT INTO dbo.CheckInLabel (id, typeID, name, minimum, maximum) VALUES (4, 4, '1 Inch - Guest', 50, 149);
INSERT INTO dbo.CheckInLabel (id, typeID, name, minimum, maximum) VALUES (5, 5, '1 Inch - Extra', 50, 149);
INSERT INTO dbo.CheckInLabel (id, typeID, name, minimum, maximum) VALUES (6, 6, '1 Inch - Name Tag', 50, 149);
INSERT INTO dbo.CheckInLabel (id, typeID, name, minimum, maximum) VALUES (7, 1, '2 Inch - Main', 150, 249);
INSERT INTO dbo.CheckInLabel (id, typeID, name, minimum, maximum) VALUES (8, 2, '2 Inch - Location', 150, 249);
INSERT INTO dbo.CheckInLabel (id, typeID, name, minimum, maximum) VALUES (9, 3, '2 Inch - Security', 150, 249);
INSERT INTO dbo.CheckInLabel (id, typeID, name, minimum, maximum) VALUES (10, 4, '2 Inch - Guest', 150, 249);
INSERT INTO dbo.CheckInLabel (id, typeID, name, minimum, maximum) VALUES (11, 5, '2 Inch - Extra', 150, 249);
INSERT INTO dbo.CheckInLabel (id, typeID, name, minimum, maximum) VALUES (12, 6, '2 Inch - Name Tag', 150, 249);
SET IDENTITY_INSERT dbo.CheckInLabel OFF;
END
GO

IF OBJECT_ID(N'dbo.CheckInLabelEntryAlignment', N'U') IS NULL
BEGIN
CREATE TABLE dbo.CheckInLabelEntryAlignment
(
	id    INT IDENTITY            NOT NULL PRIMARY KEY,
	nameX NVARCHAR(10) DEFAULT '' NOT NULL,
	nameY NVARCHAR(10) DEFAULT '' NOT NULL
)
END
GO

IF NOT EXISTS(SELECT 1 FROM dbo.CheckInLabelEntryAlignment)
BEGIN
SET IDENTITY_INSERT dbo.CheckInLabelEntryAlignment ON;
INSERT INTO dbo.CheckInLabelEntryAlignment (id, nameX, nameY) VALUES (1, 'Top', 'Left');
INSERT INTO dbo.CheckInLabelEntryAlignment (id, nameX, nameY) VALUES (2, 'Center', 'Middle');
INSERT INTO dbo.CheckInLabelEntryAlignment (id, nameX, nameY) VALUES (3, 'Bottom', 'Right');
SET IDENTITY_INSERT dbo.CheckInLabelEntryAlignment OFF;
END
GO

IF OBJECT_ID(N'dbo.CheckInLabelEntry', N'U') IS NULL
BEGIN
CREATE TABLE dbo.CheckInLabelEntry
(
	id          INT IDENTITY                             NOT NULL PRIMARY KEY,
	labelID     INT DEFAULT 0                            NOT NULL,
	typeID      INT DEFAULT 0                            NOT NULL,
	repeat      INT DEFAULT 1                            NOT NULL,
	offset      DECIMAL(4, 3) DEFAULT 0                  NOT NULL,
	font        NVARCHAR(25) DEFAULT ''                  NOT NULL,
	fontSize    INT DEFAULT 12                           NOT NULL,
	fieldID     INT DEFAULT 0                            NOT NULL,
	fieldFormat NVARCHAR(100) DEFAULT ''                 NOT NULL,
	startX      DECIMAL(4, 3) DEFAULT 0                  NOT NULL,
	startY      DECIMAL(4, 3) DEFAULT 0                  NOT NULL,
	alignX      INT DEFAULT 0                            NOT NULL,
	alignY      INT DEFAULT 0                            NOT NULL,
	endX        DECIMAL(4, 3) DEFAULT 0                  NOT NULL,
	endY        DECIMAL(4, 3) DEFAULT 0                  NOT NULL,
	width       INT DEFAULT 0                            NOT NULL,
	height      INT DEFAULT 0                            NOT NULL
)
END
GO

IF OBJECT_ID(N'dbo.PrintJob', N'U') IS NULL
BEGIN
CREATE TABLE dbo.PrintJob
(
	id         INT IDENTITY             NOT NULL PRIMARY KEY,
	kiosk      NVARCHAR(25) DEFAULT ''  NOT NULL,
	peopleID   INT DEFAULT 0            NOT NULL,
	groupID    INT DEFAULT 0            NOT NULL,
	subGroupID INT DEFAULT 0            NOT NULL,
	data       NVARCHAR(MAX) DEFAULT '' NOT NULL
)
END
GO
