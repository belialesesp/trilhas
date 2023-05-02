CREATE TABLE [dbo].[ItensDaTrilha] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [TrilhaId]             BIGINT         NULL,
    [SolucaoId]            BIGINT         NULL,
    CONSTRAINT [PK_ItensDaTrilha] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ItensDaTrilha_SolucoesEducacionais_SolucaoId] FOREIGN KEY ([SolucaoId]) REFERENCES [dbo].[SolucoesEducacionais] ([Id]),
    CONSTRAINT [FK_ItensDaTrilha_Trilhas_TrilhaId] FOREIGN KEY ([TrilhaId]) REFERENCES [dbo].[Trilhas] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ItensDaTrilha_SolucaoId]
    ON [dbo].[ItensDaTrilha]([SolucaoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ItensDaTrilha_TrilhaId]
    ON [dbo].[ItensDaTrilha]([TrilhaId] ASC);

