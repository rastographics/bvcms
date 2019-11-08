GO
--Update Married flag for all Married Marital Status
UPDATE lookup.MaritalStatus
SET Married = 1
WHERE Id = 20 -- Married
GO

--Update families and people spouse ids

UPDATE Families
SET HeadOfHouseholdSpouseId = dbo.SpouseId(HeadOfHouseholdId)
WHERE HeadOfHouseholdSpouseId IS NULL
GO
UPDATE People
SET SpouseId = dbo.SpouseId(PeopleId)
WHERE SpouseId IS NULL