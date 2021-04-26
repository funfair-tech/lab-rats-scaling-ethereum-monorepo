SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Players].[PlayerCount_Reset] (@MachineName VARCHAR(100))
AS
UPDATE [Players].[PlayerCount]
SET [PlayerCount] = 0
WHERE [MachineName] = @MachineName
GO


