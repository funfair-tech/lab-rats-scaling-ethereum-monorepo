SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Locking].[GameRound_Acquire] (
    @objectId CHAR(66),
    @machineName VARCHAR(100)
    )
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    DECLARE @now DATETIME2 = SYSUTCDATETIME();
    -- If Lock hasn't gone away in this amount of time then it has to be dead
    DECLARE @lockTimeoutMinutes INT = Locking.fnLockTimeoutMinutes();

    MERGE INTO Locking.GameRound AS TARGET
    USING (
        SELECT @objectId AS ObjectId
        ) AS SOURCE
        ON TARGET.ObjectId = SOURCE.ObjectId
    WHEN MATCHED
        AND (
            LockedAt IS NULL
            OR DATEADD(minute, @lockTimeoutMinutes, TARGET.LockedAt) < @now
            )
        THEN
            UPDATE
            SET LockedAt = @now,
                LockedBy = @machineName
    WHEN NOT MATCHED
        THEN
            INSERT (
                ObjectId,
                LockedBy,
                LockedAt
                )
            VALUES (
                SOURCE.ObjectId,
                @machineName,
                @now
                )
    OUTPUT inserted.ObjectId,
        inserted.LockedBy,
        inserted.LockedAt;
END;
GO


