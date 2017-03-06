CREATE TABLE [dbo].[Meetings]
(
[MeetingId] [int] NOT NULL IDENTITY(1, 1),
[CreatedBy] [int] NOT NULL,
[CreatedDate] [datetime] NOT NULL,
[OrganizationId] [int] NOT NULL,
[NumPresent] [int] NOT NULL CONSTRAINT [DF__MEETINGS___NUM_P__4D4B3A2F] DEFAULT ((0)),
[NumMembers] [int] NOT NULL CONSTRAINT [DF__MEETINGS___NUM_M__4F3382A1] DEFAULT ((0)),
[NumVstMembers] [int] NOT NULL CONSTRAINT [DF__MEETINGS___NUM_V__5027A6DA] DEFAULT ((0)),
[NumRepeatVst] [int] NOT NULL CONSTRAINT [DF__MEETINGS___NUM_R__511BCB13] DEFAULT ((0)),
[NumNewVisit] [int] NOT NULL CONSTRAINT [DF__MEETINGS___NUM_N__520FEF4C] DEFAULT ((0)),
[Location] [nvarchar] (200) NULL,
[MeetingDate] [datetime] NULL,
[GroupMeetingFlag] [bit] NOT NULL CONSTRAINT [DF__MEETINGS___GROUP__5AA5354D] DEFAULT ((0)),
[Description] [nvarchar] (100) NULL,
[NumOutTown] [int] NULL,
[NumOtherAttends] [int] NULL,
[AttendCreditId] [int] NULL,
[ScheduleId] AS ((datepart(weekday,[MeetingDate])*(10000)+(datepart(hour,[MeetingDate])*(100)))+datepart(minute,[MeetingDate])),
[NoAutoAbsents] [bit] NULL,
[HeadCount] [int] NULL,
[MaxCount] AS (case  when [HeadCount]>[NumPresent] then [HeadCount] else [NumPresent] end)
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
