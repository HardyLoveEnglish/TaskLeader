using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using TaskLeader.GUI;
using TaskLeader.DAL;

namespace TaskLeader.BO
{
    public class Filtre : IEquatable<Filtre>
    {
        /// <summary>
        /// Type du filtre: 1=Critères, 2=Recherche
        /// Default value is 1
        /// </summary>
        public int type { get { return _type; } }
        private int _type = 1;

        // DB d'application de ce filtre
        public String dbName { get; set; }
        private DB db { get { return TrayIcon.dbs[this.dbName]; } }

        // Tableau qui donne la liste des critères sélectionnés autre que ALL
        private Dictionary<int, List<ListValue>> _criteria;
        public Dictionary<int, List<ListValue>> criteria
        {
            get
            {
                if (_criteria == null)
                    _criteria = this.db.getFilterData(this.nom);
                return _criteria;
            }
            set
            {
                this._criteria = value;
            }
        }

        // Nom du filtre
        public String nom = "";

        // Contenu de la recherche
        public string recherche { set { this.nom = value; this._type = 2; } }

        /// <summary>
        /// Retourne une DataTable contenant les actions du filtre
        /// </summary>
        public DataTable getActions()
        {
            // Récupération des actions
            DataTable dbData;
            switch (this.type)
            {
                case (1):
                    dbData = db.getActions(this.criteria);
                    break;
                case (2):
                    dbData = db.searchActions(this.nom);
                    break;
                default:
                    dbData = new DataTable();
                    break;
            }

            // Typage des colonnes pour éviter les problèmes de Merge
            DataTable data = dbData.Clone(); // Copie du schéma uniquement
            foreach (DataColumn column in data.Columns)
            {
                //Nom de la colonne = nom de l'entité pour faciliter le merge
                column.ColumnName = this.db.entities[Int32.Parse(column.ColumnName)].nom;
                column.DataType = typeof(String);
            }

            //Typage des colonnes de type date
            foreach (DBentity entity in this.db.entities.Values)
                if (entity.type == "Date")
                    data.Columns[entity.id.ToString()].DataType = typeof(DateTime);

            foreach (DataRow row in dbData.Rows)
                data.ImportRow(row);

            // Ajout d'une colonne contenant le nom de la DB de ce filtre
            DataColumn col = new DataColumn("DB", typeof(String));
            col.DefaultValue = this.dbName;
            data.Columns.Add(col);

            // Ajout d'une colonne formalisant une ref pour chaque action
            data.Columns.Add("Ref", typeof(String), "DB+'" + Environment.NewLine + "#'+ID");

            // Création de la clé primaire à partir des colonnes DB et ID
            DataColumn[] keys = new DataColumn[2];
            keys[0] = data.Columns["DB"];
            keys[1] = data.Columns["ID"];
            data.PrimaryKey = keys;

            return data;
        }

        /// <summary>
        /// Retourne le nom du filtre
        /// </summary>
        /// <returns>Nom du filtre</returns>
        public override String ToString()
        {
            return this.nom; //Donc rien pour les filtres manuels.
        }

        /// <summary>
        /// Retourne un Dictionnaire DBentity => Valeur décrivant le filtre.
        /// Valeur = "" si All sélectionné.
        /// </summary>
        public Dictionary<String, String> description
        {
            get
            {
                Dictionary<String, String> description = new Dictionary<string, string>();

                foreach (DBentity entity in this.db.listEntities)
                    if (this._criteria.ContainsKey(entity.id))
                        description.Add(
                            entity.nom,
                            String.Join(" + ", this.criteria[entity.id].Select(ev => ev.label).ToList<String>())
                        );
                    else
                        description.Add(entity.nom, "");

                return description;
            }
        }

        #region Implémentation de IEquatable http://msdn.microsoft.com/en-us/library/ms131190.aspx

        /// <summary>
        /// Méthode permettant la comparaison de 2 Filtres
        /// </summary>
        /// <param name="compFilter">Filtre à comparer</param>
        public bool Equals(Filtre compFilter)
        {
            if (compFilter == null)
                return false;

            // Les filtres ont des types différents
            if (this._type != compFilter.type)
                return false;

            // Les 2 filtres sont des recherches
            if (this._type == 2)
                return ((compFilter.nom == this.nom) && (compFilter.dbName == this.dbName));

            // Les 2 filtres sont des filtres enregistrés
            if (!String.IsNullOrEmpty(this.nom) && !String.IsNullOrEmpty(compFilter.nom))
                return ((compFilter.nom == this.nom) && (compFilter.dbName == this.dbName));

            // Un des 2 filtres est un fitre manuel, comparaison des critères
            return this.criteria.SequenceEqual(compFilter.criteria);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            Filtre compFilter = obj as Filtre;
            if (compFilter == null)
                return false;
            else
                return Equals(compFilter);
        }

        #endregion
    }
}
