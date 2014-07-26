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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
