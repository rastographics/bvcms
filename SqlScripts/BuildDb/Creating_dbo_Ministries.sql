CREATE TABLE [dbo].[Ministries]
(
[MinistryId] [int] NOT NULL IDENTITY(1, 1),
[MinistryName] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedBy] [int] NULL,
[CreatedDate] [datetime] NULL,
[ModifiedBy] [int] NULL,
[ModifiedDate] [datetime] NULL,
[RecordStatus] [bit] NULL,
[DepartmentId] [int] NULL,
[MinistryDescription] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MinistryContactId] [int] NULL,
[ChurchId] [int] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
