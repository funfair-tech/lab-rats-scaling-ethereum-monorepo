SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_GetLastCompletedForNetwork] (
    @Network VARCHAR(50)
    )
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 [GameRoundId],
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
        [RoundTimeoutDuration],
        [DateCreated],
        [DateUpdated],
        [DateStarted],
        [DateClosed]
    FROM [Games].[GameRound]
    WHERE Network = @Network
        AND [Status] IN ('COMPLETED')
    ORDER BY [DateClosed] DESC
END
GO


