


CREATE FUNCTION [dbo].[OrgFilterPeople2]
(
	 @queryid UNIQUEIDENTIFIER
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
	, MemberCode NVARCHAR(20)
	, MemberType NVARCHAR(100)
	, Hidden BIT
	, Groups NVARCHAR(MAX)
)
AS
BEGIN
	DECLARE @grouptype VARCHAR(25)
	DECLARE @oid INT
	DECLARE @showhidden BIT
	SELECT  
		@grouptype = GroupSelect, 
		@oid = Id,
		@showhidden = ShowHidden
	FROM dbo.OrgFilter WHERE QueryId = @queryid

	IF @grouptype LIKE '%10%'
		INSERT INTO @t (PeopleId, TAB, GroupCode, AttPct, LastAttended, Joined, Dropped, InactiveDate, MemberCode, MemberType, Hidden, Groups)
		SELECT * FROM dbo.OrgFilterCurrent(@oid) 

	IF @grouptype LIKE '%20%'
		INSERT INTO @t (PeopleId, TAB, GroupCode, AttPct, LastAttended, Joined, Dropped, InactiveDate, MemberCode, MemberType, Hidden, Groups)
		SELECT * FROM dbo.OrgFilterInactive(@oid) 

	IF @grouptype LIKE '%30%'
		INSERT INTO @t (PeopleId, TAB, GroupCode, AttPct, LastAttended, Joined, Dropped, InactiveDate, MemberCode, MemberType, Hidden, Groups)
		SELECT * FROM dbo.OrgFilterPending(@oid) 

	IF @grouptype LIKE '%40%'
		INSERT INTO @t (PeopleId, TAB, GroupCode, AttPct, LastAttended, Joined, Dropped, InactiveDate, MemberCode, MemberType, Hidden, Groups)
		SELECT * FROM dbo.OrgFilterProspects(@oid, @showhidden) 

	IF @grouptype LIKE '%50%'
		INSERT INTO @t (PeopleId, TAB, GroupCode, AttPct, LastAttended, Joined, Dropped, InactiveDate, MemberCode, MemberType, Hidden, Groups)
		SELECT * FROM dbo.OrgFilterPrevious(@oid)

	IF @grouptype LIKE '%60%'
		INSERT INTO @t (PeopleId, TAB, GroupCode, AttPct, LastAttended, Joined, Dropped, InactiveDate, MemberCode, MemberType, Hidden, Groups)
		SELECT * FROM dbo.OrgFilterGuests(@oid, @showhidden) 

	RETURN
END






GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
