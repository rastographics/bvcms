CREATE TABLE [dbo].[UploadPeopleRun]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[meetingid] [int] NULL,
[started] [datetime] NULL,
[count] [int] NULL,
[processed] [int] NULL,
[completed] [datetime] NULL,
[error] [nvarchar] (200) NULL,
[running] AS (case  when [completed] IS NULL AND [error] IS NULL AND datediff(minute,[started],getdate())<(120) then (1) else (0) end),
[description] [nvarchar] (100) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
