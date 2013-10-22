using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TaskLeader.DAL;

namespace TaskLeader.GUI
{
    public partial class ListEntity : UserControl
    {
        private DBentity _entity;
        private DB _db;

        #region Constructors

        public ListEntity()
        {
            InitializeComponent();
        }

        public ListEntity(DBentity entity, DB db)
            : this()
        {

            this._entity = entity;
            this._db = db;

            this.Name = entity.nom; //Permet de sélectionner ce contrôle avec son nom
            this.nameLabel.Text = entity.nom;
            this.valuesList.Items.Clear();
            if(entity.parentID == 0)
                this.valuesList.Items.AddRange(db.getEntitiesLabels(entity));
            //TODO: il faut récupérer la valeur affichée par l'action
        }

        #endregion

        public void addParent(ListEntity widget)
        {
            //TODO: si parent affiche une valeur, l'enfant doit récupérer les valeurs correspondantes
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
