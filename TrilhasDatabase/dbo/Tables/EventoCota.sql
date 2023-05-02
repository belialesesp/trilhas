CREATE TABLE [dbo].[EventoCota] (
    [Id]         BIGINT IDENTITY (1, 1) NOT NULL,
    [EntidadeId] BIGINT NULL,
    [Quantidade] INT    NOT NULL,
    [EventoId]   BIGINT NULL,
    CONSTRAINT [PK_EventoCota] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EventoCota_Entidades_EntidadeId] FOREIGN KEY ([EntidadeId]) REFERENCES [dbo].[Entidades] ([Id]),
    CONSTRAINT [FK_EventoCota_Eventos_EventoId] FOREIGN KEY ([EventoId]) REFERENCES [dbo].[Eventos] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_EventoCota_EventoId]
    ON [dbo].[EventoCota]([EventoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EventoCota_EntidadeId]
    ON [dbo].[EventoCota]([EntidadeId] ASC);

