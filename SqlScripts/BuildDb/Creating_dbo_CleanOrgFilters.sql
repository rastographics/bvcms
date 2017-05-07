
CREATE PROC [dbo].[CleanOrgFilters]
AS
    BEGIN
		-- get today at 4:00 AM
		DECLARE @lastdt DATETIME = (SELECT DATETIMEFROMPARTS(DATEPART(YEAR, GETDATE()), DATEPART(MONTH, GETDATE()), DATEPART(DAY, GETDATE()), 4, 0, 0, 0))
		-- This way, the procedure will only run at most once a day when the first org is visited that day

        DECLARE @t TABLE ( id UNIQUEIDENTIFIER )
        INSERT  @t ( id )
                SELECT  QueryId
                FROM    dbo.OrgFilter
                WHERE   LastUpdated < DATEADD(HOUR, -24, @lastdt)

        DELETE  dbo.OrgFilter
		WHERE EXISTS (SELECT NULL FROM @t WHERE id = QueryId)

        DELETE  dbo.Query
		WHERE EXISTS (SELECT NULL FROM @t WHERE id = QueryId)

        DELETE  dbo.TagPerson
        FROM    dbo.TagPerson tp
                JOIN dbo.Tag t ON t.Id = tp.Id AND t.TypeId = 3
		WHERE EXISTS (SELECT NULL FROM @t WHERE id = t.Name)

        DELETE  dbo.Tag
		WHERE EXISTS (SELECT NULL FROM @t WHERE id = Name)
        AND TypeId = 3

		-- Clean LongRunningOperation records too
        DELETE  dbo.LongRunningOperation
        WHERE   completed < DATEADD(HOUR, -24, @lastdt)

        BEGIN TRAN;
	        UPDATE  dbo.Setting WITH ( SERIALIZABLE )
	        SET     Setting = @lastdt
	        WHERE   Id = 'LastOrgFilterCleanup' AND [System] = 1;
	        IF @@rowcount = 0
	            BEGIN
	                INSERT  INTO dbo.Setting ( Id , Setting , [System] )
	                VALUES  ( 'LastOrgFilterCleanup' , @lastdt , 1 )
	            END;
        COMMIT TRAN;
    END;





GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
