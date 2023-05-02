CREATE TABLE [dbo].[FuncoesDocente] (
    [Id]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [Descricao] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_FuncoesDocente] PRIMARY KEY CLUSTERED ([Id] ASC)
);

