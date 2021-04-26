SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Players].[PlayerCount_Get] (@Network VARCHAR(50))
AS
SELECT COALESCE(SUM([PlayerCount]), 0) AS [PlayerCount]
FROM [Players].[PlayerCount](NOLOCK)
WHERE NETWORK = @Network
GO


