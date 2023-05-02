CREATE TABLE [dbo].[Gestores] (
    [Id]         BIGINT IDENTITY (1, 1) NOT NULL,
    [PessoaId]   BIGINT NULL,
    [EntidadeId] BIGINT NULL,
    CONSTRAINT [PK_Gestores] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Gestores_Entidades_EntidadeId] FOREIGN KEY ([EntidadeId]) REFERENCES [dbo].[Entidades] ([Id]),
    CONSTRAINT [FK_Gestores_Pessoas_PessoaId] FOREIGN KEY ([PessoaId]) REFERENCES [dbo].[Pessoas] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Gestores_PessoaId]
    ON [dbo].[Gestores]([PessoaId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Gestores_EntidadeId]
    ON [dbo].[Gestores]([EntidadeId] ASC);

