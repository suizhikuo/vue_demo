USE [TestHekaton];
SELECT  o.[name] AS TableName ,
        f.[name] AS FileGroupName
INTO    Temp_SCC_TableName_FileGroupName
FROM    SCC.sys.indexes i
        INNER JOIN SCC.sys.filegroups f ON i.data_space_id = f.data_space_id
        INNER JOIN SCC.sys.all_objects o ON i.[object_id] = o.[object_id]
WHERE   o.type = 'U'
GO 