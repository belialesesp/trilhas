CREATE TABLE [dbo].[LocalRecursos] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [LocalId]              BIGINT         NULL,
    [RecursoId]            BIGINT         NULL,
    [Quantidade]           INT            NOT NULL,
    CONSTRAINT [PK_LocalRecursos] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LocalRecursos_Locais_LocalId] FOREIGN KEY ([LocalId]) REFERENCES [dbo].[Locais] ([Id]),
    CONSTRAINT [FK_LocalRecursos_Recursos_RecursoId] FOREIGN KEY ([RecursoId]) REFERENCES [dbo].[Recursos] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_LocalRecursos_RecursoId]
    ON [dbo].[LocalRecursos]([RecursoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LocalRecursos_LocalId]
    ON [dbo].[LocalRecursos]([LocalId] ASC);

