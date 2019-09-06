DECLARE @dbname nvarchar(100);
DECLARE @s nvarchar(1000);

SELECT @dbname = SUBSTRING(db_name(), 5, 100);
SELECT @s = ('IF NOT EXISTS(SELECT 1 FROM CMSi_'+@dbname+'.INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = ''Image'' 
	AND COLUMN_NAME=''IsPublic'')
BEGIN
    ALTER TABLE CMSi_'+@dbname+'.dbo.Image
    ADD IsPublic BIT NOT NULL DEFAULT 0
END');

exec sp_executesql @statement = @s