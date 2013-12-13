/* Création des tables temporaires */

	-- Table temporaire pour le transfert des actions
	CREATE TEMPORARY TABLE ActionsTemp (id,CtxtID,SujtID,Titre,DueDate,DestID,StatID);
	INSERT INTO ActionsTemp SELECT * FROM Actions;

	-- Table temporaire pour le transfert des valeurs des entités
	CREATE TEMPORARY TABLE [Temp_values](
		[id] INTEGER PRIMARY KEY,
		[oldID] INTEGER, 
		[entityID] INTEGER,
		[label] VARCHAR(50),
		[parentID] INTEGER DEFAULT 0
	);

	-- Création d'une table temporaire pour le transfert du contenu des filtres
	CREATE TEMPORARY TABLE Filtres_contTemp(FiltreID,FiltreType,SelectedID);
	INSERT INTO Filtres_contTemp SELECT * FROM Filtres_cont;
	
	-- Création d'une table temporaire pour le transfert des filtres
	CREATE TABLE [FiltresTemp](
		[id] INTEGER PRIMARY KEY,
		[titre] VARCHAR(100)
	);
	INSERT INTO FiltresTemp SELECT id,Titre FROM Filtres;
	
/* Mises à jour de la structure des tables */

	-- Table des actions
	DROP TABLE Actions;
	CREATE TABLE [Actions](
		[id] INTEGER NOT NULL,
		[entityID] INTEGER NOT NULL,
		[entityValue] VARCHAR(500) NOT NULL,
		PRIMARY KEY(id,entityID),
		FOREIGN KEY(entityID) REFERENCES Entities(id)
	);

	-- Table du contenu des filtres
	DROP TABLE Filtres_cont;
	CREATE TABLE [Filtres_cont](
		[filtreID] INTEGER NOT NULL,
		[entityID] INTEGER NOT NULL,
		[entityValue] VARCHAR(500),
		FOREIGN KEY(filtreID) REFERENCES Filtres(id),
		FOREIGN KEY(entityID) REFERENCES Entities(id)		
	);

	-- Table des filtres
	INSERT INTO Properties (Cle) VALUES('FiltreDefaut');
	UPDATE Properties SET Valeur=(SELECT id FROM Filtres WHERE Defaut=1) WHERE Cle='FiltreDefaut';
	DROP TABLE Filtres;
	ALTER TABLE FiltresTemp RENAME TO Filtres;
	
/* Création des tables */

	-- Entités
	CREATE TABLE [Entities](
		[id] INTEGER PRIMARY KEY,
		[label] VARCHAR(50) NOT NULL UNIQUE,
		[contentType] VARCHAR(10) NOT NULL,
		[parentID] INTEGER DEFAULT 0,
		[defaultValue] VARCHAR(500)
	);
	INSERT INTO Entities (label,contentType,parentID)
	VALUES ('Contexte','List',0),
		('Sujet','List',1),
		('Description','Text',0),
		('Deadline','Date',0),
		('Destinataire','List',0),
		('Statut','List',0);
	
	-- Valeurs des entités
	CREATE TABLE [Entities_values](
		[id] INTEGER PRIMARY KEY,
		[entityID] INTEGER NOT NULL,
		[label] VARCHAR(50) NOT NULL, /* Pas forcément UNIQUE (sujets identiques mais pour des contextes différents) */
		[parentID] INTEGER DEFAULT 0,
		FOREIGN KEY(entityID) REFERENCES Entities(id)	
	);
	
/* Migration des contextes */
	
	-- Migration des valeurs
	INSERT INTO Temp_values (oldID,entityID,label)
		SELECT id,1,Titre FROM Contextes;
	
	-- Migration de la valeur par défaut
	UPDATE Entities
	SET defaultValue = (
		SELECT id
		FROM Temp_values
		WHERE oldID = (SELECT id FROM Contextes WHERE Defaut=1) AND entityID = 1
	)
	WHERE label='Contexte';
	
	-- Migration des actions
	INSERT INTO Actions (id,entityID,entityValue)
		SELECT a.id,1,e.id
		FROM ActionsTemp a, Temp_values e
		WHERE a.CtxtID = e.oldID AND e.entityID = 1;

	-- Migration du contenu des filtres
	INSERT INTO Filtres_cont (filtreID,entityID,entityValue)
		SELECT t.FiltreID,1,e.id
		FROM Filtres_contTemp t, Temp_values e
		WHERE t.FiltreType = 'Contextes' AND t.SelectedID = e.oldID AND e.entityID = 1;
	
/* Migration des sujets */
	
	-- Migration des valeurs
	INSERT INTO Temp_values (oldID,entityID,label,parentID)
		SELECT s.id,2,s.Titre,e.id
		FROM Sujets s, Temp_values e
		WHERE s.CtxtID = e.oldID AND e.entityID = 1;
	
	-- Migration de la valeur par défaut
	UPDATE Entities
	SET defaultValue = (
		SELECT id
		FROM Temp_values
		WHERE oldID = (SELECT Titre FROM Sujets WHERE Defaut=1) AND entityID = 2
	)
	WHERE label='Sujet';

	-- Migration des actions
	INSERT INTO Actions (id,entityID,entityValue)
		SELECT a.id,2,e.id
		FROM ActionsTemp a, Temp_values e
		WHERE a.SujtID = e.oldID AND e.entityID = 2;

	-- Migration du contenu des filtres
	INSERT INTO Filtres_cont (filtreID,entityID,entityValue)
		SELECT t.FiltreID,2,e.id
		FROM Filtres_contTemp t, Temp_values e
		WHERE t.FiltreType = 'Sujets' AND t.SelectedID = e.oldID AND e.entityID = 2;
		
/* Migration des descriptions */

	INSERT INTO Actions (id,entityID,entityValue)
		SELECT id,3,Titre
		FROM ActionsTemp
		WHERE Titre NOT NULL;

/* Migration des deadlines */
	
	INSERT INTO Actions (id,entityID,entityValue)
		SELECT id,4,DueDate
		FROM ActionsTemp
		WHERE DueDate NOT NULL;

/* Migration des destinataires */

	-- Migration des valeurs
	INSERT INTO Temp_values (oldID,entityID,label)
		SELECT id,5,Titre FROM Destinataires;

	-- Migration de la valeur par défaut	
	UPDATE Entities
	SET defaultValue = (
		SELECT id
		FROM Temp_values
		WHERE oldID = (SELECT id FROM Destinataires WHERE Defaut=1) AND entityID = 5
	)
	WHERE label='Destinataire';

	-- Migration des actions
	INSERT INTO Actions (id,entityID,entityValue)
		SELECT a.id,5,e.id
		FROM ActionsTemp a, Temp_values e
		WHERE a.DestID = e.oldID AND e.entityID = 5;
	
	-- Migration du contenu des filtres
	INSERT INTO Filtres_cont (filtreID,entityID,entityValue)
		SELECT t.FiltreID,5,e.id
		FROM Filtres_contTemp t, Temp_values e
		WHERE t.FiltreType = 'Destinataires' AND t.SelectedID = e.oldID AND e.entityID = 5;
	
/* Migration des statuts */

	-- Migration des valeurs
	INSERT INTO Temp_values (oldID,entityID,label)
		SELECT id,6,Titre FROM Statuts;
		
	-- Migration de la valeur par défaut	
	UPDATE Entities
	SET defaultValue = (
		SELECT id
		FROM Temp_values
		WHERE oldID = (SELECT id FROM Statuts WHERE Defaut=1) AND entityID = 6
	)
	WHERE label='Statut';
	
	-- Migration des actions
	INSERT INTO Actions (id,entityID,entityValue)
		SELECT a.id,6,e.id
		FROM ActionsTemp a, Temp_values e
		WHERE a.StatID = e.oldID AND e.entityID = 6;
		
	-- Migration du contenu des filtres
	INSERT INTO Filtres_cont (filtreID,entityID,entityValue)
		SELECT t.FiltreID,6,e.id
		FROM Filtres_contTemp t, Temp_values e
		WHERE t.FiltreType = 'Statuts' AND t.SelectedID = e.oldID AND e.entityID = 6;

/* Migration des valeurs des entités */

	INSERT INTO Entities_values
		SELECT id,entityID,label,parentID
		FROM Temp_values;

/* Nettoyage de certaines valeurs inutiles */

	-- Actions dont la valeur de certaines entités sont vides
	DELETE FROM Actions WHERE entityValue='';
	-- Actions dont certaines valeurs correspondent à des entités vides
	DELETE FROM Actions WHERE entityValue IN (SELECT id FROM Entities_values WHERE label='');
	-- Valeurs d'entités vides
	DELETE FROM Entities_values WHERE label='';
		
/* Suppression des tables */

DROP TABLE Temp_values;
DROP TABLE Filtres_contTemp;
DROP TABLE ActionsTemp;
DROP TABLE Contextes;
DROP TABLE Sujets;
DROP VIEW VueActions;
DROP VIEW VueSujets;
DROP TABLE Destinataires;
DROP TABLE Statuts;

/* Mise à jour de la table VerComp */

UPDATE Properties SET Valeur='0.8' WHERE Cle='ActionsDBVer';
