CREATE TABLE [dbo].[Recursos] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [Nome]                 NVARCHAR (MAX) NULL,
    [Descricao]            NVARCHAR (MAX) NULL,
    [Custo]                DECIMAL (7, 2) NOT NULL,
    CONSTRAINT [PK_Recursos] PRIMARY KEY CLUSTERED ([Id] ASC)
);



