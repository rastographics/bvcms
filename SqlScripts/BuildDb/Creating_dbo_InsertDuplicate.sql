-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertDuplicate](@i1 INT, @i2 INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    IF NOT EXISTS(SELECT NULL FROM dbo.Duplicate WHERE (id1 = @i1 AND id2 = @i2) OR (id1 = @i2 AND id2 = @i1))
		IF @i1 < @i2
			INSERT dbo.Duplicate
			        ( id1, id2 )
			VALUES  ( @i1, @i2 )
		ELSE
			INSERT dbo.Duplicate			
			        ( id1, id2 )
			VALUES  ( @i2, @i1 )
	
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
