
CREATE PROC [dbo].[CleanOrgFilters]
AS
    BEGIN
        DECLARE @t TABLE ( id UNIQUEIDENTIFIER )
        INSERT  @t ( id )
                SELECT  QueryId
                FROM    dbo.OrgFilter
                WHERE   LastUpdated < DATEADD(HOUR, -24, GETDATE())

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

        DELETE  dbo.LongRunningOperation
        WHERE   completed < DATEADD(HOUR, -24, GETDATE())

        BEGIN TRAN;
	        UPDATE  dbo.Setting WITH ( SERIALIZABLE )
	        SET     Setting = GETDATE()
	        WHERE   Id = 'LastOrgFilterCleanup' AND [System] = 1;
	        IF @@rowcount = 0
	            BEGIN
	                INSERT  INTO dbo.Setting ( Id , Setting , [System] )
	                VALUES  ( 'LastOrgFilterCleanup' , GETDATE() , 1 )
	            END;
        COMMIT TRAN;
    END;





GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
