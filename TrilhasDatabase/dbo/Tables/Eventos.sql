CREATE TABLE [dbo].[Eventos] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [CoordenadorPessoaId]  BIGINT         NULL,
    [CursoId]              BIGINT         NULL,
    [EntidadeDemandanteId] BIGINT         NULL,
    [LocalId]              BIGINT         NULL,
    [Observacoes]          NVARCHAR (MAX) NULL,
    [UrlEad]               NVARCHAR (MAX) NULL,
    [Finalizado]           BIT            DEFAULT ((0)) NOT NULL,
    [LimitarInscricoes]    BIT            DEFAULT ((0)) NOT NULL,
    [VagasPorEntidade]     INT            DEFAULT ((0)) NOT NULL,
    [CertificadoId]        BIGINT         NULL,
    [MotivoExclusao]       NVARCHAR (MAX) NULL,
    [DeclaracaoCursistaId] BIGINT         NULL,
    [DeclaracaoDocenteId]  BIGINT         NULL,
    CONSTRAINT [PK_Eventos] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Eventos_Certificados_CertificadoId] FOREIGN KEY ([CertificadoId]) REFERENCES [dbo].[Certificados] ([Id]),
    CONSTRAINT [FK_Eventos_Certificados_DeclaracaoCursistaId] FOREIGN KEY ([DeclaracaoCursistaId]) REFERENCES [dbo].[Certificados] ([Id]),
    CONSTRAINT [FK_Eventos_Certificados_DeclaracaoDocenteId] FOREIGN KEY ([DeclaracaoDocenteId]) REFERENCES [dbo].[Certificados] ([Id]),
    CONSTRAINT [FK_Eventos_Entidades_EntidadeDemandanteId] FOREIGN KEY ([EntidadeDemandanteId]) REFERENCES [dbo].[Entidades] ([Id]),
    CONSTRAINT [FK_Eventos_Locais_LocalId] FOREIGN KEY ([LocalId]) REFERENCES [dbo].[Locais] ([Id]),
    CONSTRAINT [FK_Eventos_Pessoas_CoordenadorPessoaId] FOREIGN KEY ([CoordenadorPessoaId]) REFERENCES [dbo].[Pessoas] ([Id]),
    CONSTRAINT [FK_Eventos_SolucoesEducacionais_CursoId] FOREIGN KEY ([CursoId]) REFERENCES [dbo].[SolucoesEducacionais] ([Id])
);










GO
CREATE NONCLUSTERED INDEX [IX_Eventos_CursoId]
    ON [dbo].[Eventos]([CursoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Eventos_LocalId]
    ON [dbo].[Eventos]([LocalId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Eventos_EntidadeDemandanteId]
    ON [dbo].[Eventos]([EntidadeDemandanteId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Eventos_CoordenadorPessoaId]
    ON [dbo].[Eventos]([CoordenadorPessoaId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Eventos_CertificadoId]
    ON [dbo].[Eventos]([CertificadoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Eventos_DeclaracaoDocenteId]
    ON [dbo].[Eventos]([DeclaracaoDocenteId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Eventos_DeclaracaoCursistaId]
    ON [dbo].[Eventos]([DeclaracaoCursistaId] ASC);

