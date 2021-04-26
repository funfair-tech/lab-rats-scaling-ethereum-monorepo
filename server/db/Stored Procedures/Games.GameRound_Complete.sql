SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_Complete] (
    @GameRoundId CHAR(66),
    @BlockNumber INT,
    @TransactionHash CHAR(66),
    @WinAmounts [Games].[WinAmount] READONLY,
    @HouseWinLoss BIGINT,
    @ProgressivePotWinLoss BIGINT,
    @GameResult VARBINARY(MAX),
    @History VARBINARY(MAX)
    )
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;-- So TXN will be rolled back if there are any errors

    BEGIN TRANSACTION

    UPDATE [Games].[GameRound]
    SET [Status] = 'COMPLETED',
        [DateUpdated] = SYSUTCDATETIME(),
        [DateClosed] = SYSUTCDATETIME(),
        [HouseWinLoss] = @HouseWinLoss,
        [ProgressiveWinLoss] = @ProgressivePotWinLoss,
        [GameResult] = @GameResult,
        [History] = @History
    OUTPUT inserted.[GameRoundId],
        inserted.[Status],
        inserted.[DateUpdated],
        @blockNumber,
        @TransactionHash
    INTO [Games].[GameRoundBlockHistory]([GameRoundId], [Status], [ChangeDate], [BlockNumber], [TransactionHash])
    WHERE GameRoundId = @GameRoundId;

    INSERT INTO [Games].[GameRoundPlayerWin] (
        [GameRoundId],
        [AccountAddress],
        [WinAmount],
        [DateCreated]
        )
    SELECT @GameRoundId,
        [AccountAddress],
        [WinAmount],
        SYSUTCDATETIME()
    FROM @WinAmounts;

    COMMIT TRANSACTION
END
GO


