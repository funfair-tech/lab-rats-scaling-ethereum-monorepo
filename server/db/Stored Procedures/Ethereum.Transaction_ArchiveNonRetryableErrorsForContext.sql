SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].Transaction_ArchiveNonRetryableErrorsForContext (
    @contextType VARCHAR(66),
    @contextId VARCHAR(66)
    )
AS
BEGIN
    SET NOCOUNT ON;

    -- check that we're not being called directly
    IF (@@NESTLEVEL < 2)
    BEGIN
        RAISERROR (
                'Can only be called from another stored proc',
                18,
                1
                );

        RETURN;
    END;

    -- check that we're being called inside a transaction
    IF (@@TRANCOUNT = 0)
    BEGIN
        RAISERROR (
                'Must be called from within a transaction',
                18,
                1
                );

        RETURN;
    END;

    -- Move 'broken' transactions (bad instruction) to transactions_complete
    DECLARE db_cursor CURSOR LOCAL FORWARD_ONLY READ_ONLY STATIC
    FOR
    SELECT T.TransactionId
    FROM Ethereum.[ChainTransaction] T
    INNER JOIN Ethereum.TransactionStatus TS(NOLOCK)
        ON T.STATUS = TS.STATUS
    WHERE T.ContextType = @contextType
        AND T.ContextId = @contextId
        AND (
            (
                TS.IsError = 1
                AND ShouldRetry = 0
                )
            OR T.STATUS = 'BAD_INSTRUCTION'
            );

    DECLARE @brokenTransactionId BIGINT;

    OPEN db_cursor;

    FETCH NEXT
    FROM db_cursor
    INTO @brokenTransactionId;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        EXEC Ethereum.Transaction_Archive @transactionId = @brokenTransactionId;

        FETCH NEXT
        FROM db_cursor
        INTO @brokenTransactionId;
    END

    CLOSE db_cursor;

    DEALLOCATE db_cursor;
END
GO


