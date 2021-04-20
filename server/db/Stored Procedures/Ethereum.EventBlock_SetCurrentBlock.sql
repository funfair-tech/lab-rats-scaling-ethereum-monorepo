SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[EventBlock_SetCurrentBlock] (
    @network VARCHAR(50),
    @contracts [Ethereum].[ContractAddress] READONLY,
    @blockNumber INT,
    @machineName VARCHAR(100)
    )
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @now DATETIME2 = SYSUTCDATETIME();

    MERGE [Ethereum].[EventBlock] WITH (INDEX = PK_EventBlock) AS TARGET
    USING @contracts AS SOURCE
        ON TARGET.Network = @network
            AND TARGET.ContractAddress = SOURCE.ContractAddress
            AND TARGET.MachineName = @machineName
    WHEN MATCHED
        AND TARGET.CurrentBlock < @blockNumber
        THEN
            UPDATE
            SET TARGET.CurrentBlock = @blockNumber,
                TARGET.LastUpdated = @now
    WHEN NOT MATCHED
        THEN
            INSERT (
                Network,
                ContractAddress,
                StartBlock,
                CurrentBlock,
                MachineName,
                LastUpdated
                )
            VALUES (
                @network,
                SOURCE.ContractAddress,
                @blockNumber,
                @blockNumber,
                @machineName,
                @now
                );
END
GO


