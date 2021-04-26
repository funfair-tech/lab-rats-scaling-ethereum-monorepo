SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_GetBlockNumberForClosingBetting] (@Network VARCHAR(50))
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 R.[BlockNumberStarted] AS BlockNumber
    FROM [Games].[GameRound] R WITH (NOLOCK)
    WHERE R.[Network] = @Network
        AND R.[Status] = 'BETTING_STOPPING'
    ORDER BY R.[BlockNumberStarted];
END
GO


