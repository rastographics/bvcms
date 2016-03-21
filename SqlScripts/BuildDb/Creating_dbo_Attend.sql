CREATE TABLE [dbo].[Attend]
(
[PeopleId] [int] NOT NULL,
[MeetingId] [int] NOT NULL,
[OrganizationId] [int] NOT NULL,
[MeetingDate] [datetime] NOT NULL,
[AttendanceFlag] [bit] NOT NULL CONSTRAINT [DF_Attend_AttendanceFlag] DEFAULT ((0)),
[OtherOrgId] [int] NULL,
[AttendanceTypeId] [int] NULL,
[CreatedBy] [int] NULL,
[CreatedDate] [datetime] NULL,
[MemberTypeId] [int] NOT NULL,
[AttendId] [int] NOT NULL IDENTITY(1, 1),
[OtherAttends] [int] NOT NULL CONSTRAINT [DF_Attend_OtherAttends] DEFAULT ((0)),
[BFCAttendance] [bit] NULL,
[Registered] [bit] NULL,
[SeqNo] [int] NULL,
[Commitment] [int] NULL,
[NoShow] [bit] NULL,
[EffAttendFlag] AS (CONVERT([bit],case when [AttendanceFlag]=(1) then (1) when [AttendanceTypeId]=(90) then NULL when [AttendanceTypeId]=(70) AND [OtherAttends]>(0) then (1) when [OtherAttends]>(0) AND [BFCAttendance]=(1) then NULL when [AttendanceFlag]=(1) then (1) when [OtherAttends]>(0) then NULL else (0) end,(0)))
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
