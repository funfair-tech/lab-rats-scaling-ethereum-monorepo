SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Transaction_GetPending] (
    @network VARCHAR(50),
    @accountAddress CHAR(42)
    )
AS
BEGIN
    -- Called by TransactionPollerProcessor to get the transactions to monitor for completion
    SET NOCOUNT ON;

    SELECT t.TransactionId,
        t.Network,
        t.Account,
        t.TransactionHash,
        t.Network,
        t.GasLimit,
        t.GasPrice,
        t.Nonce,
        t.DateCreated AS DateSubmitted,
        t.STATUS,
        t.RetryCount,
        t.DateLastRetried,
        0 AS GasPolicyExecution,
        t.Priority
    FROM Ethereum.[ChainTransaction] t
    INNER JOIN Ethereum.TransactionStatus S(NOLOCK)
        ON T.STATUS = S.STATUS
            AND S.IsPending = 1
    WHERE T.Network = @network
        AND T.Account = @accountAddress
        AND T.BlockHash IS NULL
    ORDER BY t.DateCreated ASC,
        t.TransactionHash;
END;
GO


