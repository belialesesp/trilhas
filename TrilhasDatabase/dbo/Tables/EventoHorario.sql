CREATE TABLE [dbo].[EventoHorario] (
    [Id]             BIGINT        IDENTITY (1, 1) NOT NULL,
    [EventoId]       BIGINT        NULL,
    [ModuloId]       BIGINT        NULL,
    [DocenteId]      BIGINT        NULL,
    [LocalSalaId]    BIGINT        NULL,
    [DataHoraInicio] DATETIME2 (7) NOT NULL,
    [DataHoraFim]    DATETIME2 (7) NOT NULL,
    [FuncaoId]       BIGINT        NULL,
    CONSTRAINT [PK_EventoHorario] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EventoHorario_Docentes_DocenteId] FOREIGN KEY ([DocenteId]) REFERENCES [dbo].[Docentes] ([Id]),
    CONSTRAINT [FK_EventoHorario_Eventos_EventoId] FOREIGN KEY ([EventoId]) REFERENCES [dbo].[Eventos] ([Id]),
    CONSTRAINT [FK_EventoHorario_FuncoesDocente_FuncaoId] FOREIGN KEY ([FuncaoId]) REFERENCES [dbo].[FuncoesDocente] ([Id]),
    CONSTRAINT [FK_EventoHorario_LocalSalas_LocalSalaId] FOREIGN KEY ([LocalSalaId]) REFERENCES [dbo].[LocalSalas] ([Id]),
    CONSTRAINT [FK_EventoHorario_Modulos_ModuloId] FOREIGN KEY ([ModuloId]) REFERENCES [dbo].[Modulos] ([Id])
);






GO
CREATE NONCLUSTERED INDEX [IX_EventoHorario_EventoId]
    ON [dbo].[EventoHorario]([EventoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EventoHorario_ModuloId]
    ON [dbo].[EventoHorario]([ModuloId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EventoHorario_LocalSalaId]
    ON [dbo].[EventoHorario]([LocalSalaId] ASC);


GO



GO
CREATE NONCLUSTERED INDEX [IX_EventoHorario_DocenteId]
    ON [dbo].[EventoHorario]([DocenteId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EventoHorario_FuncaoId]
    ON [dbo].[EventoHorario]([FuncaoId] ASC);

