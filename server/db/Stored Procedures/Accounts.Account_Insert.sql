SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Accounts].[Account_Insert] (
    @network VARCHAR(50),
    @accountAddress CHAR(42),
    @wallet VARCHAR(MAX),
    @tokenFundingAccountAddress CHAR(42),
    @unlock VARCHAR(200),
    @version INT,
    @enabled BIT
    )
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Accounts.Account (
        Network,
        AccountAddress,
        Wallet,
        TokenFundingAccountAddress,
        Unlock,
        Version,
        Enabled
        )
    VALUES (
        @network,
        @accountAddress,
        @wallet,
        @tokenFundingAccountAddress,
        @unlock,
        @version,
        @enabled
        );
END
GO


