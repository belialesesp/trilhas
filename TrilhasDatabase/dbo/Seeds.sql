SET IDENTITY_INSERT Municipios ON; 
MERGE INTO Municipios AS Target 
USING (VALUES 
 (1, 3205309, 32, 'VITÓRIA', 'ES'),
 (2, 3205200, 32, 'VILA VELHA', 'ES'),
 (3, 3205002, 32, 'SERRA', 'ES'),
 (4, 3201308, 32, 'CARIACICA', 'ES')
) 
AS Source (Id, codigoMunicipio,codigoUf,NomeMunicipio,Uf) 
ON Target.Id = Source.Id 
WHEN MATCHED THEN 
UPDATE SET codigoMunicipio = Source.codigoMunicipio ,
codigoUf = Source.codigoUf ,
NomeMunicipio = Source.NomeMunicipio ,
Uf = Source.Uf 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Id, codigoMunicipio,codigoUf,NomeMunicipio,Uf) 
VALUES (Id, codigoMunicipio,codigoUf,NomeMunicipio,Uf) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;
SET IDENTITY_INSERT Municipios OFF; 



SET IDENTITY_INSERT TiposDeEntidade ON 
; 

MERGE INTO TiposDeEntidade AS Target 
USING (VALUES 
  (1, 'MUNICIPAL'), 
  (2, 'ESTADUAL'),
  (3, 'FEDERAL'),
  (4, 'ORGANIZAÇÃO NÃO GOVERNAMENTAL')
) 
AS Source (Id, Descricao) 
ON Target.Id = Source.Id 
WHEN MATCHED THEN 
UPDATE SET Descricao = Source.Descricao 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (id, Descricao) 
VALUES (id, Descricao) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

SET IDENTITY_INSERT TiposDeEntidade OFF
; 


SET IDENTITY_INSERT NiveisDeCurso ON 
; 

MERGE INTO NiveisDeCurso AS Target 
USING (VALUES 
  (1, 'CAPACITAÇÃO'), 
  (2, 'FORMAÇÃO')
) 
AS Source (Id, Descricao) 
ON Target.Id = Source.Id 
WHEN MATCHED THEN 
UPDATE SET Descricao = Source.Descricao 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (id, Descricao) 
VALUES (id, Descricao) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

SET IDENTITY_INSERT NiveisDeCurso OFF
; 

SET IDENTITY_INSERT TiposDeCurso ON 
; 

MERGE INTO TiposDeCurso AS Target 
USING (VALUES 
  (1, 'FÓRUM'), 
  (2, 'MESA REDONDA'),
  (3, 'PALESTRA'),
  (4, 'SEMINÁRIO'),
  (5, 'WORKSHOP')
) 
AS Source (Id, Descricao) 
ON Target.Id = Source.Id 
WHEN MATCHED THEN 
UPDATE SET Descricao = Source.Descricao 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (id, Descricao) 
VALUES (id, Descricao) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

SET IDENTITY_INSERT TiposDeCurso OFF
; 

SET IDENTITY_INSERT Deficiencias ON 
; 

MERGE INTO Deficiencias AS Target 
USING (VALUES 
  (1, 'FÍSICA'), 
  (2, 'AUDITIVA'), 
  (3, 'VISUAL'), 
  (4, 'MENTAL')
) 
AS Source (Id, Nome) 
ON Target.Id = Source.Id 
WHEN MATCHED THEN 
UPDATE SET Nome = Source.Nome 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (id, Nome) 
VALUES (id, Nome) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

SET IDENTITY_INSERT Deficiencias OFF
; 


SET IDENTITY_INSERT Escolaridades ON; 

MERGE INTO Escolaridades AS Target 
USING (VALUES 
  (1, 'PRIMEIRO GRAU INCOMPLETO'), 
  (2, 'PRIMEIRO GRAU COMPLETO'), 
  (3, 'ENSINO SUPERIOR INCOMPLETO'), 
  (4, 'ENSINO SUPERIOR COMPLETO'),
  (5, 'ALFABETIZADO'),
  (6, 'ANALFABETO'),
  (7, 'DOUTOR'),
  (8, 'MESTRE'),
  (9, 'PÓS GRADUADO'),
  (10, 'SEGUNDO GRAU INCOMPLETO'),
  (11, 'SEGUNDO GRAU COMPLETO')
) 
AS Source (Id, Nome) 
ON Target.Id = Source.Id 
WHEN MATCHED THEN 
UPDATE SET Nome = Source.Nome 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (id, Nome) 
VALUES (id, Nome) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

SET IDENTITY_INSERT Escolaridades OFF
; 



SET IDENTITY_INSERT OrgaoExpedidor ON; 

MERGE INTO OrgaoExpedidor AS Target 
USING (VALUES 
  (1, 'SECRETARIA DE SEGURANÇA PÚBLICA', 'SSP')
  
) 
AS Source (Id, Nome, Sigla) 
ON Target.Id = Source.Id 
WHEN MATCHED THEN 
UPDATE SET Nome = Source.Nome ,Sigla = Source.Sigla
WHEN NOT MATCHED BY TARGET THEN 
INSERT (id, Nome, Sigla) 
VALUES (id, Nome, Sigla) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

SET IDENTITY_INSERT OrgaoExpedidor OFF
; 



SET IDENTITY_INSERT Sexo ON; 

MERGE INTO Sexo AS Target 
USING (VALUES 
  (1, 'MASCULINO'),
  (2, 'FEMININO'),
  (3, 'OUTRO')
) 
AS Source (Id, Nome) 
ON Target.Id = Source.Id 
WHEN MATCHED THEN 
UPDATE SET Nome = Source.Nome 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (id, Nome) 
VALUES (id, Nome) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

SET IDENTITY_INSERT Sexo OFF
; 



SET IDENTITY_INSERT TipoLocalContato ON; 

MERGE INTO TipoLocalContato AS Target 
USING (VALUES 
  (1, 'FIXO'),
  (2, 'CELULAR'),
  (3, 'FAX')
) 
AS Source (Id, Nome) 
ON Target.Id = Source.Id 
WHEN MATCHED THEN 
UPDATE SET Nome = Source.Nome 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (id, Nome) 
VALUES (id, Nome) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

SET IDENTITY_INSERT TipoLocalContato OFF
; 


SET IDENTITY_INSERT TipoPessoaContato ON; 

MERGE INTO TipoPessoaContato AS Target 
USING (VALUES 
  (1, 'FIXO'),
  (2, 'CELULAR'),
  (3, 'FAX')
) 
AS Source (Id, Nome) 
ON Target.Id = Source.Id 
WHEN MATCHED THEN 
UPDATE SET Nome = Source.Nome 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (id, Nome) 
VALUES (id, Nome) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

SET IDENTITY_INSERT TipoPessoaContato OFF
; 

SET IDENTITY_INSERT FuncoesDocente ON; 

MERGE INTO FuncoesDocente AS Target 
USING (VALUES 
  (1, 'DOCENTE'),
  (2, 'DOCENTE ASSISTENTE'),
  (3, 'CONFERENCISTA'),
  (4, 'PALESTRANTE'),
  (5, 'PAINELISTA'),
  (6, 'DEBATEDOR'),
  (7, 'MODERADOR')
) 
AS Source (Id, Descricao) 
ON Target.Id = Source.Id 
WHEN MATCHED THEN 
UPDATE SET Descricao = Source.Descricao 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (id, Descricao) 
VALUES (id, Descricao) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

SET IDENTITY_INSERT FuncoesDocente OFF
; 