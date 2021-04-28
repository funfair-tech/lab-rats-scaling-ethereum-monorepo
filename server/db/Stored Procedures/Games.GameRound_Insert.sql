SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Games].[GameRound_Insert] (
    @GameRoundId CHAR(66),
    @GameManagerContract CHAR(42),
    @GameContract CHAR(42),
    @Network VARCHAR(50),
    @CreatedByAccount [char](42),
    @BlockNumberCreated INT,
    @SeedCommit CHAR(66),
    @SeedReveal CHAR(66),
    @RoundDuration INT,
    @RoundTimeoutDuration INT,
    @TransactionHash CHAR(66)
    )
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [Games].[GameRound] (
        [GameRoundId],
        [GameManagerContract],
        [GameContract],
        [CreatedByAccount],
        [Network],
        [BlockNumberCreated],
        [Status],
        [SeedCommit],
        [SeedReveal],
        [RoundDuration],
        [RoundTimeoutDuration],
        [DateCreated],
        [DateUpdated],
        [DateClosed]
        )
    OUTPUT inserted.[GameRoundId],
        inserted.[Status],
        inserted.[DateCreated],
        inserted.[BlockNumberCreated],
        @TransactionHash
    INTO [Games].[GameRoundBlockHistory]([GameRoundId], [Status], [ChangeDate], [BlockNumber], [TransactionHash])
    VALUES (
        @GameRoundId,
        @GameManagerContract,
        @GameContract,
        @CreatedByAccount,
        @Network,
        @BlockNumberCreated,
        'PENDING',
        @SeedCommit,
        @SeedReveal,
        @RoundDuration,
        @RoundTimeoutDuration,
        SYSUTCDATETIME(),
        SYSUTCDATETIME(),
        NULL
        )
END
GO


