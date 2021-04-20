CREATE TABLE [Faucet].[AddressMintingHistory] (
    [Address] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [LastMint] [datetime2] NOT NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [Faucet].[AddressMintingHistory] ADD CONSTRAINT [CK_AddressMintingHistory_Address_Valid] CHECK ((len(rtrim(ltrim([Address]))) = (42)))
GO

ALTER TABLE [Faucet].[AddressMintingHistory] ADD CONSTRAINT [PK_AddressMintingHistory] PRIMARY KEY CLUSTERED ([Address]) ON [PRIMARY]
GO


