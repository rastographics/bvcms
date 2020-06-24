ALTER TABLE Organizations ADD RedirectUrl nvarchar(MAX);
ALTER TABLE GivingPages ADD DefaultPage bit;
ALTER TABLE GivingPages ADD CONSTRAINT FK_GivingPages_Campus FOREIGN KEY (CampusId) REFERENCES lookup.Campus(Id);
ALTER TABLE GivingPages ADD MainCampusPageFlag bit;
