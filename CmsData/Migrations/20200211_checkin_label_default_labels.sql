IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'invert'
          AND Object_ID = Object_ID(N'dbo.CheckInLabelEntry'))
BEGIN
    DROP TABLE CheckInLabelEntry
END
GO

IF OBJECT_ID(N'dbo.CheckInLabelEntry', N'U') IS NULL
BEGIN
    -- recreate the label entry table with named constraints, invert and order columns, and width and height as decimals instead of ints
    CREATE TABLE dbo.CheckInLabelEntry
    (
	    id          INT IDENTITY                             NOT NULL PRIMARY KEY,
	    labelID     INT CONSTRAINT labelID_default DEFAULT 0                            NOT NULL,
	    typeID      INT CONSTRAINT typeID_default DEFAULT 0                            NOT NULL,
	    [repeat]    INT CONSTRAINT repeat_default DEFAULT 1                            NOT NULL,
	    offset      DECIMAL(4, 3) CONSTRAINT offset_default DEFAULT 0                  NOT NULL,
	    font        NVARCHAR(25) CONSTRAINT font_default DEFAULT ''                  NOT NULL,
	    fontSize    INT CONSTRAINT fontSize_default DEFAULT 12                           NOT NULL,
	    fieldID     INT CONSTRAINT fieldID_default DEFAULT 0                            NOT NULL,
	    fieldFormat NVARCHAR(100) CONSTRAINT fieldFormat_default DEFAULT ''                 NOT NULL,
	    startX      DECIMAL(4, 3) CONSTRAINT startX_default DEFAULT 0                  NOT NULL,
	    startY      DECIMAL(4, 3) CONSTRAINT startY_default DEFAULT 0                  NOT NULL,
	    alignX      INT CONSTRAINT alignX_default DEFAULT 0                            NOT NULL,
	    alignY      INT CONSTRAINT alignY_default DEFAULT 0                            NOT NULL,
	    endX        DECIMAL(4, 3) CONSTRAINT endX_default DEFAULT 0                  NOT NULL,
	    endY        DECIMAL(4, 3) CONSTRAINT endY_default DEFAULT 0                  NOT NULL,
	    width       DECIMAL(4, 3) CONSTRAINT width_default DEFAULT 0                            NOT NULL,
	    height      DECIMAL(4, 3) CONSTRAINT height_default DEFAULT 0                            NOT NULL,
        invert      BIT CONSTRAINT invert_default DEFAULT 0 NOT NULL,
	    [order]     INT CONSTRAINT order_default DEFAULT 0 NOT NULL
    )
END
GO

IF (select COUNT(*) from CheckInLabelEntry) = 0
BEGIN
    INSERT INTO [dbo].[CheckInLabelEntry]
           ([labelID],[typeID],[repeat],[offset],[font],[fontSize],[fieldID],[fieldFormat],[startX],[startY],[alignX],[alignY],[endX],[endY],[width],[height],[invert],[order])
     VALUES
     
      -- MAIN 1"
(1, 1, 1, 0.000, 'Arial', 20,  2,  '{0}',                 0.026, 0.000, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- first name
(1, 1, 1, 0.000, 'Arial', 14,  3,  '{0}',                 0.026, 0.300, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- last name
(1, 1, 1, 0.000, 'Arial', 10,  5,  '{0}{1}{2}',           0.500, 0.360, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- A/C/T
(1, 2, 1, 0.000, '',      12,  0,  '',                    0.035, 0.600, 0, 0, 0.965, 0.600, 2, 0, 0, 0), -- line
(1, 1, 1, 0.000, 'Arial', 10,  42, '{0}',                 0.965, 0.800, 3, 3, 0.000, 0.000, 0, 0, 0, 0), -- class location
(1, 1, 1, 0.000, 'Arial', 10,  44, '{0} ({1:h\:mm tt})',  0.026, 0.800, 1, 3, 0.000, 0.000, 0, 0, 0, 0), -- class name and time
(1, 1, 1, 0.000, 'Arial', 10,  61, '{0:M/d/yy}',          0.965, 0.350, 3, 1, 0.000, 0.000, 0, 0, 0, 0), -- date
(1, 1, 1, 0.000, 'Arial', 16,  1,  '{0}',                 0.965, 0.000, 3, 1, 0.000, 0.000, 0, 0, 1, 2), -- security code
(1, 6, 1, 0.000, '',      0,   0,  '',                    1.000, 0.000, 3, 1, 0.850, 0.200, 0.667, 0.334, 1, 1), -- box for security code
(1, 1, 1, 0.000, 'Arial', 10,  4,  '{0}',                 0.500, 0.500, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- allergy

      -- LOCATION 1"
(2, 1, 2, 0.440, 'Arial', 12,  2,   '{0}',                0.035, 0.010, 1, 1, 0.000, 0.000, 0, 0, 0, 0),
(2, 1, 2, 0.440, 'Arial', 10,  42,  '{0}',                0.965, 0.220, 3, 1, 0.000, 0.000, 0, 0, 0, 0),
(2, 2, 2, 0.440, '',      0,   0,   '',                   0.035, 0.440, 0, 0, 0.965, 0.440, 1, 0, 0, 0),
(2, 1, 2, 0.440, 'Arial', 10,  44,  '{0} ({1:h\:mm tt})', 0.035, 0.220, 1, 1, 0.000, 0.000, 0, 0, 0, 0),

      -- SECURITY 1"
(3,  1, 1, 0.000, 'Arial', 32,  1,  '{0}',                 0.250, 0.550, 2, 3, 0.000, 0.000, 0, 0, 0, 0),
(3,  1, 1, 0.000, 'Arial', 32,  1,  '{0}',                 0.750, 0.550, 2, 3, 0.000, 0.000, 0, 0, 0, 0),
(3,  1, 1, 0.000, 'Arial', 16,  61, '{0:M/d/yy}',          0.250, 0.550, 2, 1, 0.000, 0.000, 0, 0, 0, 0),
(3,  1, 1, 0.000, 'Arial', 16,  61, '{0:M/d/yy}',          0.750, 0.550, 2, 1, 0.000, 0.000, 0, 0, 0, 0),
(3,  2, 1, 0.000, '',      0,   0,  '',                    0.500, 0.100, 0, 0, 0.500, 0.900, 2, 0, 0, 0),

      -- GUEST 1"
(4, 1, 1, 0.000, 'Arial', 20,  2,  '{0}',                 0.026, 0.000, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- first name
(4, 1, 1, 0.000, 'Arial', 14,  3,  '{0}',                 0.026, 0.300, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- last name
(4, 1, 1, 0.000, 'Arial', 10,  5,  '{0}{1}{2}',           0.500, 0.360, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- A/C/T
(4, 2, 1, 0.000, '',      12,  0,  '',                    0.035, 0.600, 0, 0, 0.965, 0.600, 2, 0, 0, 0), -- line
(4, 1, 1, 0.000, 'Arial', 10,  42, '{0}',                 0.965, 0.800, 3, 3, 0.000, 0.000, 0, 0, 0, 0), -- class location
(4, 1, 1, 0.000, 'Arial', 10,  44, '{0} ({1:h\:mm tt})',  0.026, 0.800, 1, 3, 0.000, 0.000, 0, 0, 0, 0), -- class name and time
(4, 1, 1, 0.000, 'Arial', 10,  61, '{0:M/d/yy}',          0.965, 0.350, 3, 1, 0.000, 0.000, 0, 0, 0, 0), -- date
(4, 1, 1, 0.000, 'Arial', 16,  1,  '{0}',                 0.965, 0.000, 3, 1, 0.000, 0.000, 0, 0, 1, 2), -- security code
(4, 6, 1, 0.000, '',      0,   0,  '',                    1.000, 0.000, 3, 1, 0.850, 0.200, 0.667, 0.334, 1, 1), -- box for security code
(4, 1, 1, 0.000, 'Arial', 10,  4,  '{0}',                 0.500, 0.500, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- allergy
(4, 4, 1, 0.000, 'Arial', 10,  0,  'Guest',               0.500, 0.885, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- guest text

     -- EXTRA 1"
(5, 1, 1, 0.000, 'Arial', 20,  2,  '{0}',                 0.026, 0.000, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- first name
(5, 1, 1, 0.000, 'Arial', 14,  3,  '{0}',                 0.026, 0.300, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- last name
(5, 1, 1, 0.000, 'Arial', 10,  5,  '{0}{1}{2}',           0.500, 0.360, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- A/C/T
(5, 2, 1, 0.000, '',      12,  0,  '',                    0.035, 0.600, 0, 0, 0.965, 0.600, 2, 0, 0, 0), -- line
(5, 1, 1, 0.000, 'Arial', 10,  42, '{0}',                 0.965, 0.800, 3, 3, 0.000, 0.000, 0, 0, 0, 0), -- class location
(5, 1, 1, 0.000, 'Arial', 10,  44, '{0} ({1:h\:mm tt})',  0.026, 0.800, 1, 3, 0.000, 0.000, 0, 0, 0, 0), -- class name and time
(5, 1, 1, 0.000, 'Arial', 10,  61, '{0:M/d/yy}',          0.965, 0.350, 3, 1, 0.000, 0.000, 0, 0, 0, 0), -- date
(5, 1, 1, 0.000, 'Arial', 16,  1,  '{0}',                 0.965, 0.000, 3, 1, 0.000, 0.000, 0, 0, 1, 2), -- security code
(5, 6, 1, 0.000, '',      0,   0,  '',                    1.000, 0.000, 3, 1, 0.850, 0.200, 0.667, 0.334, 1, 1), -- box for security code
(5, 1, 1, 0.000, 'Arial', 10,  4,  '{0}',                 0.500, 0.500, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- allergy
(5, 4, 1, 0.000, 'Arial', 10,  0,  'Extra',               0.500, 0.885, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- extra text

      -- NAME TAG 1"
(6, 1, 1, 0.000, 'Arial', 32,  2, '{0}',                  0.500, 0.440, 2, 3, 0.000, 0.000, 0, 0, 0, 0), -- first name
(6, 1, 1, 0.000, 'Arial', 24,  3, '{0}',                  0.500, 0.440, 2, 1, 0.000, 0.000, 0, 0, 0, 0), -- last name

     -- MAIN 2"
(7, 1, 1, 0.000, 'Arial', 20,  2,  '{0}',                 0.026, 0.000, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- first name
(7, 1, 1, 0.000, 'Arial', 14,  3,  '{0}',                 0.026, 0.140, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- last name
(7, 1, 1, 0.000, 'Arial', 10,  5,  '{0}{1}{2}',           0.500, 0.200, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- A/C/T
(7, 2, 1, 0.000, '',      12,  0,  '',                    0.035, 0.380, 0, 0, 0.965, 0.380, 2, 0, 0, 0), -- line
(7, 1, 3, 0.120, 'Arial', 10,  42, '{0}',                 0.965, 0.500, 3, 3, 0.000, 0.000, 0, 0, 0, 0), -- class location
(7, 1, 3, 0.120, 'Arial', 10,  44, '{0} ({1:h\:mm tt})',  0.026, 0.500, 1, 3, 0.000, 0.000, 0, 0, 0, 0), -- class name and time
(7, 1, 1, 0.000, 'Arial', 10,  61, '{0:M/d/yy}',          0.965, 0.160, 3, 1, 0.000, 0.000, 0, 0, 0, 0), -- date
(7, 6, 1, 0.000, '',      0,   0,  '',                    1.000, 0.000, 3, 1, 0.850, 0.200, 0.667, 0.334, 1, 1), -- box for security code
(7, 1, 1, 0.000, 'Arial', 16,  1,  '{0}',                 0.965, 0.000, 3, 1, 0.000, 0.000, 0, 0, 1, 2), -- security code
(7, 1, 1, 0.000, 'Arial', 10,  4,  '{0}',                 0.500, 0.285, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- allergy

     -- LOCATION 2"
(8,  1, 4, 0.220, 'Arial', 12,  2,  '{0}',                 0.035, 0.010, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- first name
(8,  1, 4, 0.220, 'Arial', 10,  42, '{0}',                 0.965, 0.110, 3, 1, 0.000, 0.000, 0, 0, 0, 0), -- class location
(8,  2, 4, 0.220, '',      0,   0,  '',                    0.035, 0.225, 0, 0, 0.965, 0.225, 1, 0, 0, 0), -- line
(8,  1, 4, 0.220, 'Arial', 10,  44, '{0} ({1:h\:mm tt})',  0.035, 0.110, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- class name and time

      -- SECURITY 2"
(9,  1, 1, 0.000, 'Arial', 32,  1,  '{0}',                 0.250, 0.550, 2, 3, 0.000, 0.000, 0, 0, 0, 0),
(9,  1, 1, 0.000, 'Arial', 32,  1,  '{0}',                 0.750, 0.550, 2, 3, 0.000, 0.000, 0, 0, 0, 0),
(9,  1, 1, 0.000, 'Arial', 16,  61, '{0:M/d/yy}',          0.250, 0.550, 2, 1, 0.000, 0.000, 0, 0, 0, 0),
(9,  1, 1, 0.000, 'Arial', 16,  61, '{0:M/d/yy}',          0.750, 0.550, 2, 1, 0.000, 0.000, 0, 0, 0, 0),
(9,  2, 1, 0.000, '',      0,   0,  '',                    0.500, 0.100, 0, 0, 0.500, 0.900, 2, 0, 0, 0),

     -- GUEST 2"
(10, 1, 1, 0.000, 'Arial', 20,  2,  '{0}',                 0.026, 0.000, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- first name
(10, 1, 1, 0.000, 'Arial', 14,  3,  '{0}',                 0.026, 0.140, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- last name
(10, 1, 1, 0.000, 'Arial', 10,  5,  '{0}{1}{2}',           0.500, 0.200, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- A/C/T
(10, 2, 1, 0.000, '',      12,  0,  '',                    0.035, 0.380, 0, 0, 0.965, 0.380, 2, 0, 0, 0), -- line
(10, 1, 3, 0.120, 'Arial', 10,  42, '{0}',                 0.965, 0.500, 3, 3, 0.000, 0.000, 0, 0, 0, 0), -- class location
(10, 1, 3, 0.120, 'Arial', 10,  44, '{0} ({1:h\:mm tt})',  0.026, 0.500, 1, 3, 0.000, 0.000, 0, 0, 0, 0), -- class name and time
(10, 1, 1, 0.000, 'Arial', 10,  61, '{0:M/d/yy}',          0.965, 0.160, 3, 1, 0.000, 0.000, 0, 0, 0, 0), -- date
(10, 6, 1, 0.000, '',      0,   0,  '',                    1.000, 0.000, 3, 1, 0.850, 0.200, 0.667, 0.334, 1, 1), -- box for security code
(10, 1, 1, 0.000, 'Arial', 16,  1,  '{0}',                 0.965, 0.000, 3, 1, 0.000, 0.000, 0, 0, 1, 2), -- security code
(10, 1, 1, 0.000, 'Arial', 10,  4,  '{0}',                 0.500, 0.285, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- allergy
(10, 4, 1, 0.000, 'Arial', 10,  0,  'Guest',               0.500, 0.885, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- guest text

     -- EXTRA 2"
(11, 1, 1, 0.000, 'Arial', 20,  2,  '{0}',                 0.026, 0.000, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- first name
(11, 1, 1, 0.000, 'Arial', 14,  3,  '{0}',                 0.026, 0.140, 1, 1, 0.000, 0.000, 0, 0, 0, 0), -- last name
(11, 1, 1, 0.000, 'Arial', 10,  5,  '{0}{1}{2}',           0.500, 0.200, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- A/C/T
(11, 2, 1, 0.000, '',      12,  0,  '',                    0.035, 0.380, 0, 0, 0.965, 0.380, 2, 0, 0, 0), -- line
(11, 1, 3, 0.120, 'Arial', 10,  42, '{0}',                 0.980, 0.500, 3, 3, 0.000, 0.000, 0, 0, 0, 0), -- class location
(11, 1, 3, 0.120, 'Arial', 10,  44, '{0} ({1:h\:mm tt})',  0.026, 0.500, 1, 3, 0.000, 0.000, 0, 0, 0, 0), -- class name and time
(11, 1, 1, 0.000, 'Arial', 10,  61, '{0:M/d/yy}',          0.965, 0.160, 3, 1, 0.000, 0.000, 0, 0, 0, 0), -- date
(11, 6, 1, 0.000, '',      0,   0,  '',                    1.000, 0.000, 3, 1, 0.850, 0.200, 0.667, 0.334, 1, 1), -- box for security code
(11, 1, 1, 0.000, 'Arial', 16,  1,  '{0}',                 0.965, 0.000, 3, 1, 0.000, 0.000, 0, 0, 1, 2), -- security code
(11, 1, 1, 0.000, 'Arial', 10,  4,  '{0}',                 0.500, 0.285, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- allergy
(11, 4, 1, 0.000, 'Arial', 10,  0,  'Extra',               0.500, 0.885, 2, 2, 0.000, 0.000, 0, 0, 0, 0), -- extra text

      -- NAME TAG 2"
(12, 1, 1, 0.000, 'Arial', 32,  2, '{0}',                  0.500, 0.520, 2, 3, 0.000, 0.000, 0, 0, 0, 0), -- first name
(12, 1, 1, 0.000, 'Arial', 24,  3, '{0}',                  0.500, 0.520, 2, 1, 0.000, 0.000, 0, 0, 0, 0)  -- last name
END
GO

-- keep track of default "system" wide check in labels so that we can change them later if need be in future migrations

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'system'
          AND Object_ID = Object_ID(N'dbo.CheckInLabel'))
BEGIN
    ALTER TABLE CheckInLabel
    ADD [system] BIT CONSTRAINT system_default DEFAULT 0 NOT NULL
END
GO

IF (select count(*) from CheckInLabel where [system] = 1) = 0
BEGIN
    UPDATE dbo.CheckInLabel
    SET [system] = 1
    WHERE id <= 12
END
GO
