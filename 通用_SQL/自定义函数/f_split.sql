SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

ALTER FUNCTION [dbo].[f_split]
    (
      @strText VARCHAR(MAX) ,--�������ԭ�ַ���
      @strSplit VARCHAR(100)--�ָ���
    )
RETURNS @temp TABLE
    (
      ID INT IDENTITY
             PRIMARY KEY ,
      Code VARCHAR(1000)
    )
AS
    BEGIN
        DECLARE @intLen INT; --�����洢������ԭ�ַ�������
        DECLARE @intSplitLen INT; --�ָ�������
        DECLARE @intIndex INT; --�����洢�����ַ�����ԭ�ַ�����λ��
        DECLARE @strVal VARCHAR(1000);--�����洢�����������ַ���
     --��ȡԭ�ַ����ĳ���
        SET @intLen = LEN(RTRIM(LTRIM(@strText)));
        SET @intSplitLen = LEN(RTRIM(LTRIM(@strSplit)));
     --ԭ�ַ�����Ϊ�գ��ż�������
        IF ( @intLen > 0 )
            BEGIN
         --ѭ��ԭ�ַ�����ֱ��ԭ�ַ������������
                WHILE CHARINDEX(@strSplit, @strText) > 0
                    BEGIN
             --��ȡ�����ַ�����ԭ�ַ�����λ��
                        SET @intIndex = CHARINDEX(@strSplit, @strText);
             --��ȡ��������ַ��������������
                        SET @strVal = RTRIM(LTRIM(LEFT(@strText, @intIndex - 1)));
                        IF ( LEN(@strVal) > 0 )
                            BEGIN
                                INSERT  INTO @temp
                                        ( Code )
                                VALUES  ( @strVal );
                            END;
             --����󣬽���������ַ����������ָ�������ԭ�ַ�����ɾ��
                        SET @strText = SUBSTRING(@strText,
                                                 @intIndex + @intSplitLen,
                                                 @intLen - @intIndex);
             --��������ԭ�ַ����ĳ���
                        SET @intLen = LEN(@strText);
                    END;
         --���������ԭ�ַ�����Ȼ��Ϊ�գ���ҲӦ�ò������
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

