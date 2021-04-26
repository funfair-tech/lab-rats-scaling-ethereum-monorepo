SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_MarkAsBroken] (
    @GameRoundId CHAR(66),
    @BlockNumber INT,
    @Reason VARCHAR(MAX)
    )
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION

    DECLARE @now DATETIME2 = SYSUTCDATETIME();

    UPDATE [Games].[GameRound]
    SET [Status] = 'BROKEN',
        [DateUpdated] = @now
    OUTPUT inserted.[GameRoundId],
        inserted.[Status],
        inserted.[DateUpdated],
        @blockNumber,
        NULL
    INTO [Games].[GameRoundBlockHistory]([GameRoundId],  [Status], [ChangeDate], [BlockNumber], [TransactionHash])
    WHERE GameRoundId = @GameRoundId;

    INSERT INTO [Games].[BrokenGames] (
        [GameRoundId],
        [GameContract],
        [Details],
        [DateCreated]
        )
    SELECT [GameRoundId],
        [GameContract],
        @Reason,
        @now
    FROM [Games].[GameRound]
    WHERE GameRoundId = @GameRoundId;

    COMMIT;
END
GO


