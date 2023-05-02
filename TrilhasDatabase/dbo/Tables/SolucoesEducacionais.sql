CREATE TABLE [dbo].[SolucoesEducacionais] (
    [Id]                          BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]                DATETIME2 (7)  NOT NULL,
    [CreatorUserId]               NVARCHAR (MAX) NULL,
    [LastModificationTime]        DATETIME2 (7)  NULL,
    [LastModifierUserId]          NVARCHAR (MAX) NULL,
    [DeletionTime]                DATETIME2 (7)  NULL,
    [DeletionUserId]              NVARCHAR (MAX) NULL,
    [EstacaoId]                   BIGINT         NULL,
    [TipoDoCursoId]               BIGINT         NULL,
    [ConteudoProgramatico]        NVARCHAR (MAX) NULL,
    [Descricao]                   NVARCHAR (MAX) NULL,
    [FrequenciaMinimaCertificado] INT            NULL,
    [FrequenciaMinimaDeclaracao]  INT            NULL,
    [NivelDoCursoId]              BIGINT         NULL,
    [PermiteCertificado]          BIT            NULL,
    [PreRequisitos]               NVARCHAR (MAX) NULL,
    [PublicoAlvo]                 NVARCHAR (MAX) NULL,
    [Sigla]                       NVARCHAR (MAX) NULL,
    [Titulo]                      NVARCHAR (MAX) NULL,
    [Autor]                       NVARCHAR (MAX) NULL,
    [DataPublicacao]              DATETIME2 (7)  NULL,
    [Edicao]                      NVARCHAR (MAX) NULL,
    [Editora]                     NVARCHAR (MAX) NULL,
    [OutrasInformacoes]           NVARCHAR (MAX) NULL,
    [Url]                         NVARCHAR (MAX) NULL,
    [TipoDeSolucao]               NVARCHAR (MAX) DEFAULT (N'') NOT NULL,
    [DataProducao]                DATETIME2 (7)  NULL,
    [Duracao]                     NVARCHAR (MAX) NULL,
    [Video_OutrasInformacoes]     NVARCHAR (MAX) NULL,
    [Responsavel]                 NVARCHAR (MAX) NULL,
    [Video_Url]                   NVARCHAR (MAX) NULL,
    [Modalidade]                  INT            NULL,
    CONSTRAINT [PK_SolucoesEducacionais] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SolucoesEducacionais_Estacoes_EstacaoId] FOREIGN KEY ([EstacaoId]) REFERENCES [dbo].[Estacoes] ([Id]),
    CONSTRAINT [FK_SolucoesEducacionais_NiveisDeCurso_NivelDoCursoId] FOREIGN KEY ([NivelDoCursoId]) REFERENCES [dbo].[NiveisDeCurso] ([Id]),
    CONSTRAINT [FK_SolucoesEducacionais_TiposDeCurso_TipoDoCursoId] FOREIGN KEY ([TipoDoCursoId]) REFERENCES [dbo].[TiposDeCurso] ([Id])
);








GO
CREATE NONCLUSTERED INDEX [IX_SolucoesEducacionais_EstacaoId]
    ON [dbo].[SolucoesEducacionais]([EstacaoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SolucoesEducacionais_TipoDoCursoId]
    ON [dbo].[SolucoesEducacionais]([TipoDoCursoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SolucoesEducacionais_NivelDoCursoId]
    ON [dbo].[SolucoesEducacionais]([NivelDoCursoId] ASC);


GO


