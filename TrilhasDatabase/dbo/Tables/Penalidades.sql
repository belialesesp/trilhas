CREATE TABLE [dbo].[Penalidades] (
    [Id]                          BIGINT         IDENTITY (1, 1) NOT NULL,
    [PessoaId]                    BIGINT         NULL,
    [DataInicioPenalidade]        DATETIME2 (7)  NOT NULL,
    [DataFimPenalidade]           DATETIME2 (7)  NOT NULL,
    [DataDaPenalidade]            DATETIME2 (7)  DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    [JustificativaDeCancelamento] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Penalidades] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Penalidades_Pessoas_PessoaId] FOREIGN KEY ([PessoaId]) REFERENCES [dbo].[Pessoas] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Penalidades_PessoaId]
    ON [dbo].[Penalidades]([PessoaId] ASC);

