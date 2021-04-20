SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Accounts].[Account_GetEnabled]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT A.Network,
        A.AccountAddress,
        A.TokenFundingAccountAddress,
        A.Wallet,
        A.Unlock,
        A.Version,
        A.Enabled
    FROM Accounts.Account A
    WHERE A.Enabled = 1
    ORDER BY A.Network,
        A.AccountAddress;
END
GO


