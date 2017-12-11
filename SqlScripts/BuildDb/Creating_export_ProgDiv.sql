CREATE VIEW [export].[ProgDiv] AS 
SELECT 
	ProgId ,
    DivId
FROM dbo.ProgDiv
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
