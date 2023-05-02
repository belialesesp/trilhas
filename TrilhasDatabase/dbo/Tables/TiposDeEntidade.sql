CREATE TABLE [dbo].[TiposDeEntidade] (
    [Id]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [Descricao] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TiposDeEntidade] PRIMARY KEY CLUSTERED ([Id] ASC)
);

