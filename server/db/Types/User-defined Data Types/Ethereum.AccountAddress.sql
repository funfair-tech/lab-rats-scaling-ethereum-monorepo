CREATE TYPE [Ethereum].[AccountAddress] AS TABLE (
    [AccountAddress] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    PRIMARY KEY CLUSTERED ([AccountAddress])
    )
GO


