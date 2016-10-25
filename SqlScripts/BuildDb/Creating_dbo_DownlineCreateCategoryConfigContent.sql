CREATE PROCEDURE [dbo].[DownlineCreateCategoryConfigContent]
AS
BEGIN
	DECLARE @body VARCHAR(MAX)
	IF NOT EXISTS(SELECT NULL FROM dbo.Content WHERE Name = 'DownlineCategories')
	BEGIN
		SET @body = 
'<root>
	<!--Note the category id="1" line, that specifies a single category which is the default.
		mainfellowship="true" will include any organization which is a main fellowship in the downline
		The other two lines are commented out 
		and show examples using programs and divisions to select the participating orgs.
	 -->
	<category id="1" name="Main Fellowships" mainfellowship="true" />
	<!--
	<category id="2" name="LifeGroups and DiscipleLife" programs="101,103" />
	<category id="3" name="Discipleship" divisions="6366" />
	-->
</root>'
		INSERT dbo.Content (Name, Title, Body, DateCreated, TypeID )
		VALUES('DownlineCategories', 'Downline Category XML Config', @body, GETDATE(), 1)
    END
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
