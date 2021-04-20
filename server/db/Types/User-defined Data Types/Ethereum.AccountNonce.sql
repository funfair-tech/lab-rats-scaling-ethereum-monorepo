CREATE TYPE [Ethereum].[AccountNonce] AS TABLE (
    [Account] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
    [Nonce] [bigint] NOT NULL,
    PRIMARY KEY NONCLUSTERED (
        [Account],
        [Network]
        )
    )
GO


