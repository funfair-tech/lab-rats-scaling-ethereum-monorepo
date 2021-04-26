SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_Activate] (
    @GameRoundId CHAR(66),
    @BlockNumber INT,
    @DateStarted DATETIME2,
    @TransactionHash CHAR(66)
    )
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [Games].[GameRound]
    SET [Status] = 'STARTED',
        [DateUpdated] = SYSUTCDATETIME(),
        [DateStarted] = @DateStarted,
        [BlockNumberStarted] = @BlockNumber
    OUTPUT inserted.[GameRoundId],
        inserted.[Status],
        inserted.[DateUpdated],
        inserted.[BlockNumberStarted],
        @TransactionHash
    INTO [Games].[GameRoundBlockHistory]([GameRoundId], [Status], [ChangeDate], [BlockNumber], [TransactionHash])
    WHERE GameRoundId = @GameRoundId
        AND [Status] = 'PENDING';
END
GO


