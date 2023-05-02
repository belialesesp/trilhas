CREATE TABLE [dbo].[Habilitacao] (
    [Id]        BIGINT IDENTITY (1, 1) NOT NULL,
    [DocenteId] BIGINT NULL,
    [CursoId]   BIGINT NULL,
    CONSTRAINT [PK_Habilitacao] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Habilitacao_Docente_DocenteId] FOREIGN KEY ([DocenteId]) REFERENCES [dbo].[Docentes] ([Id]),
    CONSTRAINT [FK_Habilitacao_SolucoesEducacionais_CursoId] FOREIGN KEY ([CursoId]) REFERENCES [dbo].[SolucoesEducacionais] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Habilitacao_DocenteId]
    ON [dbo].[Habilitacao]([DocenteId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Habilitacao_CursoId]
    ON [dbo].[Habilitacao]([CursoId] ASC);

