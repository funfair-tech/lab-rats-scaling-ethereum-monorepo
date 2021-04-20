CREATE TYPE [Ethereum].[Account] AS TABLE (
    [Account] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
    PRIMARY KEY CLUSTERED (
        [Account],
        [Network]
        )
    )
GO


