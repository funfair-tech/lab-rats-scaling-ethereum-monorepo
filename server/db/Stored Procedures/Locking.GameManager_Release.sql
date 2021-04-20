SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Locking].[GameManager_Release] (@objectId CHAR(66))
AS
BEGIN
    SET NOCOUNT ON;

    DELETE
    FROM Locking.GameManager
    WHERE ObjectId = @objectId;
END
GO


