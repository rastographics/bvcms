-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeletePeopleExtras](@field nvarchar)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

DELETE dbo.PeopleExtra
WHERE Field = @field

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
