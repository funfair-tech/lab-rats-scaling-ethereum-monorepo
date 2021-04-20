SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Transaction_GetBroken] (
    @network VARCHAR(50),
    @account CHAR(66),
    @currentTimeOnNetwork DATETIME2
    )
AS
BEGIN
    -- Called by TransactionResubmissionProcessor
    SET NOCOUNT ON;

    DECLARE @retryPoint DATETIME2 = DATEADD(minute, - 1, @currentTimeOnNetwork);

    UPDATE T
    SET RetryCount = RetryCount + 1,
        DateLastRetried = @currentTimeOnNetwork
    OUTPUT inserted.[TransactionId],
        inserted.[TransactionHash],
        inserted.[Network],
        inserted.[Account],
        inserted.[ContractAddress],
        inserted.[FunctionName],
        inserted.[ChainStatus],
        inserted.[Status],
        inserted.[DateCreated],
        inserted.[TransactionData],
        inserted.[Value],
        inserted.[GasPrice],
        inserted.[EstimatedGas],
        inserted.[GasLimit],
        inserted.[Nonce],
        inserted.[ContextType],
        inserted.[ContextId],
        inserted.[BlockHash],
        inserted.[TransactionIndex] AS TransactionIndex,
        inserted.[BlockNumber] AS BlockNumber,
        inserted.[GasUsed],
        inserted.[DateMined],
        inserted.[RetryCount],
        inserted.[DateLastRetried],
        inserted.[ReplacedByTransactionId],
        inserted.[ReplacesTransactionId],
        inserted.[ReasonForSubmission],
        inserted.[SubmissionContext],
        inserted.[ReplacementCount],
        inserted.[HasReachedMaximumGasPriceLimit],
        inserted.[Priority]
    FROM Ethereum.[ChainTransaction] T
    INNER JOIN Ethereum.RetryableStatuses S(NOLOCK)
        ON T.STATUS = S.STATUS
    WHERE T.Network = @network
        AND T.Account = @account
        AND T.STATUS <> 'REPLACED'
        AND (
            T.DateLastRetried IS NULL
            OR (
                T.DateLastRetried IS NOT NULL
                AND T.DateLastRetried < @retryPoint
                )
            )
        AND (
            T.STATUS <> 'STALE'
            OR T.HasReachedMaximumGasPriceLimit = 0
            );
END;
GO


