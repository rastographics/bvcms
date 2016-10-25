CREATE TABLE [dbo].[SubRequest]
(
[AttendId] [int] NOT NULL,
[RequestorId] [int] NOT NULL,
[Requested] [datetime] NOT NULL,
[SubstituteId] [int] NOT NULL,
[Responded] [datetime] NULL,
[CanSub] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
