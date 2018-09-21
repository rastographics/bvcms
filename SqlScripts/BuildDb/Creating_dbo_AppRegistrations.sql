CREATE VIEW [dbo].[AppRegistrations]
AS
SELECT        o.OrganizationId, o.RegistrationTitle AS Title, o.OrganizationName, o.Description, CASE WHEN ISNULL(o.AppCategory, '') <> '' THEN o.AppCategory ELSE 'Other' END AS AppCategory, o.PublicSortOrder, 
                         o.UseRegisterLink2, o.RegStart, o.RegEnd
FROM            dbo.Organizations AS o INNER JOIN
                         dbo.ActiveRegistrations ON dbo.ActiveRegistrations.OrganizationId = o.OrganizationId AND o.RegStart IS NOT NULL AND o.RegEnd > GETDATE() AND o.OrganizationStatusId = 30
WHERE        (ISNULL(o.AppCategory, '') <> 'InvitationOnly')
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
