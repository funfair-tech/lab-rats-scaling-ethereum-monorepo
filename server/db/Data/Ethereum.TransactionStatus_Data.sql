INSERT INTO [Ethereum].[TransactionStatus] (
    [Status],
    [ShouldRetry],
    [IsPending],
    [IsError]
    )
VALUES (
    'ALWAYS_FAILING_AT_RESUBMIT',
    0,
    0,
    1
    )

INSERT INTO [Ethereum].[TransactionStatus] (
    [Status],
    [ShouldRetry],
    [IsPending],
    [IsError]
    )
VALUES (
    'BAD_INSTRUCTION',
    0,
    0,
    1
    )

INSERT INTO [Ethereum].[TransactionStatus] (
    [Status],
    [ShouldRetry],
    [IsPending],
    [IsError]
    )
VALUES (
    'MINED',
    0,
    0,
    0
    )

INSERT INTO [Ethereum].[TransactionStatus] (
    [Status],
    [ShouldRetry],
    [IsPending],
    [IsError]
    )
VALUES (
    'NEW',
    0,
    1,
    0
    )

INSERT INTO [Ethereum].[TransactionStatus] (
    [Status],
    [ShouldRetry],
    [IsPending],
    [IsError]
    )
VALUES (
    'RAN_OUT_OF_GAS',
    1,
    0,
    1
    )

INSERT INTO [Ethereum].[TransactionStatus] (
    [Status],
    [ShouldRetry],
    [IsPending],
    [IsError]
    )
VALUES (
    'REPLACED',
    1,
    1,
    0
    )

INSERT INTO [Ethereum].[TransactionStatus] (
    [Status],
    [ShouldRetry],
    [IsPending],
    [IsError]
    )
VALUES (
    'STALE',
    1,
    1,
    0
    )

INSERT INTO [Ethereum].[TransactionStatus] (
    [Status],
    [ShouldRetry],
    [IsPending],
    [IsError]
    )
VALUES (
    'UNKNOWN_CONTRACT_AT_RESUBMIT',
    0,
    0,
    1
    )
