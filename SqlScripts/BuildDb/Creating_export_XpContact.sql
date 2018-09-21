CREATE VIEW [export].[XpContact] AS 
SELECT 
	c.ContactId ,
    ContactType = ct.[Description] ,
    c.ContactDate ,
    ContactReason = r.[Description],
    Ministry = m.MinistryName ,
    c.NotAtHome ,
    c.LeftDoorHanger ,
    c.LeftMessage ,
    c.GospelShared ,
    c.PrayerRequest ,
    c.ContactMade ,
    c.GiftBagGiven ,
    c.Comments ,
    c.OrganizationId
FROM dbo.Contact c
LEFT JOIN lookup.ContactType ct ON ct.Id = c.ContactTypeId
LEFT JOIN dbo.Ministries m ON m.MinistryId = c.MinistryId
LEFT JOIN lookup.ContactReason r ON r.Id = c.ContactReasonId
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
