SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Faucet].[Minting_Allow] @IPAddress VARCHAR(45),
    @Address CHAR(42)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;-- So TXN will be rolled back if there are any errors

    BEGIN TRANSACTION

    DECLARE @mostRecentMint DATETIME2;
    DECLARE @now DATETIME2 = SYSUTCDATETIME();
    DECLARE @mintingAllowed BIT;

    -- assume this minting is not allowed
    SET @mintingAllowed = 0;

    DECLARE @maxMintingFrequencyInMinutes INT = 20;

    SELECT @mostRecentMint = LastMint
    FROM Faucet.IpMintingHistory
    WHERE IpAddress = @IPAddress;

    SELECT @mostRecentMint = LastMint
    FROM Faucet.AddressMintingHistory
    WHERE [Address] = @Address
        AND LastMint > @mostRecentMint;

    IF (
            @mostRecentMint IS NULL
            OR DATEDIFF(MINUTE, @mostRecentMint, @now) > @maxMintingFrequencyInMinutes
            )
    BEGIN
        UPDATE Faucet.IpMintingHistory
        SET LastMint = @now
        WHERE IPAddress = @IPAddress;

        IF (@@ROWCOUNT = 0)
        BEGIN
            INSERT INTO Faucet.IpMintingHistory (
                IpAddress,
                LastMint
                )
            VALUES (
                @IPAddress,
                @now
                );
        END

        UPDATE Faucet.AddressMintingHistory
        SET LastMint = @now
        WHERE [Address] = @Address;

        IF (@@ROWCOUNT = 0)
        BEGIN
            INSERT INTO Faucet.AddressMintingHistory (
                [Address],
                LastMint
                )
            VALUES (
                @Address,
                @Now
                );
        END

        SET @mintingAllowed = 1;
    END

    SELECT @mintingAllowed AS MintingAllowed;

    COMMIT TRANSACTION;
END
GO


