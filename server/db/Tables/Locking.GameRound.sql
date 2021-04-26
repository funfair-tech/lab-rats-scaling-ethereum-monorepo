CREATE TABLE [Locking].[GameRound] (
    [ObjectId] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [LockedBy] [varchar](100) COLLATE Latin1_General_CI_AS NOT NULL,
    [LockedAt] [datetime2] NOT NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [Locking].[GameRound] ADD CONSTRAINT [PK_GameRound] PRIMARY KEY CLUSTERED ([ObjectId]) ON [PRIMARY]
GO


