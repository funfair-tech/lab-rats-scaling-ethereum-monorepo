SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[EventBlock_GetLatestBlocks] (
    @network VARCHAR(50),
    @contracts [Ethereum].[ContractAddress] READONLY,
    @machineName VARCHAR(100)
    )
AS
BEGIN
    SET NOCOUNT ON;

    SELECT E.Network,
        E.[ContractAddress] AS ContractAddress,
        E.[StartBlock] AS StartBlock,
        E.[CurrentBlock] AS CurrentBlock,
        E.LastUpdated
    FROM [Ethereum].[EventBlock] E WITH (NOLOCK)
    INNER JOIN @contracts C
        ON E.Network = @network
            AND E.ContractAddress = C.ContractAddress
            AND E.MachineName = @machineName;
END
GO


