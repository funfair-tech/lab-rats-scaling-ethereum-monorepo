SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_BeginBettingClose] (
    @GameRoundId CHAR(66),
    @BlockNumber INT,
    @TransactionHash CHAR(66)
    )
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [Games].[GameRound]
    SET [Status] = 'BETTING_STOPPING',
        [DateUpdated] = SYSUTCDATETIME()
    OUTPUT inserted.[GameRoundId],
        inserted.[Status],
        inserted.[DateUpdated],
        @blockNumber,
        @TransactionHash
    INTO [Games].[GameRoundBlockHistory]([GameRoundId], [Status], [ChangeDate], [BlockNumber], [TransactionHash])
    WHERE GameRoundId = @GameRoundId
        AND [Status] = 'STARTED';
END
GO


