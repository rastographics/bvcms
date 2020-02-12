IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'FamilyId'
          AND Object_ID = Object_ID(N'dbo.CheckInPending'))
BEGIN
    ALTER TABLE dbo.CheckInPending
    ADD FamilyId INT DEFAULT 0 NOT NULL,
    PeopleId INT DEFAULT 0 NOT NULL

    ALTER TABLE dbo.CheckInPending ADD CONSTRAINT
    FK_CheckInPending_People FOREIGN KEY (PeopleId) REFERENCES dbo.People (PeopleId)
    ON DELETE CASCADE
    
    ALTER TABLE dbo.CheckInPending ADD CONSTRAINT
    FK_CheckInPending_Families FOREIGN KEY (FamilyId) REFERENCES dbo.Families (FamilyId)
    ON DELETE CASCADE
END
GO
