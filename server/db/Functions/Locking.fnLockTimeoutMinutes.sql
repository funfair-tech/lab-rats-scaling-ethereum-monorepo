SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE FUNCTION [Locking].[fnLockTimeoutMinutes] ()
RETURNS INT
    WITH SCHEMABINDING
AS
BEGIN
    RETURN 15;
END
GO

