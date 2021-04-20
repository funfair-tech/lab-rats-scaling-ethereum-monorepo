SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE VIEW [Ethereum].[EventTransactionStatus]
    WITH SCHEMABINDING
AS
SELECT ET.Network,
    ET.TransactionHash,
    SUM(CAST(ET.Completed AS INT)) AS Completed,
    COUNT_BIG(*) AS Total
FROM Ethereum.EventTransaction ET
GROUP BY ET.Network,
    ET.TransactionHash;
GO

CREATE UNIQUE CLUSTERED INDEX [PK_EventTransactionStatus] ON [Ethereum].[EventTransactionStatus] (
    [Network],
    [TransactionHash]
    ) ON [PRIMARY]
GO


