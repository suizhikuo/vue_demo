SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO
-- =============================================
-- Author:		
-- Create date: 2017-12-06
-- Description:获取数据库结构  sql server 
-- =============================================
 
CREATE PROCEDURE [dbo].[UP_Common_GetDatabaseStruct]
AS
    BEGIN
        PRINT '表信息:';
        SELECT  表名 = CASE WHEN a.colorder = 1 THEN d.name
                          ELSE ''
                     END ,
                表说明 = CASE WHEN a.colorder = 1 THEN ISNULL(f.value, '')
                           ELSE ''
                      END ,
                字段序号 = a.colorder ,
                字段名 = a.name ,
                标识 = CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1
                          THEN '√'
                          ELSE ''
                     END ,
                主键 = CASE WHEN EXISTS ( SELECT  1
                                        FROM    sysobjects
                                        WHERE   xtype = 'PK'
                                                AND name IN (
                                                SELECT  name
                                                FROM    sysindexes
                                                WHERE   indid IN (
                                                        SELECT
                                                              indid
                                                        FROM  sysindexkeys
                                                        WHERE id = a.id
                                                              AND colid = a.colid ) ) )
                          THEN '√'
                          ELSE ''
                     END ,
                类型 = b.name ,
                占用字节数 = a.length ,
                长度 = COLUMNPROPERTY(a.id, a.name, 'PRECISION') ,
                小数位数 = ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0) ,
                允许空 = CASE WHEN a.isnullable = 1 THEN '√'
                           ELSE ''
                      END ,
                默认值 = ISNULL(e.text, '') ,
                字段说明 = ISNULL(g.[value], '')
        FROM    syscolumns a
                LEFT JOIN systypes b ON a.xtype = b.xusertype
                INNER JOIN sysobjects d ON a.id = d.id
                                           AND d.xtype IN ( 'U' )--, 'IT', 'S'
                                           AND d.name <> 'dtproperties'
                LEFT JOIN syscomments e ON a.cdefault = e.id
                LEFT JOIN sys.extended_properties g ON a.id = g.major_id
                                                       AND a.colid = g.minor_id
                LEFT JOIN sys.extended_properties f ON d.id = f.major_id
                                                       AND f.minor_id = 0
        ORDER BY a.id ,
                a.colorder;
 

        PRINT '视图信息';
	   
        SELECT  视图 = CASE WHEN a.colorder = 1 THEN d.name
                          ELSE ''
                     END ,
                表说明 = CASE WHEN a.colorder = 1 THEN ISNULL(f.value, '')
                           ELSE ''
                      END ,
                字段序号 = a.colorder ,
                字段名 = a.name ,
                标识 = CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1
                          THEN '√'
                          ELSE ''
                     END ,
                主键 = CASE WHEN EXISTS ( SELECT  1
                                        FROM    sysobjects
                                        WHERE   xtype = 'PK'
                                                AND name IN (
                                                SELECT  name
                                                FROM    sysindexes
                                                WHERE   indid IN (
                                                        SELECT
                                                              indid
                                                        FROM  sysindexkeys
                                                        WHERE id = a.id
                                                              AND colid = a.colid ) ) )
                          THEN '√'
                          ELSE ''
                     END ,
                类型 = b.name ,
                占用字节数 = a.length ,
                长度 = COLUMNPROPERTY(a.id, a.name, 'PRECISION') ,
                小数位数 = ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0) ,
                允许空 = CASE WHEN a.isnullable = 1 THEN '√'
                           ELSE ''
                      END ,
                默认值 = ISNULL(e.text, '') ,
                字段说明 = ISNULL(g.[value], '')
        FROM    syscolumns a
                LEFT JOIN systypes b ON a.xtype = b.xusertype
                INNER JOIN sysobjects d ON a.id = d.id
                                           AND d.xtype = 'V'
                                           AND d.name <> 'dtproperties'
                LEFT JOIN syscomments e ON a.cdefault = e.id
                LEFT JOIN sys.extended_properties g ON a.id = g.major_id
                                                       AND a.colid = g.minor_id
                LEFT JOIN sys.extended_properties f ON d.id = f.major_id
                                                       AND f.minor_id = 0
        ORDER BY a.id ,
                a.colorder;


        PRINT '存储过程信息';
        SELECT  过程 = CASE WHEN a.colorder = 1 THEN d.name
                          ELSE ''
                     END ,
                表说明 = CASE WHEN a.colorder = 1 THEN ISNULL(f.value, '')
                           ELSE ''
                      END ,
                字段序号 = a.colorder ,
                字段名 = a.name ,
                标识 = CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1
                          THEN '√'
                          ELSE ''
                     END ,
                主键 = CASE WHEN EXISTS ( SELECT  1
                                        FROM    sysobjects
                                        WHERE   xtype = 'PK'
                                                AND name IN (
                                                SELECT  name
                                                FROM    sysindexes
                                                WHERE   indid IN (
                                                        SELECT
                                                              indid
                                                        FROM  sysindexkeys
                                                        WHERE id = a.id
                                                              AND colid = a.colid ) ) )
                          THEN '√'
                          ELSE ''
                     END ,
                类型 = b.name ,
                占用字节数 = a.length ,
                长度 = COLUMNPROPERTY(a.id, a.name, 'PRECISION') ,
                小数位数 = ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0) ,
                允许空 = CASE WHEN a.isnullable = 1 THEN '√'
                           ELSE ''
                      END ,
                默认值 = ISNULL(e.text, '') ,
                字段说明 = ISNULL(g.[value], '')
        FROM    syscolumns a
                LEFT JOIN systypes b ON a.xtype = b.xusertype
                INNER JOIN sysobjects d ON a.id = d.id
                                           AND d.xtype = 'P'
                                           AND d.name <> 'dtproperties'
                LEFT JOIN syscomments e ON a.cdefault = e.id
                LEFT JOIN sys.extended_properties g ON a.id = g.major_id
                                                       AND a.colid = g.minor_id
                LEFT JOIN sys.extended_properties f ON d.id = f.major_id
                                                       AND f.minor_id = 0
        ORDER BY a.id ,
                a.colorder;

		   --表类型
        SELECT  表类型 = CASE WHEN a.colorder = 1 THEN d.name
                           ELSE ''
                      END ,
                表说明 = CASE WHEN a.colorder = 1 THEN ISNULL(f.value, '')
                           ELSE ''
                      END ,
                字段序号 = a.colorder ,
                字段名 = a.name ,
                标识 = CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1
                          THEN '√'
                          ELSE ''
                     END ,
                主键 = CASE WHEN EXISTS ( SELECT  1
                                        FROM    sysobjects
                                        WHERE   xtype = 'PK'
                                                AND name IN (
                                                SELECT  name
                                                FROM    sysindexes
                                                WHERE   indid IN (
                                                        SELECT
                                                              indid
                                                        FROM  sysindexkeys
                                                        WHERE id = a.id
                                                              AND colid = a.colid ) ) )
                          THEN '√'
                          ELSE ''
                     END ,
                类型 = b.name ,
                占用字节数 = a.length ,
                长度 = COLUMNPROPERTY(a.id, a.name, 'PRECISION') ,
                小数位数 = ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0) ,
                允许空 = CASE WHEN a.isnullable = 1 THEN '√'
                           ELSE ''
                      END ,
                默认值 = ISNULL(e.text, '') ,
                字段说明 = ISNULL(g.[value], '')
        FROM    syscolumns a
                LEFT JOIN systypes b ON a.xtype = b.xusertype
                INNER JOIN sysobjects d ON a.id = d.id
                                           AND d.xtype = 'TT'
                                           AND d.name <> 'dtproperties'
                LEFT JOIN syscomments e ON a.cdefault = e.id
                LEFT JOIN sys.extended_properties g ON a.id = g.major_id
                                                       AND a.colid = g.minor_id
                LEFT JOIN sys.extended_properties f ON d.id = f.major_id
                                                       AND f.minor_id = 0
        ORDER BY a.id ,
                a.colorder;
		   --函数
        SELECT TOP 100
                函数 = CASE WHEN a.colorder = 1 THEN d.name
                          ELSE ''
                     END ,
                表说明 = CASE WHEN a.colorder = 1 THEN ISNULL(f.value, '')
                           ELSE ''
                      END ,
                字段序号 = a.colorder ,
                字段名 = a.name ,
                标识 = CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1
                          THEN '√'
                          ELSE ''
                     END ,
                主键 = CASE WHEN EXISTS ( SELECT  1
                                        FROM    sysobjects
                                        WHERE   xtype = 'PK'
                                                AND name IN (
                                                SELECT  name
                                                FROM    sysindexes
                                                WHERE   indid IN (
                                                        SELECT
                                                              indid
                                                        FROM  sysindexkeys
                                                        WHERE id = a.id
                                                              AND colid = a.colid ) ) )
                          THEN '√'
                          ELSE ''
                     END ,
                类型 = b.name ,
                占用字节数 = a.length ,
                长度 = COLUMNPROPERTY(a.id, a.name, 'PRECISION') ,
                小数位数 = ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0) ,
                允许空 = CASE WHEN a.isnullable = 1 THEN '√'
                           ELSE ''
                      END ,
                默认值 = ISNULL(e.text, '') ,
                字段说明 = ISNULL(g.[value], '')
        FROM    syscolumns a
                LEFT JOIN systypes b ON a.xtype = b.xusertype
                INNER JOIN sysobjects d ON a.id = d.id
                                           AND d.xtype IN ( 'FN', 'FS', 'FT',
                                                            'IF', 'TF' )
                                           AND d.name <> 'dtproperties'
                LEFT JOIN syscomments e ON a.cdefault = e.id
                LEFT JOIN sys.extended_properties g ON a.id = g.major_id
                                                       AND a.colid = g.minor_id
                LEFT JOIN sys.extended_properties f ON d.id = f.major_id
                                                       AND f.minor_id = 0
        ORDER BY a.id ,
                a.colorder;
   
                
           

    END;












GO

