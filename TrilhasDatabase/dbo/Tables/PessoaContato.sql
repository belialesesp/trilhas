CREATE TABLE [dbo].[PessoaContato] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [Numero]               NVARCHAR (MAX) NULL,
    [TipoPessoaContatoId]  BIGINT         NULL,
    [PessoaId]             BIGINT         NULL,
    CONSTRAINT [PK_PessoaContato] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PessoaContato_Pessoas_PessoaId] FOREIGN KEY ([PessoaId]) REFERENCES [dbo].[Pessoas] ([Id]),
    CONSTRAINT [FK_PessoaContato_TipoPessoaContato_TipoPessoaContatoId] FOREIGN KEY ([TipoPessoaContatoId]) REFERENCES [dbo].[TipoPessoaContato] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_PessoaContato_TipoPessoaContatoId]
    ON [dbo].[PessoaContato]([TipoPessoaContatoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PessoaContato_PessoaId]
    ON [dbo].[PessoaContato]([PessoaId] ASC);

