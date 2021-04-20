CREATE TYPE [Ethereum].[ContractAddress] AS TABLE (
    [ContractAddress] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    PRIMARY KEY CLUSTERED ([ContractAddress])
    )
GO


