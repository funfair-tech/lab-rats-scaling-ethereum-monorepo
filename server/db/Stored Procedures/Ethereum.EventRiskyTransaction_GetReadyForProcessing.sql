SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[EventRiskyTransaction_GetReadyForProcessing] (
    @network VARCHAR(50),
    @blockNumber INT
    )
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @now DATETIME2 = SYSUTCDATETIME();
    DECLARE @retryPoint DATETIME2 = DATEADD(second, - 15, @now);
    DECLARE @normalToRiskyRetryPoint DATETIME2 = DATEADD(second, - 60, @now);
    -- If Lock hasn't gone away in this amount of time then it has to be dead
    DECLARE @lockTimeoutMinutes INT = [Ethereum].[fnLockTimeoutMinutes]();
    DECLARE @lockTimeoutOffset DATETIME2 = DATEADD(MINUTE, - @lockTimeoutMinutes, @now);
    DECLARE @confirmationsToWaitFor INT = 5;

    SET XACT_ABORT ON;-- So TXN will be rolled back if there are any errors

    BEGIN TRANSACTION

    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    CREATE TABLE #riskyTransactionsToProcess (
        [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
        [TransactionHash] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
        [ContractAddress] CHAR(42) COLLATE Latin1_General_CI_AS NOT NULL,
        [EventSignature] CHAR(66) COLLATE Latin1_General_CI_AS NOT NULL,
        [EventIndex] INT NOT NULL,
        [BlockNumberFirstSeen] [int] NOT NULL,
        [EarliestBlockNumberForProcessing] BIGINT NOT NULL,
        [ConfirmationsToWaitFor] [int] NOT NULL,
        [DateCreated] [datetime2](2) NOT NULL,
        [DateLastRetried] [datetime2](2) NOT NULL,
        [RetryCount] [int] NOT NULL,
        [GasUsed] INT NOT NULL,
        [GasPrice] BIGINT NOT NULL
        );

    UPDATE TOP (1000) [Ethereum].EventTransaction
    SET DateLastRetried = @now,
        RetryCount = RetryCount + 1,
        IsRisky = 1,
        EarliestBlockNumberForProcessing = COALESCE(EarliestBlockNumberForProcessing, BlockNumberFirstSeen + @confirmationsToWaitFor),
        ConfirmationsToWaitFor = COALESCE(ConfirmationsToWaitFor, @confirmationsToWaitFor)
    OUTPUT inserted.[Network],
        inserted.[TransactionHash],
        inserted.[ContractAddress],
        inserted.[EventSignature],
        inserted.[EventIndex],
        inserted.[BlockNumberFirstSeen],
        inserted.[EarliestBlockNumberForProcessing],
        inserted.[ConfirmationsToWaitFor],
        inserted.[DateCreated],
        inserted.[DateLastRetried],
        inserted.[RetryCount],
        inserted.[GasUsed],
        inserted.[GasPrice]
    INTO #riskyTransactionsToProcess([Network], [TransactionHash], [ContractAddress], [EventSignature], [EventIndex], [BlockNumberFirstSeen], [EarliestBlockNumberForProcessing], [ConfirmationsToWaitFor], [DateCreated], [DateLastRetried], [RetryCount], [GasUsed], [GasPrice])
    WHERE Network = @network
        AND Completed = 0
        AND (
            LockedBy IS NULL
            OR LockedAt < @lockTimeoutOffset
            )
        AND (
            (
                EarliestBlockNumberForProcessing IS NOT NULL
                AND EarliestBlockNumberForProcessing <= @blockNumber
                AND (
                    DateLastRetried IS NULL
                    OR DateLastRetried < @retryPoint
                    )
                )
            OR (
                EarliestBlockNumberForProcessing IS NULL
                AND (
                    DateLastRetried IS NULL
                    OR DateLastRetried < @normalToRiskyRetryPoint
                    )
                )
            );

    SELECT DISTINCT [Network],
        [TransactionHash] AS [TransactionHash],
        [ContractAddress] AS ContractAddress,
        [EventSignature] AS EventSignature,
        EventIndex,
        [BlockNumberFirstSeen] AS [BlockNumberFirstSeen],
        [EarliestBlockNumberForProcessing] AS [EarliestBlockNumberForProcessing],
        [ConfirmationsToWaitFor],
        [DateCreated],
        [DateLastRetried],
        [RetryCount],
        [DateLastRetried] AS [DateUpdated],
        GasUsed,
        GasPrice
    FROM #riskyTransactionsToProcess P
    ORDER BY P.EarliestBlockNumberForProcessing,
        P.DateCreated;

    DROP TABLE #riskyTransactionsToProcess

    COMMIT TRANSACTION;
END;
GO


