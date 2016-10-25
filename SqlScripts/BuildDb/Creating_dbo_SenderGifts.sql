CREATE FUNCTION [dbo].[SenderGifts](@oids VARCHAR(MAX))
RETURNS TABLE
AS RETURN
(
	SELECT 
		gs.OrgId
		,Trip = o.OrganizationName
		,SenderId = gs.SupporterId
		,Sender = s.Name2
		,GoerId = gs.GoerId
		,Goer = g.Name2
		,DateGiven = gs.Created
		,Amt = gs.Amount
		,CASE 
			WHEN gs.NoNoticeToGoer = 1 THEN 'no notice'
			WHEN gs.NoNoticeToGoer = 0 THEN 'sent'
			ELSE ''
		END NoticeSent
	FROM dbo.GoerSenderAmounts gs
	JOIN dbo.Organizations o ON o.OrganizationId = gs.OrgId
	JOIN dbo.SplitInts(@oids) i ON i.Value = gs.OrgId
	JOIN dbo.People s ON s.PeopleId = gs.SupporterId
	LEFT JOIN dbo.People g ON g.PeopleId = gs.GoerId
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
