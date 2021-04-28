SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_GetRunning](
    @GameManagerContract CHAR(42)
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
    FROM [Games].[GameRound]
    WHERE [GameManagerContract] = @GameManagerContract
        AND [Status] IN ('STARTED', 'BETTING_STOPPING', 'BETTING_OVER');
END
GO


