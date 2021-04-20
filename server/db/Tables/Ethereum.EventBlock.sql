CREATE TABLE [Ethereum].[EventBlock] (
    [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
    [MachineName] [varchar](100) COLLATE Latin1_General_CI_AS NOT NULL,
    [CurrentBlock] [int] NOT NULL,
    [LastUpdated] [datetime2](2) NOT NULL,
    [ContractAddress] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [StartBlock] [int] NOT NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [Ethereum].[EventBlock] ADD CONSTRAINT [PK_EventBlock] PRIMARY KEY CLUSTERED (
    [Network],
    [ContractAddress],
    [MachineName]
    ) ON [PRIMARY]
GO


