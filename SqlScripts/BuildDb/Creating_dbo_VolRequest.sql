CREATE TABLE [dbo].[VolRequest]
(
[MeetingId] [int] NOT NULL,
[RequestorId] [int] NOT NULL,
[Requested] [datetime] NOT NULL,
[VolunteerId] [int] NOT NULL,
[Responded] [datetime] NULL,
[CanVol] [bit] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
