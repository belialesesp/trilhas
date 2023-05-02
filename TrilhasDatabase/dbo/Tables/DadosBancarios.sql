CREATE TABLE [dbo].[DadosBancarios] (
    [Id]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [DocenteId]     BIGINT         NULL,
    [Banco]         NVARCHAR (MAX) NULL,
    [ContaCorrente] NVARCHAR (MAX) NULL,
    [Agencia]       NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_DadosBancarios] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DadosBancarios_Docente_DocenteId] FOREIGN KEY ([DocenteId]) REFERENCES [dbo].[Docentes] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_DadosBancarios_DocenteId]
    ON [dbo].[DadosBancarios]([DocenteId] ASC);

