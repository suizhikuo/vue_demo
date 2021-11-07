CREATE TABLE [dbo].[B2B_Refresh_GreenPurse_UserMap] (
[ID] uniqueidentifier NOT NULL , 
[SortNum] int NULL DEFAULT ((0)) ,
[IsDelete] bit NULL DEFAULT ((0)) ,
[RecordStatus] int NULL DEFAULT ((0)) ,
[CreateUser] nvarchar(50) NULL DEFAULT (N'创建人') ,
[CreateDatetime] datetime NULL DEFAULT (getdate()) ,
[LastUpdateUser] nvarchar(50) NULL DEFAULT (N'最后修改人') ,
[LastUpdateDatetime] datetime NULL DEFAULT (getdate())
)

GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'B2B_Refresh_GreenPurse_UserMap', 
'COLUMN', N'ID')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'ID'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'ID'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'ID'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'ID'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'B2B_Refresh_GreenPurse_UserMap', 
'COLUMN', N'SortNum')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'排序号'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'SortNum'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'排序号'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'SortNum'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'B2B_Refresh_GreenPurse_UserMap', 
'COLUMN', N'IsDelete')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'删除标志'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'IsDelete'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'删除标志'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'IsDelete'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'B2B_Refresh_GreenPurse_UserMap', 
'COLUMN', N'RecordStatus')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'记录状态'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'RecordStatus'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'记录状态'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'RecordStatus'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'B2B_Refresh_GreenPurse_UserMap', 
'COLUMN', N'CreateUser')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'创建人'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'CreateUser'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'创建人'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'CreateUser'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'B2B_Refresh_GreenPurse_UserMap', 
'COLUMN', N'CreateDatetime')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'创建时间'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'CreateDatetime'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'CreateDatetime'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'B2B_Refresh_GreenPurse_UserMap', 
'COLUMN', N'LastUpdateUser')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'最后更新人'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'LastUpdateUser'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'最后更新人'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'LastUpdateUser'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'B2B_Refresh_GreenPurse_UserMap', 
'COLUMN', N'LastUpdateDatetime')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'最后更新时间'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'LastUpdateDatetime'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'最后更新时间'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'B2B_Refresh_GreenPurse_UserMap'
, @level2type = 'COLUMN', @level2name = N'LastUpdateDatetime'
GO

-- ----------------------------
-- Indexes structure for table B2B_Refresh_GreenPurse_UserMap
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table B2B_Refresh_GreenPurse_UserMap
-- ----------------------------
ALTER TABLE [dbo].[B2B_Refresh_GreenPurse_UserMap] ADD PRIMARY KEY ([ID])
GO
