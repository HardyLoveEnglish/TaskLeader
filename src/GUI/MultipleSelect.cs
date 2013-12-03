using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TaskLeader.BO;
using TaskLeader.DAL;

namespace TaskLeader.GUI
{
    public class NewValueEventArgs : EventArgs
    {
        public int valueID { get; set; }
    }
    public delegate void NewValueEventHandler(object sender,NewValueEventArgs args);

    /// <summary>
    /// Classe générique pour communaliser l'IHM de tous les widgets MultipleSelect
    /// </summary>
    public partial class MultipleSelect : UserControl
    {
        /// <summary>
        /// Constructeur pour le Designer
        /// </summary>
        public MultipleSelect(bool displayBox = true)
        {
            InitializeComponent();
            this.box.Visible = displayBox;

            this.Margin = Padding.Empty;
        }

        /// <summary>
        /// Méthodes virtuelles à surcharger dans les classes héritées.
        /// Permettent de détecter une mise à jour de la liste complète
        /// </summary>
        protected virtual void beginListUpdate() { }
        protected virtual void endListUpdate() { }

        /// <summary>
        /// Méthode appelée si checkbox 'Tous' sélectionnée
        /// </summary>
        private void box_Click(object sender, EventArgs e)
        {
            this.beginListUpdate();

            for (int i = 0; i < this.liste.Items.Count; i++)
                this.liste.SetItemChecked(i, this.box.Checked);

            this.endListUpdate();
        }

        /// <summary>
        /// Méthode appelée si click sur la liste
        /// </summary>
        private void liste_Click(object sender, EventArgs e)
        {
            if (box.Checked && box.Enabled)
                box.Checked = false;
        }

        /// <summary>
        /// Déselectionne tous les éléments de la liste
        /// </summary>
        /// <param name="selectAll">true si tous les éléments doivent être sélectionnés</param>
        public void clearChecked(bool selectAll)
        {
            this.beginListUpdate();

            this.liste.ClearSelected();
            if (selectAll)
                for (int i = 0; i < this.liste.Items.Count; i++)
                    this.liste.SetItemChecked(i, true);
            else
                while (this.liste.CheckedIndices.Count > 0)
                    this.liste.SetItemChecked(this.liste.CheckedIndices[0], false);

            this.endListUpdate();   
        }

        /// <summary>
        /// Vide la liste et remet à zéro le contrôle
        /// </summary>
        protected void raz()
        {
            this.liste.Items.Clear();
            this.box.Checked = true;
            this.box.Enabled = false;
        }
    }

    /// <summary>
    /// Classe permettant une sélection multiple des valeurs d'un Criterium
    /// </summary>
    public class CritereSelect : MultipleSelect
    {
        /// <summary>
        /// Un CritereSelect peut se rafraîchir sur les triggers suivants:
        /// - Changement de base de référence (sauf pour les widgets enfants)
        /// - Nouvelle valeur pour la base courante
        /// </summary>

        private DBentity type;
        /// <summary>
        /// ID de l'entité représentée par ce widget
        /// </summary>
        public int entityID { get { return type.id; } }

        private DB db;

        /// <summary>
        /// Constructeur pour un Criterium
        /// </summary>
        /// <param name="title">Titre du critère (et aussi nom du contrôle)</param>
        public CritereSelect(DBentity entity)
            : base()
        {
            this.Name = entity.nom;
            this.titre.Text = entity.nom;
            this.type = entity;
        }

        #region Membres 'parent'

        /// <summary>
        /// Evènement déclenché quand une seule valeur de la grille est sélectionnée
        /// </summary>
        private NewValueEventHandler v_NewParentValue;
        public event NewValueEventHandler NewParentValue
        {
            add
            {
                this.v_NewParentValue += value;
                if (this.v_NewParentValue.GetInvocationList().Length == 1) // C'est la première souscription
                    this.liste.ItemCheck += new ItemCheckEventHandler(this.liste_ItemCheck); // Activation de la surveilance
            }
            remove
            {
                this.v_NewParentValue -= value;
                if (this.v_NewParentValue.GetInvocationList().Length == 0)
                    this.liste.ItemCheck -= new ItemCheckEventHandler(this.liste_ItemCheck); // Désactivation de la surveillance
            }
        }
        private void OnNewParentValue(int parentValueID)
        {
            if (this.v_NewParentValue != null)
                this.v_NewParentValue(this,new NewValueEventArgs(){valueID=parentValueID});
        }

        /// <summary>
        /// Evènement déclenché quand:
        /// - la seule valeur qui était sélectionné ne l'est plus
        /// - la liste a été intégralement rafraîchie
        /// </summary>
        public event EventHandler NoParentValue;
        private void OnNoParentValue()
        {
            if (this.NoParentValue != null)
                this.NoParentValue(this,new EventArgs());
        }

        private void liste_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!this.listeInProgress) // Si on n'est pas en train de toucher à la liste entière
            {
                // On ne lève l'évènement que si un seul item est tické
                if ((this.liste.CheckedIndices.Count == 0) && (e.NewValue == CheckState.Checked))
                    this.OnNewParentValue(((ListValue)this.liste.Items[e.Index]).id);
                else if ((this.liste.CheckedIndices.Count == 2) && (e.NewValue == CheckState.Unchecked))
                {
                    int index;
                    if (this.liste.CheckedIndices[0] == e.Index) // C'est l'autre qui va rester tické
                        index = 1;
                    else
                        index = 0;

                    this.OnNewParentValue(((ListValue)this.liste.Items[this.liste.CheckedIndices[index]]).id);
                }
                else if (this.liste.CheckedIndices.Count == 1)
                    this.OnNoParentValue();
            }
        }

        /// <summary>
        /// Permet d'éviter de dessiner plusieurs fois les contrôles enfants
        /// </summary>
        private bool listeInProgress = false;
        protected override void beginListUpdate() { this.listeInProgress = true; }
        protected override void endListUpdate()
        {
            this.listeInProgress = false;
            this.OnNoParentValue();
        }

        #endregion

        #region Membres 'enfant'

        private bool hasParent { get { return (this.type.parentID > 0); } }

        /// <summary>esrrsttt
        /// Rend dépendant ce widget d'un autre
        /// </summary>
        /// <param name="widget">CritereSelect parent</param>
        public void addParent(CritereSelect widget)
        {
            widget.NewParentValue += (object sender, NewValueEventArgs args) => this.maj(args.valueID);
            widget.NoParentValue += (object sender,EventArgs args) => this.raz();
        }

        #endregion

        /// <summary>
        /// Renvoie le Criterium correspondant ou null
        /// </summary>
        public List<ListValue> criterium
        {
            get
            {
                if (!box.Checked)
                    return liste.CheckedItems.Cast<ListValue>().ToList<ListValue>();
                else
                    return null;
            }
        }

        /// <summary>
        /// Changement de la DB de référence
        /// </summary>
        /// <param name="db">Nouvelle DB</param>
        public void changeDB(DB database)
        {
            // Unregister de l'ancienne DB
            if (this.db != null)
                this.db.unsubscribe_NewValue(this.type.nom, new EventHandler(newValue));
            // Mémorisation de la "nouvelle" DB
            this.db = database;
            // Register de la nouvelle DB
            this.db.subscribe_NewValue(this.type.nom, new EventHandler(newValue));

            if (!this.hasParent) // Les contrôles enfants ne doivent pas être mis à jour directement
                this.maj();
        }

        private void maj(int parentID = 0)
        {
            this.beginListUpdate();

            this.liste.Items.Clear(); // Vidage de la liste

            foreach (ListValue item in this.db.getEntitiesValues(this.type.id, parentID))
                this.liste.Items.Add(item, true); // Sélection de toutes les valeurs

            this.box.Checked = true;
            this.box.Enabled = true;

            this.endListUpdate();
        }

        /// <summary>
        /// Méthode appelée lorsque une nouvelle valeur est créée en base pour l'entité
        /// </summary>
        private void newValue(object sender,EventArgs args)
        {
            if (!this.hasParent) // Ce widget n'a pas de parent
                this.maj();
        }
    }

    /// <summary>
    /// Classe permettant une sélection multiple des bases d'actions
    /// </summary>
    public class DBSelect : MultipleSelect
    {
        /// <summary>
        /// Un DBSelect peut se rafraîchir sur les triggers suivants:
        /// - Base activée ou désactivée
        /// </summary>

        public DBSelect()
            : base()
        {
            this.titre.Text = "Base d'actions";
            this.pictureBox1.Image = TaskLeader.Properties.Resources.database_go;
            this.pictureBox1.Visible = true;
        }

        /// <summary>
        /// Ajoute une DB à la liste du widget
        /// </summary>
        /// <param name="db">La DB à ajouter</param>
        public void addDB(DB db)
        {
            this.liste.Items.Add(db, true);
        }

        /// <summary>
        /// Supprimer une DB de la liste de widget
        /// </summary>
        /// <param name="db">La DB à supprimer</param>
        public void removeDB(DB db)
        {
            this.liste.Items.Remove(db);
        }

        public object[] getDBs()
        {
            return new ArrayList(this.liste.CheckedItems).ToArray();
        }
    }

    /// <summary>
    /// Classe permettant une sélection mutiple des filtres enregistrés d'une base
    /// </summary>
    public class FiltreSelect : MultipleSelect
    {
        /// <summary>
        /// Un FiltreSelect peut se rafraîchir sur les triggers suivants:
        /// - Nouvelle filtre pour la base courante
        /// </summary>

        private DB db;

        public FiltreSelect(DB database)
            : base(false)
        {
            this.db = database;

            // On attribue un nom au contrôle pour pouvoir le récupérer ensuite
            this.Name = this.db.name;
            this.titre.Text = this.db.name;
            this.pictureBox1.Image = TaskLeader.Properties.Resources.database;
            this.pictureBox1.Visible = true;

            this.liste.Items.AddRange(this.db.getFilters().ToArray());
            this.db.subscribe_NewValue("Filtre", new EventHandler(maj));
        }

        /// <summary>
        /// Met à jour la liste des filtres de cette base
        /// </summary>
        private void maj(object sender,EventArgs args)
        {
            this.liste.Items.Clear();
            this.liste.Items.AddRange(this.db.getFilters().ToArray());
        }

        public List<Filtre> getSelected()
        {
            List<Filtre> result = new List<Filtre>();
            foreach (Filtre filtre in this.liste.CheckedItems)
                result.Add(filtre);

            return result;
        }
    }
}
