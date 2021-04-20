SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Event_Release] (
    @eventTransactionId BIGINT,
    @completed BIT
    )
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    UPDATE [Ethereum].[EventTransaction]
    SET Completed = @completed,
        LockedBy = NULL,
        LockedAt = NULL,
        IsRisky = CASE WHEN @completed = 1 THEN 0 ELSE IsRisky END,
        DateCompleted = CASE @completed WHEN 1 THEN SYSUTCDATETIME() ELSE NULL END,
        CompletionReason = CASE @completed WHEN 1 THEN 'PROCESSED' ELSE NULL END
    WHERE EventTransactionId = @eventTransactionId
        AND Completed = 0;
END
GO


