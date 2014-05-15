CREATE TABLE [dbo].[Users]
(
[UserId] [int] NOT NULL IDENTITY(1, 1),
[PeopleId] [int] NULL,
[Username] [nvarchar] (50) NOT NULL,
[Comment] [nvarchar] (255) NULL,
[Password] [nvarchar] (128) NOT NULL,
[PasswordQuestion] [nvarchar] (255) NULL,
[PasswordAnswer] [nvarchar] (255) NULL,
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
[CurrentCart] [nvarchar] (100) NULL,
[MustChangePassword] [bit] NOT NULL CONSTRAINT [DF_Users_MustChangePassword] DEFAULT ((0)),
[Host] [nvarchar] (100) NULL,
[TempPassword] [nvarchar] (128) NULL,
[Name] [nvarchar] (50) NULL,
[Name2] [nvarchar] (50) NULL,
[ResetPasswordCode] [uniqueidentifier] NULL,
[DefaultGroup] [nvarchar] (50) NULL,
[ResetPasswordExpires] [datetime] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
