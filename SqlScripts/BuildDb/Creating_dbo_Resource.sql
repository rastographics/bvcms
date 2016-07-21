CREATE TABLE [dbo].[Resource]
(
[ResourceId] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (100) NULL,
[Description] [nvarchar] (max) NULL,
[CreationDate] [datetime] NULL,
[UpdateDate] [datetime] NULL,
[PeopleId] [int] NULL,
[OrganizationId] [int] NULL,
[CampusId] [int] NULL,
[DivisionId] [int] NULL,
[MemberTypeIds] [nvarchar] (50) NULL,
[DisplayOrder] [int] NULL,
[ResourceTypeId] [int] NOT NULL,
[ResourceCategoryId] [int] NOT NULL,
[OrganizationTypeId] [int] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
