SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Players].[PlayerCount_Increment] (
    @MachineName VARCHAR(100),
    @Network VARCHAR(50)
    )
AS
MERGE Players.PlayerCount AS TARGET
USING (
    SELECT @MachineName AS MachineName,
        @Network AS Network,
        1 AS [PlayerCount]
    ) AS Source
    ON TARGET.MachineName = Source.MachineName
        AND TARGET.Network = Source.Network
WHEN MATCHED
    THEN
        UPDATE
        SET TARGET.PlayerCount = TARGET.PlayerCount + 1
WHEN NOT MATCHED
    THEN
        INSERT (
            MachineName,
            Network,
            PlayerCount
            )
        VALUES (
            Source.MachineName,
            Source.Network,
            Source.PlayerCount
            );

SELECT PlayerCount
FROM Players.PlayerCount
WHERE MachineName = @MachineName
    AND Network = @Network
GO


