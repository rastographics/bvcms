CREATE TABLE [dbo].[PeopleCanEmailFor]
(
[CanEmail] [int] NOT NULL,
[OnBehalfOf] [int] NOT NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
