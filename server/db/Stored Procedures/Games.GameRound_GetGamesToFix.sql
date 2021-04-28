SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_GetGamesToFix] (
    @Network VARCHAR(50),
    @DateTimeOnNetwork DATETIME2
    )
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @startGameTimeout INT = 1;
    DECLARE @closeGameTimeout INT = 1;

    SELECT G.[GameRoundId],
        G.[GameManagerContract],
        G.[GameContract],
        G.[Network],
        G.[CreatedByAccount],
        G.[BlockNumberCreated],
        G.[BlockNumberStarted],
        G.[Status],
        G.[SeedCommit],
        G.[SeedReveal],
        G.[RoundDuration],
        G.[BettingCloseDuration],
        G.[RoundTimeoutDuration],
        G.[DateCreated],
        G.[DateUpdated],
        G.[DateStarted],
        G.[DateClosed]
    FROM Games.GameRound G
    WHERE G.[Network] = @Network
        AND (
            (
                G.STATUS = 'STARTING'
                AND DATEADD(MINUTE, @startGameTimeout, G.DateUpdated) < @DateTimeOnNetwork
                )
            OR (
                G.STATUS = 'BETTING_STOPPING'
                AND DATEADD(MINUTE, @closeGameTimeout, G.DateUpdated) < @DateTimeOnNetwork
                )
            OR (
                G.STATUS = 'COMPLETING'
                AND DATEADD(MINUTE, @closeGameTimeout, G.DateUpdated) < @DateTimeOnNetwork
                )
            )
END
GO


