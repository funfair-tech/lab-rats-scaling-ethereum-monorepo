CREATE TABLE [Ethereum].[EventTransaction] (
    [EventTransactionId] [bigint] NOT NULL IDENTITY(1, 1),
    [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
    [EventIndex] [int] NOT NULL,
    [Completed] [bit] NOT NULL,
    [LockedBy] [varchar](100) COLLATE Latin1_General_CI_AS NULL,
    [LockedAt] [datetime2](2) NULL,
    [DateCreated] [datetime2](2) NOT NULL CONSTRAINT [DF_DateCreated] DEFAULT(sysutcdatetime()),
    [DateCompleted] [datetime2](2) NULL,
    [DateLastRetried] [datetime2](2) NOT NULL CONSTRAINT [DF_DateLastRetried] DEFAULT(sysutcdatetime()),
    [ContractAddress] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [EventSignature] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [TransactionHash] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [BlockNumberFirstSeen] [int] NOT NULL,
    [GasUsed] [int] NOT NULL,
    [GasPrice] [bigint] NOT NULL,
    [IsRisky] [bit] NOT NULL CONSTRAINT [DF_IsRisky] DEFAULT((0)),
    [RetryCount] [int] NOT NULL CONSTRAINT [DF_RetryCount] DEFAULT((0)),
    [ConfirmationsToWaitFor] [int] NULL,
    [EarliestBlockNumberForProcessing] [int] NULL,
    [CompletionReason] [varchar](30) COLLATE Latin1_General_CI_AS NULL,
    [Strategy] VARCHAR(20) COLLATE Latin1_General_CI_AS NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [Ethereum].[EventTransaction] ADD CONSTRAINT [CK_EventTransaction_Completed_Cannot_Be_Locked] CHECK (
    (
        [Completed] = (0)
        OR [Completed] = (1)
        AND [LockedBy] IS NULL
        AND [LockedAt] IS NULL
        )
    )
GO

ALTER TABLE [Ethereum].[EventTransaction] ADD CONSTRAINT [PK_EventTransaction] PRIMARY KEY CLUSTERED ([EventTransactionId] DESC) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_EventSignature_EventIndex_ContractAddress] ON [Ethereum].[EventTransaction] (
    [EventSignature],
    [EventIndex],
    [ContractAddress]
    ) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_LockedBy] ON [Ethereum].[EventTransaction] (
    [Network],
    [Completed],
    [DateLastRetried],
    [LockedBy],
    [LockedAt]
    ) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Network_TransactionHash] ON [Ethereum].[EventTransaction] (
    [Network],
    [TransactionHash]
    ) INCLUDE (
    [BlockNumberFirstSeen],
    [DateLastRetried],
    [LockedAt],
    [LockedBy]
    )
WHERE (
        [Completed] = (0)
        AND [LockedBy] IS NULL
        )
    ON [PRIMARY]
GO

ALTER TABLE [Ethereum].[EventTransaction] ADD CONSTRAINT [IXU_EventTransaction_TransactionHash_Network_EventSignature_EventIndex_ContractAddress] UNIQUE NONCLUSTERED (
    [TransactionHash],
    [Network],
    [EventSignature],
    [EventIndex],
    [ContractAddress]
    ) ON [PRIMARY]
GO


