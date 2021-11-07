SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

ALTER FUNCTION [dbo].[f_split]
    (
      @strText VARCHAR(MAX) ,--待分离的原字符串
      @strSplit VARCHAR(100)--分隔符
    )
RETURNS @temp TABLE
    (
      ID INT IDENTITY
             PRIMARY KEY ,
      Code VARCHAR(1000)
    )
AS
    BEGIN
        DECLARE @intLen INT; --用来存储待分离原字符串长度
        DECLARE @intSplitLen INT; --分隔符长度
        DECLARE @intIndex INT; --用来存储分离字符串在原字符串的位置
        DECLARE @strVal VARCHAR(1000);--用来存储分离出来后的字符串
     --获取原字符串的长度
        SET @intLen = LEN(RTRIM(LTRIM(@strText)));
        SET @intSplitLen = LEN(RTRIM(LTRIM(@strSplit)));
     --原字符串不为空，才继续分离
        IF ( @intLen > 0 )
            BEGIN
         --循环原字符串，直至原字符串被分离完毕
                WHILE CHARINDEX(@strSplit, @strText) > 0
                    BEGIN
             --获取分离字符串在原字符串的位置
                        SET @intIndex = CHARINDEX(@strSplit, @strText);
             --获取分离出的字符串，并插入表中
                        SET @strVal = RTRIM(LTRIM(LEFT(@strText, @intIndex - 1)));
                        IF ( LEN(@strVal) > 0 )
                            BEGIN
                                INSERT  INTO @temp
                                        ( Code )
                                VALUES  ( @strVal );
                            END;
             --分离后，将分离出的字符串（包括分隔符）从原字符串中删除
                        SET @strText = SUBSTRING(@strText,
                                                 @intIndex + @intSplitLen,
                                                 @intLen - @intIndex);
             --重新设置原字符串的长度
                        SET @intLen = LEN(@strText);
                    END;
         --如果分离后的原字符串依然不为空，则也应该插入表中
                IF ( LEN(RTRIM(LTRIM(@strText))) > 0 )
                    BEGIN
                        INSERT  INTO @temp
                                ( Code )
                        VALUES  ( @strText );
                    END;
            END;
        RETURN;
    END;

GO

