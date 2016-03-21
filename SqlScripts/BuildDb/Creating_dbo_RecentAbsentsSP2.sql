CREATE PROCEDURE [dbo].[RecentAbsentsSP2] ( @orgs VARCHAR(MAX) )
AS
    BEGIN

        SET NOCOUNT ON;

        DECLARE @to TABLE
            (
             orgid INT PRIMARY KEY
            ,cathreshhold INT
            ,orgname NVARCHAR(100)
            ,leader NVARCHAR(70)
            );

        INSERT  @to
                SELECT  OrganizationId
                       ,ConsecutiveAbsentsThreshold
                       ,OrganizationName
                       ,LeaderName
                FROM    Organizations o
                        JOIN SplitInts(@orgs) i ON i.Value = o.OrganizationId;

        CREATE TABLE #t
            (
             OrganizationId INT
            ,PeopleId INT
            ,Attended BIT
            ,WeekDate DATE
            ,OtherAttends INT
            );
        INSERT  #t
                SELECT  ac.OrganizationId
                       ,ac.PeopleId
                       ,ac.Attended
                       ,ac.WeekDate
                       ,ac.OtherAttends
                FROM    dbo.AttendCredits ac
                        JOIN SplitInts(@orgs) i ON i.Value = ac.OrganizationId;

        WITH    omConsecutive
                  AS ( SELECT   OrganizationId
                               ,PeopleId
                               ,COUNT(*) consecutive
							   ,OtherAttends = SUM(a.OtherAttends)
                       FROM     #t a
                       WHERE    a.WeekDate > ( SELECT   MAX(WeekDate)
                                               FROM     #t
                                               WHERE    Attended = 1
                                                        AND a.PeopleId = PeopleId
                                                        AND a.OrganizationId = OrganizationId
                                               GROUP BY PeopleId
                                             )
                                AND EXISTS ( SELECT NULL
                                             FROM   #t
                                             WHERE  Attended = 1
                                                    AND WeekDate = a.WeekDate
                                                    AND OrganizationId = a.OrganizationId )
                       GROUP BY OrganizationId
                               ,PeopleId
                     )
            SELECT  o.OrganizationId
                   ,o.OrganizationName
                   ,o.LeaderName
                   ,ISNULL(o.ConsecutiveAbsentsThreshold, 2) ConsecutiveAbsentsThreshold
                   ,consecutive
                   ,om.PeopleId
                   ,p.Name2
                   ,p.HomePhone
                   ,p.CellPhone
                   ,p.EmailAddress
				   ,OtherAttends
                   ,( SELECT    MAX(mm.MeetingDate)
                      FROM      Attend
                                JOIN dbo.Meetings mm ON dbo.Attend.MeetingId = mm.MeetingId
                      WHERE     AttendanceFlag = 1
                                AND mm.OrganizationId = o.OrganizationId
                                AND Attend.PeopleId = om.PeopleId
                    ) lastattend
                   ,( SELECT    MAX(MeetingDate)
                      FROM      dbo.Attend
                      WHERE     AttendanceFlag = 1
                                AND OrganizationId = omc.OrganizationId
                    ) lastmeeting
            FROM    omConsecutive omc
                    JOIN OrganizationMembers om ON omc.OrganizationId = om.OrganizationId
                                                   AND omc.PeopleId = om.PeopleId
                    JOIN dbo.People p ON om.PeopleId = p.PeopleId
                    JOIN Organizations o ON om.OrganizationId = o.OrganizationId
                    JOIN lookup.MemberType mt ON om.MemberTypeId = mt.Id
                    JOIN lookup.AttendType at ON at.Id = mt.AttendanceTypeId
            WHERE   consecutive >= ISNULL(o.ConsecutiveAbsentsThreshold, 2)
                    AND at.Id NOT IN ( 70, 100 ) --inservice and homebound
                    AND om.MemberTypeId NOT IN ( 230, 310 ) --inactive
            ORDER BY o.OrganizationName
                   ,o.OrganizationId
                   ,consecutive
                   ,p.Name2;
        DROP TABLE #t;
    END;
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
