CREATE TYPE [Games].[WinAmount] AS TABLE (
    [AccountAddress] [char](42) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [WinAmount] [bigint] NOT NULL,
    PRIMARY KEY CLUSTERED ([AccountAddress])
    )
GO


