/****** 
Object:  UserDefinedFunction [dbo].[TRIM]    
Script Date: 11/18/2011 09:10:14 
Author: EF
******/
--IF EXISTS ( SELECT  *
--            FROM    sys.objects
--            WHERE   object_id = OBJECT_ID(N'[dbo].[trim]')
--                    AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ) )
--    DROP FUNCTION [dbo].[trim];
--GO
CREATE FUNCTION dbo.trim
    (
      @Source VARCHAR(MAX) ,
      @Char CHAR(1)
    )
RETURNS VARCHAR(MAX)
AS
    BEGIN
        DECLARE @i INT;
        DECLARE @returnString VARCHAR(MAX);
        SET @returnString = @Source;
    --清除前后空格
        SET @returnString = LTRIM(RTRIM(@returnString));
    --删除左侧字符
        SET @i = 0;
        WHILE @i = 0
            BEGIN
                IF LEFT(@returnString, 1) = @Char
                    SET @returnString = RIGHT(@returnString,
                                              LEN(@returnString) - 1);
                ELSE
                    SET @i = 1;
            END;
    --删除右侧字符
        SET @i = 0;
        WHILE @i = 0
            BEGIN
                IF RIGHT(@returnString, 1) = @Char
                    SET @returnString = LEFT(@returnString,
                                             LEN(@returnString) - 1);
                ELSE
                    SET @i = 1;
            END;
    --清除前后空格
        SET @returnString = LTRIM(RTRIM(@returnString));
        RETURN @returnString;
    END;

GO

----测试
--select dbo.trim('asdfas;asdfasdfa;',';')
--union all select dbo.trim(';asdfas;asdfasdfa;',';')
--union all select dbo.trim('  ;asdfas;asdfasdfa;',';')
--union all select dbo.trim('; asdfas;asdfasdfa;',';')

----结果
---------------------------------------------------------------------------

--asdfas;asdfasdfa
--asdfas;asdfasdfa
--asdfas;asdfasdfa
--asdfas;asdfasdfa
--(4 行受影响)