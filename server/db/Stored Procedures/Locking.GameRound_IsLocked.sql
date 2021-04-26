SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Locking].[GameRound_IsLocked] (@objectId CHAR(66))
AS
BEGIN
    SET NOCOUNT ON;

    SELECT L.ObjectId,
        L.LockedBy,
        L.LockedAt
    FROM Locking.GameRound L
    WHERE ObjectId = @objectId;
END
GO


