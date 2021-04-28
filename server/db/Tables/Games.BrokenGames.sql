CREATE TABLE [Games].[BrokenGames] (
    [BrokenGameId] BIGINT IDENTITY(1, 1) NOT NULL,
    [GameRoundId] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [GameContract] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [Details] [varchar](max) COLLATE Latin1_General_CI_AS NOT NULL,
    [DateCreated] [datetime2] NOT NULL,
    ) ON [PRIMARY]
GO

ALTER TABLE [Games].[BrokenGames] ADD CONSTRAINT [PK_BrokenGames] PRIMARY KEY CLUSTERED ([BrokenGameId] DESC) ON [PRIMARY]
GO


