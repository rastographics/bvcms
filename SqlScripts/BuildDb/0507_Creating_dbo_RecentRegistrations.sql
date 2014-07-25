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
		CONVERT(XML, Data) xdata
		, d.Stamp
		, d.completed
	FROM dbo.ExtraData d
	WHERE Stamp > DATEADD(dd, -@days, GETDATE())
	AND Data LIKE '%<item>CompleteRegistration<%'
) tt
JOIN dbo.Organizations o ON  xdata.value('(/OnlineRegModel/orgid)[1]', 'int') = o.OrganizationId
WHERE o.OrganizationId IS NOT NULL
GROUP BY o.OrganizationId, o.OrganizationName, tt.completed
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
