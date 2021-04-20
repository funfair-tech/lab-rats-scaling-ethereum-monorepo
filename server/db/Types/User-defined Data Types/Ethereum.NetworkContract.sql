CREATE TYPE [Ethereum].[NetworkContract] AS TABLE (
    [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
    [ContractAddress] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    PRIMARY KEY CLUSTERED (
        [Network],
        [ContractAddress]
        )
    )
GO


