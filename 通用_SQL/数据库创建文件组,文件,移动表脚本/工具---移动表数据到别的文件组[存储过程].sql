USE TestHekaton
IF OBJECT_ID('sp_MoveTable') IS NOT NULL
    DROP PROC sp_MoveTable
GO
/*移动表数据到别的文件组 2008-12-29*/
CREATE PROC sp_MoveTable
    (
      @objectname sysname ,
      @NewFileGroup sysname = NULL
    )
AS
    SET NOCOUNT ON
    DECLARE @objectid INT

    SELECT  @objectid = object_id ,
            @objectname = name
    FROM    sys.objects AS a
    WHERE   name = @objectname
            AND Type = 'U'
            AND is_ms_shipped = 0
            AND NOT EXISTS ( SELECT 1
                             FROM   sys.extended_properties
                             WHERE  major_id = a.object_id
                                    AND minor_id = 0
                                    AND class = 1
                                    AND name = N'microsoft_database_tools_support' )
    IF @objectid IS NULL
        BEGIN
            
            RAISERROR (50001, -- Message id.
           10,    -- Severity,
           1,     -- State,
           N'无效的表名'); -- First argument supplies the string.
            RETURN
        END

    IF FILEGROUP_ID(@NewFileGroup) IS NULL
        AND @NewFileGroup > ''
        BEGIN
           
            RAISERROR (50001, -- Message id.
           10,    -- Severity,
           1,     -- State,
           N'错误的文件组'); -- First argument supplies the string.
            RETURN
        END

    IF @NewFileGroup IS NULL
        SELECT  @NewFileGroup = name
        FROM    sys.filegroups
        WHERE   is_default = 1 

    IF EXISTS ( SELECT  1
                FROM    sys.indexes AS a
                        INNER JOIN sys.filegroups AS b ON b.data_space_id = a.data_space_id
                WHERE   a.object_id = @objectid
                        AND b.name = @NewFileGroup
                        AND ( a.type = 0
                              OR is_primary_key = 1
                            ) )
        BEGIN
            PRINT N'表' + @objectname + N'已在文件组' + @NewFileGroup + N' .不需要移动! '
            RETURN
        END

    DECLARE @sql NVARCHAR(4000) ,
        @Enter NVARCHAR(20) ,
        @PrimaryKey sysname

    SELECT  @sql = '' ,
            @Enter = CHAR(13) + CHAR(10)

--删除主键、外键、索引
    SELECT  @sql = @sql + 'Alter Table '
            + QUOTENAME(OBJECT_NAME(a.parent_object_id)) + ' Drop Constraint '
            + QUOTENAME(a.name) + @Enter
    FROM    sys.Foreign_keys AS a
    WHERE   a.referenced_object_id = @objectid

    SELECT  @sql = @sql
            + CASE WHEN b.object_id IS NOT NULL
                   THEN 'Alter Table ' + QUOTENAME(@objectname)
                        + ' Drop Constraint ' + QUOTENAME(a.name)
                        + CASE b.Type
                            WHEN 'PK'
                            THEN ' With (Move To ' + QUOTENAME(@NewFileGroup)
                                 + ')'
                            ELSE ''
                          END
                   ELSE 'Drop Index ' + QUOTENAME(a.name) + '.'
                        + QUOTENAME(@objectname)
              END + @Enter
    FROM    sys.indexes AS a
            LEFT OUTER JOIN sys.objects AS b ON b.parent_object_id = a.object_id
                                                AND b.Type IN ( 'PK', 'UQ' )
                                                AND b.name = a.name
    WHERE   a.object_id = @objectid
            AND a.name IS NOT NULL


    IF NOT EXISTS ( SELECT  *
                    FROM    sys.indexes
                    WHERE   object_id = @objectid
                            AND is_primary_key = 1 )
        BEGIN
            SET @PrimaryKey = 'ID' + REPLACE(NEWID(), '-', '')
--创建主键（在表没有主键的情况）
            SET @sql = @sql + 'Alter Table ' + QUOTENAME(@objectname)
                + ' Add ' + @PrimaryKey
                + ' uniqueidentifier Not Null ,Constraint DF_' + @objectname
                + '_' + @PrimaryKey + ' Default(newid()) For ' + @PrimaryKey
                + '' + ',Constraint PK_' + @objectname + '_' + @PrimaryKey
                + ' Primary Key (' + @PrimaryKey + ' Asc)' + @Enter 
--删除主键
            SET @sql = @sql + 'Alter Table ' + QUOTENAME(@objectname)
                + ' Drop Constraint PK_' + @objectname + '_' + @PrimaryKey
                + ' With (Move To ' + QUOTENAME(@NewFileGroup) + ')' + @Enter
            SET @sql = @sql + 'Alter Table ' + QUOTENAME(@objectname)
                + ' Drop Constraint DF_' + @objectname + '_' + @PrimaryKey
                + @Enter
            SET @sql = @sql + 'Alter Table ' + QUOTENAME(@objectname)
                + ' Drop Column ' + @PrimaryKey + @Enter
        END

--创建主键、外键、索引
    SELECT  @sql = @sql
            + CASE WHEN b.object_id IS NOT NULL
                   THEN 'Alter Table ' + QUOTENAME(@objectname)
                        + ' Add Constraint ' + QUOTENAME(a.name)
                        + CASE a.is_primary_key
                            WHEN 1 THEN ' Primary Key '
                            ELSE 'Unique '
                          END + '(' + c.x + ')'
                   ELSE 'Create Index ' + CASE a.is_unique
                                            WHEN 1 THEN 'Unique '
                                            ELSE ''
                                          END + CASE a.type
                                                  WHEN 1 THEN 'Clustered '
                                                  ELSE ''
                                                END + QUOTENAME(a.name)
                        + ' On ' + QUOTENAME(@objectname) + '(' + c.x + ')'
                        + ISNULL(' Include(' + d.x + ')', '')
              END + @Enter
    FROM    sys.indexes AS a
            LEFT OUTER JOIN sys.objects AS b ON b.parent_object_id = a.object_id
                                                AND b.Type IN ( 'PK', 'UQ' )
                                                AND b.name = a.name
            OUTER APPLY ( SELECT    x = STUFF(( SELECT  ',' + QUOTENAME(y.name)
                                                        + CASE x.is_descending_key
                                                            WHEN 1
                                                            THEN ' Desc'
                                                            ELSE ' Asc'
                                                          END
                                                FROM    sys.index_columns AS x
                                                        INNER JOIN sys.columns
                                                        AS y ON y.object_id = x.object_id
                                                              AND x.column_id = y.column_id
                                                WHERE   x.object_id = a.object_id
                                                        AND x.index_id = a.index_id
                                                        AND x.is_included_column = 0
                                              FOR
                                                XML PATH('')
                                              ), 1, 1, '')
                        ) AS c
            OUTER APPLY ( SELECT    x = STUFF(( SELECT  ',' + QUOTENAME(y.name)
                                                FROM    sys.index_columns AS x
                                                        INNER JOIN sys.columns
                                                        AS y ON y.object_id = x.object_id
                                                              AND x.column_id = y.column_id
                                                WHERE   x.object_id = a.object_id
                                                        AND x.index_id = a.index_id
                                                        AND x.is_included_column = 1
                                              FOR
                                                XML PATH('')
                                              ), 1, 1, '')
                        ) AS d
    WHERE   a.object_id = @objectid
            AND a.name IS NOT NULL


    SELECT  @sql = @sql + 'Alter Table '
            + QUOTENAME(OBJECT_NAME(a.parent_object_id)) + ' Add Constraint '
            + QUOTENAME(a.name) + ' Foreign Key (' + b.x + ') References '
            + QUOTENAME(@objectname) + '(' + c.x + ')' + @Enter
    FROM    sys.Foreign_keys AS a
            OUTER APPLY ( SELECT    x = STUFF(( SELECT  ',' + QUOTENAME(y.name)
                                                FROM    sys.Foreign_key_columns
                                                        AS x
                                                        INNER JOIN sys.columns
                                                        AS y ON y.object_id = x.parent_object_id
                                                              AND y.column_id = x.parent_column_id
                                                WHERE   x.constraint_object_id = a.object_id
                                              FOR
                                                XML PATH('')
                                              ), 1, 1, '')
                        ) AS b
            OUTER APPLY ( SELECT    x = STUFF(( SELECT  ',' + QUOTENAME(y.name)
                                                FROM    sys.Foreign_key_columns
                                                        AS x
                                                        INNER JOIN sys.columns
                                                        AS y ON y.object_id = x.referenced_object_id
                                                              AND y.column_id = x.referenced_column_id
                                                WHERE   x.constraint_object_id = a.object_id
                                              FOR
                                                XML PATH('')
                                              ), 1, 1, '')
                        ) AS c
    WHERE   a.referenced_object_id = @objectid

--执行脚本
    BEGIN TRY
        BEGIN TRAN
        EXEC(@sql)
        COMMIT TRAN
        PRINT N'表' + @objectname + N'数据移动到到文件组' + @NewFileGroup + N' .成功! '
    END TRY
    BEGIN CATCH
        DECLARE @Error NVARCHAR(1024)
        SET @Error = ERROR_MESSAGE()
        
        RAISERROR (50001, -- Message id.
           10,    -- Severity,
           1,     -- State,
            @Error); -- First argument supplies the string.
        PRINT N'表' + @objectname + N'数据移动到到文件组' + @NewFileGroup + N' .失败! '
        ROLLBACK TRAN
    END CATCH
GO

--EXEC sp_MoveTable A, 'MyFileGroup2'


--Select * From sys.filegroups 
--Select * From sys.indexes

/*
If object_id('test2') Is Not Null
Drop Table test2
If object_id('test1') Is Not Null
Drop Table test1
Go
Create Table test1
(
id int Identity(1,1) Not Null ,
x nvarchar(50),
Constraint PK_test1_id Primary Key(id Asc)
)
Create nonClustered Index IX_test1_x On Test1(x Asc)
Create Table test2
(
id int Identity(1,1) Not Null,
test1id int not null,
x nvarchar(50),
Constraint PK_test2_id Primary Key(id Asc),
Constraint FK_test2_test1id Foreign Key (test1id) References Test1(id)
)
*/
