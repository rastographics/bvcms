IF object_id('CHAIDonationsReport2') IS NULL
BEGIN
EXEC ('
	CREATE PROCEDURE [dbo].[CHAIDonationsReport2] (
	 @fund int,
	 @authorizedFundIds varchar(max)
	)
	AS 
	BEGIN
	 declare @authorizedFunds table (
	   FundId int
	 )

	 insert into @authorizedFunds
	 select value from dbo.SplitInts(@authorizedFundIds)

	 SELECT
	   c.ContributionId AS donations_id,
	   CONVERT(varchar, c.ContributionDate, 20) AS donations_created_at,
	   CONVERT(varchar, c.ModifiedDate, 20) AS donations_updated_at,
	   ct.Description AS donations_transactions_type,
	   c.ContributionAmount AS donations_amount,
	   c.PeopleId AS members_id,
	   REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(p.TitleCode, ''.'', ''''), '''''''', ''''), ''"'', ''''), '','', ''''), ''-'', '''') AS members_salutation,
	   (CASE
	     WHEN ISNULL(p.FirstName, '''') = '''' THEN ''X''
	     ELSE REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(p.FirstName, ''.'', ''''), '''''''', ''''), ''"'', ''''), '','', ''''), ''-'', '''')
	   END) AS members_first_name,
	   (CASE
	     WHEN ISNULL(p.LastName, '''') = '''' THEN ''Y''
	     ELSE REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(p.LastName, ''.'', ''''), '''''''', ''''), ''"'', ''''), '','', ''''), ''-'', '''')
	   END) AS members_last_name,
	   REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(fp.Description, ''.'', ''''), '''''''', ''''), ''"'', ''''), '','', ''''), ''-'', '''') AS members_family_role,
	   REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(p.PrimaryAddress, ''.'', ''''), '''''''', ''''), ''"'', ''''), '','', ''''), ''-'', '''') AS members_street_1,
	   REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(p.PrimaryAddress2, ''.'', ''''), '''''''', ''''), ''"'', ''''), '','', ''''), ''-'', '''') AS members_street_2,
	   p.PrimaryCity AS members_city,
	   p.PrimaryState AS members_state,
	   REPLACE(p.PrimaryZip, ''-'', '''') AS members_postal_code,
	   REPLACE(REPLACE(REPLACE(COALESCE(p.CellPhone, p.HomePhone, p.WorkPhone), ''('', ''''), '')'', ''''), ''-'', '''') AS members_phone,
	   REPLACE(REPLACE(REPLACE(COALESCE(p.EmailAddress, p.EmailAddress2), '''''''', ''''), ''"'', ''''), '','', '''') AS members_email,
	   (CASE
	     WHEN p.BDate IS NULL THEN ''1900-01-01''
	     ELSE CONVERT(varchar(10), p.BDate, 126)
	   END) AS members_dob,
	   CASE
	     WHEN p.PositionInFamilyId = 1 THEN 1
	     ELSE 0
	   END AS members_head_of_household,
	   CONVERT(varchar, p.CreatedDate, 20) AS members_created_at,
	   CONVERT(varchar, p.ModifiedDate, 20) AS members_updated_at,
	   p.FamilyId AS families_id,
	   CONVERT(varchar, f.CreatedDate, 20) AS families_created_at,
	   CONVERT(varchar, f.ModifiedDate, 20) AS families_updated_at,
	   c.FundId AS fund_id,
	   cf.FundName AS fund_name
	 FROM dbo.Contribution c
	   JOIN dbo.People p ON p.PeopleId = c.PeopleId
	   INNER JOIN Families f ON p.FamilyId = f.FamilyId
	   INNER JOIN lookup.FamilyPosition fp ON p.PositionInFamilyId = fp.Id
	   INNER JOIN lookup.ContributionType ct ON c.ContributionTypeId = ct.Id
	   INNER JOIN dbo.contributionfund cf ON c.Fundid = cf.FundId
	    INNER JOIN @authorizedFunds af ON af.FundId = cf.FundId
	 WHERE ISNULL(c.PledgeFlag, 0) = 0
	 AND c.ContributionStatusId = 0
	 AND c.ContributionTypeId NOT IN (6, 7, 8, 9)
	 AND (c.FundId = @fund OR @fund = 0)
	 AND CAST(c.ContributionDate AS date) >= CAST(DATEADD(YEAR, -5, GETDATE()) AS date)
	 ORDER BY c.ContributionDate

	END')
END