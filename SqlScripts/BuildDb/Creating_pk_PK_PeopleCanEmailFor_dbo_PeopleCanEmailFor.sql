ALTER TABLE [dbo].[PeopleCanEmailFor] ADD CONSTRAINT [PK_PeopleCanEmailFor] PRIMARY KEY CLUSTERED  ([CanEmail], [OnBehalfOf])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
