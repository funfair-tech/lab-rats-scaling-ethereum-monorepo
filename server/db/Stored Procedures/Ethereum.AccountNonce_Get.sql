SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[AccountNonce_Get] (
    @network VARCHAR(50),
    @account CHAR(42),
    @machineName VARCHAR(100)
    )
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    -- note we want the OLD value output, not the new value so TXN can start with 0
    UPDATE Ethereum.AccountNonce
    SET Nonce = Nonce + 1
    OUTPUT deleted.Nonce
    WHERE Network = @network
        AND Account = @account;
END;
GO


