SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[EventRiskyTransaction_Ignore] (
    @network VARCHAR(50),
    @transactionHash CHAR(66)
    )
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    UPDATE [Ethereum].EventTransaction
    SET Completed = 1,
        LockedBy = NULL,
        LockedAt = NULL,
        DateCompleted = SYSUTCDATETIME(),
        CompletionReason = 'RISKY_TRANSACTION_IGNORE'
    WHERE Network = @network
        AND TransactionHash = @transactionHash
        AND Completed = 0;
END
GO


