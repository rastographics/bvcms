CREATE TABLE [dbo].[OrgSchedule]
(
[OrganizationId] [int] NOT NULL,
[Id] [int] NOT NULL,
[ScheduleId] [int] NULL,
[SchedTime] [datetime] NULL,
[SchedDay] [int] NULL,
[MeetingTime] [datetime] NULL,
[AttendCreditId] [int] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
