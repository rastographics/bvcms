CREATE PROCEDURE [dbo].[FetchCreateFund]
    (
      @name nvarchar(256)
    )
AS
BEGIN
	DECLARE @fid INT
	
	SELECT @fid = FundId
	FROM dbo.ContributionFund
	WHERE FundName = @name
	
	IF (@fid IS NULL)
	BEGIN
		SELECT @fid = MAX(FundId) + 1 FROM dbo.ContributionFund
		INSERT INTO dbo.ContributionFund
		        ( FundId,
		          CreatedBy ,
		          CreatedDate ,
		          FundName ,
		          FundDescription ,
		          FundStatusId ,
		          FundTypeId ,
		          FundPledgeFlag 
		        )
		VALUES  ( @fid,
		          1 , -- CreatedBy - int
		          GETDATE() , -- CreatedDate - datetime
		          @name , -- FundName - nvarchar(256)
		          @name , -- FundDescription - nvarchar(256)
		          1 , -- FundStatusId - int
		          1 , -- FundTypeId - int
		          0  -- FundPledgeFlag - bit
		        )
	END
	RETURN @fid
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
