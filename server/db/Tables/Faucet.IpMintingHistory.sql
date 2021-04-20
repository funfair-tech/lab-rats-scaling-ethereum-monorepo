CREATE TABLE [Faucet].[IpMintingHistory] (
    [IpAddress] [varchar](45) COLLATE Latin1_General_CI_AS NOT NULL,
    [LastMint] [datetime2] NOT NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [Faucet].[IpMintingHistory] ADD CONSTRAINT [PK_IpMintingHistory] PRIMARY KEY CLUSTERED ([IpAddress]) ON [PRIMARY]
GO


