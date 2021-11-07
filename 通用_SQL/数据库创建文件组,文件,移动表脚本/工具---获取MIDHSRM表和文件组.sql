USE [TestHekaton];
SELECT  o.[name] AS TableName ,
        f.[name] AS FileGroupName
INTO    Temp_MIDHSRM_TableName_FileGroupName
FROM    MIDHSRM.sys.indexes i
        INNER JOIN MIDHSRM.sys.filegroups f ON i.data_space_id = f.data_space_id
        INNER JOIN MIDHSRM.sys.all_objects o ON i.[object_id] = o.[object_id]
WHERE   o.type = 'U'
GO 