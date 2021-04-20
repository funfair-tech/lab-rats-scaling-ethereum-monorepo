CREATE TABLE [Ethereum].[ChainTransaction] (
    [TransactionId] [bigint] NOT NULL IDENTITY(1, 1),
    [TransactionHash] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
    [Account] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [ContractAddress] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [FunctionName] [varchar](255) COLLATE Latin1_General_CI_AS NOT NULL,
    [ChainStatus] [varchar](40) COLLATE Latin1_General_CI_AS NOT NULL CONSTRAINT [DF_ChainTransaction_ChainStatus] DEFAULT('NEW'),
    [Status] [varchar](40) COLLATE Latin1_General_CI_AS NOT NULL,
    [DateCreated] [datetime2](2) NOT NULL,
    [TransactionData] [varchar](max) NOT NULL,
    [Value] [varchar](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [GasPrice] [bigint] NOT NULL,
    [EstimatedGas] [int] NOT NULL,
    [GasLimit] [int] NOT NULL,
    [Nonce] [bigint] NOT NULL,
    [ContextType] [varchar](66) COLLATE Latin1_General_CI_AS NULL,
    [ContextId] [varchar](66) COLLATE Latin1_General_CI_AS NULL,
    [BlockHash] [char](66) COLLATE Latin1_General_CI_AS NULL,
    [TransactionIndex] [int] NULL,
    [BlockNumber] [int] NULL,
    [GasUsed] [varchar](66) COLLATE Latin1_General_CI_AS NULL,
    [DateMined] [datetime2](2) NULL,
    [RetryCount] [int] NOT NULL,
    [DateLastRetried] [datetime2](2) NULL,
    [ReplacedByTransactionId] [bigint] NULL,
    [ReplacesTransactionId] [bigint] NULL,
    [ReasonForSubmission] [varchar](40) COLLATE Latin1_General_CI_AS NOT NULL CONSTRAINT [DF_ChainTransaction_ReasonForSubmission] DEFAULT('NEW'),
    [SubmissionContext] [uniqueidentifier] NOT NULL CONSTRAINT [DF_ChainTransaction_SubmissionContext] DEFAULT(newid()),
    [ReplacementCount] [int] NOT NULL CONSTRAINT DF_ChainTransaction_ReplacementCount DEFAULT((0)),
    [HasReachedMaximumGasPriceLimit] [bit] NOT NULL CONSTRAINT [DF_ChainTransaction_HasReachedMaximumGasPriceLimit] DEFAULT((0)),
    [EstimatedCost] AS ([GasPrice] * [GasLimit]) PERSISTED NOT NULL,
    [ActualCost] AS ([GasPrice] * [GasUsed]) PERSISTED,
    [Priority] [varchar](10) COLLATE Latin1_General_CI_AS NOT NULL CONSTRAINT [DF_ChainTransaction_Priority] DEFAULT('NORMAL')
    ) ON [PRIMARY]
GO

ALTER TABLE [Ethereum].[ChainTransaction] ADD CONSTRAINT [PK_ChainTransaction] PRIMARY KEY CLUSTERED ([TransactionId] DESC) ON [PRIMARY]
GO

ALTER TABLE [Ethereum].[ChainTransaction] ADD CONSTRAINT [IXU_ChainTransaction_TransactionHash] UNIQUE NONCLUSTERED ([TransactionHash]) ON [PRIMARY]
GO

ALTER TABLE [Ethereum].[ChainTransaction] ADD CONSTRAINT [FK_ChainTransaction_TransactionStatus] FOREIGN KEY ([Status]) REFERENCES [Ethereum].[TransactionStatus] ([Status])
GO

CREATE NONCLUSTERED INDEX [IX_ChainTransaction_Account] ON [Ethereum].[ChainTransaction] (
    [Network],
    [Account],
    [Status]
    ) INCLUDE ([Nonce]) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_ChainTransaction_Context] ON [Ethereum].[ChainTransaction] (
    [Network],
    [ContextType],
    [ContextId],
    [Status]
    ) ON [PRIMARY]
GO

CREATE INDEX [IX_ChainTransaction_SubmissionContext] ON [Ethereum].[ChainTransaction] (SubmissionContext) ON [PRIMARY]
GO


