CREATE TABLE [dbo].[TipoPessoaContato] (
    [Id]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [Nome] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TipoPessoaContato] PRIMARY KEY CLUSTERED ([Id] ASC)
);

