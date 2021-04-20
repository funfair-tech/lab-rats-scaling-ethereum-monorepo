SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Transaction_InsertReplacement] (
    @transactionHash CHAR(66),
    @network VARCHAR(50),
    @account CHAR(42),
    @contractAddress CHAR(42),
    @functionName VARCHAR(255),
    @transactionData VARCHAR(MAX),
    @value VARCHAR(66),
    @gasPrice BIGINT,
    @gasLimit INT,
    @estimatedGas INT,
    @nonce BIGINT,
    @replacesTransactionHash CHAR(66),
    @reasonForSubmission VARCHAR(40),
    @chainStatusOfPreviousTransaction VARCHAR(40),
    @priority VARCHAR(10)
    )
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @now DATETIME2 = SYSUTCDATETIME();

    SET XACT_ABORT ON;-- So TXN will be rolled back if there are any errors

    BEGIN TRANSACTION

    DECLARE @replacesTransactionId BIGINT = NULL;
    DECLARE @replacesNonce BIGINT = NULL;
    DECLARE @contextType VARCHAR(66) = NULL;
    DECLARE @contextId VARCHAR(66) = NULL;
    DECLARE @submissionContext UNIQUEIDENTIFIER = NULL;
    DECLARE @replacementCount INT = 0;

    SELECT TOP 1 @replacesTransactionId = TXN.TransactionId,
        @replacesNonce = TXN.Nonce,
        @contextType = TXN.ContextType,
        @contextId = TXN.ContextId,
        @submissionContext = TXN.SubmissionContext,
        @replacementCount = TXN.ReplacementCount
    FROM (
        SELECT T.TransactionId,
            T.Nonce,
            T.ContextType,
            T.ContextId,
            T.SubmissionContext,
            T.ReplacementCount
        FROM Ethereum.[ChainTransaction] T
        WHERE T.Network = @network
            AND T.TransactionHash = @replacesTransactionHash
        
        UNION
        
        SELECT T.TransactionId,
            T.Nonce,
            T.ContextType,
            T.ContextId,
            T.SubmissionContext,
            T.ReplacementCount
        FROM Ethereum.[ChainTransaction_Complete] T
        WHERE T.Network = @network
            AND T.TransactionHash = @replacesTransactionHash
        ) TXN
    ORDER BY TXN.TransactionId;

    INSERT INTO Ethereum.[ChainTransaction] (
        TransactionHash,
        Network,
        Account,
        ContractAddress,
        FunctionName,
        TransactionData,
        [Value],
        GasPrice,
        GasLimit,
        EstimatedGas,
        Nonce,
        ContextType,
        ContextId,
        ReplacesTransactionId,
        DateCreated,
        STATUS,
        ChainStatus,
        RetryCount,
        ReasonForSubmission,
        SubmissionContext,
        ReplacementCount,
        Priority
        )
    VALUES (
        @transactionHash,
        @network,
        @account,
        @contractAddress,
        @functionName,
        @transactionData,
        @value,
        @gasPrice,
        @gasLimit,
        @estimatedGas,
        @nonce,
        @contextType,
        @contextId,
        @replacesTransactionId,
        @now, -- DateCreated
        'NEW', -- Status,
        'NEW', -- ChainStatus
        0, -- RetryCount
        @reasonForSubmission, -- ReasonForSubmission
        @submissionContext, -- SubmissionContext
        COALESCE(@replacementCount, 0) + 1,
        @priority
        );

    DECLARE @transactionId BIGINT = SCOPE_IDENTITY();

    UPDATE Ethereum.[ChainTransaction]
    SET ReplacedByTransactionId = @transactionId,
        STATUS = 'REPLACED',
        ChainStatus = @chainStatusOfPreviousTransaction
    WHERE TransactionHash = @replacesTransactionHash;

    COMMIT TRANSACTION;
END;
GO


