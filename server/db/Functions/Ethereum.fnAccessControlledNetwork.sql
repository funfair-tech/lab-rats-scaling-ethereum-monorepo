SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE FUNCTION [Ethereum].[fnAccessControlledNetwork] (@network VARCHAR(50))
RETURNS BIT
    WITH SCHEMABINDING
AS
BEGIN
    DECLARE @applyAccessControlChecks BIT = 0;

    IF @network = 'MAINNET'
        OR @network = 'OPTIMISMMAINNET'
    BEGIN
        SET @applyAccessControlChecks = 1;
    END

    RETURN @applyAccessControlChecks;
END
GO
