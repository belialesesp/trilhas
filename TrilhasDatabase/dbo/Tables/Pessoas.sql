CREATE TABLE [dbo].[Pessoas] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [Nome]                 NVARCHAR (MAX) NULL,
    [Cpf]                  NVARCHAR (MAX) NULL,
    [NumeroFuncional]      NVARCHAR (MAX) NULL,
    [NomeSocial]           NVARCHAR (MAX) NULL,
    [DataNascimento]       DATETIME2 (7)  NOT NULL,
    [NumeroIdentidade]     NVARCHAR (MAX) NULL,
    [Email]                NVARCHAR (MAX) NULL,
    [UfIdentidade]         NVARCHAR (MAX) NULL,
    [NumeroTitulo]         NVARCHAR (MAX) NULL,
    [Pis]                  NVARCHAR (MAX) NULL,
    [FlagDeficiente]       BIT            NOT NULL,
    [DeficienciaId]        BIGINT         NULL,
    [EscolaridadeId]       BIGINT         NULL,
    [Cep]                  NVARCHAR (MAX) NULL,
    [Logradouro]           NVARCHAR (MAX) NULL,
    [Bairro]               NVARCHAR (MAX) NULL,
    [Numero]               NVARCHAR (MAX) NULL,
    [Complemento]          NVARCHAR (MAX) NULL,
    [EntidadeId]           BIGINT         NULL,
    [OrgaoExpedidorId]     BIGINT         NULL,
    [SexoId]               BIGINT         NULL,
    [MunicipioId]          BIGINT         NULL,
    CONSTRAINT [PK_Pessoas] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Pessoas_Deficiencias_DeficienciaId] FOREIGN KEY ([DeficienciaId]) REFERENCES [dbo].[Deficiencias] ([Id]),
    CONSTRAINT [FK_Pessoas_Entidades_EntidadeId] FOREIGN KEY ([EntidadeId]) REFERENCES [dbo].[Entidades] ([Id]),
    CONSTRAINT [FK_Pessoas_Escolaridades_EscolaridadeId] FOREIGN KEY ([EscolaridadeId]) REFERENCES [dbo].[Escolaridades] ([Id]),
    CONSTRAINT [FK_Pessoas_Municipios_MunicipioId] FOREIGN KEY ([MunicipioId]) REFERENCES [dbo].[Municipios] ([Id]),
    CONSTRAINT [FK_Pessoas_OrgaoExpedidor_OrgaoExpedidorId] FOREIGN KEY ([OrgaoExpedidorId]) REFERENCES [dbo].[OrgaoExpedidor] ([Id]),
    CONSTRAINT [FK_Pessoas_Sexo_SexoId] FOREIGN KEY ([SexoId]) REFERENCES [dbo].[Sexo] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Pessoas_MunicipioId]
    ON [dbo].[Pessoas]([MunicipioId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Pessoas_SexoId]
    ON [dbo].[Pessoas]([SexoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Pessoas_OrgaoExpedidorId]
    ON [dbo].[Pessoas]([OrgaoExpedidorId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Pessoas_EntidadeId]
    ON [dbo].[Pessoas]([EntidadeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Pessoas_EscolaridadeId]
    ON [dbo].[Pessoas]([EscolaridadeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Pessoas_DeficienciaId]
    ON [dbo].[Pessoas]([DeficienciaId] ASC);

