CREATE TABLE [dbo].[OrgaoExpedidor] (
    [Id]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [Sigla] NVARCHAR (MAX) NULL,
    [Nome]  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_OrgaoExpedidor] PRIMARY KEY CLUSTERED ([Id] ASC)
);

