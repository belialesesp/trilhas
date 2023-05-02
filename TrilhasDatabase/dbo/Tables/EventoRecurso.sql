CREATE TABLE [dbo].[EventoRecurso] (
    [Id]         BIGINT IDENTITY (1, 1) NOT NULL,
    [RecursoId]  BIGINT NULL,
    [Quantidade] INT    NOT NULL,
    [EventoId]   BIGINT NULL,
    CONSTRAINT [PK_EventoRecurso] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EventoRecurso_Eventos_EventoId] FOREIGN KEY ([EventoId]) REFERENCES [dbo].[Eventos] ([Id]),
    CONSTRAINT [FK_EventoRecurso_Recursos_RecursoId] FOREIGN KEY ([RecursoId]) REFERENCES [dbo].[Recursos] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_EventoRecurso_RecursoId]
    ON [dbo].[EventoRecurso]([RecursoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EventoRecurso_EventoId]
    ON [dbo].[EventoRecurso]([EventoId] ASC);

