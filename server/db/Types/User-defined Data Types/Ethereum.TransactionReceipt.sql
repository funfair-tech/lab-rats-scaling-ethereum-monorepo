CREATE TYPE [Ethereum].[TransactionReceipt] AS TABLE (
    [TransactionHash] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [BlockHash] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [TransactionIndex] [int] NOT NULL,
    [BlockNumber] [int] NULL,
    [GasUsed] [int] NOT NULL,
    [Status] [varchar](20) COLLATE Latin1_General_CI_AS NOT NULL,
    PRIMARY KEY NONCLUSTERED ([TransactionHash])
    )
GO


