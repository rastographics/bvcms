-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[VisitNumberSinceDate]
(	
	@dt DATETIME,
	@n INT
)
RETURNS TABLE
AS
RETURN
	SELECT * FROM
	(
		SELECT p.PeopleId, p.Name, a.MeetingDate ,RANK() OVER 
		    (PARTITION BY a.PeopleId ORDER BY a.MeetingDate) AS Rank
		FROM dbo.Attend a 
		JOIN People p ON a.PeopleId = p.PeopleId
	) tt
	WHERE tt.Rank = @n
	AND MeetingDate >= @dt
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
