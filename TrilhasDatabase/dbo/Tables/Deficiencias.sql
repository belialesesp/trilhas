CREATE TABLE [dbo].[Deficiencias] (
    [Id]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [Nome] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Deficiencias] PRIMARY KEY CLUSTERED ([Id] ASC)
);

