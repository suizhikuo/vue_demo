USE [TestHekaton]

SELECT DISTINCT
        MIDHSRM.TableName ,
        MIDHSRM.FileGroupName
FROM    Temp_MIDHSRM_TableName_FileGroupName AS MIDHSRM
        INNER JOIN Temp_SCC_TableName_FileGroupName AS SCC ON ( MIDHSRM.TableName = SCC.TableName
                                                              OR MIDHSRM.TableName = REPLACE(SCC.TableName,
                                                              '_', '')
                                                              )