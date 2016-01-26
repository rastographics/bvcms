-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[AttendMemberTypeAsOf]
(	
	@from DATETIME,
	@to DATETIME,
	@progid INT,
	@divid INT,
	@orgid INT,
	@ids nvarchar(4000),
	@notids NVARCHAR(4000)
)
RETURNS @t TABLE ( PeopleId INT, MemberTypeId INT, MemberType VARCHAR(100) )
AS
BEGIN
	INSERT INTO @t (PeopleId, MemberTypeId, MemberType) 
		SELECT PeopleId, MemberTypeId, mt.Description
		FROM dbo.Attend a
		LEFT JOIN lookup.MemberType mt ON mt.Id = a.MemberTypeId
		WHERE a.AttendanceFlag = 1
		AND (a.MemberTypeId IN (SELECT id FROM CsvTable(@ids)) OR @ids IS NULL OR @ids = '')
		AND a.MemberTypeId NOT IN (SELECT id FROM CsvTable(@notids))
		AND a.MeetingDate >= @from
		AND a.MeetingDate < @to
		AND (a.OrganizationId = @orgid OR @orgid = 0)
		AND (EXISTS(SELECT NULL FROM DivOrg d1
				WHERE d1.OrgId = a.OrganizationId
				AND d1.DivId = @divid) OR @divid = 0)
		AND (EXISTS(SELECT NULL FROM DivOrg d2
				WHERE d2.OrgId = a.OrganizationId
				AND EXISTS(SELECT NULL FROM Division d
						WHERE d2.DivId = d.Id
						AND d.ProgId = @progid)) OR @progid = 0)
	RETURN
END

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
