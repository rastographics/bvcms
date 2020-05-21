ALTER TABLE CheckInLabelEntry 
DROP CONSTRAINT fieldFormat_default;
ALTER TABLE CheckInLabelEntry
ALTER COLUMN fieldFormat NVARCHAR(MAX) NOT NULL;
ALTER TABLE CheckInLabelEntry
ADD CONSTRAINT fieldFormat_default 
DEFAULT '' for fieldFormat;

-- add some label options
IF (select count(*) from CheckInLabel where [name] like 'Standard with birthday badge') = 0
BEGIN

    INSERT INTO [dbo].[CheckInLabel]
           ([typeID],[name],[minimum],[maximum],[system],[active])
     VALUES
           (1,'Standard with birthday badge',150,249,1,0),
           (6,'Standard with background check/ministry safe',150,249,1,0)
END
GO

IF (select count(*) from CheckInLabel l join CheckInLabelEntry e on e.labelID = l.id where [name] like 'Standard with birthday badge') = 0
BEGIN

    DECLARE @id INT = (select top 1 id from CheckInLabel
                        where [name] like 'Standard with birthday badge'
                        and [typeID] = 1
                        and [minimum] = 150
                        order by id desc)

    INSERT INTO [dbo].[CheckInLabelEntry]
           ([labelID],[typeID],[repeat],[offset],[font],[fontSize],[fieldID],[fieldFormat],[startX],[startY],[alignX],[alignY],[endX],[endY],[width],[height],[invert],[order],[orgEV],[personFlag])
     VALUES
           
      -- MAIN 2" WITH BIRTHDAY BADGE
(@id, 1, 1, 0.000, 'Arial', 20,  2,  '{0}',                 0.026, 0.000, 1, 1, 0.000, 0.000, 0, 0, 0, 0, NULL, NULL), -- first name
(@id, 1, 1, 0.000, 'Arial', 14,  3,  '{0}',                 0.026, 0.140, 1, 1, 0.000, 0.000, 0, 0, 0, 0, NULL, NULL), -- last name
(@id, 1, 1, 0.000, 'Arial', 10,  5,  '{0}{1}{2}',           0.500, 0.200, 2, 2, 0.000, 0.000, 0, 0, 1, 4, NULL, NULL), -- A/C/T
(@id, 6, 1, 0.000, '',      0,   5,  '{0}{1}{2}',           0.500, 0.215, 2, 2, 0.000, 0.000, 0.30, 0.170, 1, 3, NULL, NULL), -- A/C/T box
(@id, 2, 1, 0.000, '',      12,  0,  '',                    0.035, 0.380, 0, 0, 0.965, 0.380, 2, 0, 0, 0, NULL, NULL), -- line
(@id, 1, 3, 0.120, 'Arial', 10,  42, '{0}',                 0.965, 0.500, 3, 3, 0.000, 0.000, 0, 0, 0, 0, NULL, NULL), -- class location
(@id, 1, 3, 0.120, 'Arial', 10,  44, '{0} ({1:h\:mm tt})',  0.026, 0.500, 1, 3, 0.000, 0.000, 0, 0, 0, 0, NULL, NULL), -- class name and time
(@id, 1, 1, 0.000, 'Arial', 10,  61, '{0:M/d/yy}',          0.965, 0.160, 3, 1, 0.000, 0.000, 0, 0, 0, 0, NULL, NULL), -- date
(@id, 6, 1, 0.000, '',      0,   1,  '{0}',                 1.000, 0.000, 3, 1, 0.850, 0.200, 0.60, 0.334, 1, 1, NULL, NULL), -- security code box
(@id, 1, 1, 0.000, 'Arial', 16,  1,  '{0}',                 0.965, 0.000, 3, 1, 0.000, 0.000, 0, 0, 1, 2, NULL, NULL), -- security code
(@id, 5, 3, 0.000, '',      0,   0,  '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',0.960, 0.255, 3, 1, 0.000, 0.000, 0.200, 0.200, 0, 0, 'ShowBirthdayOnLabel', 'F80'), -- birthday graphic

     -- NAMETAG 2" WITH BACKGROUND CHECK/MINISTRY SAFE
(@id + 1, 1, 1, 0.000, 'Arial', 32,  2, '{0}',                  0.500, 0.520, 2, 3, 0.000, 0.000, 0, 0, 0, 0, NULL, NULL), -- first name
(@id + 1, 1, 1, 0.000, 'Arial', 24,  3, '{0}',                  0.500, 0.520, 2, 1, 0.000, 0.000, 0, 0, 0, 0, NULL, NULL),  -- last name
(@id + 1, 5, 1, 0.000, '',      0,   0, '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', 0.865, 0.875, 2, 1, 0.000, 0.000, 0.200, 0.200, 0, 0, 'minsitrysafeonlabel', 'F82'),  -- heart
(@id + 1, 5, 1, 0.000, '',      0,   0, 'iVBORw0KGgoAAAANSUhEUgAAAIAAAACACAQAAABpN6lAAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QAAKqNIzIAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAAHdElNRQfkBAkOJRrWhh7pAAADIElEQVR42u2cu2sUURSHv80iiEpQSCtoIf4JQWxsBF+VRSwUS1OI2mlpYwoLLeysVMTCQmsRjEUqK1MEFLHYLoXKqghBXfdaDAubsI+Z+5hz7uz53Xrmnu87Z4Zl77JgsVgsFovFYrFYLJbZS0u6gKQ5zGmOsECXDq/ZkC6n3izyBrdtveeMdFF1pcUt/u3AL9ZTdksXlz5zPB4JX6xV9koXKInvcKyxT7pISfxGKyiH31gFczwpie9wvGWPdMGx8ct2f7BeNOlTULXuD9Y16bLj4VftfrG+MC9dehx8n+4Xa7ktXX0E/Edc9r7aSZcfju83/IPVkQYIxfcf/mJtSSOE4Yd13+H4Kg0Rgh/afYdjXRrDHz+8+w7HPWkQX/wY3Xc4jkuj+OHH6b7jlTSKH36s7n/nqDSMJP5fzkrD+ODHGv4eF6VhDN/wDd/wDd/wDd/wDd/wDd/wDd/wDV8Kf9wB8S4WOcQ8XT6yTj0HSGGHXMPpcYnn/pcf5CHdIZubrLC/Bvx4X3ZdCCnkClsjbvqNU7OA3+L+hKfqavPxH0y8eZ8bifBVvPmn4adSkBF+CgWZ4cdWoAQfblfarM/1SPgqXn1wjn7FDWMoUIPf5oPHpqEPgprhhyXPjUOmQE33AZ55b+47BYq6D/A5oACfKVDVfYBfQUVUnQJl3Qf4GVhIFQUK8eFTcDFlHwR1w18kRk/KTIHK7gOcj1LUtClQ2n2ANhuRFIyfArXdL3KSXtIpUNz9QW5GKnCUggzwAe5EU7D9QVA+/KkVZISfQkFm+LEVZIgPcDeagneR7pT01ZdyCjLsvj4FYie8OhSIHnDLKxA/35dVII4vq0AFvpwCNfgyClTh169AHX69ClTi16dALX49ClTjp1egHh9gZbbx0ynIBj+Ngqzw4yvIDj+ugizx4ynIFj+OgqzxwxVkjx+moBH4/goag++noFH41RU0Dr+agkbil1fQWPxyChqNP11B4/EnK5gJ/PEKZgYfRv2+4E/d5/vSWebHEH6HE9IFTU6KP5deYIljHGCTVV7yWxrRYrFYLBaLxWKxWCyWnfkPGjtWWtczAH8AAAAldEVYdGRhdGU6Y3JlYXRlADIwMjAtMDQtMDlUMTQ6Mzc6MjYrMDA6MDA/8qERAAAAJXRFWHRkYXRlOm1vZGlmeQAyMDIwLTA0LTA5VDE0OjM3OjI2KzAwOjAwTq8ZrQAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAAASUVORK5CYII=', 0.950, 0.875, 2, 1, 0.000, 0.000, 0.200, 0.200, 0, 0, 'backgroundcheckonlabel', 'F81')  -- check mark

END
GO
