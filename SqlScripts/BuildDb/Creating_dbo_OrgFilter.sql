CREATE TABLE [dbo].[OrgFilter]
(
[QueryId] [uniqueidentifier] NOT NULL,
[Id] [int] NOT NULL,
[GroupSelect] [varchar] (50) NULL,
[FirstName] [varchar] (50) NULL,
[LastName] [varchar] (50) NULL,
[SgFilter] [varchar] (900) NULL,
[ShowHidden] [bit] NULL,
[FilterIndividuals] [bit] NULL,
[FilterTag] [bit] NULL,
[TagId] [int] NULL,
[LastUpdated] [datetime] NOT NULL,
[UserId] [int] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
