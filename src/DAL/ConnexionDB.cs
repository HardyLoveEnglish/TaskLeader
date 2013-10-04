using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Configuration;
using TaskLeader.BO;
using System.Runtime.Serialization;

namespace TaskLeader.DAL
{
    // Structure listant les différentes informations liées à une entité de la base (Contexte, Destinataire ...)
    [DataContract]
    public class DBentity
    {
        /// <summary>
        /// Nom de l'entité pour IHM, !!doit être unique !!
        /// </summary>
        [DataMember]
        public String nom;
        /// <summary>
        /// Nom de la table principale
        /// </summary>
        public String mainTable;
        /// <summary>
        /// Nom de la colonne dans vueActions
        /// </summary>
        public String viewColName;
        /// <summary>
        /// Nom de la colonne "All" dans la table Filtre
        /// </summary>
        public String allColName;

        [DataMember]
        public int parent = -1;
        /// <summary>
        /// Nom de la colonne foreign key si entity parente
        /// TODO: sera normalisé dans la base 0.8
        /// </summary>
        public String foreignID;
    }

    public delegate void NewValueEventHandler(String parentValue);
    public delegate void ActionEditedEventHandler(String dbName, String actionID);

    public partial class DB //TODO: détecter les ouvertures de fichier pour les limiter
    {
        // Caractéristiques de la DB
        public String path = "";
        public String name = "";

        /// <summary>
        /// Retourne le nom de la base
        /// </summary>
        public override string ToString() { return this.name; }

        public DB(String chemin, String nom)
        {
            this.path = System.IO.Path.GetFullPath(chemin);
            this.name = nom;

            // ConnectionString definition
            _builder.DataSource = this.path;
            _builder.FailIfMissing = true;
            _builder.Pooling = true;

            //TODO: ne pas harcoder les différents types
            this.NewValue.Add(contexte.nom, null);
            this.NewValue.Add(sujet.nom, null);
            this.NewValue.Add(destinataire.nom, null);
            this.NewValue.Add(statut.nom, null);
            this.NewValue.Add(filtre.nom, null);
        }

        SQLiteConnectionStringBuilder _builder = new SQLiteConnectionStringBuilder();
        private String _connectionString { get { return _builder.ConnectionString; } }

        // "Schéma de base" = Nom de l'entité pour IHM, Nom de la colonne dans vueActions, Nom de la table principale, Nom de la colonne "All" dans la table Filtre
        public static DBentity contexte = new DBentity() { nom = "Contextes", viewColName = "Contexte", mainTable = "Contextes", allColName = "AllCtxt" };
        public static DBentity sujet = new DBentity() { nom = "Sujets", viewColName = "Sujet", mainTable = "Sujets", allColName = "AllSuj", parent = 0, foreignID = "CtxtID" };
        public static DBentity destinataire = new DBentity() { nom = "Destinataires", viewColName = "Destinataire", mainTable = "Destinataires", allColName = "AllDest" };
        public static DBentity statut = new DBentity() { nom = "Statuts", viewColName = "Statut", mainTable = "Statuts", allColName = "AllStat" };
        public static DBentity filtre = new DBentity() { nom = "Filtres", viewColName = "", mainTable = "Filtres", allColName = "" };
        public static DBentity[] entities = { contexte, sujet, destinataire, statut };

        #region Events

        // Gestion des évènements NewValue - http://msdn.microsoft.com/en-us/library/z4ka55h8(v=vs.80).aspx
        private Dictionary<String, Delegate> NewValue = new Dictionary<String, Delegate>();
        public void subscribe_NewValue(DBentity entity, NewValueEventHandler value) { this.NewValue[entity.nom] = (NewValueEventHandler)this.NewValue[entity.nom] + value; }
        public void unsubscribe_NewValue(DBentity entity, NewValueEventHandler value) { this.NewValue[entity.nom] = (NewValueEventHandler)this.NewValue[entity.nom] - value; }
        /// <summary>
        /// Génération de l'évènement NewValue
        /// </summary>
        /// <param name="entity">DBentity concernée</param>
        /// <param name="parentValue">La valeur courante de la DBentity parente</param>
        private void OnNewValue(DBentity entity, String parentValue = null)
        {
            NewValueEventHandler handler;
            if (null != (handler = (NewValueEventHandler)this.NewValue[entity.nom]))
                handler(parentValue);
        }

        // Gestion de l'évènement ActionEdited
        public event ActionEditedEventHandler ActionEdited;
        /// <summary>
        /// Génération de l'évènement ActionEdited
        /// </summary>
        /// <param name="action">Action ayant généré l'event</param>
        private void OnActionEdited(String actionID)
        {
            if (this.ActionEdited != null)
                this.ActionEdited(this.name, actionID); //Invoque le délégué
        }

        #endregion
    }
}
