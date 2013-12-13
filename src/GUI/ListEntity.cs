using System;
using System.Windows.Forms;
using TaskLeader.DAL;
using TaskLeader.BO;

namespace TaskLeader.GUI
{
    public interface IValueRetrievable
    {
        int entityID { get; }
        EntityValue value { get; }
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
        public EntityValue value {
			get {
                if (this.valuesList.SelectedIndex >= 0)
                    return this.valuesList.SelectedItem as ListValue;
                else if (!String.IsNullOrWhiteSpace(this.valuesList.Text))
                    return new ListValue(this.valuesList.Text);
                else
                    return new ListValue();
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
        public ListEntity(String dbName, int entityID, EntityValue selectedValue, Control parent)
            : this()
        {
            this._dbName = dbName;
            this.entityID = entityID;

            DBentity entity = _db.entities[entityID];
            this.Name = entity.nom; //Permet de sélectionner ce contrôle avec son nom
            this.nameLabel.Text = entity.nom;

            if (entity.parentID == 0) {
                this.valuesList.Items.AddRange(_db.getEntitiesValues(entityID).ToArray());
            } else {
                ListEntity widget = parent.Controls[_db.entities[entity.parentID].nom] as ListEntity; 
                ListValue parentValue = widget.value as ListValue;
                if (parentValue.id > 0)
                    this.valuesList.Items.AddRange(_db.getEntitiesValues(this.entityID, parentValue.id).ToArray());
                widget.valuesList.SelectedIndexChanged += new EventHandler(newParentValue);
                widget.valuesList.TextUpdate += new EventHandler(parentValuesList_TextUpdate);
            }
            this.valuesList.Text = selectedValue.ToString();
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

        #region Gestion des évènements parents

        private void newParentValue(object sender, EventArgs e)
        {
            this.valuesList.Items.Clear(); // Dans tous les cas on clear la liste

            ListEntity parent = ((ComboBox)sender).Parent.Parent as ListEntity;
            ListValue parentValue = parent.value as ListValue;
            if (parentValue.id > 0)
                this.valuesList.Items.AddRange(_db.getEntitiesValues(this.entityID, parentValue.id).ToArray());
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
