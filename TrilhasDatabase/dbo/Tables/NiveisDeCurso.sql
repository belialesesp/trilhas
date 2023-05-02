CREATE TABLE [dbo].[NiveisDeCurso] (
    [Id]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [Descricao] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_NiveisDeCurso] PRIMARY KEY CLUSTERED ([Id] ASC)
);

