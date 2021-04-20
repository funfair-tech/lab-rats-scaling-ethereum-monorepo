SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Transaction_Insert] (
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
    @contextType VARCHAR(66),
    @contextId VARCHAR(66),
    @priority VARCHAR(10)
    )
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @now DATETIME2 = SYSUTCDATETIME();

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
        DateCreated,
        STATUS,
        ChainStatus,
        RetryCount,
        ReasonForSubmission,
        SubmissionContext,
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
        @now, -- DateCreated
        'NEW', -- Status,
        'NEW', -- ChainStatus
        0, -- RetryCount
        'NEW', -- ReasonForSubmission
        NEWID(), -- SubmissionContext
        @priority
        );
END;
GO


