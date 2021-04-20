CREATE TABLE [Ethereum].[TransactionStatus] (
    [Status] [varchar](40) COLLATE Latin1_General_CI_AS NOT NULL,
    [ShouldRetry] [bit] NOT NULL CONSTRAINT [DF_TransactionStatus_ShouldRetry] DEFAULT((1)),
    [IsPending] [bit] NOT NULL,
    [IsError] [bit] NOT NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [Ethereum].[TransactionStatus] ADD CONSTRAINT [PK_TransactionStatus] PRIMARY KEY CLUSTERED ([Status]) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Status_ShouldRetry] ON [Ethereum].[TransactionStatus] (
    [Status],
    [ShouldRetry]
    ) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Status_IsPending] ON [Ethereum].[TransactionStatus] (
    [Status],
    [IsPending]
    ) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Status_IsError] ON [Ethereum].[TransactionStatus] (
    [Status],
    [IsError]
    ) ON [PRIMARY]
GO


