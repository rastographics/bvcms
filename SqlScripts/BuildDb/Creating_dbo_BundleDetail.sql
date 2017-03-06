CREATE TABLE [dbo].[BundleDetail]
(
[BundleDetailId] [int] NOT NULL IDENTITY(1, 1),
[BundleHeaderId] [int] NOT NULL,
[ContributionId] [int] NOT NULL,
[CreatedBy] [int] NOT NULL,
[CreatedDate] [datetime] NOT NULL,
[BundleSort1] [int] NULL,
[RefOrgId] [int] NULL,
[RefPeopleId] [int] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
