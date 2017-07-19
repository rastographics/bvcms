
CREATE PROCEDURE [dbo].[RecentAbsents1]
    (
     @orgid INT
    ,@otherorgfilterid INT = NULL
	,@queryid UNIQUEIDENTIFIER = NULL
    )
AS
    BEGIN
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
                WHERE   ac.OrganizationId = @orgid
                        AND ( ISNULL(@otherorgfilterid, 0) = 0
                              OR EXISTS ( SELECT    NULL
                                          FROM      dbo.OrganizationMembers
                                          WHERE     OrganizationId = @otherorgfilterid
                                                    AND PeopleId = ac.PeopleId )
                            );

        WITH    weekattends
                  AS ( SELECT   PeopleId
                               ,COUNT(*) consecutive
                               ,OtherAttends = SUM(a.OtherAttends)
                       FROM     #t a
                       WHERE    a.WeekDate > ( SELECT   MAX(WeekDate)
                                               FROM     #t
                                               WHERE    Attended = 1
                                                        AND a.PeopleId = PeopleId
                                               GROUP BY PeopleId
                                             )
                       GROUP BY PeopleId
                     )
            SELECT  @orgid OrganizationId
                   ,consecutive
                   ,om.PeopleId
                   ,p.Name2
                   ,p.HomePhone
                   ,p.CellPhone
                   ,p.EmailAddress
				   ,tt1.OtherAttends
                   ,lastattend = ( SELECT   MAX(mm.MeetingDate)
                                   FROM     Attend
                                            JOIN dbo.Meetings mm ON dbo.Attend.MeetingId = mm.MeetingId
                                   WHERE    AttendanceFlag = 1
                                            AND mm.OrganizationId = o.OrganizationId
                                            AND Attend.PeopleId = om.PeopleId
                                 )
            FROM    weekattends tt1
                    JOIN OrganizationMembers om ON @orgid = om.OrganizationId
                                                   AND tt1.PeopleId = om.PeopleId
                    JOIN dbo.People p ON om.PeopleId = p.PeopleId
                    JOIN Organizations o ON om.OrganizationId = o.OrganizationId
                    JOIN lookup.MemberType mt ON om.MemberTypeId = mt.Id
                    JOIN lookup.AttendType at ON at.Id = mt.AttendanceTypeId
            WHERE   o.OrganizationId = @orgid
                    AND consecutive >= ISNULL(o.ConsecutiveAbsentsThreshold, 2)
                    AND at.Id NOT IN ( 70, 100 ) --inservice and homebound
                    AND om.MemberTypeId NOT IN ( 230, 310 ) --inactive
                    AND ( ISNULL(@otherorgfilterid, 0) = 0
                          OR EXISTS ( SELECT    NULL
                                      FROM      dbo.OrganizationMembers
                                      WHERE     OrganizationId = @otherorgfilterid
                                                AND PeopleId = p.PeopleId )
                        )
                    AND ( @queryid IS NULL 
                          OR EXISTS ( SELECT    NULL
                                      FROM      dbo.OrgFilterIds(@queryid) fi
                                      WHERE     fi.PeopleId = om.PeopleId )
                        )
            ORDER BY o.OrganizationName
                   ,o.OrganizationId
                   ,consecutive
                   ,p.Name2;
    END;


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
