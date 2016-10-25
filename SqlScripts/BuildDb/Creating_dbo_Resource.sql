CREATE TABLE [dbo].[Resource]
(
[ResourceId] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Description] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreationDate] [datetime] NULL,
[UpdateDate] [datetime] NULL,
[PeopleId] [int] NULL,
[OrganizationId] [int] NULL,
[CampusId] [int] NULL,
[DivisionId] [int] NULL,
[MemberTypeIds] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DisplayOrder] [int] NULL,
[ResourceTypeId] [int] NOT NULL,
[ResourceCategoryId] [int] NOT NULL,
[OrganizationTypeId] [int] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
