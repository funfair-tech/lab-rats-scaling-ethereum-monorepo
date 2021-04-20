SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Transaction_MarkAsBroken] (
    @network VARCHAR(50),
    @transactionHash CHAR(66),
    @status VARCHAR(40)
    )
AS
BEGIN
    -- Called by TransactionResubmissionProcessor
    SET NOCOUNT ON;

    DECLARE @now DATETIME2 = SYSUTCDATETIME();

    SET XACT_ABORT ON;-- So TXN will be rolled back if there are any errors

    BEGIN TRANSACTION

    DECLARE @broken TABLE (
        TransactionId BIGINT NOT NULL,
        ChainStatus VARCHAR(40) COLLATE Latin1_General_CI_AS NOT NULL,
        PRIMARY KEY (TransactionId)
        );

    UPDATE Ethereum.[ChainTransaction]
    SET STATUS = @status,
        DateLastRetried = @now,
        RetryCount = RetryCount + 1
    OUTPUT inserted.TransactionId,
        inserted.ChainStatus
    INTO @broken
    WHERE Network = @network
        AND TransactionHash = @transactionHash;

    DECLARE @transactionId BIGINT;

    SELECT @transactionId = TransactionId
    FROM @broken B
    INNER JOIN Ethereum.TransactionStatus TX
        ON B.ChainStatus = TX.STATUS
    WHERE TX.IsPending = 1;

    IF @transactionId IS NOT NULL
    BEGIN
        EXEC Ethereum.Transaction_Archive @transactionId = @transactionId;
    END

    COMMIT TRANSACTION;
END;
GO


