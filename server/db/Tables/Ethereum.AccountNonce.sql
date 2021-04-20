CREATE TABLE [Ethereum].[AccountNonce] (
    [Account] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
    [Nonce] [bigint] NOT NULL,
    [LastMinedNonce] [bigint] NULL,
    [LastMinedDate] [datetime2] NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [Ethereum].[AccountNonce] ADD CONSTRAINT [CK_AccountNonce_Account_Valid] CHECK ((len(rtrim(ltrim([Account]))) = (42)))
GO

ALTER TABLE [Ethereum].[AccountNonce] ADD CONSTRAINT [CK_AccountNonce_Nonce] CHECK (([Nonce] >= (0)))
GO

ALTER TABLE [Ethereum].[AccountNonce] ADD CONSTRAINT [PK_AccountNonce] PRIMARY KEY CLUSTERED (
    [Account],
    [Network]
    ) ON [PRIMARY]
GO


