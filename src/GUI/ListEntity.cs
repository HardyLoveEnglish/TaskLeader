using System;
using System.Windows.Forms;
using TaskLeader.DAL;

namespace TaskLeader.GUI
{
    public interface EntityControl
    {
        int entityID;
        object value;
    }

    public partial class ListEntity : UserControl, EntityControl
    {
        private DB _db;

        // EntityControl members
        public int entityID;
        public object value { get { return this.valuesList.Text; } }

        private DBentity _entity { get { return this._db.entities[entityID]; } }

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
        public ListEntity(DB db, int entityID, object selectedValue)
            : this()
        {

            this.entityID = entityID;
            this._db = db;

            this.Name = _entity.nom; //Permet de sélectionner ce contrôle avec son nom
            this.nameLabel.Text = _entity.nom;
            if (_entity.parentID == 0)
                this.valuesList.Items.AddRange(db.getEntitiesLabels(_entity));
            this.valuesList.Text = selectedValue as String;
        }

        #endregion

        /// <summary>
        /// Add a parent widget
        /// </summary>
        /// <param name="widget">Parent ListEntity widget</param>
        public void addParent(ListEntity widget)
        {
            String parentValue = widget.valuesList.Text;
            if(parentValue != "")
                this.valuesList.Items.AddRange(_db.getEntitiesLabels(_entity, parentValue));
            widget.valuesList.SelectedIndexChanged += new EventHandler(newParentValue);
        }

        private void newParentValue(object sender, EventArgs e)
        {
            String parentValue = ((ComboBox)sender).Text;
            this.valuesList.Items.Clear();
            this.valuesList.Items.AddRange(_db.getEntitiesLabels(_entity,parentValue));
        }

    }
}
