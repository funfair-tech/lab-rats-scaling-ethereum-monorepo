SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_GetById] (@GameRoundId CHAR(66))
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
    FROM [Games].[GameRound]
    WHERE GameRoundId = @GameRoundId;
END
GO


