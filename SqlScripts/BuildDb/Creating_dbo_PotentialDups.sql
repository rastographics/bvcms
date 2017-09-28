SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE FUNCTION [dbo].[PotentialDups](@pid INT)
RETURNS TABLE 
AS
RETURN 
(
	WITH persons AS (
		SELECT 
			p.PeopleId
			,p.FamilyId
			,FirstName
			,nick = ISNULL(NULLIF(p.NickName, ''), '--')
			,maid = ISNULL(NULLIF(p.MaidenName, ''), '--')
			,p.LastName
			,bda = ISNULL(p.BirthDay, -1)
			,bmn = ISNULL(p.BirthMonth, -1)
			,byr = ISNULL(p.BirthYear, -1)
			,p.BirthDay
			,p.BirthMonth
			,p.BirthYear
			,email = ISNULL(p.EmailAddress, '--')
			,Street = dbo.GetStreet(p.PrimaryAddress)
			,Cell = ISNULL(NULLIF(p.CellPhoneLU, ''), '--')
			,Work = ISNULL(NULLIF(p.WorkPhoneLU, ''), '--')
			,Home = ISNULL(NULLIF(f.HomePhoneLU, ''), '--')
		FROM dbo.People p
		JOIN dbo.Families f ON f.FamilyId = p.FamilyId
		WHERE p.PeopleId = @pid
	),
	nonulls AS (
		SELECT PeopleId
			,p.FamilyId
			,p.FirstName
			,p.LastName
			,nick = ISNULL(p.NickName, '')
			,middle = ISNULL(p.MiddleName, '')
			,maid = ISNULL(p.MaidenName, '')
			,bda = ISNULL(p.BirthDay, -2) 
			,bmn = ISNULL(p.BirthMonth, -2) 
			,byr = ISNULL(p.BirthYear, -2) 
			,p.BirthDay
			,p.BirthMonth
			,p.BirthYear
			,email = ISNULL(p.EmailAddress, '---')
			,Addr = p.AddressLineOne
			,FAddr = f.AddressLineOne
			,p.CellPhoneLU
			,f.HomePhoneLU
			,p.WorkPhoneLU
		FROM dbo.People p
		JOIN dbo.Families f ON f.FamilyId = p.FamilyId
	),
	matchs AS (
		SELECT 
			PeopleId0 = m.PeopleId
			,p.PeopleId
			,FirstMatch = IIF(p.FirstName = m.FirstName 
							OR p.nick = m.FirstName
							OR p.middle = m.FirstName
							OR p.FirstName = m.nick
							OR p.nick = m.nick
							OR p.middle = m.nick,
							1, 0)
			,LastMatch = IIF(p.LastName = m.LastName 
							OR p.maid = m.LastName
							OR p.maid = m.maid
							OR p.LastName = m.maid,
							1, 0)
			,NoBDay = IIF((p.BirthMonth IS NULL AND p.BirthYear IS NULL AND p.BirthDay IS NULL)
							OR (m.BirthMonth IS NULL AND m.BirthYear IS NULL AND m.BirthDay IS NULL),
							1, 0)
			,BdMatch = IIF(p.bda = m.bda AND p.bmn = m.bmn AND p.byr = m.byr, 1, 0)
			,BdMatchPart = IIF(p.bda = m.bda AND p.bmn = m.bmn, 1, 0)
			,EmailMatch = IIF(p.email = m.email, 1, 0)
			,AddrMatch = IIF(p.Addr LIKE '%' + m.Street+ '%' 
						OR p.FAddr LIKE '%' + m.Street + '%' , 1, 0)
			,PhoneMatch = 0 /*IIF(p.CellPhoneLU = m.Cell
							OR p.CellPhoneLU = m.Home
							OR p.CellPhoneLU = m.Work
							OR p.HomePhoneLU = m.Cell
							OR p.HomePhoneLU = m.Home
							OR p.HomePhoneLU = m.Work
							OR p.WorkPhoneLU = m.Cell
							OR p.WorkPhoneLU = m.Home
							OR p.WorkPhoneLU = m.Work, 1, 0) */
			,SameFamily = IIF(p.FamilyId = m.FamilyId, 1, 0)
		FROM persons m
		JOIN nonulls p ON p.PeopleId <> m.PeopleId
	),
	matchs2 AS (
		SELECT 
			p.PeopleId0
			,p.PeopleId 
			,p.FirstMatch
			,p.LastMatch
			,p.BdMatchPart
			,S0 = CAST(IIF((p.LastMatch=1 AND p.SameFamily=0 
							AND (p.FirstMatch + p.BdMatch + p.EmailMatch + p.PhoneMatch + p.AddrMatch) >= 3)
						--OR (p.FirstMatch=1 AND p.LastMatch=1 AND p.BdMatchPart=1) , 0, 1) AS BIT)
						, 1, 0) AS BIT)
			,S1 = CAST(IIF(p.FirstMatch=1 AND p.BdMatchPart=1, 1, 0) AS BIT)
			,S2 = CAST(IIF(p.FirstMatch=1 AND p.BdMatch=1, 1, 0) AS BIT)
			,S3 = CAST(IIF(p.FirstMatch=1 AND p.LastMatch=1 AND p.NoBDay=1, 1, 0) AS BIT)
			,S4 = CAST(IIF(p.FirstMatch=1 AND p.AddrMatch=1, 1, 0) AS BIT)
			,S5 = CAST(IIF(p.FirstMatch=1 AND p.EmailMatch=1, 1, 0) AS BIT)
			,S6 = CAST(IIF(p.LastMatch=1 AND p.BdMatch=1, 1, 0) AS BIT)
		FROM matchs p
	)
	SELECT 
		PeopleId0
		,p.PeopleId
		,S0, S1, S2, S3, S4, S5, S6
		,[First] = p.FirstName
		,[Last] = p.LastName
		,Nick = p.NickName
		,Middle = p.MiddleName
		,Maiden = p.MaidenName
		,BMon = p.BirthMonth
		,BDay = p.BirthDay
		,BYear = p.BirthYear
		,Email = p.EmailAddress
		,FamAddr = f.AddressLineOne
		,PerAddr = p.AddressLineOne
		,[Member] = ms.[Description]
	FROM persons m
	JOIN matchs2 pp ON pp.PeopleId0 = m.PeopleId
	JOIN dbo.People p ON p.PeopleId = pp.PeopleId
	JOIN dbo.Families f ON f.FamilyId = p.FamilyId
	JOIN lookup.MemberStatus ms ON ms.Id = p.MemberStatusId
	WHERE S0=1 OR S1=1 OR S2=1 OR S3=1 OR S4=1 OR S5=1 OR S6=1
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
