CREATE TABLE [dbo].[Habilidades] (
    [Id]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [Descricao] NVARCHAR (MAX) NULL,
    [CursoId]   BIGINT         NULL,
    CONSTRAINT [PK_Habilidades] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Habilidades_SolucoesEducacionais_CursoId] FOREIGN KEY ([CursoId]) REFERENCES [dbo].[SolucoesEducacionais] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Habilidades_CursoId]
    ON [dbo].[Habilidades]([CursoId] ASC);


GO


