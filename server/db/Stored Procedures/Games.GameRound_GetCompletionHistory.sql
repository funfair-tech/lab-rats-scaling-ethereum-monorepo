SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_GetCompletionHistory] (
    @GameManagerContract CHAR(42),
    @GameContract CHAR(42),
    @Items INT
    )
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@Items) R.GameRoundId,
        R.DateClosed,
        R.GameResult AS Result,
        R.History
    FROM [Games].[GameRound] R
    WHERE [GameManagerContract] = @GameManagerContract
        AND R.GameContract = @GameContract
        AND R.STATUS = 'COMPLETED'
        AND R.History IS NOT NULL
    ORDER BY R.DateClosed DESC;
END
GO


