CREATE TABLE [dbo].[Estacoes] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [Nome]                 NVARCHAR (MAX) NULL,
    [Descricao]            NVARCHAR (MAX) NULL,
    [EixoId]               BIGINT         NULL,
    CONSTRAINT [PK_Estacoes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Estacoes_Eixos_EixoId] FOREIGN KEY ([EixoId]) REFERENCES [dbo].[Eixos] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Estacoes_EixoId]
    ON [dbo].[Estacoes]([EixoId] ASC);

