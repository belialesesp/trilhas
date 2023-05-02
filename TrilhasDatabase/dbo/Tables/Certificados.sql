CREATE TABLE [dbo].[Certificados] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreationTime]         DATETIME2 (7)  NOT NULL,
    [CreatorUserId]        NVARCHAR (MAX) NULL,
    [LastModificationTime] DATETIME2 (7)  NULL,
    [LastModifierUserId]   NVARCHAR (MAX) NULL,
    [DeletionTime]         DATETIME2 (7)  NULL,
    [DeletionUserId]       NVARCHAR (MAX) NULL,
    [Nome]                 NVARCHAR (MAX) NULL,
    [Dados]                NVARCHAR (MAX) NULL,
    [Padrao]               BIT            DEFAULT ((0)) NOT NULL,
    [TipoCertificado]      INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Certificados] PRIMARY KEY CLUSTERED ([Id] ASC)
);



