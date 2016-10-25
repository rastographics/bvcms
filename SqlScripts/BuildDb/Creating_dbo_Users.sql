CREATE TABLE [dbo].[Users]
(
[UserId] [int] NOT NULL IDENTITY(1, 1),
[PeopleId] [int] NULL,
[Username] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Comment] [nvarchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Password] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[PasswordQuestion] [nvarchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[PasswordAnswer] [nvarchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[IsApproved] [bit] NOT NULL CONSTRAINT [DF_Users_IsApproved] DEFAULT ((0)),
[LastActivityDate] [datetime] NULL,
[LastLoginDate] [datetime] NULL,
[LastPasswordChangedDate] [datetime] NULL,
[CreationDate] [datetime] NULL,
[IsLockedOut] [bit] NOT NULL CONSTRAINT [DF_Users_IsLockedOut] DEFAULT ((0)),
[LastLockedOutDate] [datetime] NULL,
[FailedPasswordAttemptCount] [int] NOT NULL CONSTRAINT [DF_Users_FailedPasswordAttemptCount] DEFAULT ((0)),
[FailedPasswordAttemptWindowStart] [datetime] NULL,
[FailedPasswordAnswerAttemptCount] [int] NOT NULL CONSTRAINT [DF_Users_FailedPasswordAnswerAttemptCount] DEFAULT ((0)),
[FailedPasswordAnswerAttemptWindowStart] [datetime] NULL,
[EmailAddress] AS ([dbo].[UEmail]([PeopleId])),
[ItemsInGrid] [int] NULL,
[CurrentCart] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MustChangePassword] [bit] NOT NULL CONSTRAINT [DF_Users_MustChangePassword] DEFAULT ((0)),
[Host] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[TempPassword] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Name2] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ResetPasswordCode] [uniqueidentifier] NULL,
[DefaultGroup] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ResetPasswordExpires] [datetime] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
