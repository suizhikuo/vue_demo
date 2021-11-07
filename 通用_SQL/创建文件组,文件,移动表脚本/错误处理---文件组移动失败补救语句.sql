

ALTER TABLE [TblScheduleCheckRangeDetail] DROP CONSTRAINT [PK_TblScheduleCheckRangeDetail];
ALTER TABLE [TblScheduleCheckRangeDetail] ADD CONSTRAINT [PK_TblScheduleCheckRangeDetail] PRIMARY KEY ([OID] ASC);
ALTER TABLE [TblScheduleCheckRangeDetail] DROP CONSTRAINT [PK_TblScheduleCheckRangeDetail] WITH (MOVE TO [Schedule]);
ALTER TABLE [TblScheduleCheckRangeDetail] ADD CONSTRAINT [PK_TblScheduleCheckRangeDetail] PRIMARY KEY ([OID] ASC);



ALTER TABLE [TblScheduleCheckRangeHeader] DROP CONSTRAINT [PK_TblScheduleCheckRangeHeader] ;
ALTER TABLE [TblScheduleCheckRangeHeader] ADD CONSTRAINT [PK_TblScheduleCheckRangeHeader] PRIMARY KEY ([VersionID] ASC);
ALTER TABLE [TblScheduleCheckRangeHeader] DROP CONSTRAINT [PK_TblScheduleCheckRangeHeader] WITH (MOVE TO [Schedule]);
ALTER TABLE [TblScheduleCheckRangeHeader] ADD CONSTRAINT [PK_TblScheduleCheckRangeHeader] PRIMARY KEY ([VersionID] ASC);


ALTER TABLE [TblScheduleDayHistory] DROP CONSTRAINT [PK_TblScheduleDayHistory];
ALTER TABLE [TblScheduleDayHistory] ADD CONSTRAINT [PK_TblScheduleDayHistory] PRIMARY KEY ([SSVersionDate] ASC,[SSItemID] ASC,[SSDetailDate] ASC,[SSDetailNum] ASC);
ALTER TABLE [TblScheduleDayHistory] DROP CONSTRAINT [PK_TblScheduleDayHistory] WITH (MOVE TO [His]);
ALTER TABLE [TblScheduleDayHistory] ADD CONSTRAINT [PK_TblScheduleDayHistory] PRIMARY KEY ([SSVersionDate] ASC,[SSItemID] ASC,[SSDetailDate] ASC,[SSDetailNum] ASC);


ALTER TABLE [TblScheduleDemandHistory] DROP CONSTRAINT [PK_TblScheduleDemandHistory];
ALTER TABLE [TblScheduleDemandHistory] ADD CONSTRAINT [PK_TblScheduleDemandHistory] PRIMARY KEY ([ID] ASC);
ALTER TABLE [TblScheduleDemandHistory] DROP CONSTRAINT [PK_TblScheduleDemandHistory] WITH (MOVE TO [His]);
ALTER TABLE [TblScheduleDemandHistory] ADD CONSTRAINT [PK_TblScheduleDemandHistory] PRIMARY KEY ([ID] ASC);


ALTER TABLE [TblScheduleHistory] DROP CONSTRAINT [PK_TblScheduleHistory];
ALTER TABLE [TblScheduleHistory] ADD CONSTRAINT [PK_TblScheduleHistory] PRIMARY KEY ([OID] ASC);
ALTER TABLE [TblScheduleHistory] DROP CONSTRAINT [PK_TblScheduleHistory] WITH (MOVE TO [His]);
ALTER TABLE [TblScheduleHistory] ADD CONSTRAINT [PK_TblScheduleHistory] PRIMARY KEY ([OID] ASC);


ALTER TABLE [TblScheduleMessage] DROP CONSTRAINT [PK_TblScheduleMessage] ;
ALTER TABLE [TblScheduleMessage] ADD CONSTRAINT [PK_TblScheduleMessage] PRIMARY KEY ([OID] ASC);
ALTER TABLE [TblScheduleMessage] DROP CONSTRAINT [PK_TblScheduleMessage] WITH (MOVE TO [Schedule]);
ALTER TABLE [TblScheduleMessage] ADD CONSTRAINT [PK_TblScheduleMessage] PRIMARY KEY ([OID] ASC);


ALTER TABLE [TblScheduleVersionParam] DROP CONSTRAINT [PK_TblScheduleVersionParam] ;
ALTER TABLE [TblScheduleVersionParam] ADD CONSTRAINT [PK_TblScheduleVersionParam] PRIMARY KEY ([OID] ASC);
ALTER TABLE [TblScheduleVersionParam] DROP CONSTRAINT [PK_TblScheduleVersionParam] WITH (MOVE TO [His]);
ALTER TABLE [TblScheduleVersionParam] ADD CONSTRAINT [PK_TblScheduleVersionParam] PRIMARY KEY ([OID] ASC);
