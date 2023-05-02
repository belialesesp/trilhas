CREATE TABLE [dbo].[Docentes] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [PessoaId]             BIGINT         NULL,
    [Observacoes]          NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Docentes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Docentes_Pessoas_PessoaId] FOREIGN KEY ([PessoaId]) REFERENCES [dbo].[Pessoas] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Docentes_PessoaId]
    ON [dbo].[Docentes]([PessoaId] ASC);

