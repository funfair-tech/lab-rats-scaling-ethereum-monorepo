SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Locking].[GameRound_Release] (@objectId CHAR(66))
AS
BEGIN
    SET NOCOUNT ON;

    DELETE
    FROM Locking.GameRound
    WHERE ObjectId = @objectId;
END
GO


