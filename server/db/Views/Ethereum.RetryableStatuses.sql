SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE VIEW [Ethereum].[RetryableStatuses]
    WITH SCHEMABINDING
AS
SELECT [Status]
FROM Ethereum.TransactionStatus
WHERE ShouldRetry = 1
GO

CREATE UNIQUE CLUSTERED INDEX [IX_RetryableStatuses] ON [Ethereum].[RetryableStatuses] ([Status]) ON [PRIMARY]
GO


