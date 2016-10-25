
CREATE FUNCTION [dbo].[ActivityLogSearch]
(
	@machine VARCHAR(30),
	@activity VARCHAR(100),
	@userid INT,
	@orgid INT,
	@peopleid INT,
	@enddate DATETIME,
	@lookback INT,
	@pagesize INT,
	@pagenum INT
)
RETURNS 
@t TABLE 
(
	Machine VARCHAR(50),
	[date] DATETIME,
	UserId INT,
	UserName NVARCHAR(50),
	Activity NVARCHAR(200),
	OrgName NVARCHAR(100),
	OrgId INT,
	PeopleId INT,
	PersonName NVARCHAR(139),
	DatumId INT,
	MaxRows INT
)
AS
BEGIN
	SET @enddate = ISNULL(@enddate, GETDATE())
	SET @lookback = ISNULL(@lookback, 90)
	;WITH results AS (
		SELECT 
			Machine,
			ActivityDate,
			a.UserId,
			u.Username,
			Activity,
			OrgId,
			o.OrganizationName,
			a.PeopleId,
			p.Name2,
			rd.Id DatumId
		FROM dbo.ActivityLog a
		LEFT JOIN dbo.Organizations o ON o.OrganizationId = a.OrgId
		LEFT JOIN dbo.People p ON p.PeopleId = a.PeopleId
		LEFT JOIN dbo.Users u ON u.UserId = a.UserId
		LEFT JOIN dbo.RegistrationData rd ON rd.Id = a.DatumId
		WHERE 1 = 1
		AND a.ActivityDate >= DATEADD(DAY, -@lookback, @enddate)
		AND a.ActivityDate <= @enddate
		AND (@machine IS NULL OR @machine = a.Machine)
		AND (@userid IS NULL OR @userid = a.UserId)
		AND (@orgid IS NULL OR @orgid = a.OrgId)
		AND (@peopleid IS NULL OR @peopleid = a.PeopleId)
		AND (@activity IS NULL OR a.Activity LIKE '%' + @activity + '%')
	), TempCount AS (
	    SELECT COUNT(*) AS MaxRows FROM results
	)
	INSERT @t
	        ( machine ,
	          [date] ,
	          userid ,
	          username ,
	          activity ,
	          orgname ,
	          orgid ,
	          peopleid ,
	          personname ,
			  DatumId,
	          maxrows
	        )
	SELECT 
		r.Machine,
		r.ActivityDate,
		r.UserId,
		r.Username,
		r.Activity,
		r.OrganizationName,
		r.OrgId,
		r.PeopleId,
		r.Name2,
		r.DatumId,
		c.MaxRows
	FROM results r, TempCount c
	ORDER BY r.ActivityDate DESC
    OFFSET (@pagenum-1)*@pagesize ROWS
	FETCH NEXT @pagesize ROWS ONLY
	
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
