CREATE TABLE [Games].[GameRoundPlayerWin] (
    [Id] [bigint] NOT NULL IDENTITY(1, 1),
    [GameRoundId] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [AccountAddress] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [WinAmount] [varchar](66) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [DateCreated] [datetime2] NOT NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [Games].[GameRoundPlayerWin] ADD CONSTRAINT [PK_GameRoundPlayerWins] PRIMARY KEY CLUSTERED ([Id] DESC) ON [PRIMARY]
GO


