CREATE PROCEDURE [dbo].[sp_who3]
AS
BEGIN
DECLARE @sp_who2 TABLE 
( 
    SPID INT, 
    Status nvarchar(255) NULL, 
    Login SYSNAME NULL, 
    HostName SYSNAME NULL, 
    BlkBy SYSNAME NULL, 
    DBName SYSNAME NULL, 
    Command nvarchar(255) NULL, 
    CPUTime INT NULL, 
    DiskIO INT NULL, 
    LastBatch nvarchar(255) NULL, 
    ProgramName nvarchar(255) NULL, 
    SPID2 INT,
    REQUESTID INT
) 
 
INSERT @sp_who2 EXEC sp_who2 
 
SELECT * FROM @sp_who2
    WHERE DbName LIKE 'CMS_%' AND Status <> 'sleeping' AND Command <> 'CHECKPOINT'	AND ProgramName NOT LIKE 'Microsoft%'	
    RETURN
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
