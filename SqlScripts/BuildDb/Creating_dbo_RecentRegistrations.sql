-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[RecentRegistrations] ( @days INT )
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
SELECT MIN(stamp) dt1, MAX(stamp) dt2, COUNT(*) cnt, o.OrganizationId, o.OrganizationName, completed FROM 
(
	SELECT 
		 d.Data
		,d.Stamp
		,d.completed
		,d.OrganizationId
	FROM dbo.RegistrationData d
	WHERE Stamp > DATEADD(dd, -@days, GETDATE())
	AND completed = 1
) tt
JOIN dbo.Organizations o ON tt.OrganizationId = o.OrganizationId
WHERE o.OrganizationId IS NOT NULL
GROUP BY o.OrganizationId, o.OrganizationName, tt.completed
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
