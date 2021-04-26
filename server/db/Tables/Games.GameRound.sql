CREATE TABLE [Games].[GameRound] (
    [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
    [GameRoundId] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [GameContract] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [CreatedByAccount] [char](42) COLLATE Latin1_General_CI_AS NOT NULL,
    [Status] [varchar](20) COLLATE Latin1_General_CI_AS NOT NULL,
    [BlockNumberCreated] [int] NOT NULL,
    [BlockNumberStarted] [int] NULL,
    [DateStarted] [datetime2] NULL,
    [ScheduledDateForClosing] AS (dateadd(second, [RoundDuration], [DateStarted])) PERSISTED,
    [SeedCommit] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [SeedReveal] [char](66) COLLATE Latin1_General_CI_AS NOT NULL,
    [RoundDuration] [int] NOT NULL,
    [RoundTimeoutDuration] [int] NOT NULL,
    [DateCreated] [datetime2] NOT NULL,
    [DateUpdated] [datetime2] NOT NULL,
    [DateClosed] [datetime2] NULL,
    [HouseWinLoss] [bigint] NULL,
    [ProgressiveWinLoss] [bigint] NULL,
    [GameResult] [varbinary](max) NULL,
    [History] [varbinary](max) NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [Games].[GameRound] ADD CONSTRAINT [PK_GameRound] PRIMARY KEY CLUSTERED ([GameRoundId]) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_GameRound_ScheduledRoundEndDate] ON [Games].[GameRound] (
    [Network],
    [Status],
    [ScheduledDateForClosing]
    ) INCLUDE (
    [DateCreated],
    [DateStarted]
    ) ON [PRIMARY]
GO

