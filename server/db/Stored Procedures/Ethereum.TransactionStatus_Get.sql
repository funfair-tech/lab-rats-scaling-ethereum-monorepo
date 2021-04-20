SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[TransactionStatus_Get] @network VARCHAR(50),
    @accountAddress CHAR(42)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;-- So TXN will be rolled back if there are any errors

    BEGIN TRANSACTION

    DECLARE @results TABLE (
        Network VARCHAR(50) COLLATE Latin1_General_CI_AS NOT NULL,
        Account CHAR(42) COLLATE Latin1_General_CI_AS NOT NULL,
        TotalTransactions BIGINT NOT NULL,
        CurrentNonce BIGINT NOT NULL,
        LastMinedNonce BIGINT,
        LastMinedDate DATETIME2 NULL,
        FirstUnMinedNonce BIGINT,
        FirstUnMinedDate DATETIME2 NULL,
        LastUnMinedNonce BIGINT,
        LastUnMinedDate DATETIME2 NULL,
        UnMinedNonceCount BIGINT NOT NULL,
        PRIMARY KEY (
            Network,
            Account
            )
        );

    -- Populate the nonces from all the accounts in dbo.AccountNonce
    INSERT INTO @results (
        Network,
        Account,
        CurrentNonce,
        UnMinedNonceCount,
        TotalTransactions,
        LastMinedNonce,
        LastMinedDate
        )
    SELECT AN.Network,
        AN.Account,
        AN.Nonce AS CurrentNonce,
        0 AS UnMinedNonceCount,
        (
            SELECT COUNT_BIG(*)
            FROM Ethereum.[ChainTransaction] T(NOLOCK)
            WHERE T.Network = AN.Network
                AND T.Account = AN.Account
            ) * (
            SELECT COUNT_BIG(*)
            FROM Ethereum.[ChainTransaction_Complete] T(NOLOCK)
            WHERE T.Network = AN.Network
                AND T.Account = AN.Account
            ) AS TotalTransactions,
        AN.LastMinedNonce,
        AN.LastMinedDate
    FROM Ethereum.AccountNonce AN
    WHERE AN.Network = @network
        AND AN.Account = @accountAddress
    GROUP BY AN.Network,
        AN.Account,
        AN.Nonce,
        AN.LastMinedNonce,
        AN.LastMinedDate;

    -- Merge in the Unmined
    UPDATE @results
    SET FirstUnMinedNonce = SOURCE.SrcFirstUnMinedNonce,
        FirstUnMinedDate = SOURCE.SrcFirstUnMinedDate,
        LastUnMinedNonce = SOURCE.SrcLastUnMinedNonce,
        LastUnMinedDate = SOURCE.SrcLastUnMinedDate,
        UnMinedNonceCount = SOURCE.SrcUnMinedNonceCount
    FROM (
        SELECT T.Network AS SrcNetwork,
            T.Account AS SrcAccount,
            MIN(Nonce) AS SrcFirstUnMinedNonce,
            MIN(T.DateCreated) AS SrcFirstUnMinedDate,
            MAX(Nonce) AS SrcLastUnMinedNonce,
            MAX(T.DateCreated) AS SrcLastUnMinedDate,
            COUNT_BIG(*) AS SrcUnMinedNonceCount
        FROM Ethereum.[ChainTransaction] T(NOLOCK)
        WHERE T.BlockHash IS NULL
            AND T.STATUS <> 'REPLACED'
        GROUP BY T.Network,
            T.Account
        ) SOURCE
    WHERE Network = SOURCE.SrcNetwork
        AND Account = SOURCE.SrcAccount;

    -- Summary Results
    SELECT Network,
        Account,
        TotalTransactions,
        CurrentNonce,
        LastMinedNonce,
        LastMinedDate,
        FirstUnMinedNonce,
        FirstUnMinedDate,
        LastUnMinedNonce,
        LastUnMinedDate,
        UnMinedNonceCount
    FROM @results
    ORDER BY Network;

    COMMIT
END
GO


