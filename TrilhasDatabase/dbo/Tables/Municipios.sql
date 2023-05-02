CREATE TABLE [dbo].[Municipios] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [codigoMunicipio] BIGINT         NOT NULL,
    [NomeMunicipio]   NVARCHAR (MAX) NULL,
    [codigoUf]        INT            NOT NULL,
    [Uf]              NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Municipios] PRIMARY KEY CLUSTERED ([Id] ASC)
);

