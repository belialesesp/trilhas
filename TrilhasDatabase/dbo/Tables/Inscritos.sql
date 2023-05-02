CREATE TABLE [dbo].[Inscritos] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [PessoaId]             BIGINT         NULL,
    [DataDeInscricao]      DATETIME2 (7)  NOT NULL,
    [Frequencia]           FLOAT (53)     NOT NULL,
    [ListaDeInscricaoId]   BIGINT         NULL,
    [Situacao]             INT            DEFAULT ((0)) NOT NULL,
    [PenalidadeId]         BIGINT         NULL,
    CONSTRAINT [PK_Inscritos] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Inscritos_ListaDeInscricao_ListaDeInscricaoId] FOREIGN KEY ([ListaDeInscricaoId]) REFERENCES [dbo].[ListaDeInscricao] ([Id]),
    CONSTRAINT [FK_Inscritos_Penalidades_PenalidadeId] FOREIGN KEY ([PenalidadeId]) REFERENCES [dbo].[Penalidades] ([Id]),
    CONSTRAINT [FK_Inscritos_Pessoas_PessoaId] FOREIGN KEY ([PessoaId]) REFERENCES [dbo].[Pessoas] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Inscritos_PenalidadeId]
    ON [dbo].[Inscritos]([PenalidadeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Inscritos_PessoaId]
    ON [dbo].[Inscritos]([PessoaId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Inscritos_ListaDeInscricaoId]
    ON [dbo].[Inscritos]([ListaDeInscricaoId] ASC);

