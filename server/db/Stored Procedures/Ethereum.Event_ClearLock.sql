SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Event_ClearLock] (@machineName VARCHAR(100))
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Ethereum.EventTransaction
    SET LockedAt = NULL,
        LockedBy = NULL
    WHERE LockedBy = @machineName;
END
GO


