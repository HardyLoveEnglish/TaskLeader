/* Création d'une table temporaire pour le transfert des actions */
CREATE TEMPORARY TABLE ActionsTemp (Id,Contexte,Sujet,Titre,Liens,Deadline,Destinataire,Statut);
INSERT INTO ActionsTemp SELECT Id,Contexte,Sujet,Titre,Liens,Deadline,Destinataire,Statut FROM VueActions;

/* Mise à jour de la structure de la table Actions */
DROP TABLE Actions;
CREATE TABLE [Actions](
	[id] INTEGER NOT NULL,
	[entityID] REFERENCES [Entities]([id]),
	[entityValue] VARCHAR(500)
);

/* Création de la table Entities */
CREATE TABLE [Entities](
	[id] INTEGER PRIMARY KEY AUTOINCREMENT,
	[label] VARCHAR(50) NOT NULL UNIQUE,
	[contentType] VARCHAR(10) NOT NULL,
	[parentID] INTEGER DEFAULT 0,
	[defaultValueID] REFERENCES [Entities_values]([id]) /* Exclut de mettre des valeurs par défaut pour des entités autres que List */
);
INSERT INTO Entities (label,contentType,parentID) VALUES
	('Contexte','List',0),
	('Sujet','List',1),
	('Description','Text',0),
	('Deadline','Date',0),
	('Destinataire','List',0),
	('Statut','List',0);

/* Création de la table Entities_values */
CREATE TABLE [Entities_values](
	[id] INTEGER PRIMARY KEY AUTOINCREMENT,
	[entityID] REFERENCES [Entities]([id]),
	[label] VARCHAR(50) NOT NULL, /* Pas forcément UNIQUE (sujets identiques mais pour des contextes différents) */
	[parentID] INTEGER DEFAULT 0
);

/* Insertion des valeurs de contextes */
INSERT INTO Entities_values (entityID,label) SELECT 1,Titre FROM Contextes;
UPDATE Entities SET defaultValueID = (SELECT id FROM Entities_values WHERE label = (SELECT Titre FROM Contextes WHERE Defaut=1)) WHERE label='Contexte';
INSERT INTO Actions (id,entityID,entityValue) SELECT a.id,1,e.id FROM ActionsTemp a, Entities_values e WHERE a.Contexte=e.label;

/* Insertion des valeurs de sujets */
INSERT INTO Entities_values (entityID,label,parentID) SELECT 2,v.Titre,e.id FROM VueSujets v, Entities_values e WHERE v.Contexte=e.label;
UPDATE Entities SET defaultValueID = (SELECT id FROM Entities_values WHERE label = (SELECT Titre FROM Sujets WHERE Defaut=1)) WHERE label='Sujet';
INSERT INTO Actions (id,entityID,entityValue) SELECT a.id,2,v.id FROM ActionsTemp a, (SELECT e.id,e.label as Sujet,f.label as Contexte FROM Entities_values e JOIN Entities_values f ON e.parentID = f.id WHERE e.entityId=2) v WHERE a.Sujet=v.Sujet AND a.Contexte=v.Contexte ;

/* Insertion des descriptions */
INSERT INTO Actions (id,entityID,entityValue) SELECT id,3,Titre FROM ActionsTemp WHERE Titre NOT NULL;

/* Insertion des deadlines */
INSERT INTO Actions (id,entityID,entityValue) SELECT id,4,Deadline FROM ActionsTemp WHERE Deadline NOT NULL;

/* Insertion des destinataires */
INSERT INTO Entities_values (entityID,label) SELECT 5,Titre FROM Destinataires;
UPDATE Entities SET defaultValueID = (SELECT id FROM Entities_values WHERE label = (SELECT Titre FROM Destinataires WHERE Defaut=1)) WHERE label='Destinataire';
INSERT INTO Actions (id,entityID,entityValue) SELECT a.id,5,e.id FROM ActionsTemp a, Entities_values e WHERE a.Destinataire=e.label;

/* Insertion des statuts */
INSERT INTO Entities_values (entityID,label) SELECT 6,Titre FROM Statuts;
UPDATE Entities SET defaultValueID = (SELECT id FROM Entities_values WHERE label = (SELECT Titre FROM Statuts WHERE Defaut=1)) WHERE label='Statut';
INSERT INTO Actions (id,entityID,entityValue) SELECT a.id,6,e.id FROM ActionsTemp a, Entities_values e WHERE a.Statut=e.label;

/* Suppression des tables */
DROP TABLE ActionsTemp;
DROP TABLE Contextes;
DROP TABLE Sujets;
DROP VIEW VueActions;
DROP VIEW VueSujets;
DROP TABLE Destinataires;
DROP TABLE Statuts;

/* Mise à jour de la table VerComp */
UPDATE Properties SET Valeur='0.8' WHERE Cle='ActionsDBVer';
