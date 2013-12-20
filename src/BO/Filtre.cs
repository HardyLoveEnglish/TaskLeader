using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using TaskLeader.GUI;
using TaskLeader.DAL;
using System.Runtime.Serialization;

namespace TaskLeader.BO
{
    [DataContract]
    public class Filtre : IEquatable<Filtre>
    {
        /// <summary>
        /// Type du filtre: 1=Critères, 2=Recherche
        /// Default value is 1
        /// </summary>
        private int _type;
        public int type
        {
            get
            {
                if (_type == 0)
                    _type = 1;
                return _type;
            }
        }

        // DB d'application de ce filtre
        [DataMember]
        public String dbName { get; set; }
        private DB db { get { return TrayIcon.dbs[this.dbName]; } }

        // Tableau qui donne la liste des critères sélectionnés autre que ALL
        private Dictionary<int, List<ListValue>> _criteria;
        [DataMember]
        public Dictionary<int, List<ListValue>> criteria
        {
            get
            {
                if (_criteria == null)
                    _criteria = this.db.getFilterData(this.id);
                return _criteria;
            }
            set
            {
                this._criteria = value;
            }
        }

        // Nom du filtre
        [DataMember]
        public String nom = "";

        /// <summary>
        /// ID du filtre en base
        /// </summary>
        [DataMember]
        public int id;

        // Contenu de la recherche
        [DataMember(EmitDefaultValue = false)]
        private string _recherche;
        public string recherche {
            set { this._recherche = value; this._type = 2; }
            get { return this._recherche;}
        }

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
                    dbData = db.searchActions(this.recherche);
                    break;
                default:
                    dbData = new DataTable();
                    break;
            }

            // Typage des colonnes pour éviter les problèmes de Merge
            DataTable data = dbData.Clone(); // Copie du schéma uniquement

            data.Columns["Liens"].DataType = typeof(Int32); //Typage de la colonne Liens

            for (int i = 2; i < data.Columns.Count; i++) // Les 2 premières colonnes sont "id" et "liens"
            {
                //Nom de la colonne = nom de l'entité pour faciliter le merge
                String entityName = this.db.entities[Int32.Parse(data.Columns[i].ColumnName)].nom;
                data.Columns[i].ColumnName = entityName;
                dbData.Columns[i].ColumnName = entityName;
                data.Columns[i].DataType = typeof(String);
            }

            //Typage des colonnes de type date
            foreach (DBentity entity in this.db.entities.Values)
                if (entity.type == "Date")
                    data.Columns[entity.nom].DataType = typeof(DateTime);

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
            if (this.type == 1)
                return this.nom; //Donc rien pour les filtres manuels.
            else
                return this.recherche;
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
                    if (this.criteria.ContainsKey(entity.id))
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
            if (this.type != compFilter.type)
                return false;

            // Les 2 filtres sont des recherches
            if (this.type == 2)
                return ((compFilter.recherche == this.recherche) && (compFilter.dbName == this.dbName));

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
