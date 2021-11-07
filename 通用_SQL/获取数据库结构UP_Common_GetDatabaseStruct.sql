SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO
-- =============================================
-- Author:		
-- Create date: 2017-12-06
-- Description:��ȡ���ݿ�ṹ  sql server 
-- =============================================
 
CREATE PROCEDURE [dbo].[UP_Common_GetDatabaseStruct]
AS
    BEGIN
        PRINT '����Ϣ:';
        SELECT  ���� = CASE WHEN a.colorder = 1 THEN d.name
                          ELSE ''
                     END ,
                ��˵�� = CASE WHEN a.colorder = 1 THEN ISNULL(f.value, '')
                           ELSE ''
                      END ,
                �ֶ���� = a.colorder ,
                �ֶ��� = a.name ,
                ��ʶ = CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1
                          THEN '��'
                          ELSE ''
                     END ,
                ���� = CASE WHEN EXISTS ( SELECT  1
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
                          THEN '��'
                          ELSE ''
                     END ,
                ���� = b.name ,
                ռ���ֽ��� = a.length ,
                ���� = COLUMNPROPERTY(a.id, a.name, 'PRECISION') ,
                С��λ�� = ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0) ,
                ����� = CASE WHEN a.isnullable = 1 THEN '��'
                           ELSE ''
                      END ,
                Ĭ��ֵ = ISNULL(e.text, '') ,
                �ֶ�˵�� = ISNULL(g.[value], '')
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
 

        PRINT '��ͼ��Ϣ';
	   
        SELECT  ��ͼ = CASE WHEN a.colorder = 1 THEN d.name
                          ELSE ''
                     END ,
                ��˵�� = CASE WHEN a.colorder = 1 THEN ISNULL(f.value, '')
                           ELSE ''
                      END ,
                �ֶ���� = a.colorder ,
                �ֶ��� = a.name ,
                ��ʶ = CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1
                          THEN '��'
                          ELSE ''
                     END ,
                ���� = CASE WHEN EXISTS ( SELECT  1
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
                          THEN '��'
                          ELSE ''
                     END ,
                ���� = b.name ,
                ռ���ֽ��� = a.length ,
                ���� = COLUMNPROPERTY(a.id, a.name, 'PRECISION') ,
                С��λ�� = ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0) ,
                ����� = CASE WHEN a.isnullable = 1 THEN '��'
                           ELSE ''
                      END ,
                Ĭ��ֵ = ISNULL(e.text, '') ,
                �ֶ�˵�� = ISNULL(g.[value], '')
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


        PRINT '�洢������Ϣ';
        SELECT  ���� = CASE WHEN a.colorder = 1 THEN d.name
                          ELSE ''
                     END ,
                ��˵�� = CASE WHEN a.colorder = 1 THEN ISNULL(f.value, '')
                           ELSE ''
                      END ,
                �ֶ���� = a.colorder ,
                �ֶ��� = a.name ,
                ��ʶ = CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1
                          THEN '��'
                          ELSE ''
                     END ,
                ���� = CASE WHEN EXISTS ( SELECT  1
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
                          THEN '��'
                          ELSE ''
                     END ,
                ���� = b.name ,
                ռ���ֽ��� = a.length ,
                ���� = COLUMNPROPERTY(a.id, a.name, 'PRECISION') ,
                С��λ�� = ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0) ,
                ����� = CASE WHEN a.isnullable = 1 THEN '��'
                           ELSE ''
                      END ,
                Ĭ��ֵ = ISNULL(e.text, '') ,
                �ֶ�˵�� = ISNULL(g.[value], '')
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

		   --������
        SELECT  ������ = CASE WHEN a.colorder = 1 THEN d.name
                           ELSE ''
                      END ,
                ��˵�� = CASE WHEN a.colorder = 1 THEN ISNULL(f.value, '')
                           ELSE ''
                      END ,
                �ֶ���� = a.colorder ,
                �ֶ��� = a.name ,
                ��ʶ = CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1
                          THEN '��'
                          ELSE ''
                     END ,
                ���� = CASE WHEN EXISTS ( SELECT  1
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
                          THEN '��'
                          ELSE ''
                     END ,
                ���� = b.name ,
                ռ���ֽ��� = a.length ,
                ���� = COLUMNPROPERTY(a.id, a.name, 'PRECISION') ,
                С��λ�� = ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0) ,
                ����� = CASE WHEN a.isnullable = 1 THEN '��'
                           ELSE ''
                      END ,
                Ĭ��ֵ = ISNULL(e.text, '') ,
                �ֶ�˵�� = ISNULL(g.[value], '')
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
		   --����
        SELECT TOP 100
                ���� = CASE WHEN a.colorder = 1 THEN d.name
                          ELSE ''
                     END ,
                ��˵�� = CASE WHEN a.colorder = 1 THEN ISNULL(f.value, '')
                           ELSE ''
                      END ,
                �ֶ���� = a.colorder ,
                �ֶ��� = a.name ,
                ��ʶ = CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1
                          THEN '��'
                          ELSE ''
                     END ,
                ���� = CASE WHEN EXISTS ( SELECT  1
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
                          THEN '��'
                          ELSE ''
                     END ,
                ���� = b.name ,
                ռ���ֽ��� = a.length ,
                ���� = COLUMNPROPERTY(a.id, a.name, 'PRECISION') ,
                С��λ�� = ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0) ,
                ����� = CASE WHEN a.isnullable = 1 THEN '��'
                           ELSE ''
                      END ,
                Ĭ��ֵ = ISNULL(e.text, '') ,
                �ֶ�˵�� = ISNULL(g.[value], '')
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

