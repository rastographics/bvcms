-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[RenumberWords]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.words
	SET n = nn
	FROM words w JOIN 
	(SELECT word, ROW_NUMBER() OVER (ORDER BY word) AS nn FROM dbo.words) AS tt
	ON w.word = tt.word
	END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
