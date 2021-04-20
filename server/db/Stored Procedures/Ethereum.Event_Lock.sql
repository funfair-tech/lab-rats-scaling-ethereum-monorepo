SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Event_Lock] (
    @network VARCHAR(50),
    @contractAddress CHAR(42),
    @eventSignature CHAR(66),
    @transactionHash CHAR(66),
    @eventIndex INT,
    @machineName VARCHAR(100),
    @blockNumber INT,
    @gasUsed VARCHAR(66),
    @gasPrice VARCHAR(66),
    @strategy VARCHAR(20)
    )
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    DECLARE @now DATETIME2 = SYSUTCDATETIME();
    -- If Lock hasn't gone away in this amount of time then it has to be dead
    DECLARE @lockTimeoutMinutes INT = [Ethereum].[fnLockTimeoutMinutes]();
    DECLARE @lockTimeoutOffset DATETIME2 = DATEADD(MINUTE, - @lockTimeoutMinutes, @now);

    MERGE INTO [Ethereum].[EventTransaction] AS TARGET
    USING (
        SELECT @network AS Network,
            @contractAddress AS ContractAddress,
            @eventSignature AS EventSignature,
            @transactionHash AS TransactionHash,
            @eventIndex AS EventIndex
        ) AS SOURCE
        ON TARGET.Network = SOURCE.Network
            AND TARGET.ContractAddress = SOURCE.ContractAddress
            AND TARGET.EventSignature = SOURCE.EventSignature
            AND TARGET.TransactionHash = SOURCE.TransactionHash
            AND TARGET.EventIndex = SOURCE.EventIndex
    WHEN MATCHED
        AND Completed = 0
        AND (
            LockedAt IS NULL
            OR TARGET.LockedAt < @lockTimeoutOffset
            )
        THEN
            UPDATE
            SET LockedAt = @now,
                LockedBy = @machineName,
                DateLastRetried = @now,
                GasUsed = @gasUsed,
                GasPrice = @gasPrice
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
                EarliestBlockNumberForProcessing,
                Strategy
                )
            VALUES (
                SOURCE.Network,
                @contractAddress,
                @eventSignature,
                @transactionHash,
                SOURCE.EventIndex,
                @blockNumber,
                0, -- Completed
                @machineName,
                @now, -- LockedAt
                @now, -- DateCreated
                @now, -- DateLastRetried,
                @gasUsed,
                @gasPrice,
                0, -- IsRisky
                NULL, -- ConfirmationsToWaitFor
                NULL, -- EarliestBlockNumberForProcessing
                @strategy
                )
    OUTPUT inserted.*;
END
GO


