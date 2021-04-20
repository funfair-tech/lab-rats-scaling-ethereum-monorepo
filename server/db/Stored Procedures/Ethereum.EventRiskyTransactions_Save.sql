SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[EventRiskyTransactions_Save] (@transactions [Ethereum].AwaitingConfirmationTransaction READONLY)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
    SET XACT_ABORT ON

    BEGIN TRANSACTION

    DECLARE @now DATETIME2 = SYSUTCDATETIME();

    CREATE TABLE #incompleteTransactions (
        Network VARCHAR(50) COLLATE Latin1_General_CI_AS NOT NULL,
        ContractAddress CHAR(42) COLLATE Latin1_General_CI_AS NOT NULL,
        EventSignature CHAR(66) COLLATE Latin1_General_CI_AS NOT NULL,
        TransactionHash CHAR(66) COLLATE Latin1_General_CI_AS NOT NULL,
        EventIndex INT NOT NULL,
        BlockNumberFirstSeen INT NOT NULL,
        EarliestBlockNumberForProcessing INT NULL,
        ConfirmationsToWaitFor INT NOT NULL,
        GasUsed INT NOT NULL,
        GasPrice BIGINT NOT NULL,
        PRIMARY KEY CLUSTERED (
            [Network],
            [TransactionHash],
            [EventSignature],
            [EventIndex]
            )
        );

    INSERT INTO #incompleteTransactions (
        Network,
        ContractAddress,
        EventSignature,
        TransactionHash,
        EventIndex,
        BlockNumberFirstSeen,
        EarliestBlockNumberForProcessing,
        ConfirmationsToWaitFor,
        GasPrice,
        GasUsed
        )
    SELECT DISTINCT T.Network,
        T.ContractAddress,
        T.EventSignature,
        T.TransactionHash,
        T.EventIndex,
        T.BlockNumberFirstSeen,
        T.EarliestBlockNumberForProcessing,
        T.ConfirmationsToWaitFor,
        T.GasPrice,
        T.GasUsed
    FROM @transactions T
    WHERE NOT EXISTS (
            SELECT ET.TransactionHash
            FROM [Ethereum].EventTransactionStatus ET WITH (
                    NOLOCK,
                    INDEX ([PK_EventTransactionStatus])
                    )
            WHERE T.Network = ET.Network
                AND T.TransactionHash = ET.TransactionHash
                AND ET.Completed = ET.Total
            );

    -- need to lock the table so that don't get multiple PK inserts at the same time
    MERGE INTO [Ethereum].EventTransaction AS TARGET
    USING #incompleteTransactions AS SOURCE
        ON TARGET.Network = SOURCE.Network
            AND TARGET.TransactionHash = SOURCE.TransactionHash
            AND TARGET.EventSignature = SOURCE.EventSignature
            AND TARGET.EventIndex = SOURCE.EventIndex
            AND TARGET.ContractAddress = SOURCE.ContractAddress
    WHEN MATCHED
        AND TARGET.Completed = 0
        AND (
            TARGET.EarliestBlockNumberForProcessing IS NULL
            OR TARGET.EarliestBlockNumberForProcessing < SOURCE.EarliestBlockNumberForProcessing
            )
        THEN
            UPDATE
            SET -- Don't go backwards in terms of what's being waited for, just extend.
                TARGET.EarliestBlockNumberForProcessing = SOURCE.EarliestBlockNumberForProcessing,
                TARGET.ConfirmationsToWaitFor = SOURCE.ConfirmationsToWaitFor,
                TARGET.IsRisky = 1,
                TARGET.DateLastRetried = @now
    WHEN NOT MATCHED
        THEN
            INSERT (
                Network,
                ContractAddress,
                EventSignature,
                TransactionHash,
                EventIndex,
                BlockNumberFirstSeen,
                Completed,
                LockedBy,
                LockedAt,
                DateCreated,
                DateLastRetried,
                GasUsed,
                GasPrice,
                IsRisky,
                ConfirmationsToWaitFor,
                EarliestBlockNumberForProcessing
                )
            VALUES (
                SOURCE.Network,
                SOURCE.ContractAddress,
                SOURCE.EventSignature,
                SOURCE.TransactionHash,
                SOURCE.EventIndex,
                SOURCE.BlockNumberFirstSeen,
                0, -- Completed
                NULL, -- MachineName,
                NULL, -- LockedAt
                @now, -- DateCreated
                @now, -- DateLastRetried,
                SOURCE.GasUsed, -- @gasUsed,
                SOURCE.GasPrice, --@gasPrice,
                1, -- IsRisky
                SOURCE.ConfirmationsToWaitFor,
                SOURCE.EarliestBlockNumberForProcessing
                );

    DROP TABLE #incompleteTransactions;

    COMMIT;
END;
GO


