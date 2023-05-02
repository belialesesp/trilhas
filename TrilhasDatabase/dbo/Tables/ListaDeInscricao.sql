CREATE TABLE [dbo].[ListaDeInscricao] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [EventoId]             BIGINT         NULL,
    [CreationTime]         DATETIME2 (7)  DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ListaDeInscricao] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ListaDeInscricao_Evento_EventoId] FOREIGN KEY ([EventoId]) REFERENCES [dbo].[Eventos] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ListaDeInscricao_EventoId]
    ON [dbo].[ListaDeInscricao]([EventoId] ASC) WHERE ([EventoId] IS NOT NULL);

