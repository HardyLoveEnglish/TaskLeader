using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using TaskLeader.BO;

namespace TaskLeader.DAL
{
    public partial class DB
    {
        // Méthode générique pour récupérer une seule colonne
        private object[] getList(String requete)
        {
            // Si le mode debug est activé, on affiche les requêtes
            if (System.Configuration.ConfigurationManager.AppSettings["debugMode"] == "true")
                MessageBox.Show(requete, "Base " + this.name, MessageBoxButtons.OK, MessageBoxIcon.Information);

            ArrayList liste = new ArrayList();

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

                        using (SQLiteDataReader SQLDReader = SQLCmd.ExecuteReader())
                        {
                            // La méthode Read() lit l'entrée actuelle puis renvoie true tant qu'il y a des entrées à lire.
                            while (SQLDReader.Read())
                                liste.Add(SQLDReader[0]); // On retourne la seule et unique colonne
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(Ex.Message + Environment.NewLine + Ex.StackTrace,
                    "Exception sur getList",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }

            return liste.ToArray();
        }

        // Méthode générique pour récupérer une table
        private DataTable getTable(String requete)
        {
            // Si le mode debug est activé, on affiche les requêtes
            if (System.Configuration.ConfigurationManager.AppSettings["debugMode"] == "true")
                MessageBox.Show(requete, "Base " + this.name, MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Création de la DataTable
            DataTable data = new DataTable();

            try
            {
                using (SQLiteConnection SQLC = new SQLiteConnection(this._connectionString))
                {

                    SQLC.Open();

                    using (SQLiteDataAdapter SQLAdap = new SQLiteDataAdapter(requete, SQLC))
                        SQLAdap.Fill(data);// Remplissage avec les données de l'adaptateur
                }
            }
            catch (Exception Ex)
            {
                SQLiteException ex = Ex as SQLiteException;

                // On affiche l'erreur.
                String message = requete + Environment.NewLine +
                    ex.ErrorCode.ToString() + Environment.NewLine +
                    Ex.Message + Environment.NewLine +
                    Ex.StackTrace;

                MessageBox.Show(message,
                    "Fichier: " + this.path,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }

            return data;
        }

        // Méthode générique pour récupérer un entier
        private int getInteger(String requete)
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
                        SQLCmd.CommandText = requete;
                        return Convert.ToInt32(SQLCmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message + Environment.NewLine + Ex.StackTrace,
                    "Exception sur getInteger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return -1;
            }
        }

        // =====================================================================================

        // Vérification si la table est bien compatible
        public bool isVersionComp(String version)
        {
            try
            {
                using (SQLiteConnection SQLC = new SQLiteConnection(this._connectionString))
                {
                    if (File.Exists(this.path))
                        SQLC.Open();
                    else
                        throw new Exception("Base inaccessible");

                    if (SQLC.GetSchema("Tables").Select("Table_Name = 'Properties'").Length > 0) // Version >= 0.7                   
                        using (SQLiteCommand SQLCmd = new SQLiteCommand(SQLC))
                        {
                            // Création d'une nouvelle commande à partir de la connexion
                            SQLCmd.CommandText = "SELECT rowid FROM Properties WHERE Cle='ActionsDBVer' AND Valeur='" + version + "';";

                            using (SQLiteDataReader SQLDReader = SQLCmd.ExecuteReader())
                            {
                                ArrayList liste = new ArrayList();
                                // La méthode Read() lit l'entrée actuelle puis renvoie true tant qu'il y a des entrées à lire.
                                while (SQLDReader.Read())
                                    liste.Add(SQLDReader[0]); // On retourne la seule et unique colonne
                                return (liste.Count > 0);
                            }
                        }
                    else
                        return false;
                }
            }
            catch (SQLiteException Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(Ex.Message + Environment.NewLine + Ex.StackTrace,
                    "Exception sur isVersionComp",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return false;
            }
        }

        // On vérifie la version la plus haute compatible avec cette base
        public String getLastVerComp()
        {
            try
            {
                using (SQLiteConnection SQLC = new SQLiteConnection(this._connectionString))
                {
                    if (File.Exists(this.path))
                        SQLC.Open();
                    else
                        throw new Exception("Base inaccessible");

                    if (SQLC.GetSchema("Tables").Select("Table_Name = 'Properties'").Length > 0) // Version >= 0.7
                        return getList("SELECT Valeur FROM Properties WHERE Cle='ActionsDBVer';")[0].ToString();
                    else // Version < 0.7
                        return (String)getList("SELECT Num FROM VerComp WHERE rowid=(SELECT max(rowid) FROM VerComp)")[0];
                }
            }
            catch (SQLiteException Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(Ex.Message + Environment.NewLine + Ex.StackTrace,
                    "Impossible de récupérer la version de la base",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return String.Empty;
            }
        }

        // =====================================================================================

        /// <summary>
        /// Vérification de la présence d'une nouvelle valeur d'une entité de type List
        /// </summary>
        /// <returns>true si 'title' est une nouvelle valeur de 'entity'</returns>
        public bool isNvo(DBentity entity, String title, String parentValue = "")
        {
            String titre = "'" + title.Replace("'", "''") + "'";
            String parent = "'" + parentValue.Replace("'", "''") + "'";
            String requete;

            if (entity.parentID == 0) // No parent entity
                requete = "SELECT count(id) FROM Entities_values WHERE label=" + titre + " AND entityID=" + entity.id + ";";
            else
                requete = "SELECT count(Child.id) FROM Entities_values Child, Entities_values Parent " +
                "WHERE Child.entityID = " + entity.id + " AND Child.label=" + titre +
                " AND Parent.label =" + parent + " AND Child.parentID = Parent.id;";

            return (getInteger(requete) == 0);
        }

        public bool isNvoFiltre(String title)
        {
            //TODO:
            return true;
        }

        // =====================================================================================

        /// <summary>
        /// Récupération de liste des entités
        /// </summary>
        /// <returns>List of DBentity</returns>
        public List<DBentity> getEntities()
        {
            var data = new List<DBentity>();

            DataRowCollection results = getTable("SELECT id,label,contentType,parentID FROM Entities;").Rows;
            foreach (DataRow row in results)
            {
                data.Add(new DBentity {
                    id = (int)row["id"],
                    nom = row["label"] as String,
                    type = row["contentType"] as String,
                    parentID = (int)row["parentID"]
                });
            }

            return data;
        }

        /// <summary>
        /// Récupération de la liste des valeurs d'une entité
        /// </summary>
        /// <returns>Object array des valeurs</returns>
        public object[] getEntitiesLabels(DBentity entity, string parentValue = "")
        {
            String parent = "'" + parentValue.Replace("'", "''") + "'";
            string request;

            if (entity.parentID == 0) // No parent entity
                request = "SELECT label FROM Entities_values WHERE entityID=" + entity.id + " ORDER BY label ASC";
            else
                request = "SELECT Child.label FROM Entities_values Child, Entities_values Parent " +
                    "WHERE Child.entityId=" + entity.id +
                    " AND Child.parentID = Parent.id AND Parent.label =" + parent +
                    " ORDER BY Child.label ASC";

            return getList(request);
        }

        public object[] getFiltersLabels()
        {
            string request = "";
            //TODO
            return getList(request);
        }

        /// <summary>
        /// Récupération des valeurs par défaut pour tous les entités
        /// </summary>
        /// <returns>Une DataRow </returns>
        public DataRow getDefault()
        {
            //TODO: non il faut faire une jointure
            //TODO: il faudrait retourner une DataRow avec toutes les valeurs plutôt (évite les lectures unitaires)

            Object[] resultat = getList("SELECT v.label FROM Entities_values v, Entities e WHERE e.id=" + entity.id + " AND v.id=e.defaultValue;");

            if (resultat.Length == 1)
                return new DataRow();
            else
                return new DataRow();
        }

        // Récupère un filtre en fonction de son titre
        public List<Criterium> getFilterData(String name)
        {
            var data = new List<Criterium>();

            // On récupère d'abord les checkbox all
            String titre = "'" + name.Replace("'", "''") + "'";
            String requete = "SELECT AllCtxt, AllSuj, AllDest, AllStat FROM Filtres WHERE Titre=" + titre;
            DataRowCollection results = getTable(requete).Rows;

            if (results.Count == 0)
                return null;

            DataRow resultat = results[0];

            if (!(bool)resultat["AllCtxt"])
                data.Add(new Criterium(DB.contexte));

            if (!(bool)resultat["AllSuj"])
                data.Add(new Criterium(DB.sujet));

            if (!(bool)resultat["AllDest"])
                data.Add(new Criterium(DB.destinataire));

            if (!(bool)resultat["AllStat"])
                data.Add(new Criterium(DB.statut));

            object[] liste;

            // On récupère les sélections si nécessaire
            foreach (Criterium critere in data)
            {
                // Récupération du nom de la table correspondante
                String table = critere.entity.mainTable;
                // Création de la requête
                requete = "SELECT TP.Titre FROM " + table + " TP, Filtres_cont TF, Filtres F ";
                requete += "WHERE F.Titre =" + titre + " AND TF.FiltreID=F.rowid AND TF.FiltreType='" + table + "' AND TF.SelectedID=TP.rowid";
                // Récupération de la liste
                liste = getList(requete);

                // On met à jour le critère du filtre correspondant
                foreach (String item in liste)
                    critere.valuesSelected.Add(item);
            }

            return data;
        }

        /// <summary>
        /// Obtient un tableau de tous les filtres de la base
        /// </summary>
        /// <returns>Tableau de 'Filtre'</returns>
        public List<Filtre> getFilters()
        {
            List<Filtre> liste = new List<Filtre>();

            foreach (String filtreName in this.getFiltersLabels())
                liste.Add(new Filtre() { dbName = this.name, nom = filtreName });

            return liste;
        }

        /// <summary>
        /// Méthode permettant de factoriser la construction de la table Actions
        /// </summary>
        /// <param name="WHEREclause">WHERE part of the SQL request</param>
        /// <returns>Requête complète</returns>
        private String getActionsRequest(String WHEREclause)
        {
            // Création de la requête de filtrage
            String requete = "SELECT a.id,";

            // Définition des colonnes suivantes
            foreach (DBentity entity in this.entities)
                if (entity.type == "List")
                    requete += "MAX(CASE WHEN a.entityID = " + entity.id + " THEN v.label ELSE NULL END) AS " + entity.id + ",";
                else
                    requete += "MAX(CASE WHEN a.entityID = " + entity.id + " THEN a.entityValue ELSE NULL END) AS " + entity.id + ",";
            requete = requete.Substring(0, requete.Length - 1); //Suppression de la dernière virgule

            requete += " FROM Actions a LEFT JOIN Entities_values v ON v.id = a.entityValue ";
            requete += WHEREclause;
            requete += " GROUP BY a.id;";

            return requete;
        }

        // Renvoie un DataTable de toutes les actions
        public DataTable getActions(List<Criterium> criteria)
        {
            String requete = "";
            String selection, nomColonne;

            if (criteria.Count > 0) // Il n'y a de WHERE que si au moins un criterium a été entré
            {
                requete += "WHERE ";

                foreach (Criterium critere in criteria) // On boucle sur tous les critères du filtre
                {
                    // On récupère le nom de la colonne correspondant au critère (c'est l'entityID !)
                    nomColonne = critere.entity.id.ToString();

                    if (critere.valuesSelected.Count > 0) // Requête SQL si au moins un élément a été sélectionné
                    {
                        requete += nomColonne + " IN (";
                        foreach (String item in critere.valuesSelected)
                        {
                            selection = item.Replace("'", "''"); // On gère les simple quote
                            requete += "'" + selection + "', ";
                        }
                        requete = requete.Substring(0, requete.Length - 2); // On efface la dernière virgule et le dernier space en trop
                        requete += ")";
                    }
                    else
                        requete += nomColonne + " IS NULL";

                    requete += " AND ";
                }

                requete = requete.Substring(0, requete.Length - 5); // On enlève le dernier AND en trop
            }

            return getTable(getActionsRequest(requete));
        }
        
        /// <summary>
        /// Renvoie une DataRow de l'action 'ID'
        /// </summary>
        public DataRow getAction(String ID)
        {
            DataTable result = getTable(getActionsRequest("WHERE a.id=" + ID));
            return result.Rows[0];
        }

        /// <summary>Récupération des liens attachés à une action</summary>
        public List<Enclosure> getPJ(String actionID)
        {
            DataTable linksData = getTable("SELECT EncType,EncID FROM Enclosures WHERE ActionID=" + actionID);
            List<Enclosure> liste = new List<Enclosure>();

            foreach (DataRow pj in linksData.Rows)
                switch (pj["EncType"].ToString())
                {
                    case ("Mails"):
                        liste.Add(new Mail(pj["EncID"].ToString(), this));
                        break;
                    case ("Links"):
                        liste.Add(new Link(pj["EncID"].ToString(), this));
                        break;
                }

            return liste;
        }

        // Récupération des informations d'un mail à partir de son ID
        public DataRow getMailData(String id)
        {
            DataTable result = getTable("SELECT * FROM Mails WHERE id=" + id);
            return result.Rows[0];
        }

        // Récupération des informations d'un lien à partir de son ID
        public DataRow getLinkData(String id)
        {
            DataTable result = getTable("SELECT * FROM Links WHERE id=" + id);
            return result.Rows[0];
        }

        // Recherche de mots clés dans la colonne Action
        public DataTable searchActions(String keywords)
        {
            String words = keywords.Replace("'", "''");
            String requete = "WHERE ";

            // Recherche sur toutes les colonnes
            foreach(DBentity entity in this.entities)
                requete += entity.id + " LIKE '%" + words + "%' OR ";

            // Recherche dans les titres de mail
            requete += "id IN("; 
            requete += "   SELECT E.ActionID FROM Mails M, Enclosures E";
            requete += "      WHERE M.Titre LIKE '%" + words + "%'";
            requete += "      AND M.id=E.EncID";
            requete += "      AND E.EncType='Mails'";
            requete += ") OR ";

            // Recherche dans les titres ou chemins des liens
            requete += "id IN("; 
            requete += "   SELECT E.ActionID FROM Links L, Enclosures E";
            requete += "      WHERE L.Titre LIKE '%" + words + "%'";
            requete += "      AND L.id=E.EncID";
            requete += "      AND E.EncType='Links'";
            requete += ")";

            return getTable(getActionsRequest(requete));
        }
    }
}
