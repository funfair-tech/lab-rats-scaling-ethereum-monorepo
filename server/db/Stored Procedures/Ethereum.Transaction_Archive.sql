SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Transaction_Archive] (@transactionId BIGINT)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;-- So TXN will be rolled back if there are any errors

    -- Move 'completed' transactions (mined/replaced) to transactions_complete
    DELETE
    FROM Ethereum.[ChainTransaction]
    OUTPUT deleted.TransactionId,
        deleted.TransactionHash,
        deleted.Network,
        deleted.Account,
        deleted.ContractAddress,
        deleted.FunctionName,
        deleted.ChainStatus,
        deleted.STATUS,
        deleted.DateCreated,
        deleted.TransactionData,
        deleted.Value,
        deleted.GasPrice,
        deleted.EstimatedGas,
        deleted.GasLimit,
        deleted.Nonce,
        deleted.ContextType,
        deleted.ContextId,
        deleted.BlockHash,
        deleted.TransactionIndex,
        deleted.BlockNumber,
        deleted.GasUsed,
        deleted.DateMined,
        deleted.RetryCount,
        deleted.DateLastRetried,
        deleted.ReplacedByTransactionId,
        deleted.ReplacesTransactionId,
        deleted.ReasonForSubmission,
        deleted.SubmissionContext,
        deleted.ReplacementCount,
        deleted.HasReachedMaximumGasPriceLimit,
        deleted.[Priority]
    INTO Ethereum.ChainTransaction_Complete(TransactionId, TransactionHash, Network, Account, ContractAddress, FunctionName, ChainStatus, STATUS, DateCreated, TransactionData, Value, GasPrice, EstimatedGas, GasLimit, Nonce, ContextType, ContextId, BlockHash, TransactionIndex, BlockNumber, GasUsed, DateMined, RetryCount, DateLastRetried, ReplacedByTransactionId, ReplacesTransactionId, ReasonForSubmission, SubmissionContext, ReplacementCount, HasReachedMaximumGasPriceLimit, [Priority])
    WHERE TransactionId = @transactionId;
END
GO


