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
        AND [Status] = 'STARTED'
        AND [ScheduledDateForClosing] < @DateTimeOnNetwork
END
GO


