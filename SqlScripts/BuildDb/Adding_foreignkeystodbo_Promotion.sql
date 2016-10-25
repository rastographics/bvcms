ALTER TABLE [dbo].[Promotion] WITH NOCHECK  ADD CONSTRAINT [FromPromotions__FromDivision] FOREIGN KEY ([FromDivId]) REFERENCES [dbo].[Division] ([Id])
GO
ALTER TABLE [dbo].[Promotion] WITH NOCHECK  ADD CONSTRAINT [ToPromotions__ToDivision] FOREIGN KEY ([ToDivId]) REFERENCES [dbo].[Division] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
