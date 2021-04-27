CREATE TABLE [Faucet].[MintingHistory] (
    [MintId] [BIGINT] IDENTITY(1, 1) NOT NULL,
    [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
    [Address] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [NativeCurrencyIssued] [varchar](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [TokenIssued] [varchar](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [IpAddress] [varchar](45) COLLATE Latin1_General_CI_AS NOT NULL,
    [IssueDate] [datetime2] NOT NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [Faucet].[MintingHistory] ADD CONSTRAINT [PK_MintingHistory] PRIMARY KEY CLUSTERED ([MintId] DESC) ON [PRIMARY]
GO


