SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Locking].[GameManager_Clear] (@machineName VARCHAR(100))
AS
BEGIN
    SET NOCOUNT ON

    DELETE
    FROM Locking.GameManager
    WHERE LockedBy = @machineName;
END;
GO


