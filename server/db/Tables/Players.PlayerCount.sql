CREATE TABLE [Players].[PlayerCount] (
    [MachineName] [varchar](100) COLLATE Latin1_General_CI_AS NOT NULL,
    [Network] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
    [PlayerCount] [int] NOT NULL CONSTRAINT [DF_PlayerCount_PlayerCount] DEFAULT((0))
    ) ON [PRIMARY]
GO

ALTER TABLE [Players].[PlayerCount] ADD CONSTRAINT [PK_PlayerCount] PRIMARY KEY CLUSTERED (
    [MachineName],
    [Network]
    ) ON [PRIMARY]
GO


