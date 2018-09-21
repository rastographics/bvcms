CREATE VIEW [export].[XpAttendance] AS 
SELECT PeopleId ,
       MeetingId ,
       OrganizationId ,
       MeetingDate ,
       AttendanceFlag ,
       AttendanceType = aty.Description,
       MemberType = mt.Description
FROM dbo.Attend a
LEFT JOIN lookup.AttendType aty ON aty.Id = a.AttendanceTypeId
LEFT JOIN lookup.MemberType mt ON mt.Id = a.MemberTypeId
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
