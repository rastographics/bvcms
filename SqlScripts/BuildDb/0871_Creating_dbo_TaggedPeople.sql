-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[TaggedPeople](@tagid INT) 
RETURNS @t TABLE ( PeopleId INT)
AS
BEGIN
	INSERT INTO @t (PeopleId)
	SELECT p.PeopleId FROM dbo.People p
	WHERE EXISTS(
    SELECT NULL
    FROM dbo.TagPerson t
    WHERE (t.Id = @tagid) AND (t.PeopleId = p.PeopleId))
    RETURN
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
