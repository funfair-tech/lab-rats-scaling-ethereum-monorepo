CREATE TABLE [Games].[GameRoundBlockHistory] (
    [Id] BIGINT IDENTITY(1, 1) NOT NULL,
    [GameRoundId] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [Status] [varchar](20) COLLATE Latin1_General_CI_AS NOT NULL,
    [ChangeDate] [datetime2] NOT NULL,
    [BlockNumber] [int] NOT NULL,
    [TransactionHash] [char](66) COLLATE Latin1_General_CI_AS NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [Games].[GameRoundBlockHistory] ADD CONSTRAINT [PK_GameRoundBlockHistory] PRIMARY KEY CLUSTERED ([Id] DESC) ON [PRIMARY]
GO


