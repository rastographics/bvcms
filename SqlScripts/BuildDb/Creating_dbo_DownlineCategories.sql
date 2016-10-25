CREATE FUNCTION [dbo].[DownlineCategories](@categoryId INT)
RETURNS 
@categories TABLE 
(
	rownum INT IDENTITY (1, 1) PRIMARY KEY NOT NULL, 
    id INT,
	name VARCHAR(50),
	mainfellowship BIT,
	programs VARCHAR(100),
	divisions VARCHAR(100)
)
AS
BEGIN

	DECLARE @x XML = CONVERT(XML, (SELECT Body FROM dbo.Content WHERE Name = 'DownlineCategories'))

	DECLARE @id INT, @mf BIT, @progs VARCHAR(100), @divs VARCHAR(100)
	DECLARE @n INT = 1, @maxrows INT

	;WITH cte AS
	(
	    SELECT
	        id = c.value('(@id)[1]', 'int')
	        ,name = c.value('(@name)[1]', 'varchar(50)')
	        ,mainfellowship = CAST(CASE WHEN c.value('(@mainfellowship)[1]', 'char(4)') = 'true' THEN 1 ELSE 0 END AS BIT)
	        ,programs = c.value('(@programs)[1]', 'varchar(100)')
	        ,divisions = c.value('(@divisions)[1]', 'varchar(100)')
	    FROM 
	        @x.nodes('/root/category') AS xd(c)
	)
	INSERT INTO @categories
	        ( id ,
	          name ,
	          mainfellowship ,
	          programs ,
	          divisions
	        )
	SELECT id, name, mainfellowship, programs, divisions
	FROM cte
	WHERE id = @categoryId OR @categoryId IS NULL
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
