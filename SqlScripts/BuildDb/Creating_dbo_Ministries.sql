CREATE TABLE [dbo].[Ministries]
(
[MinistryId] [int] NOT NULL IDENTITY(1, 1),
[MinistryName] [nvarchar] (50) NULL,
[CreatedBy] [int] NULL,
[CreatedDate] [datetime] NULL,
[ModifiedBy] [int] NULL,
[ModifiedDate] [datetime] NULL,
[RecordStatus] [bit] NULL,
[DepartmentId] [int] NULL,
[MinistryDescription] [nvarchar] (512) NULL,
[MinistryContactId] [int] NULL,
[ChurchId] [int] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
