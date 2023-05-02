CREATE TABLE [dbo].[EventoAgenda] (
    [Id]                      BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]            DATETIME2 (7)  NOT NULL,
    [CreatorUserId]           NVARCHAR (MAX) NULL,
    [LastModificationTime]    DATETIME2 (7)  NULL,
    [LastModifierUserId]      NVARCHAR (MAX) NULL,
    [DeletionTime]            DATETIME2 (7)  NULL,
    [DeletionUserId]          NVARCHAR (MAX) NULL,
    [DataHoraInicio]          DATETIME2 (7)  NOT NULL,
    [DataHoraFim]             DATETIME2 (7)  NOT NULL,
    [DataHoraInscricaoInicio] DATETIME2 (7)  NOT NULL,
    [DataHoraInscricaoFim]    DATETIME2 (7)  NOT NULL,
    [NumeroVagas]             INT            NOT NULL,
    [Justificativa]           NVARCHAR (MAX) NULL,
    [EventoId]                BIGINT         NULL,
    CONSTRAINT [PK_EventoAgenda] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EventoAgenda_Eventos_EventoId] FOREIGN KEY ([EventoId]) REFERENCES [dbo].[Eventos] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_EventoAgenda_EventoId]
    ON [dbo].[EventoAgenda]([EventoId] ASC);

