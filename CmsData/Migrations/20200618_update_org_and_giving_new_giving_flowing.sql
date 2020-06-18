--IF COL_LENGTH('dbo.Organizations', 'RedirectUrl') IS NOT NULL
--BEGIN
--    ALTER TABLE Organizations ADD RedirectUrl nvarchar(MAX);
--END
--IF COL_LENGTH('dbo.GivingPages', 'DefaultPage') IS NOT NULL
--BEGIN
--    ALTER TABLE GivingPages ADD DefaultPage bit;
--END
ALTER TABLE Organizations ADD RedirectUrl nvarchar(MAX);
ALTER TABLE GivingPages ADD DefaultPage bit;
