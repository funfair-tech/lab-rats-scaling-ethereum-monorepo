SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_GetAllForClosingBetting] (
    @Network VARCHAR(50),
    @GameManagerContract,
    @DateTimeOnNetwork DATETIME2
    )
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [GameRoundId],
        [GameManagerContract]
        [GameContract],
        [Network],
        [CreatedByAccount],
        [BlockNumberCreated],
        [BlockNumberStarted],
        [Status],
        [SeedCommit],
        [SeedReveal],
        [RoundDuration],
        [RoundTimeoutDuration],
        [DateCreated],
        [DateUpdated],
        [DateStarted],
        [DateClosed]
    FROM Games.GameRound
    WHERE [Network] = @Network
        AND [GameManagerContract] = @GameManagerContract
        AND [Status] = 'BETTING_OVER'
        AND [ScheduledDateForClosing] < @DateTimeOnNetwork
END
GO


