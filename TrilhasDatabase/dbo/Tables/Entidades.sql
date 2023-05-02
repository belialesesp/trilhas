CREATE TABLE [dbo].[Entidades] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [Nome]                 NVARCHAR (MAX) NULL,
    [Sigla]                NVARCHAR (MAX) NULL,
    [MunicipioId]          BIGINT         NULL,
    [TipoId]               BIGINT         NOT NULL,
    CONSTRAINT [PK_Entidades] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Entidades_Municipios_MunicipioId] FOREIGN KEY ([MunicipioId]) REFERENCES [dbo].[Municipios] ([Id]),
    CONSTRAINT [FK_Entidades_TiposDeEntidade_TipoId] FOREIGN KEY ([TipoId]) REFERENCES [dbo].[TiposDeEntidade] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_Entidades_MunicipioId]
    ON [dbo].[Entidades]([MunicipioId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Entidades_TipoId]
    ON [dbo].[Entidades]([TipoId] ASC);

