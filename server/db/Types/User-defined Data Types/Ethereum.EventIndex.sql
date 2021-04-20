CREATE TYPE [Ethereum].[EventIndex] AS TABLE (
    [EventSignature] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [ContractAddress] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [Index] [int] NOT NULL,
    PRIMARY KEY CLUSTERED (
        [EventSignature],
        [ContractAddress],
        [Index]
        )
    )
GO


