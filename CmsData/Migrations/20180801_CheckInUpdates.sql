-- @formatter:off
-- SELECT *
-- FROM OrgSchedule
--
-- ALTER TABLE OrgSchedule
-- 	ADD MeetingTimeOnly AS (CAST(CAST( MeetingTime AS TIME )AS DATETIME ))
--
-- ALTER TABLE OrgSchedule
-- 	ADD SchedDayOffset AS (CASE WHEN SchedDay - (DATEPART( dw, GETDATE( ) ) - 1) < 0 THEN 7 + SchedDay - (DATEPART( dw, GETDATE( ) ) - 1)
-- 									WHEN SchedDay - (DATEPART( dw, GETDATE( ) ) - 1) = 0 THEN 0
-- 								  ELSE SchedDay - (DATEPART( dw, GETDATE( ) ))
-- 								  END)
--
-- ALTER TABLE OrgSchedule DROP COLUMN MeetingTimeOnly
-- ALTER TABLE OrgSchedule DROP COLUMN SchedDayOffset
-- ALTER TABLE OrgSchedule DROP COLUMN NextMeetingDate

ALTER TABLE OrgSchedule
	ADD NextMeetingDate AS (CAST( DATEADD( dd, CASE WHEN SchedDay - (DATEPART( dw, GETDATE( ) ) - 1) < 0 THEN 7 + SchedDay - (DATEPART( dw, GETDATE( ) ) - 1)
									WHEN SchedDay - (DATEPART( dw, GETDATE( ) ) - 1) = 0 THEN 0
								  ELSE SchedDay - (DATEPART( dw, GETDATE( ) ) - 1)
								  END, CAST( GETDATE( ) AS DATE )) AS DATETIME) + CAST(CAST( MeetingTime AS TIME )AS DATETIME ))

-- Misc
ALTER TABLE lookup.OrganizationStatus ADD Active BIT NOT NULL DEFAULT 0
UPDATE lookup.OrganizationStatus SET Active = 1 WHERE Id = 30

ALTER TABLE lookup.MemberType ADD Pending BIT NOT NULL DEFAULT 0
UPDATE lookup.MemberType SET Pending = 1 WHERE Id = 311

ALTER TABLE lookup.MemberType ADD Inactive bit not null default 0
UPDATE lookup.MemberType SET Inactive = 1 WHERE Id = 230

ALTER TABLE lookup.AttendType ADD Worker BIT NOT NULL DEFAULT 0
UPDATE lookup.AttendType SET Worker = 1 WHERE Id IN( 10, 20 )

ALTER TABLE lookup.AttendType ADD Guest BIT NOT NULL DEFAULT 0
UPDATE lookup.AttendType SET Guest = 1 WHERE Id IN( 40, 50, 60 )

ALTER TABLE lookup.MemberStatus ADD Member BIT DEFAULT 0 NOT NULL
UPDATE lookup.MemberStatus SET Member = 1 WHERE Id IN( 10, 40 )

ALTER TABLE lookup.MemberStatus ADD Previous BIT DEFAULT 0 NOT NULL
UPDATE lookup.MemberStatus SET Previous = 1 WHERE Id IN( 40 )

ALTER TABLE lookup.MemberStatus ADD Pending BIT DEFAULT 0 NOT NULL
UPDATE lookup.MemberStatus SET Pending = 1 WHERE Id IN( 30 )

-- CheckInAPIv2Controller: Authenticate
ALTER TABLE CheckInSettings ADD version INT NOT NULL DEFAULT 1

-- CheckInAPIv2Controller: UpdateAttend
ALTER TABLE Attend ADD SubGroupID INT NOT NULL DEFAULT 0
ALTER TABLE Attend ADD SubGroupName NVARCHAR(200) NOT NULL DEFAULT ''
ALTER TABLE Attend ADD Pager NVARCHAR(20) NOT NULL DEFAULT ''

-- Subgroup: forGroupID
ALTER TABLE MemberTags ADD CheckIn BIT NOT NULL DEFAULT 0
ALTER TABLE MemberTags ADD CheckInOpen BIT NOT NULL DEFAULT 1
ALTER TABLE MemberTags ADD CheckInCapacity INT NOT NULL DEFAULT 0

-- Person: computePositionInFamily
ALTER TABLE lookup.FamilyPosition ADD PrimaryAdult INT DEFAULT 0 NOT NULL
UPDATE lookup.FamilyPosition SET PrimaryAdult = 1 WHERE Id IN( 10 )

ALTER TABLE lookup.FamilyPosition ADD SecondaryAdult INT DEFAULT 0 NOT NULL
UPDATE lookup.FamilyPosition SET SecondaryAdult = 1 WHERE Id IN( 20 )

ALTER TABLE lookup.FamilyPosition ADD Child INT DEFAULT 0 NOT NULL
UPDATE lookup.FamilyPosition SET Child = 1 WHERE Id IN( 30 )

ALTER TABLE lookup.MaritalStatus ADD Single INT DEFAULT 0 NOT NULL
UPDATE lookup.MaritalStatus SET Single = 1 WHERE Id IN( 10, 40, 50 )

ALTER TABLE lookup.MaritalStatus ADD Married INT DEFAULT 0 NOT NULL
UPDATE lookup.MaritalStatus SET Married = 1 WHERE Id IN( 20, 30 )
-- @formatter:on

-- LabelFormat
CREATE TABLE CheckInLabelType
(
	id        INT IDENTITY             NOT NULL PRIMARY KEY,
	name      NVARCHAR(20) DEFAULT ''  NOT NULL,
	canRepeat BIT DEFAULT 0            NOT NULL
)

SET IDENTITY_INSERT CheckInLabelType ON;
INSERT INTO CheckInLabelType (id, name, canRepeat) VALUES (1, 'Main', 1);
INSERT INTO CheckInLabelType (id, name, canRepeat) VALUES (2, 'Location', 1);
INSERT INTO CheckInLabelType (id, name, canRepeat) VALUES (3, 'Security', 0);
INSERT INTO CheckInLabelType (id, name, canRepeat) VALUES (4, 'Guest', 1);
INSERT INTO CheckInLabelType (id, name, canRepeat) VALUES (5, 'Extra', 1);
INSERT INTO CheckInLabelType (id, name, canRepeat) VALUES (6, 'Name Tag', 0);
SET IDENTITY_INSERT CheckInLabelType OFF;

CREATE TABLE CheckInLabel
(
	id      INT IDENTITY            NOT NULL PRIMARY KEY,
	typeID  INT DEFAULT 0           NOT NULL,
	name    NVARCHAR(50) DEFAULT '' NOT NULL,
	minimum INT DEFAULT 0           NOT NULL,
	maximum INT DEFAULT 0           NOT NULL
)

SET IDENTITY_INSERT CheckInLabel ON;
INSERT INTO CheckInLabel (id, typeID, name, minimum, maximum) VALUES (1, 1, '1 Inch - Main', 50, 149);
INSERT INTO CheckInLabel (id, typeID, name, minimum, maximum) VALUES (2, 2, '1 Inch - Location', 50, 149);
INSERT INTO CheckInLabel (id, typeID, name, minimum, maximum) VALUES (3, 3, '1 Inch - Security', 50, 149);
INSERT INTO CheckInLabel (id, typeID, name, minimum, maximum) VALUES (4, 4, '1 Inch - Guest', 50, 149);
INSERT INTO CheckInLabel (id, typeID, name, minimum, maximum) VALUES (5, 5, '1 Inch - Extra', 50, 149);
INSERT INTO CheckInLabel (id, typeID, name, minimum, maximum) VALUES (6, 6, '1 Inch - Name Tag', 50, 149);
INSERT INTO CheckInLabel (id, typeID, name, minimum, maximum) VALUES (7, 1, '2 Inch - Main', 150, 249);
INSERT INTO CheckInLabel (id, typeID, name, minimum, maximum) VALUES (8, 2, '2 Inch - Location', 150, 249);
INSERT INTO CheckInLabel (id, typeID, name, minimum, maximum) VALUES (9, 3, '2 Inch - Security', 150, 249);
INSERT INTO CheckInLabel (id, typeID, name, minimum, maximum) VALUES (10, 4, '2 Inch - Guest', 150, 249);
INSERT INTO CheckInLabel (id, typeID, name, minimum, maximum) VALUES (11, 5, '2 Inch - Extra', 150, 249);
INSERT INTO CheckInLabel (id, typeID, name, minimum, maximum) VALUES (12, 6, '2 Inch - Name Tag', 150, 249);
SET IDENTITY_INSERT CheckInLabel OFF;

CREATE TABLE CheckInLabelEntryAlignment
(
	id    INT IDENTITY            NOT NULL PRIMARY KEY,
	nameX NVARCHAR(10) DEFAULT '' NOT NULL,
	nameY NVARCHAR(10) DEFAULT '' NOT NULL
)

SET IDENTITY_INSERT CheckInLabelEntryAlignment ON;
INSERT INTO CheckInLabelEntryAlignment (id, nameX, nameY) VALUES (1, 'Top', 'Left');
INSERT INTO CheckInLabelEntryAlignment (id, nameX, nameY) VALUES (2, 'Center', 'Middle');
INSERT INTO CheckInLabelEntryAlignment (id, nameX, nameY) VALUES (3, 'Bottom', 'Right');
SET IDENTITY_INSERT CheckInLabelEntryAlignment OFF;

CREATE TABLE CheckInLabelEntry
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

CREATE TABLE PrintJob
(
	id         INT IDENTITY             NOT NULL PRIMARY KEY,
	kiosk      NVARCHAR(25) DEFAULT ''  NOT NULL,
	peopleID   INT DEFAULT 0            NOT NULL,
	groupID    INT DEFAULT 0            NOT NULL,
	subGroupID INT DEFAULT 0            NOT NULL,
	data       NVARCHAR(MAX) DEFAULT '' NOT NULL
)