using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using TaskLeader.BO;

namespace TaskLeader.DAL
{
    public class EditedActionEventArgs {
        public String actionID { get; set; }
    }
    public delegate void ActionEditedEventHandler(DB sender, EditedActionEventArgs args);

    public partial class DB
    {
        // Caractéristiques de la DB
        public String path = "";
        public String name = "";

        /// <summary>
        /// Dictionnaire entityID => DBentity des entités de cette base
        /// </summary>
        public Dictionary<int,DBentity> entities = new Dictionary<int,DBentity>();
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

            // ConnectionString definition
            _builder.DataSource = this.path;
            _builder.FailIfMissing = true;
            _builder.Pooling = true;

            this.getEntities();
            this.listEntities = entities.Where(kvp => kvp.Value.type == "List")
                    .ToDictionary(kvp => kvp.Key,kvp => kvp.Value).Values
                    .ToList<DBentity>().ToArray();

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
        public void subscribe_NewValue(String entityName, EventHandler handler) { this.NewValue[entityName] = (EventHandler)this.NewValue[entityName] + handler; }
        public void unsubscribe_NewValue(String entityName, EventHandler handler) { this.NewValue[entityName] = (EventHandler)this.NewValue[entityName] - handler; }
        /// <summary>
        /// Génération de l'évènement NewValue
        /// </summary>
        /// <param name="entity">Nom de la DBentity concernée</param>
        private void OnNewValue(String entityName)
        {
            EventHandler handler;
            if (null != (handler = (EventHandler)this.NewValue[entityName]))
                handler(this,new EventArgs());
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
                this.ActionEdited(this, new EditedActionEventArgs() { actionID = actionID }); //Invoque le délégué
        }

        #endregion
    }
}
