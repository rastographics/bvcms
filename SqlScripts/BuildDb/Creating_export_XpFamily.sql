CREATE VIEW [export].[XpFamily] AS 
SELECT FamilyId ,
       AddressFromDate ,
       AddressToDate ,
       AddressLineOne ,
       AddressLineTwo ,
       CityName ,
       StateCode ,
       ZipCode ,
       CountryName ,
       ResCode = rc.Description ,
       HomePhone ,
       HeadOfHouseholdId ,
       HeadOfHouseholdSpouseId ,
       CoupleFlag ,
       Comments 
FROM dbo.Families
LEFT JOIN lookup.ResidentCode rc ON rc.Id = Families.ResCodeId
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
