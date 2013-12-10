using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using TaskLeader.BO;
using TaskLeader.GUI;
using System.IO;

namespace TaskLeader.DAL
{
    public partial class DB
    {
        #region Méthodes utilitaires

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

        #endregion

        #region Filtres

        /// <summary>
        /// Insertion en base d'un nouveau filtre
        /// </summary>
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
                        foreach (var critere in filtre.criteria)
                        {
                            if (critere.Value.Count > 0)
                            {
                                // On crée la requête pour insertion des critères dans les tables annexes
                                String requete = "INSERT INTO Filtres_cont VALUES "+
                                    "((SELECT max(id) FROM Filtres)," + critere.Key + ",@entityValueID);";
                                // On récupère le rowid du filtre frâichement créé

                                SQLCmd.CommandText = requete;
                                SQLiteParameter p_ValueID = new SQLiteParameter("@entityValueID");
                                SQLCmd.Parameters.Add(p_ValueID);

                                foreach (ListValue item in critere.Value)
                                {
                                    p_ValueID.Value = item.id;
                                    SQLCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                SQLCmd.CommandText = "INSERT INTO Filtres_cont (filtreID,entityID) " +
                                    "VALUES ((SELECT max(id) FROM Filtres)," + critere.Key + ");";
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

        /// <summary>
        /// Modifie le filtre par défaut
        /// </summary>
        /// <param name="name">Nom du filtre</param>
        public void insertDefaultFilter(String name)
        {
            //TODO: serait plus propre par un ID
            String nomFiltre = "'" + name.Replace("'", "''") + "'";
            execSQL("UPDATE Properties SET Valeur=(SELECT id FROM Filtres WHERE titre=" + nomFiltre + ") WHERE Cle='FiltreDefaut';");
        }

        #endregion

        #region Entités

        /// <summary>
        /// Insertion des valeurs d'entités de type List
        /// </summary>
        /// <param name="entity">DBentity de la valeur à insérer</param>
        /// <param name="value">ListValue à insérer</param>
        /// <param name="parentValueID">ID de la valeur parent le cas échéant</param>
        /// <returns>ID de la valeur nouvellement créée</returns>
        public int insert(DBentity entity, ListValue value, int parentValueID = -1)
        {
            String requete;
            String parentCol = "";
            String parentValue= "";

            if (entity.parentID > 0){
                parentCol = ",parentID";
                parentValue = ","+parentValueID;
            }
            requete = "INSERT INTO Entities_values (entityID,label" + parentCol + ") VALUES (" + entity.id + "," + value.sqlLabel + parentValue + ");";

            int result = execSQL(requete);
            if (result == 1)
            {
                this.OnNewValue(entity.nom);
                return this.getInteger("SELECT max(id) FROM Entities_values;");
            }
            else
                return -1;
        }

        /// <summary>
        /// Insertion des valeurs par défaut
        /// </summary>
        /// <param name="values"></param>
        public void insertDefaut(Dictionary<int, EntityValue> values)
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
                        foreach (var kvp in values)
                        {
                            SQLCmd.CommandText = "UPDATE Entities SET defaultValue=" + kvp.Value.sqlValue + " WHERE id=" + kvp.Key + ";";
                            SQLCmd.ExecuteNonQuery();
                        }
                    }
                    mytransaction.Commit();
                }
            }
        }

        #endregion

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

        #region Actions

        /// <summary>
        /// Insertion d'une nouvelle action
        /// </summary>
        /// <param name="action">TLaction à insérer en base</param>
        /// <returns>ID de l'action</returns>
        public String insertAction(TLaction action)
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
                        bool first = true;
                        foreach (int entityID in action.entitiesIDs)
                        {
                            String value = action.getValue(entityID).sqlValue;
                            if (value != "NULL")
                            {
                                SQLCmd.CommandText = "INSERT INTO Actions (id,entityID,entityValue) " +
                                    "SELECT COALESCE(max(id),0)"; // max(id)=NULL pour une table vide
                                if (first)
                                {
                                    SQLCmd.CommandText += "+1"; // La première valeur insérée incrémentera l'ID
                                    first = false;
                                }
                                SQLCmd.CommandText += "," + entityID + "," + value + " FROM Actions;";
                                SQLCmd.ExecuteNonQuery();
                            }
                        }
                    }
                    mytransaction.Commit();
                }
            }

            String actionID = this.getInteger("SELECT max(id) FROM Actions;").ToString();
            this.OnActionEdited(actionID);
            return actionID;
        }

        // Mise à jour d'une action (flexible)
        public int updateAction(TLaction action)
        {
            int result = 0;

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
                        foreach (int entityID in action.entitiesIDs)
                            if (action.hasChanged(entityID))
                            {
                                String value = action.getValue(entityID).sqlValue;
                                if (value == "NULL") //Si valeur NULL, on supprime toutes les lignes
                                {
                                    SQLCmd.CommandText = "DELETE FROM Actions WHERE id=" + action.ID + " AND entityID=" + entityID + ";";
                                    result += SQLCmd.ExecuteNonQuery();
                                }
                                else //Si valeur non nulle, INSERT OR UPDATE
                                {
                                    SQLCmd.CommandText = "INSERT OR REPLACE INTO Actions VALUES(" + action.ID + "," + entityID + "," + value + ");";
                                    result += SQLCmd.ExecuteNonQuery();
                                }
                            }
                    }
                    mytransaction.Commit();
                }
            }

            if (result > 0)
                this.OnActionEdited(action.ID);           
            return result;
        }

        #endregion
    }
}
