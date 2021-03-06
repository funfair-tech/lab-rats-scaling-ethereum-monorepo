SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_GetAllForClosing] (
    @Network VARCHAR(50),
    @DateTimeOnNetwork DATETIME2
    )
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [GameRoundId],
        [GameManagerContract],
        [GameContract],
        [Network],
        [CreatedByAccount],
        [BlockNumberCreated],
        [BlockNumberStarted],
        [Status],
        [SeedCommit],
        [SeedReveal],
        [RoundDuration],
        [BettingCloseDuration],
        [RoundTimeoutDuration],
        [DateCreated],
        [DateUpdated],
        [DateStarted],
        [DateClosed]
    FROM Games.GameRound
    WHERE [Network] = @Network
        AND [Status] = 'BETTING_OVER'
        AND [ScheduledDateForClosing] < @DateTimeOnNetwork
END
GO


