SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Event_ExcludeChangedLogIndexes] (
    @network VARCHAR(50),
    @transactionHash CHAR(66),
    @eventIndexes [Ethereum].[EventIndex] READONLY
    )
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    DELETE ET
    FROM [Ethereum].EventTransaction ET
    LEFT JOIN @eventIndexes I
        ON ET.Network = @network
            AND ET.TransactionHash = @transactionHash
            AND ET.EventSignature = I.EventSignature
            AND ET.ContractAddress = I.ContractAddress
            AND ET.EventIndex = I.[Index]
    WHERE ET.Network = @network
        AND ET.TransactionHash = @transactionHash
        AND ET.Completed = 0
        AND I.EventSignature IS NULL;
END
GO


