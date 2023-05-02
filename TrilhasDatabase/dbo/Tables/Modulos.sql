CREATE TABLE [dbo].[Modulos] (
    [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [Nome]         NVARCHAR (MAX) NULL,
    [Descricao]    NVARCHAR (MAX) NULL,
    [CargaHoraria] INT            NOT NULL,
    [CursoId]      BIGINT         NULL,
    CONSTRAINT [PK_Modulos] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Modulos_SolucoesEducacionais_CursoId] FOREIGN KEY ([CursoId]) REFERENCES [dbo].[SolucoesEducacionais] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Modulos_CursoId]
    ON [dbo].[Modulos]([CursoId] ASC);


GO


