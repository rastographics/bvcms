CREATE TABLE [dbo].[VolRequest]
(
[MeetingId] [int] NOT NULL,
[RequestorId] [int] NOT NULL,
[Requested] [datetime] NOT NULL,
[VolunteerId] [int] NOT NULL,
[Responded] [datetime] NULL,
[CanVol] [bit] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
