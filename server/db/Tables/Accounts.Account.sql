CREATE TABLE [Accounts].[Account] (
    [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
    [AccountAddress] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [Wallet] [varchar](max) COLLATE Latin1_General_CI_AS NOT NULL,
    [TokenFundingAccountAddress] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [Unlock] [varchar](200) COLLATE Latin1_General_CI_AS NOT NULL,
    [Version] [int] NOT NULL CONSTRAINT [DF_Account_Version] DEFAULT((1)),
    [Enabled] [bit] NOT NULL,
    [DateCreated] [datetime2] NOT NULL CONSTRAINT [DF_Account_DateCreated] DEFAULT(sysutcdatetime()),
    [DateUpdated] [datetime2] NOT NULL CONSTRAINT [DF_Account_DateUpdated] DEFAULT(sysutcdatetime())
    ) ON [PRIMARY]
GO

ALTER TABLE [Accounts].[Account] ADD CONSTRAINT [CK_Account_AccountAddress_Valid] CHECK ((len(rtrim(ltrim([AccountAddress]))) = (42)))
GO

ALTER TABLE [Accounts].[Account] ADD CONSTRAINT [CK_Account_TokenFunding] CHECK (([AccountAddress] <> [TokenFundingAccountAddress]))
GO

ALTER TABLE [Accounts].[Account] ADD CONSTRAINT [CK_Account_TokenFundingAccountAddress_Valid] CHECK ((len(rtrim(ltrim([TokenFundingAccountAddress]))) = (42)))
GO

ALTER TABLE [Accounts].[Account] ADD CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED (
    [Network],
    [AccountAddress]
    ) ON [PRIMARY]
GO


