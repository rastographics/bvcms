CREATE FUNCTION [dbo].[GetCurrentOnlinePledgeBundle] (@next DATETIME, @prev DATETIME)
RETURNS INT 
AS 
BEGIN
    
    DECLARE @id INT

	SELECT TOP 1 @id = BundleHeaderId 
	FROM dbo.BundleHeader
	WHERE BundleHeaderTypeId = 5
	AND BundleStatusId = 1
	AND ContributionDate >= @prev
	AND ContributionDate < @next
	ORDER BY ContributionDate DESC
    
	RETURN @id

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
