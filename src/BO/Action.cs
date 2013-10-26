using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using TaskLeader.DAL;
using TaskLeader.GUI;

namespace TaskLeader.BO
{
    public enum EncStatus
    {
        /// <summary>
        /// New enclosure added to the list
        /// </summary>
        added,
        /// <summary>
        /// Original enclosure removed from the list
        /// </summary>
        removed,
        /// <summary>
        /// Original enclosure renamed
        /// </summary>
        renamed, //TODO:Plutôt modified
        /// <summary>
        /// Original enclosure
        /// </summary>
        original,
        /// <summary>
        /// New enclosure deleted at the end
        /// </summary>
        bin
    }

    public class EncWithStatus
    {
        public Enclosure enclosure;
        public EncStatus status;
        public EncWithStatus(Enclosure enc, EncStatus stat) { enclosure = enc; status = stat; }
    }

    public class TLaction
    {
        // Méthode privée pour fabriquer des string compatible sql
        private String sqlFactory(String original) { return "'" + original.Replace("'", "''") + "'"; }

        // Membre privé permettant de détecter des updates
        private bool initialStateFrozen = false;

        // DB d'où provient l'action
        public String dbName = TrayIcon.defaultDB.name;
        private DB db { get { return TrayIcon.dbs[this.dbName]; } }

        // ID de l'action dans la base TaskLeader
        private String v_TLID = "";
        public String ID { get { return v_TLID; } }
        public bool isScratchpad { get { return (v_TLID == ""); } }

        #region Gestion des valeurs des entités

        /// <summary>
        /// Liste des valeurs des différentes entités: entityID => valeur
        /// Pas de typage car contenu de type String ou DateTime
        /// </summary>
        private List<object> values = new List<object>();

        /// <summary>
        /// Liste des changements des valeurs des entités: entityID => bool
        /// </summary>
        private List<bool> entityHasChanged = new List<bool>();

        /// <summary>
        /// Récupération de la valeur de 'entity' pour cette action
        /// </summary>
        public object getValue(int entityID) {
            return values[entityID];
        }

        /// <summary>
        /// Assignation de 'value' à l'entité 'entity' de cette action
        /// </summary>
        public void setValue(DBentity entity, object value){
            if (value != values[entity.id])
            {
                entityHasChanged[entity.id] = this.initialStateFrozen;
                values[entity.id] = value;
            }            
        }

        /// <summary>
        /// Récupération de la valeur de 'entity' au format SQL pour cette action
        /// </summary>
        public String getSQLvalue(DBentity entity)
        {
            if (entity.type == "Date")
                return "'" + ((DateTime)values[entity.id]).ToString("yyyy-MM-dd") + "'";
            else
                return sqlFactory(values[entity.id].ToString());
        }

        #endregion

        #region PJs de l'action

        private List<EncWithStatus> _links = new List<EncWithStatus>();
        public List<Enclosure> PJ { get { return _links.Select(ews => ews.enclosure).ToList<Enclosure>(); } }
        public bool hasPJ { get { return (_links.Count > 0); } }

        public int addPJ(Enclosure enc)
        {
            _links.Add(new EncWithStatus(enc, EncStatus.added));
            return _links.Count - 1;
        }

        public void removePJ(int encIndex)
        {
            switch (_links[encIndex].status)
            {
                case EncStatus.original:
                case EncStatus.renamed:
                    _links[encIndex].status = EncStatus.removed;
                    break;

                case EncStatus.added:
                    _links[encIndex].status = EncStatus.bin;
                    break;
            }
        }

        public void renamePJ(int encIndex, String newTitle)
        {
            _links[encIndex].enclosure.Titre = newTitle;

            if (_links[encIndex].status == EncStatus.original) // Dans tous les autres cas, on reste en 'renamed'
                _links[encIndex].status = EncStatus.renamed;
        }

        #endregion

        #region Constructeurs et mise à jour

        /// <summary>
        /// Initialise les valeurs des entités de l'action
        /// </summary>
        /// <param name="values">Dictionnaire: entityID => entityValue</param>
        private void initValues(Dictionary<int,String> values)
        {
            foreach (int entityID in this.db.entities.Keys)
            {
                if (this.db.entities[entityID].type == "Date")
                {
                    DateTime dateValue;
                    DateTime.TryParse(values[entityID], out dateValue); // Si le TryParse échoue, dateValue = DateTime.MinValue
                    this.values.Insert(entityID, dateValue);
                }
                else
                {
                    this.values.Insert(entityID, values[entityID]);
                }
                this.entityHasChanged.Insert(entityID, false);
            }
            this.initialStateFrozen = true;
        }

        /// <summary>
        /// Constructeur permettant d'initialiser les valeurs par défaut
        /// </summary>
        public TLaction() {
            this.initValues(this.db.getDefault());
        }

        /// <summary>
        /// Constructeur à partir de l'ID de stockage de l'action
        /// </summary>
        /// <param name="ID">ID dans la base d'actions</param>
        /// <param name="database">Nom de la base d'actions</param>
        public TLaction(String ID, String database)
        {
            this.dbName = database;
            this.v_TLID = ID;

           //Récupération des données de l'action
           this.initValues(db.getAction(ID));

           //Récupération des liens
           _links.AddRange(db.getPJ(ID).Select(enc => new EncWithStatus(enc, EncStatus.original)));

        }

        /// <summary>
        /// Change la base correspondant à l'action scratchpad
        /// </summary>
        /// <param name="nomDB">Nom de la nouvelle base</param>
        public void changeDB(String nomDB)
        {
            // Changement du nom de la base
            this.dbName = nomDB;

            // Changement des valeurs par défaut
            this.initValues(this.db.getDefault());
        }

        #endregion

        /// <summary>
        /// Sauvegarde de l'action dans la base correspondante
        /// </summary>
        public void save()
        {
            String bilan = "";
            int resultat;

            // Vérification des nouveautés
            foreach (DBentity entity in this.db.listEntities)
            {
                if (this.entityHasChanged[entity.id])
                    if (db.isNvo(entity, this.values[entity.id] as String))
                    {
                        if (entity.parentID > 0)
                            resultat = db.insert(entity, this.values[entity.id] as String, this.values[entity.parentID] as String);
                        else
                            resultat = db.insert(entity, this.values[entity.id] as String);
                        if(resultat == 1)
                            bilan += "Nouveau " + entity.nom + " enregistré\n";
                    }
            }

            if (this.isScratchpad)
            {
                this.v_TLID = db.insertAction(this); // Sauvegarde de l'action

                bilan += "Nouvelle action enregistrée\n";
                if (this.hasPJ)
                {
                    db.insertPJ(this.v_TLID, this.PJ); // Sauvegarde des PJ
                    bilan += _links.Count.ToString() + " PJ enregistrée";
                    if (_links.Count > 1) bilan += "s";
                    bilan += "\n";
                }
            }
            else
            {
                resultat = db.updateAction(this);
                if (resultat == 1)
                    bilan += "Action mise à jour\n";

                // Insertion des pj
                List<Enclosure> added_links =
                    this._links.
                    Where(ews => ews.status == EncStatus.added).
                    Select(ews => ews.enclosure).
                    ToList<Enclosure>();

                int nbAdded = added_links.Count;
                if (nbAdded > 0)
                {
                    db.insertPJ(this.v_TLID, added_links); // Sauvegarde des PJ
                    bilan += nbAdded.ToString() + " PJ enregistrée"; // Préparation du bilan
                    if (nbAdded > 1) bilan += "s";
                    bilan += "\n";
                }

                // Suppression des pj
                List<Enclosure> removed_links =
                    this._links.
                    Where(ews => ews.status == EncStatus.removed).
                    Select(ews => ews.enclosure).
                    ToList<Enclosure>();

                int nbSupp = removed_links.Count;
                if (nbSupp > 0)
                {
                    db.removePJ(this.v_TLID, removed_links);
                    bilan += nbSupp.ToString() + " PJ supprimée"; // Préparation du bilan
                    if (nbSupp > 1) bilan += "s";
                    bilan += "\n";
                }

                // Mise àjour des pj
                List<Enclosure> updated_links =
                    this._links.
                    Where(ews => ews.status == EncStatus.renamed).
                    Select(ews => ews.enclosure).
                    ToList<Enclosure>();

                int nbUpd = updated_links.Count;
                if (nbUpd > 0)
                {
                    db.renamePJ(this.v_TLID, updated_links);
                    bilan += nbUpd.ToString() + " PJ mise"; // Préparation du bilan
                    if (nbUpd > 1) bilan += "s";
                    bilan += " à jour\n";
                }
            }

            // On affiche un message de statut sur la TrayIcon
            if (bilan.Length > 0) // On n'affiche un bilan que s'il s'est passé qqchose
                TrayIcon.afficheMessage("Bilan sauvegarde", bilan.Substring(0, bilan.Length - 1)); // On supprime le dernier \n            
        }

    }
}
