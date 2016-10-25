CREATE FUNCTION [dbo].[ContactSummary](@dt1 DATETIME, @dt2 DATETIME, @min INT, @type INT, @reas INT)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		COUNT(*) [Count] 
		, ContactType
		, ReasonType
		, Ministry
		, Comments
		, ContactDate
		, Contactor
	FROM
	(
		SELECT 
			t.Description ContactType
			, r.Description ReasonType
			, m.MinistryName Ministry
			, CASE WHEN LEN(c.Comments) > 0 THEN '' ELSE 'No Comments' END Comments
			, CASE WHEN c.ContactDate IS NOT NULL THEN '' ELSE 'No Date' END ContactDate
			, CASE WHEN (SELECT COUNT(*) FROM dbo.Contactors) > 0 THEN '' ELSE 'No Contactor' END Contactor
		FROM dbo.Contact c
		left JOIN lookup.ContactType t ON c.ContactTypeId = t.Id
		left JOIN lookup.ContactReason r ON c.ContactReasonId = r.Id
		LEFT JOIN dbo.Ministries m ON c.MinistryId = m.MinistryId
		WHERE (@dt1 IS NULL OR c.ContactDate >= @dt1)
		AND (@dt2 IS NULL OR c.ContactDate <= @dt2)
		AND (@min = 0 OR @min = c.MinistryId)
		AND (@type = 0 OR @type = c.ContactTypeId)
		AND (@reas = 0 OR @reas = c.ContactReasonId)
	) tt
	GROUP BY  ContactType, ReasonType, Ministry, Comments, ContactDate, Contactor
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
