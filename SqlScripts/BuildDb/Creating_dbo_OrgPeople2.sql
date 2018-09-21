CREATE FUNCTION [dbo].[OrgPeople2]
(
	 @oid INT
	,@grouptype VARCHAR(20)
	,@first VARCHAR(30)
	,@last VARCHAR(30)
	,@sgfilter VARCHAR(300)
	,@showhidden BIT
	,@currtag NVARCHAR(300)
	,@currtagowner INT
	,@filterchecked BIT
	,@filtertag BIT
	,@ministryinfo BIT
	,@userpeopleid INT
)
RETURNS 
@t TABLE 
(
	PeopleId INT
	, TAB VARCHAR(30)
	, GroupCode CHAR(2)
	, AttPct REAL
	, LastAttended DATETIME
	, Joined DATETIME
	, Dropped DATETIME
	, InactiveDate DATETIME
	, MemberCode nvarchar(20)
	, MemberType nvarchar(100)
	, Hidden bit
	, Groups nvarchar(max)
	, Grade INT  
)
AS
BEGIN
	IF @grouptype LIKE '%10%'
		insert into @t (PeopleId, TAB, GroupCode, AttPct, LastAttended, Joined, Dropped, InactiveDate, MemberCode, MemberType, Hidden, Groups, Grade)
		SELECT * FROM dbo.OrgPeopleCurrent(@oid) 

	IF @grouptype LIKE '%20%'
		INSERT INTO @t (PeopleId, TAB, GroupCode, AttPct, LastAttended, Joined, Dropped, InactiveDate, MemberCode, MemberType, Hidden, Groups, Grade)
		SELECT * FROM dbo.OrgPeopleInactive(@oid) 

	IF @grouptype LIKE '%30%'
		insert into @t (PeopleId, TAB, GroupCode, AttPct, LastAttended, Joined, Dropped, InactiveDate, MemberCode, MemberType, Hidden, Groups, Grade)
		SELECT * FROM dbo.OrgPeoplePending(@oid) 

	IF @grouptype LIKE '%40%'
		INSERT INTO @t (PeopleId, TAB, GroupCode, AttPct, LastAttended, Joined, Dropped, InactiveDate, MemberCode, MemberType, Hidden, Groups, Grade)
		SELECT * FROM dbo.OrgPeopleProspects(@oid, @showhidden) 

	IF @grouptype LIKE '%50%'
		INSERT INTO @t (PeopleId, TAB, GroupCode, AttPct, LastAttended, Joined, Dropped, InactiveDate, MemberCode, MemberType, Hidden, Groups, Grade)
		SELECT * FROM dbo.OrgPeoplePrevious(@oid)

	IF @grouptype LIKE '%60%'
		INSERT INTO @t (PeopleId, TAB, GroupCode, AttPct, LastAttended, Joined, Dropped, InactiveDate, MemberCode, MemberType, Hidden, Groups, Grade)
		SELECT * FROM dbo.OrgPeopleGuests(@oid, @showhidden) 

	RETURN
END





GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
