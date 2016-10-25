CREATE PROCEDURE [dbo].[CreateSubRequests](@tagid INT, @pids VARCHAR(MAX), @attendid INT, @requestorid INT, @requested DATETIME)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT dbo.TagPerson ( Id, PeopleId )
	SELECT @tagid, Value 
	FROM dbo.SplitInts(@pids)
	WHERE NOT EXISTS(
		SELECT NULL FROM .SubRequest sr 
		WHERE sr.AttendId = @attendid 
		AND sr.RequestorId = @requestorid 
		AND sr.Requested = @requested
		AND sr.SubstituteId = Value
	)

	INSERT dbo.SubRequest
	        ( AttendId ,
	          RequestorId ,
	          Requested ,
	          SubstituteId 
	        )
	SELECT @attendid, @requestorid, @requested, t.PeopleId
	FROM dbo.TagPerson t
	WHERE t.Id = @tagid

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
