using System;
using System.Windows.Forms;
using TaskLeader.DAL;


namespace TaskLeader.GUI
{
    public interface IValueRetrievable
    {
        int entityID {get;}
        object value {get;}
    }

    public partial class ListEntity : UserControl, IValueRetrievable
    {
        private String _dbName;
        private DB _db { get { return TrayIcon.dbs[this._dbName]; } }

        #region IValueRetrievable members

        public int entityID { get; set; }

        /// <summary>
        /// Valeur de type EntityValue
        /// </summary>
        public object value {
			get {
                if (this.valuesList.SelectedIndex >= 0)
                    return this.valuesList.SelectedItem;
                else
                    return new EntityValue() { id = -1, value = this.valuesList.Text };
			}
		}

        #endregion

        #region Constructors

        /// <summary>
        /// Designer constructor
        /// </summary>
        public ListEntity()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="db">Source DB</param>
        /// <param name="entityID">Related List entityID</param>
        /// <param name="selectedValue">Value to be displayed for this list</param>
        public ListEntity(String dbName, int entityID, object selectedValue)
            : this()
        {
            this._dbName = dbName;
            this.entityID = entityID;

            DBentity entity = _db.entities[entityID];

            this.Name = entity.nom; //Permet de sélectionner ce contrôle avec son nom
            this.nameLabel.Text = entity.nom;
            if (entity.parentID == 0)
                this.valuesList.Items.AddRange(_db.getEntitiesValues(entityID).ToArray());
            this.valuesList.Text = selectedValue as String;
        }

        /// <summary>
        /// Ajouté pour couvrir le cas: le texte entré à la main correspond à une valeur de la liste.
        /// Avec l'implémentation actuelle, SelectedIndex n'est pas mis à jour.
        /// </summary>
        private void valuesList_TextUpdate(object sender, EventArgs e)
        {
            int index = valuesList.FindStringExact(valuesList.Text);
            if (index >= 0)
                valuesList.SelectedIndex = index;
        }

        #endregion

        #region Relation parent/enfant

        /// <summary>
        /// Add a parent widget
        /// </summary>
        /// <param name="widget">Parent ListEntity widget</param>
        public void addParent(ListEntity widget)
        {
            EntityValue parentValue = widget.value as EntityValue;
            if (parentValue.id > 0)
                this.valuesList.Items.AddRange(_db.getEntitiesValues(this.entityID, parentValue.id).ToArray());
            widget.valuesList.SelectedIndexChanged += new EventHandler(newParentValue);
            widget.valuesList.TextUpdate += new EventHandler(parentValuesList_TextUpdate);
        }

        private void newParentValue(object sender, EventArgs e)
        {
            EntityValue parentValue = ((ListEntity)sender).value as EntityValue;
            if (parentValue.id > 0)
            {
                this.valuesList.Items.Clear();
                this.valuesList.Items.AddRange(_db.getEntitiesValues(this.entityID, parentValue.id).ToArray());
            }
        }

        // Clear de la liste si le texte de la ComboBox est manuel
        private void parentValuesList_TextUpdate(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex < 0)
                this.valuesList.Items.Clear();
        }

        #endregion
    }
}
