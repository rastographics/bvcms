-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteQueryBitTags]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DELETE dbo.TagPerson
	FROM dbo.TagPerson tp
	JOIN dbo.Tag t ON tp.Id = t.Id
	WHERE t.TypeId = 100
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
