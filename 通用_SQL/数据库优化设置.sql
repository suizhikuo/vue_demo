--http://www.cnblogs.com/cxd4321/archive/2008/12/10/1351792.html

--返回当前会话的当前锁超时设置，单位为毫秒 
SELECT @@LOCK_TIMEOUT
--执行 EXEC SP_LOCK 报告有关锁的信息 
EXEC SP_LOCK
-- 下例将锁超时期限设置为 1000*60*10 毫秒。 设置死锁超时参数为合理范围，如：3分钟-10分种；超过时间，自动放弃本次操作，避免进程悬挂；
SET LOCK_TIMEOUT 600000

