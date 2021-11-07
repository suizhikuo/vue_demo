USE [TestHekaton]

SELECT DISTINCT
        'Exec sp_MoveTable ' + MIDHSRM.TableName + ',' + ''''
        + ( CASE MIDHSRM.FileGroupName
              WHEN 'PRIMARY' THEN 'PRIMARY'
              WHEN 'PRODProcurement_ASN' THEN 'ASN'
              WHEN 'PRODProcurement_BAS' THEN 'PRIMARY'
              WHEN 'PRODProcurement_DPS' THEN 'PRIMARY'
              WHEN 'PRODProcurement_FCST' THEN 'PRIMARY'
              WHEN 'PRODProcurement_HIS' THEN 'His'
              WHEN 'PRODProcurement_I' THEN 'Int'
              WHEN 'PRODProcurement_MR' THEN 'PRIMARY'
              WHEN 'PRODProcurement_MRHIS' THEN 'PRIMARY'
              WHEN 'PRODProcurement_PO' THEN 'PO'
              WHEN 'PRODProcurement_RPT' THEN 'Report'
              WHEN 'PRODProcurement_SS' THEN 'Schedule'
              WHEN 'PRODProcurement_SYS' THEN 'PRIMARY'
              ELSE ''
            END ) + ''''
FROM    Temp_MIDHSRM_TableName_FileGroupName AS MIDHSRM
        INNER JOIN Temp_SCC_TableName_FileGroupName AS SCC ON ( MIDHSRM.TableName = SCC.TableName
                                                              OR MIDHSRM.TableName = REPLACE(SCC.TableName,
                                                              '_', '')
                                                              )
--ORDER BY MIDHSRM.FileGroupName
--EXEC sp_MoveTable A, 'MyFileGroup2'
