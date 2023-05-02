CREATE TABLE [dbo].[TiposDeCurso] (
    [Id]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [Descricao] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TiposDeCurso] PRIMARY KEY CLUSTERED ([Id] ASC)
);

