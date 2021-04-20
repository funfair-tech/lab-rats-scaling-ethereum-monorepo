SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE FUNCTION [Ethereum].[fnBlockNumberHexToInt] (@value VARCHAR(66))
RETURNS INT
    WITH SCHEMABINDING
AS
BEGIN
    RETURN CONVERT(INT, CONVERT(VARBINARY(32), @value, 1));
END
GO

