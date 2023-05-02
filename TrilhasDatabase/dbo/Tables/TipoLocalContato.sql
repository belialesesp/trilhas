CREATE TABLE [dbo].[TipoLocalContato] (
    [Id]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [Nome] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TipoLocalContato] PRIMARY KEY CLUSTERED ([Id] ASC)
);

