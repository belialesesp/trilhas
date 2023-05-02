CREATE TABLE [dbo].[Locais] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [Nome]                 NVARCHAR (MAX) NULL,
    [Observacoes]          NVARCHAR (MAX) NULL,
    [Cep]                  NVARCHAR (MAX) NULL,
    [Logradouro]           NVARCHAR (MAX) NULL,
    [Bairro]               NVARCHAR (MAX) NULL,
    [Numero]               NVARCHAR (MAX) NULL,
    [Complemento]          NVARCHAR (MAX) NULL,
    [MunicipioId]          BIGINT         NULL,
    CONSTRAINT [PK_Locais] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Locais_Municipios_MunicipioId] FOREIGN KEY ([MunicipioId]) REFERENCES [dbo].[Municipios] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Locais_MunicipioId]
    ON [dbo].[Locais]([MunicipioId] ASC);

