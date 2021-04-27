SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Faucet].[Minting_Success](
    @Network VARCHAR(50),
    @Address CHAR(42),
    @NativeCurrencyIssued VARCHAR(66),
    @TokenIssued VARCHAR(66),
    @IPAddress VARCHAR(45)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [Faucet].[MintingHistory]
    (
        [Network],
        [Address],
        [NativeCurrencyIssued],
        [TokenIssued],
        [IpAddress],
        [IssueDate]
    )
    VALUES
    (
        @Network,
        @Address,
        @NativeCurrencyIssued,
        @TokenIssued,
        @IpAddress,
        SYSUTCDATETIME()
    );

END
GO


