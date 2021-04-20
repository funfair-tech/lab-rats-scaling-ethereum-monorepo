CREATE TABLE [Ethereum].[ChainTransaction_Complete] (
    [TransactionId] [bigint] NOT NULL,
    [TransactionHash] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
    [Account] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [ContractAddress] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [FunctionName] [varchar](255) COLLATE Latin1_General_CI_AS NOT NULL,
    [ChainStatus] [varchar](40) COLLATE Latin1_General_CI_AS NOT NULL,
    [Status] [varchar](40) COLLATE Latin1_General_CI_AS NOT NULL,
    [DateCreated] [datetime2](2) NOT NULL,
    [TransactionData] [varchar](max) COLLATE Latin1_General_CI_AS NOT NULL,
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
    [ReasonForSubmission] [varchar](40) COLLATE Latin1_General_CI_AS NOT NULL,
    [SubmissionContext] [uniqueidentifier] NOT NULL,
    [ReplacementCount] [int] NOT NULL,
    [HasReachedMaximumGasPriceLimit] [bit] NOT NULL,
    [EstimatedCost] AS ([GasPrice] * [GasLimit]) PERSISTED NOT NULL,
    [ActualCost] AS ([GasPrice] * [GasUsed]) PERSISTED,
    [Priority] [varchar](10) COLLATE Latin1_General_CI_AS NOT NULL CONSTRAINT [DF_ChainTransactionComplete_Priority] DEFAULT('NORMAL')
    ) ON [PRIMARY]
GO

ALTER TABLE [Ethereum].[ChainTransaction_Complete] ADD CONSTRAINT [PK_ChainTransaction_Complete] PRIMARY KEY CLUSTERED ([TransactionId] DESC) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_ChainTransaction_SubmissionContext] ON [Ethereum].[ChainTransaction_Complete] ([SubmissionContext]) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_ChainTransaction_TXHash_Network] ON [Ethereum].[ChainTransaction_Complete] (
    [TransactionHash],
    [Network]
    ) ON [PRIMARY]
GO


