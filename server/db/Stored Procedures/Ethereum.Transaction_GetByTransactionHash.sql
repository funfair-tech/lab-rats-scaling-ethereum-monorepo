SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Transaction_GetByTransactionHash] (
    @network VARCHAR(50),
    @account CHAR(42),
    @transactionHash CHAR(66)
    )
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;-- So TXN will be rolled back if there are any errors

    BEGIN TRANSACTION

    DECLARE @transaction TABLE (
        TransactionId BIGINT NOT NULL,
        TransactionHash CHAR(66) COLLATE Latin1_General_CI_AS NOT NULL,
        Network VARCHAR(50) COLLATE Latin1_General_CI_AS NOT NULL,
        Account CHAR(42) COLLATE Latin1_General_CI_AS NOT NULL,
        ContractAddress CHAR(42) COLLATE Latin1_General_CI_AS NOT NULL,
        FunctionName VARCHAR(255) COLLATE Latin1_General_CI_AS NOT NULL,
        ChainStatus VARCHAR(40) COLLATE Latin1_General_CI_AS NOT NULL,
        STATUS VARCHAR(40) COLLATE Latin1_General_CI_AS NOT NULL,
        DateCreated DATETIME2(2) NOT NULL,
        TransactionData VARCHAR(MAX) COLLATE Latin1_General_CI_AS NOT NULL,
        Value VARCHAR(66) COLLATE Latin1_General_CI_AS NOT NULL,
        GasPrice BIGINT NULL,
        EstimatedGas INT NOT NULL,
        GasLimit INT NOT NULL,
        Nonce BIGINT NOT NULL,
        BlockHash CHAR(66) COLLATE Latin1_General_CI_AS NULL,
        TransactionIndex INT NULL,
        BlockNumber INT NULL,
        GasUsed INT NULL,
        DateMined DATETIME2(2) NULL,
        RetryCount INT NOT NULL,
        DateLastRetried DATETIME2(2) NULL,
        ReplacedByTransactionId BIGINT NULL,
        Priority VARCHAR(10),
        PRIMARY KEY (TransactionId ASC)
        );

    INSERT INTO @transaction (
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
        BlockHash,
        TransactionIndex,
        BlockNumber,
        GasUsed,
        DateMined,
        RetryCount,
        DateLastRetried,
        ReplacedByTransactionId,
        Priority
        )
    SELECT TransactionId,
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
        BlockHash,
        TransactionIndex,
        BlockNumber,
        GasUsed,
        DateMined,
        RetryCount,
        DateLastRetried,
        ReplacedByTransactionId,
        Priority
    FROM Ethereum.[ChainTransaction] T
    WHERE T.Network = @network
        AND T.Account = @account
        AND T.TransactionHash = @transactionHash;

    IF @@ROWCOUNT = 0
    BEGIN
        INSERT INTO @transaction (
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
            BlockHash,
            TransactionIndex,
            BlockNumber,
            GasUsed,
            DateMined,
            RetryCount,
            DateLastRetried,
            ReplacedByTransactionId,
            Priority
            )
        SELECT TransactionId,
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
            BlockHash,
            TransactionIndex,
            BlockNumber,
            GasUsed,
            DateMined,
            RetryCount,
            DateLastRetried,
            ReplacedByTransactionId,
            Priority
        FROM Ethereum.[ChainTransaction_Complete] T
        WHERE T.Network = @network
            AND T.Account = @account
            AND T.TransactionHash = @transactionHash;
    END

    SELECT TransactionId,
        TransactionHash,
        Network,
        Account,
        ContractAddress,
        FunctionName,
        ChainStatus,
        STATUS,
        DateCreated,
        TransactionData,
        [Value],
        GasPrice,
        EstimatedGas,
        GasLimit,
        Nonce,
        BlockHash,
        TransactionIndex AS TransactionIndex,
        BlockNumber AS BlockNumber,
        GasUsed,
        DateMined,
        RetryCount,
        DateLastRetried,
        ReplacedByTransactionId,
        Priority
    FROM @transaction T;

    COMMIT
END;
GO


