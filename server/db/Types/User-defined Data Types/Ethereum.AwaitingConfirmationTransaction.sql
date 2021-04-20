CREATE TYPE [Ethereum].[AwaitingConfirmationTransaction] AS TABLE (
    [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
    [ContractAddress] CHAR(42) COLLATE Latin1_General_CI_AS NOT NULL,
    [EventSignature] CHAR(66) COLLATE Latin1_General_CI_AS NOT NULL,
    [TransactionHash] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [EventIndex] INT NOT NULL,
    [BlockNumberFirstSeen] INT NOT NULL,
    [EarliestBlockNumberForProcessing] INT NOT NULL,
    [ConfirmationsToWaitFor] [int] NOT NULL,
    [GasUsed] VARCHAR(66),
    [GasPrice] VARCHAR(66) PRIMARY KEY CLUSTERED (
        [Network],
        [ContractAddress],
        [EventSignature],
        [TransactionHash],
        [EventIndex]
        )
    )
GO


