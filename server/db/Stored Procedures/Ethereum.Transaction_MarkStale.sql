SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Transaction_MarkStale] (@receipts Ethereum.StaleTransaction READONLY)
AS
BEGIN
    -- Called by TransactionPollerProcessor
    SET NOCOUNT ON;

    UPDATE [Ethereum].[ChainTransaction]
    SET [ChainTransaction].STATUS = 'STALE',
        RetryCount = 0
    FROM @receipts Receipts
    WHERE [ChainTransaction].TransactionHash = Receipts.TransactionHash
        AND [ChainTransaction].STATUS = 'NEW';
END
GO


