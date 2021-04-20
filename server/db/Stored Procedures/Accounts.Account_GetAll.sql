SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Accounts].[Account_GetAll]
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
    ORDER BY A.Network,
        A.AccountAddress;
END
GO


