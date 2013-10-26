using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;

namespace TaskLeader.DAL
{
    // Structure listant les différentes informations liées à une entité de la base (Contexte, Destinataire ...)
    public class DBentity
    {
        /// <summary>
        /// ID de l'entité dans la base
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Nom de l'entité pour IHM
        /// </summary>
        public String nom { get; set; }

        /// <summary>
        /// Type de l'entité: List, Text, Date
        /// </summary>
        public String type { get; set; }

        /// <summary>
        /// ID de l'entité parente
        /// </summary>
        public int parentID { get; set; }
    }

    public delegate void NewValueEventHandler(String parentValue);
    public delegate void ActionEditedEventHandler(String dbName, String actionID);

    public partial class DB
    {
        // Caractéristiques de la DB
        public String path = "";
        public String name = "";

        /// <summary>
        /// Dictionnaire entityID => DBentity des entités de cette base
        /// </summary>
        public Dictionary<int,DBentity> entities;
        /// <summary>
        /// Liste des DBentity de type 'List'.
        /// /!\ L'index dans la liste n'est pas l'id de l'entité
        /// </summary>
        public DBentity[] listEntities;

        /// <summary>
        /// Retourne le nom de la base
        /// </summary>
        public override string ToString() { return this.name; }

        public DB(String chemin, String nom)
        {
            this.path = System.IO.Path.GetFullPath(chemin);
            this.name = nom;

            this.getEntities();
            this.listEntities = entities.Where(kvp => kvp.Value.type == "List")
                    .ToDictionary(kvp => kvp.Key,kvp => kvp.Value).Values
                    .ToList<DBentity>().ToArray();

            // ConnectionString definition
            _builder.DataSource = this.path;
            _builder.FailIfMissing = true;
            _builder.Pooling = true;

            // Création des piles correspondant aux entities List
            foreach (DBentity entity in this.listEntities)
                this.NewValue.Add(entity.nom, null);

            // Création de la pile pour les filtres
            this.NewValue.Add("Filtre", null);

        }

        private SQLiteConnectionStringBuilder _builder = new SQLiteConnectionStringBuilder();
        private String _connectionString { get { return _builder.ConnectionString; } }

        #region Events

        // Gestion des évènements NewValue - http://msdn.microsoft.com/en-us/library/z4ka55h8(v=vs.80).aspx
        private Dictionary<String, Delegate> NewValue = new Dictionary<String, Delegate>();
        public void subscribe_NewValue(String entityName, NewValueEventHandler value) { this.NewValue[entityName] = (NewValueEventHandler)this.NewValue[entityName] + value; }
        public void unsubscribe_NewValue(String entityName, NewValueEventHandler value) { this.NewValue[entityName] = (NewValueEventHandler)this.NewValue[entityName] - value; }
        /// <summary>
        /// Génération de l'évènement NewValue
        /// </summary>
        /// <param name="entity">DBentity concernée</param>
        /// <param name="parentValue">La valeur courante de la DBentity parente</param>
        private void OnNewValue(String entityName, String parentValue = null)
        {
            NewValueEventHandler handler;
            if (null != (handler = (NewValueEventHandler)this.NewValue[entityName]))
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
