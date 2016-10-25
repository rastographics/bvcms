CREATE FUNCTION [dbo].[DropForeignKeys] ()
RETURNS varchar(MAX)
AS
BEGIN
	DECLARE @tsql VARCHAR(MAX)

	DECLARE @schema_name sysname;
	DECLARE @table_name sysname;
	DECLARE @constraint_name sysname;
	DECLARE @constraint_object_id INT;
	DECLARE @referenced_object_name sysname;
	DECLARE @is_disabled BIT;
	DECLARE @is_not_for_replication BIT;
	DECLARE @is_not_trusted BIT;
	DECLARE @delete_referential_action TINYINT;
	DECLARE @update_referential_action TINYINT;
	DECLARE @fkCol sysname;
	DECLARE @pkCol sysname;
	DECLARE @col1 BIT;
	DECLARE @referenced_schema_name sysname;

	SET @tsql = ''

	DECLARE FKcursor CURSOR
	FOR
	SELECT
	    OBJECT_SCHEMA_NAME(parent_object_id, DB_ID())
	   ,OBJECT_NAME(parent_object_id, DB_ID())
	   ,name
	   ,OBJECT_NAME(referenced_object_id, DB_ID())
	   ,object_id
	   ,is_disabled
	   ,is_not_for_replication
	   ,is_not_trusted
	   ,delete_referential_action
	   ,update_referential_action
	   ,OBJECT_SCHEMA_NAME(referenced_object_id, DB_ID())
	FROM
	    sys.foreign_keys
	ORDER BY
	    1
	   ,2;
	OPEN FKcursor;

	FETCH NEXT FROM FKcursor INTO @schema_name, @table_name, @constraint_name,
	    @referenced_object_name, @constraint_object_id, @is_disabled,
	    @is_not_for_replication, @is_not_trusted, @delete_referential_action,
	    @update_referential_action, @referenced_schema_name;
	WHILE @@FETCH_STATUS = 0
	BEGIN
        SET @tsql = @tsql + CHAR(13) + CHAR(10) + 'ALTER TABLE ' + QUOTENAME(@schema_name) + '.'

            + QUOTENAME(@table_name) + ' DROP CONSTRAINT '
            + QUOTENAME(@constraint_name) + ';';
	    FETCH NEXT FROM FKcursor INTO @schema_name, @table_name, @constraint_name,
	        @referenced_object_name, @constraint_object_id, @is_disabled,
	        @is_not_for_replication, @is_not_trusted, @delete_referential_action,
	        @update_referential_action, @referenced_schema_name;
	END
	CLOSE FKcursor;
	DEALLOCATE FKcursor;

	RETURN @tsql;

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
