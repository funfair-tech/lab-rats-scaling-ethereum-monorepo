SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_CanStartAGame] (
    @GameManagerContract CHAR(42),
    @InterGameDelay INT
    )
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @allGames BIT
    DECLARE @openGames BIT
    DECLARE @lastClosed DATETIME2

    SELECT @allGames = (
            SELECT TOP 1 1
            FROM [Games].[GameRound]
            WHERE [GameManagerContract] = @GameManagerContract
                AND [Status] <> 'BROKEN'
            ),
        @openGames = (
            SELECT TOP 1 1
            FROM [Games].[GameRound]
            WHERE [GameManagerContract] = @GameManagerContract
                AND [Status] IN ('PENDING', 'STARTED', 'BETTING_STOPPING', 'BETTING_OVER', 'COMPLETING')
            ),
        @lastClosed = (
            SELECT TOP 1 [DateClosed]
            FROM [Games].[GameRound]
            WHERE [GameManagerContract] = @GameManagerContract
                AND [Status] = 'COMPLETED'
            ORDER BY [DateClosed] DESC
            );

    DECLARE @canStart BIT = 0;

    IF @allGames IS NULL
    BEGIN
        SET @canStart = 1;
    END
    ELSE
    BEGIN
        IF @openGames IS NULL
            AND DATEADD(SECOND, @InterGameDelay, @lastClosed) < SYSUTCDATETIME()
        BEGIN
            SET @canStart = 1;
        END
    END;

    SELECT @canStart AS CanStart;
END
GO


