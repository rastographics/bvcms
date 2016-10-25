-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ShowTableSizes]
AS
BEGIN
CREATE TABLE #temp (
       table_name sysname ,
       row_count int,
       reserved_size nvarchar(50),
       data_size nvarchar(50),
       index_size nvarchar(50),
       unused_size nvarchar(50))
SET NOCOUNT ON
INSERT     #temp
EXEC       sp_msforeachtable 'sp_spaceused ''?'''
SELECT     b.table_schema as owner,
		   a.table_name,
           a.row_count,
           count(*) as col_count,
           a.data_size
FROM       #temp a
INNER JOIN information_schema.columns b
           ON a.table_name collate database_default
                = b.table_name collate database_default
GROUP BY   b.table_schema, a.table_name, a.row_count, a.data_size
ORDER BY   a.row_count desc
DROP TABLE #temp
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
