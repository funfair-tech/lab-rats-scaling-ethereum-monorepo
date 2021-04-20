SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Ethereum].[Transaction_MarkAsCannotSpeedUp] (
    @network VARCHAR(50),
    @transactionHash CHAR(66)
    )
AS
BEGIN
    -- Called by TransactionResubmissionProcessor
    SET NOCOUNT ON;

    UPDATE Ethereum.[ChainTransaction]
    SET HasReachedMaximumGasPriceLimit = 1
    WHERE Network = @network
        AND TransactionHash = @transactionHash
        AND STATUS = 'STALE';
END;
GO


