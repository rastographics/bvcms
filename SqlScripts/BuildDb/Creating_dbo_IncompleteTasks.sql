
CREATE VIEW [dbo].[IncompleteTasks]
AS
SELECT        TOP (100) PERCENT t.CreatedOn, o.Name AS Owner, co.Name AS DelegatedTo, t.Description, t.DeclineReason, ab.Name AS About, t.Notes, ts.Description AS Status, 
                         t.ForceCompleteWContact, t.Id, t.OwnerId, t.CoOwnerId, t.WhoId, t.StatusId, t.SourceContactId, t.Due, p.SmallId AS AboutPictureId, p.X AS PictureX, 
                         p.Y AS PictureY
FROM            dbo.Task AS t INNER JOIN
                         lookup.TaskStatus AS ts ON ts.Id = t.StatusId INNER JOIN
                         dbo.People AS o ON o.PeopleId = t.OwnerId LEFT OUTER JOIN
                         dbo.People AS co ON co.PeopleId = t.CoOwnerId LEFT OUTER JOIN
                         dbo.People AS ab ON ab.PeopleId = t.WhoId LEFT OUTER JOIN
                         dbo.Picture AS p ON p.PictureId = ab.PictureId
WHERE        (t.StatusId <> 40) AND (t.CreatedOn >= DATEADD(MONTH, - 12, GETDATE())) AND (t.Description <> 'New Person Data Entry') AND (LEN(t.Description) > 0)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
