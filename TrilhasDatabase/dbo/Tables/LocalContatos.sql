CREATE TABLE [dbo].[LocalContatos] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [LocalId]              BIGINT         NULL,
    [TipoContatoId]        BIGINT         NULL,
    [NumeroTelefone]       NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_LocalContatos] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LocalContatos_Locais_LocalId] FOREIGN KEY ([LocalId]) REFERENCES [dbo].[Locais] ([Id]),
    CONSTRAINT [FK_LocalContatos_TipoLocalContato_TipoContatoId] FOREIGN KEY ([TipoContatoId]) REFERENCES [dbo].[TipoLocalContato] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_LocalContatos_TipoContatoId]
    ON [dbo].[LocalContatos]([TipoContatoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LocalContatos_LocalId]
    ON [dbo].[LocalContatos]([LocalId] ASC);

