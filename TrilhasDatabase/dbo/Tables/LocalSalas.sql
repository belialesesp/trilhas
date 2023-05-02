CREATE TABLE [dbo].[LocalSalas] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [LocalId]              BIGINT         NULL,
    [Sigla]                NVARCHAR (MAX) NULL,
    [Numero]               NVARCHAR (MAX) NULL,
    [Capacidade]           INT            NOT NULL,
    CONSTRAINT [PK_LocalSalas] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LocalSalas_Locais_LocalId] FOREIGN KEY ([LocalId]) REFERENCES [dbo].[Locais] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_LocalSalas_LocalId]
    ON [dbo].[LocalSalas]([LocalId] ASC);

