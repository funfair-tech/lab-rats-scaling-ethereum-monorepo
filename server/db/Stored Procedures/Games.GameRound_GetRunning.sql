SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_GetRunning]
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
    WHERE [Status] IN ('STARTED', 'BETTING_STOPPING', 'BETTING_OVER');
END
GO


