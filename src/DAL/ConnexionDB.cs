using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.SQLite;
using TaskLeader.BO;
using TaskLeader.GUI;

namespace TaskLeader.DAL
{
    public class EditedActionEventArgs {
        public String actionID { get; set; }
    }
    public delegate void ActionEditedEventHandler(DB sender, EditedActionEventArgs args);

    public partial class DB
    {
        // Caractéristiques de la DB
        private String path = "";
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

        private void checkCompatibility()
        {
            // On vérifie que la version de la GUI est bien dans la base
            bool baseCompatible = this.isVersionComp(Application.ProductVersion.Substring(0, 3));

            if (!baseCompatible)
                if (this.getLastVerComp() != "0.7")
                    throw new Exception(this.path + Environment.NewLine + "La base est trop ancienne pour une migration");
                else
                {
                    // Copie de sauvegarde du fichier db avant toute manip
                    String sourceFile = this.path;
                    String backupFile = sourceFile.Substring(0, sourceFile.Length - 4) + DateTime.Now.ToString("_Back-ddMMyyyy") + ".db3";
                    //TODO: P0 ne fonctionne qu'avec des extensions de 3 digits !
                    System.IO.File.Copy(sourceFile, backupFile, true);

                    // Récupération du script de migration
                    try
                    {
                        String script = System.IO.File.ReadAllText(@"db/Migration/07-08.sql", System.Text.Encoding.UTF8);

                        // Exécution du script
                        TrayIcon.afficheMessage("Migration", "Exécution du script de migration");
                        this.execSQL(script);

                        // Nettoyage de la base
                        this.execSQL("VACUUM;");
                        TrayIcon.afficheMessage("Migration", "Migration de la base effectuée");
                    }
                    catch
                    {
                        throw new Exception("Erreur lors de la migration"); //TODO:affiner le pourquoi
                    }
                }
        }

        public DB(String chemin, String nom)
        {
            this.path = System.IO.Path.GetFullPath(chemin);
            this.name = nom;

            // ConnectionString definition
            _builder.DataSource = this.path;
            _builder.FailIfMissing = true;
            _builder.Pooling = true;

            // Vérification de la compatibilité de la base
            try { this.checkCompatibility(); }
            catch (Exception e) { throw e; }

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
