SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_GetLastCompletedForNetwork] (@Network VARCHAR(50) @GameManagerContract CHAR(42))
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 [GameRoundId],
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
    WHERE Network = @Network
        AND [GameManagerContract] = @GameManagerContract
        AND [Status] IN ('COMPLETED')
    ORDER BY [DateClosed] DESC
END
GO


