CREATE TABLE [dbo].[Formacao] (
    [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [DocenteId]    BIGINT         NULL,
    [Curso]        NVARCHAR (MAX) NULL,
    [Titulacao]    NVARCHAR (MAX) NULL,
    [Instituicao]  NVARCHAR (MAX) NULL,
    [CargaHoraria] INT            NOT NULL,
    [DataInicio]   DATETIME2 (7)  NOT NULL,
    [DataFim]      DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Formacao] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Formacao_Docente_DocenteId] FOREIGN KEY ([DocenteId]) REFERENCES [dbo].[Docentes] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Formacao_DocenteId]
    ON [dbo].[Formacao]([DocenteId] ASC);

