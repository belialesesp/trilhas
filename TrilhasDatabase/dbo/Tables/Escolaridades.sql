CREATE TABLE [dbo].[Escolaridades] (
    [Id]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [Nome] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Escolaridades] PRIMARY KEY CLUSTERED ([Id] ASC)
);

