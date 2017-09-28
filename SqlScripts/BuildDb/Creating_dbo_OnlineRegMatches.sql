CREATE FUNCTION [dbo].[OnlineRegMatches](@pid INT)
RETURNS 
@t TABLE 
(
	PeopleId INT,
	[First] NVARCHAR(100),
	[Last] NVARCHAR(100),
	[Nick] NVARCHAR(100),
	[Middle] NVARCHAR(100),
	[Maiden] NVARCHAR(100),
	[BMon] INT,
	[BDay] INT,
	[BYear] INT,
	Email VARCHAR(100),
	Member NVARCHAR(100)
)
AS
BEGIN

DECLARE @First NVARCHAR(100),
	@Last NVARCHAR(100),
	@Nick NVARCHAR(100),
	@Middle NVARCHAR(100),
	@Maiden NVARCHAR(100),
	@Birthdate DATETIME,
	@BDay INT,
	@BMon INT,
	@BYear INT,
	@Email VARCHAR(100),
	@Email2 VARCHAR(100),
	@Cell NVARCHAR(50),
	@Home NVARCHAR(50),
	@Member NVARCHAR(50)
    
		SELECT 
			@First = FirstName
			,@Nick = NickName
			,@Last = LastName
			,@Middle = MiddleName
			,@Maiden = MaidenName
			,@Birthdate = dbo.Birthday(@pid)
			,@BDay = BirthDay
			,@BMon = BirthMonth
			,@BYear = BirthYear
			,@Email = EmailAddress
			,@Email2 = EmailAddress2
			,@Cell = CellPhone
			,@Home = HomePhone
			,@Member = ms.Description
		FROM dbo.People
		JOIN lookup.MemberStatus ms ON ms.Id = People.MemberStatusId
		WHERE PeopleId = @pid

		;WITH finds AS (
			SELECT PeopleId FROM dbo.FindPerson(@First, @Last, @Birthdate, @Email, @Cell)
			UNION SELECT PeopleId FROM dbo.FindPerson(@Nick, @Last, @Birthdate, @Email, @Cell)
			UNION SELECT PeopleId FROM dbo.FindPerson(@First, @Last, @Birthdate, @Email2, @Cell)
			UNION SELECT PeopleId FROM dbo.FindPerson(@Nick, @Last, @Birthdate, @Email2, @Cell)
			UNION SELECT PeopleId FROM dbo.FindPerson(@First, @Last, @Birthdate, @Email, @Home)
			UNION SELECT PeopleId FROM dbo.FindPerson(@Nick, @Last, @Birthdate, @Email, @Home)
			UNION SELECT PeopleId FROM dbo.FindPerson(@First, @Last, @Birthdate, @Email2, @Home)
			UNION SELECT PeopleId FROM dbo.FindPerson(@Nick, @Last, @Birthdate, @Email2, @Home)
		)
		INSERT @t (   PeopleId ,
		                   First ,
		                   Last ,
		                   Nick ,
		                   Middle ,
						   Maiden,
		                   BMon ,
		                   BDay ,
		                   BYear ,
		                   Email ,
		                   Member
		               )

		SELECT f.PeopleId
			,p.FirstName
			,p.LastName
			,p.NickName
			,p.MiddleName
			,p.MaidenName
			,p.BirthMonth
			,p.BirthDay
			,p.BirthYear
			,p.EmailAddress
			,Member = ms.Description
		FROM finds f
		JOIN dbo.People p ON p.PeopleId = f.PeopleId
		JOIN lookup.MemberStatus ms ON ms.Id = p.MemberStatusId 
		WHERE f.PeopleId <> @pid
	
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
