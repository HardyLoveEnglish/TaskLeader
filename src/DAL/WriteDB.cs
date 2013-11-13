using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using TaskLeader.BO;
using TaskLeader.GUI;
using System.IO;

namespace TaskLeader.DAL
{
    /// <summary>
    /// Classe permettant de stocker des valeurs des entités de type List
    /// </summary>
    public class EntityValue
    {
        /// <summary>
        /// Id de la valeur dans la  base
        /// </summary>
        public int id {get; set;}

        /// <summary>
        /// String représentant la valeur
        /// </summary>
        public String value { get; set; }

        public String ToString(){
            return value;
        }

        public static bool operator ==(EntityValue a, EntityValue b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
                return true;

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
                return false;

            // Return true if the fields match:
            return (a.id == b.id && (a.id > 0 || a.value == b.value));
        }

        public static bool operator !=(EntityValue a, EntityValue b)
        {
            return !(a == b);
        }
    }

    public partial class DB
    {
        // Méthode générique pour exécuter une requête sql fournie en paramètre, retourne le nombre de lignes modifiées
        public int execSQL(String requete)
        {
            // Si le mode debug est activé, on affiche les requêtes
            if (System.Configuration.ConfigurationManager.AppSettings["debugMode"] == "true")
                MessageBox.Show(requete, "Requête", MessageBoxButtons.OK, MessageBoxIcon.Information);

            try
            {
                using (SQLiteConnection SQLC = new SQLiteConnection(this._connectionString))
                {
                    if (File.Exists(this.path))
                        SQLC.Open();
                    else
                        throw new Exception("Base inaccessible");

                    using (SQLiteCommand SQLCmd = new SQLiteCommand(SQLC))
                    {
                        // Création d'une nouvelle commande à partir de la connexion
                        SQLCmd.CommandText = requete;
                        //Exécution de la commande
                        return SQLCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(Ex.Message, "Exception sur execSQL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return -1;
            }
        }

        // Insertion d'une valeur par défaut
        public void insertDefaut(Dictionary<int,String> values)
        {
            String requete = "";
            foreach (var kvp in values)
                requete += "UPDATE Entities SET defaultValue=" + kvp.Value + " WHERE id=" + kvp.Key + ";";
            //TODO: vérifier ce que rendra l'IHM: int, string ?
            execSQL(requete);
        }

        // Insertion en base d'un nouveau filtre
        public void insertFiltre(Filtre filtre)
        {
            using (SQLiteConnection SQLC = new SQLiteConnection(this._connectionString))
            {
                if (File.Exists(this.path))
                    SQLC.Open();
                else
                    throw new Exception("Base inaccessible");

                using (SQLiteTransaction mytransaction = SQLC.BeginTransaction())
                {
                    using (SQLiteCommand SQLCmd = new SQLiteCommand(SQLC))
                    {
                        // On insère le nom du filtre
                        String nomFiltre = "'" + filtre.nom.Replace("'", "''") + "'"; // Le titre du filtre ne doit pas contenir de quote
                        SQLCmd.CommandText = "INSERT INTO Filtres (titre) VALUES (" + nomFiltre + ");";
                        SQLCmd.ExecuteNonQuery();

                        // On insère ensuite dans les tables annexes les données sélectionnées
                        foreach (Criterium critere in filtre.criteria)
                        {
                            String selection;

                            // On crée la requête pour insertion des critères dans les tables annexes
                            String requete = "INSERT INTO Filtres_cont VALUES (";
                            requete += "(SELECT max(id) FROM Filtres)," + critere.entityID + ",(SELECT id FROM Entities_values WHERE Titre=@Titre));";
                            // On récupère le rowid du filtre frâichement créé
                            //TODO: il faudrait les ID des valeurs plutôt que les labels !!

                            SQLCmd.CommandText = requete;
                            SQLiteParameter p_Titre = new SQLiteParameter("@Titre");
                            SQLCmd.Parameters.Add(p_Titre);

                            foreach (String item in critere.valuesSelected)
                            {
                                selection = item.Replace("'", "''"); // On gère les simple quote
                                p_Titre.Value = selection;
                                SQLCmd.ExecuteNonQuery();
                            }
                        }
                    }
                    mytransaction.Commit();
                }
            }


            this.OnNewValue("Filtre");
            // On affiche un message de statut sur la TrayIcon
            TrayIcon.afficheMessage("Bilan création/modification", "Nouveau filtre ajouté: " + filtre.nom);

        }

        // Méthode générique d'insertion de certaines DBentity
        public int insert(DBentity entity, String value, String parentValue = "")
        {
            String sqlValue = "'" + value.Replace("'", "''") + "'";
            String parent = "'" + parentValue.Replace("'", "''") + "'";
            String requete;

            if (entity.parentID == 0)
                requete = "INSERT INTO Entities_values (entityID,label) VALUES (" + entity.id + "," + sqlValue + ");";
            else
                requete = "INSERT INTO Entities_values (entityID,label,parentID)" +
                            " SELECT " + entity.id + "," + sqlValue + ",P.id " +
                            " FROM Entities_values P" +
                            " WHERE P.label = " + parent + " AND P.entityID=" + entity.parentID + ";";

            int result = execSQL(requete);
            if (result == 1)
                this.OnNewValue(entity.nom);

            return result;
        }

        #region Pièces jointes

        // Insertion d'un nouveau mail en base
        public String insertMail(Mail mail)
        {
            String EncID = "";
            String titre = "'" + mail.Titre.Replace("'", "''") + "'";

            using (SQLiteConnection SQLC = new SQLiteConnection(this._connectionString))
            {
                if (File.Exists(this.path))
                    SQLC.Open();
                else
                    throw new Exception("Base inaccessible");

                using (SQLiteTransaction mytransaction = SQLC.BeginTransaction())
                {
                    using (SQLiteCommand SQLCmd = new SQLiteCommand(SQLC))
                    {
                        // Insertion du mail
                        SQLCmd.CommandText = "INSERT INTO Mails (Titre,StoreID,EntryID,MessageID) ";
                        SQLCmd.CommandText += "VALUES(" + titre + "," + mail.StoreIDSQL + "," + mail.EntryIDSQL + "," + mail.MessageIDSQL + ");";
                        SQLCmd.ExecuteNonQuery();

                        // Récupération du EncID
                        SQLCmd.CommandText = "SELECT max(id) FROM Mails;";
                        EncID = SQLCmd.ExecuteScalar().ToString();
                    }
                    mytransaction.Commit();
                }
            }
            return EncID;
        }

        // Insertion d'un nouveau lien en base
        public String insertLink(Link lien)
        {
            String EncID = "";
            String titre = "'" + lien.Titre.Replace("'", "''") + "'";

            using (SQLiteConnection SQLC = new SQLiteConnection(this._connectionString))
            {
                if (File.Exists(this.path))
                    SQLC.Open();
                else
                    throw new Exception("Base inaccessible");

                using (SQLiteTransaction mytransaction = SQLC.BeginTransaction())
                {
                    using (SQLiteCommand SQLCmd = new SQLiteCommand(SQLC))
                    {
                        // Insertion du mail
                        SQLCmd.CommandText = "INSERT INTO Links (Titre,Path) ";
                        SQLCmd.CommandText += "VALUES(" + titre + "," + lien.urlSQL + ");";
                        SQLCmd.ExecuteNonQuery();

                        // Récupération du EncID
                        SQLCmd.CommandText = "SELECT max(id) FROM Links;";
                        EncID = SQLCmd.ExecuteScalar().ToString();
                    }
                    mytransaction.Commit();
                }
            }
            return EncID;
        }

        // Insertion des PJ
        public void insertPJ(String actionID, List<Enclosure> PJ)
        {
            String requete;

            foreach (Enclosure enc in PJ)
            {
                //Enregistrement de la PJ dans la bonne table et récupération de l'ID
                String EncID = "";

                switch (enc.Type)
                {
                    case ("Mails"):
                        EncID = this.insertMail((Mail)enc);
                        break;
                    case ("Links"):
                        EncID = this.insertLink((Link)enc);
                        break;
                }

                // Création de la requête
                requete = "INSERT INTO Enclosures VALUES(";
                requete += actionID + ",";
                requete += "'" + enc.Type + "',";
                requete += EncID + ");";

                execSQL(requete);
                this.OnActionEdited(actionID);
            }
        }

        // Suppression d'une liste de PJs de la base (toutes liées à la même action)
        public void removePJ(String actionID, List<Enclosure> PJ)
        {
            foreach (Enclosure pj in PJ)
            {
                using (SQLiteConnection SQLC = new SQLiteConnection(this._connectionString))
                {
                    if (File.Exists(this.path))
                        SQLC.Open();
                    else
                        throw new Exception("Base inaccessible");

                    using (SQLiteTransaction mytransaction = SQLC.BeginTransaction())
                    {
                        using (SQLiteCommand SQLCmd = new SQLiteCommand(SQLC))
                        {
                            // Suppression de la pj dans la table correspondante
                            SQLCmd.CommandText = "DELETE FROM " + pj.Type + " WHERE id=" + pj.ID + ";";
                            SQLCmd.ExecuteNonQuery();

                            // Suppression de l'entrée dans la table de correspondance PJ/Action
                            SQLCmd.CommandText = "DELETE FROM Enclosures WHERE EncID=" + pj.ID + ";";
                            SQLCmd.ExecuteNonQuery();
                        }
                        mytransaction.Commit();
                    }
                }
            }

            if (PJ.Count > 0)
                this.OnActionEdited(actionID);
        }

        // Mise à jour d'une liste de PJs
        public void renamePJ(String actionID, List<Enclosure> PJ)
        {
            foreach (Enclosure pj in PJ)
            {
                using (SQLiteConnection SQLC = new SQLiteConnection(this._connectionString))
                {
                    if (File.Exists(this.path))
                        SQLC.Open();
                    else
                        throw new Exception("Base inaccessible");

                    using (SQLiteTransaction mytransaction = SQLC.BeginTransaction())
                    {
                        using (SQLiteCommand SQLCmd = new SQLiteCommand(SQLC))
                        {
                            // Suppression de la pj dans la table correspondante
                            SQLCmd.CommandText = "UPDATE " + pj.Type + " SET Titre='" + pj.Titre + "' WHERE id=" + pj.ID + ";";
                            SQLCmd.ExecuteNonQuery();
                        }
                        mytransaction.Commit();
                    }
                }
            }

            if (PJ.Count > 0)
                this.OnActionEdited(actionID);
        }

        #endregion

        // Insertion d'une nouvelle action
        // Renvoie l'ID de stockage de l'action
        public String insertAction(TLaction action)
        {
            String actionID;

            using (SQLiteConnection SQLC = new SQLiteConnection(this._connectionString))
            {
                if (File.Exists(this.path))
                    SQLC.Open();
                else
                    throw new Exception("Base inaccessible");

                using (SQLiteTransaction mytransaction = SQLC.BeginTransaction())
                {
                    using (SQLiteCommand SQLCmd = new SQLiteCommand(SQLC))
                    {
                        foreach (int entityID in this.entities.Keys)
                        {
                            switch(this.entities[entityID].type){ //TODO: à part le if tout semble identique ! à optimiser
                                case("List"):
                                    if (action.getValue(entityID) != "") //TODO: KO: il faut insérer l'ID de l'entityValue, pas son label
                                        SQLCmd.CommandText += "INSERT INTO Actions (id,entityID,entityValue)" +
                                            "SELECT max(id)+1," + entityID + "," + action.getSQLvalue(this.entities[entityID]) + " FROM Actions;";
                                    break;
                                case ("Text"):
                                    if (action.getValue(entityID) != "")
                                        SQLCmd.CommandText += "INSERT INTO Actions (id,entityID,entityValue)" +
                                            "SELECT max(id)+1," + entityID + "," + action.getSQLvalue(this.entities[entityID]) + " FROM Actions;";
                                    break;
                                case ("Date"):
                                    if (((DateTime)action.getValue(entityID)).Date != DateTime.MinValue.Date)
                                        SQLCmd.CommandText += "INSERT INTO Actions (id,entityID,entityValue)" +
                                            "SELECT max(id)+1," + entityID + "," + action.getSQLvalue(this.entities[entityID]) + " FROM Actions;";
                                    break;
                            }
                        }
                        SQLCmd.ExecuteNonQuery();

                        SQLCmd.CommandText = "SELECT max(id) FROM Actions;";
                        actionID = SQLCmd.ExecuteScalar().ToString();
                    }
                    mytransaction.Commit();
                }
            }
            this.OnActionEdited(actionID);
            return actionID;
        }

        // Mise à jour d'une action (flexible)
        public int updateAction(TLaction action)
        {
            // Préparation des sous requêtes
            String ctxtPart = "";
            if (action.ctxtHasChanged)
                ctxtPart = "CtxtID=(SELECT id FROM Contextes WHERE Titre=" + action.ContexteSQL + "),";

            String sujetPart = "";
            if (action.sujetHasChanged)
                sujetPart = "SujtID=(SELECT id FROM Sujets WHERE Titre=" + action.SujetSQL + "),";

            String actionPart = "";
            if (action.texteHasChanged)
                actionPart = "Titre=" + action.TexteSQL + ",";

            String datePart = "";
            if (action.dueDateHasChanged)
            {
                if (action.hasDueDate) // action.DueDate != DateTime.MinValue
                    datePart = "DueDate=" + action.DueDateSQL + ",";
                else
                    datePart = "DueDate=NULL,";
            }

            String destPart = "";
            if (action.destHasChanged)
                destPart = "DestID=(SELECT id FROM Destinataires WHERE Titre=" + action.DestinataireSQL + "),";

            String statPart = "";
            if (action.statusHasChanged)
                statPart = "StatID=(SELECT id FROM Statuts WHERE Titre=" + action.StatutSQL + "),";
            // Il y a volontairement une virgule à la fin dans le cas où le statut n'a pas été mis à jour

            String updatePart = ctxtPart + sujetPart + actionPart + datePart + destPart + statPart;

            String requete;
            if (updatePart.Length > 0)
            {
                requete = "UPDATE Actions SET " + updatePart.Substring(0, updatePart.Length - 1) + " WHERE id='" + action.ID + "'";

                int result = execSQL(requete);
                this.OnActionEdited(action.ID);
                return result;
            }
            else
                return 0;
        }
    }
}
