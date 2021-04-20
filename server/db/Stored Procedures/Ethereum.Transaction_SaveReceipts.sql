SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Transaction_SaveReceipts] @receipts Ethereum.TransactionReceipt READONLY
AS
BEGIN
    -- Called by TransactionPollerProcessor to save the status of transactions
    SET NOCOUNT ON;
    SET XACT_ABORT ON;-- So TXN will be rolled back if there are any errors

    BEGIN TRANSACTION

    DECLARE @errorStatuses TABLE (
        STATUS VARCHAR(40) COLLATE Latin1_General_CI_AS NOT NULL,
        PRIMARY KEY (STATUS)
        );
    DECLARE @dateMined DATETIME2 = SYSUTCDATETIME();

    INSERT INTO @errorStatuses (STATUS)
    SELECT s.[Status]
    FROM Ethereum.TransactionStatus s
    WHERE S.IsError = 1;-- If another thread has marked it as being in error, don't change its status again.

    DECLARE @changed TABLE (
        TransactionId BIGINT NOT NULL,
        Network VARCHAR(50) COLLATE Latin1_General_CI_AS NOT NULL,
        Account CHAR(42) COLLATE Latin1_General_CI_AS NOT NULL,
        Nonce BIGINT NOT NULL,
        BlockHash CHAR(66) COLLATE Latin1_General_CI_AS NULL,
        DateMined DATETIME2 NULL,
        STATUS VARCHAR(20) COLLATE Latin1_General_CI_AS NOT NULL INDEX IX_Status NONCLUSTERED,
        SubmissionContext UNIQUEIDENTIFIER NOT NULL,
        PRIMARY KEY (TransactionId)
        );

    -- Update the transactions with their new status
    UPDATE [Ethereum].[ChainTransaction]
    SET [ChainTransaction].BlockHash = Receipts.BlockHash,
        [ChainTransaction].TransactionIndex = Receipts.TransactionIndex,
        [ChainTransaction].BlockNumber = Receipts.BlockNumber,
        [ChainTransaction].GasUsed = Receipts.GasUsed,
        [ChainTransaction].DateMined = @dateMined,
        [ChainTransaction].ChainStatus = Receipts.STATUS,
        [ChainTransaction].STATUS = CASE WHEN [ChainTransaction].STATUS = 'REPLACED' THEN
                    -- ONLY replace 'REPLACED' with 'MINED' and Error status
                    CASE WHEN Receipts.STATUS = 'MINED' THEN Receipts.STATUS WHEN EXISTS (
                                SELECT 1
                                FROM @errorStatuses S
                                WHERE S.STATUS = Receipts.STATUS
                                ) THEN Receipts.STATUS ELSE [ChainTransaction].STATUS END ELSE Receipts.STATUS END
    OUTPUT inserted.TransactionId,
        inserted.Network,
        inserted.Account,
        inserted.Nonce,
        inserted.DateMined,
        inserted.BlockHash,
        inserted.STATUS,
        inserted.SubmissionContext
    INTO @changed(TransactionId, Network, Account, Nonce, DateMined, BlockHash, STATUS, SubmissionContext)
    FROM @receipts Receipts
    WHERE [ChainTransaction].TransactionHash = Receipts.TransactionHash
        AND [ChainTransaction].STATUS NOT IN (
            SELECT S.STATUS
            FROM @errorStatuses S
            );

    -- Move 'completed' transactions (mined/replaced) to transactions_complete including those for the same account
    -- and nonce
    DECLARE db_cursor CURSOR LOCAL FORWARD_ONLY READ_ONLY STATIC
    FOR
    SELECT RCT.TransactionId
    FROM Ethereum.ChainTransaction CT
    INNER JOIN Ethereum.ChainTransaction RCT
        ON RCT.Network = CT.Network
            AND RCT.Account = CT.Account
            AND RCT.Nonce = CT.Nonce
    WHERE CT.SubmissionContext IN (
            SELECT C.SubmissionContext
            FROM @changed C
            WHERE C.STATUS = 'MINED'
            )

    DECLARE @transactionId BIGINT;

    OPEN db_cursor;

    FETCH NEXT
    FROM db_cursor
    INTO @transactionId;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        BEGIN TRANSACTION

        EXEC Ethereum.Transaction_Archive @transactionId = @transactionId;

        COMMIT;

        FETCH NEXT
        FROM db_cursor
        INTO @transactionId;
    END

    CLOSE db_cursor;

    DEALLOCATE db_cursor;

    -- UPDATE AccountNonce WITH Latest Mined statistics
    UPDATE Ethereum.AccountNonce
    SET AccountNonce.LastMinedNonce = t.LastMinedNonce,
        AccountNonce.LastMinedDate = t.LastMinedDate
    FROM (
        SELECT TR.Network,
            TR.Account AS Account,
            MAX(TR.Nonce) AS LastMinedNonce,
            MAX(TR.DateMined) AS LastMinedDate
        FROM @changed TR
        WHERE TR.BlockHash IS NOT NULL
        GROUP BY TR.Network,
            TR.Account
        ) T
    WHERE AccountNonce.Network = T.Network
        AND AccountNonce.Account = T.Account;

    COMMIT TRANSACTION
END
GO


