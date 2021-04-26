SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_GetTransactions] (
    @GameRoundId CHAR(66),
    @FunctionName VARCHAR(255)
    )
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TransactionHash
    FROM Ethereum.ChainTransaction
    WHERE ContextType = 'GAMEROUND'
        AND ContextId = @GameRoundId
        AND FunctionName = @FunctionName
    
    UNION
    
    SELECT TransactionHash
    FROM Ethereum.ChainTransaction_Complete
    WHERE ContextType = 'GAMEROUND'
        AND ContextId = @GameRoundId;
END
GO


