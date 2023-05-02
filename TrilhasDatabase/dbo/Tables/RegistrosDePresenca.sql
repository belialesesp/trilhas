CREATE TABLE [dbo].[RegistrosDePresenca] (
    [Id]              BIGINT IDENTITY (1, 1) NOT NULL,
    [EventoHorarioId] BIGINT NULL,
    [PessoaId]        BIGINT NULL,
    [Presente]        BIT    NOT NULL,
    CONSTRAINT [PK_RegistrosDePresenca] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RegistrosDePresenca_EventoHorario_EventoHorarioId] FOREIGN KEY ([EventoHorarioId]) REFERENCES [dbo].[EventoHorario] ([Id]),
    CONSTRAINT [FK_RegistrosDePresenca_Pessoas_PessoaId] FOREIGN KEY ([PessoaId]) REFERENCES [dbo].[Pessoas] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_RegistrosDePresenca_PessoaId]
    ON [dbo].[RegistrosDePresenca]([PessoaId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RegistrosDePresenca_EventoHorarioId]
    ON [dbo].[RegistrosDePresenca]([EventoHorarioId] ASC);

