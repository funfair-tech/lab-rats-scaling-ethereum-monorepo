SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[AccountNonce_Initialise] @network VARCHAR(50),
    @account CHAR(42),
    @nonce BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    MERGE Ethereum.AccountNonce AS TARGET
    USING (
        SELECT @network AS Network,
            @account AS Account,
            @nonce AS Nonce
        ) AS Source
        ON TARGET.Account = Source.Account
            AND TARGET.Network = Source.Network
    WHEN MATCHED
        THEN
            UPDATE
            SET TARGET.Nonce = Source.Nonce
    WHEN NOT MATCHED
        THEN
            INSERT (
                Network,
                Account,
                Nonce
                )
            VALUES (
                Source.Network,
                Source.Account,
                Source.Nonce
                );
END;
GO


