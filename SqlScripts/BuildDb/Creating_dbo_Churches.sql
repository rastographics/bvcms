
CREATE VIEW [dbo].[Churches]
AS
SELECT c FROM (
SELECT OtherNewChurch c FROM dbo.People
UNION 
SELECT OtherPreviousChurch c FROM dbo.People
) AS t
GROUP BY c

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
