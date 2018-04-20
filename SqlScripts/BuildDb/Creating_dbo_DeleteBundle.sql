CREATE PROCEDURE [dbo].[DeleteBundle] @id INT
AS
BEGIN
	SET NOCOUNT ON;

	delete dbo.Contribution
	from contribution c
	join dbo.BundleDetail d on d.ContributionId = c.ContributionId
	where d.BundleHeaderId = @id

	delete dbo.BundleDetail
	where BundleHeaderId = @id

	delete dbo.BundleHeader
	where BundleHeaderId = @id

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
