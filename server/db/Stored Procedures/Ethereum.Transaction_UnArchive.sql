SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Transaction_UnArchive] (@transactionId BIGINT)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;-- So TXN will be rolled back if there are any errors

    BEGIN TRANSACTION

    -- Move 'completed' transactions (mined/replaced) to transactions_complete
    SET IDENTITY_INSERT Ethereum.[ChainTransaction] ON;

    INSERT INTO Ethereum.[ChainTransaction] (
        TransactionId,
        TransactionHash,
        Network,
        Account,
        ContractAddress,
        FunctionName,
        ChainStatus,
        STATUS,
        DateCreated,
        TransactionData,
        Value,
        GasPrice,
        EstimatedGas,
        GasLimit,
        Nonce,
        ContextType,
        ContextId,
        BlockHash,
        TransactionIndex,
        BlockNumber,
        GasUsed,
        DateMined,
        RetryCount,
        DateLastRetried,
        ReplacedByTransactionId,
        ReplacesTransactionId,
        ReasonForSubmission,
        SubmissionContext,
        ReplacementCount,
        HasReachedMaximumGasPriceLimit,
        Priority
        )
    SELECT TransactionId,
        TransactionHash,
        Network,
        Account,
        ContractAddress,
        FunctionName,
        ChainStatus,
        [Status],
        DateCreated,
        TransactionData,
        Value,
        GasPrice,
        EstimatedGas,
        GasLimit,
        Nonce,
        ContextType,
        ContextId,
        BlockHash,
        TransactionIndex,
        BlockNumber,
        GasUsed,
        DateMined,
        RetryCount,
        DateLastRetried,
        ReplacedByTransactionId,
        ReplacesTransactionId,
        ReasonForSubmission,
        SubmissionContext,
        ReplacementCount,
        HasReachedMaximumGasPriceLimit,
        Priority
    FROM Ethereum.[ChainTransaction_Complete]
    WHERE TransactionId = @transactionId;

    DELETE
    FROM Ethereum.[ChainTransaction_Complete]
    WHERE TransactionId = @transactionId;

    SET IDENTITY_INSERT Ethereum.[ChainTransaction] OFF;

    COMMIT;
END
GO


