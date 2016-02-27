CREATE FUNCTION [dbo].[OrgDayStats] ( @oids VARCHAR(MAX), @dt DATETIME )
RETURNS @t TABLE
    (
     OrganizationId INT
    ,OrganizationName VARCHAR(200)
	,Leader NVARCHAR(80)
	,Location NVARCHAR(80)
	,MeetingTime DATETIME
	,Attends INT
	,Visitors INT
    ,Members INT
    ,NewMembers INT
    ,Dropped INT
    ,CurrentCount INT
    )
AS
BEGIN
    DECLARE @enddt1 DATETIME = DATEADD(DAY, 1, @dt);
    DECLARE @enddt7 DATETIME = DATEADD(DAY, 7, @dt);

    INSERT  @t
            (OrganizationId
            ,OrganizationName
			,Leader
			,Location
			,MeetingTime
			,Attends
			,Visitors
            ,Members
            ,NewMembers
            ,Dropped
            ,CurrentCount
            )
    SELECT  o.OrganizationId
           ,o.OrganizationName
		   ,o.LeaderName
		   ,m.Location
		   ,m.MeetingDate
		   ,Attends = (
				SELECT COUNT(*)
				FROM dbo.Attend a
				WHERE a.MeetingId = m.MeetingId
				AND a.AttendanceFlag = 1
				AND a.MeetingDate >= @dt
				AND a.MeetingDate < @enddt1
			)
		   ,Visitors = (
				SELECT COUNT(*)
				FROM dbo.Attend a
				WHERE a.MeetingId = m.MeetingId
				AND a.AttendanceFlag = 1
				AND a.MeetingDate >= @dt
				AND a.MeetingDate < @enddt1
				AND a.AttendanceTypeId IN (40,50,60)
			)
           ,Members = ( 
				SELECT  COUNT(*)
                FROM    dbo.EnrollmentTransaction et
                WHERE   et.TransactionTypeId <= 3
                AND et.TransactionStatus = 0
                AND et.TransactionDate <= @enddt1
                AND et.MemberTypeId <> 311
                AND ISNULL(et.Pending, 0) = 0
                AND ISNULL(et.NextTranChangeDate, GETDATE()) >= @dt
                AND et.OrganizationId = o.OrganizationId
            )
           ,NewMembers = ( 
				SELECT   COUNT(*)
                FROM     dbo.EnrollmentTransaction et
                WHERE    et.TransactionTypeId <= 3
                AND et.TransactionStatus = 0
                AND et.TransactionDate <= @enddt7
                AND et.TransactionDate > @dt
                AND et.MemberTypeId <> 311
                AND ISNULL(et.Pending, 0) = 0
                AND ISNULL(et.NextTranChangeDate, GETDATE()) >= @dt
                AND et.OrganizationId = o.OrganizationId
			)
           ,Dropped = ( 
				SELECT  COUNT(*)
                FROM    dbo.EnrollmentTransaction et
                WHERE   et.TransactionTypeId > 3
                AND et.TransactionStatus = 0
                AND et.TransactionDate <= @enddt7
                AND et.TransactionDate > @dt
                AND et.MemberTypeId <> 311
                AND ISNULL(et.Pending, 0) = 0
                AND et.OrganizationId = o.OrganizationId
            )
           ,CurrentCount = o.MemberCount
    FROM dbo.SplitInts(@oids) ids
    JOIN dbo.Organizations o ON o.OrganizationId = ids.Value
	JOIN dbo.Meetings m ON m.OrganizationId = o.OrganizationId
	WHERE CONVERT(DATE, m.MeetingDate) = @dt
    ORDER BY o.OrganizationId;
    RETURN; 
END;
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
