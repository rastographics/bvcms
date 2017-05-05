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
[OrganizationTypeId] [int] NULL,
[StatusFlagIds] [nvarchar] (max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
