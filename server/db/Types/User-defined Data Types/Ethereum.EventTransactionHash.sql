CREATE TYPE [Ethereum].[EventTransactionHash] AS TABLE (
    [TransactionHash] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    PRIMARY KEY CLUSTERED ([TransactionHash])
    )
GO


