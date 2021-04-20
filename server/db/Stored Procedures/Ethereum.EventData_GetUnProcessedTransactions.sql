SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[EventData_GetUnProcessedTransactions] (
    @network VARCHAR(50),
    @transactions Ethereum.EventTransactionHash READONLY
    )
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    SELECT ST.TransactionHash AS TransactionHash
    FROM @transactions ST
    LEFT JOIN Ethereum.EventTransactionStatus ET WITH (
            NOLOCK,
            INDEX ([PK_EventTransactionStatus])
            )
        ON ET.TransactionHash = ST.TransactionHash
            AND ET.Network = @network
            AND ET.Total = ET.Completed
    WHERE ET.TransactionHash IS NULL;
END
GO


