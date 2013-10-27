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
    public partial class DateEntity : UserControl, IValueRetrievable
    {
        // EntityControl members
        private int _entityID;
        public int entityID { get { return _entityID; } }
        public object value {
            get {
                if (noDate.Checked)
                    return DateTime.MinValue;
                else
                    return datePicker.Value;
            }
        }

        public DateEntity()
        {
            InitializeComponent();
        }

        public DateEntity(DB db, int entityID, object entityValue)
            :this()
        {
            this._entityID = entityID;
            String entityName = db.entities[entityID].nom;

            this.Name = entityName; //Permet de sélectionner ce contrôle avec son nom
            this.nameLabel.Text = entityName;

            DateTime value = (DateTime)entityValue;
            if (value.Date != DateTime.MinValue.Date)
                datePicker.Value = value;
            else
                noDate.Checked = true;

        }

        // Mise à jour du widget date en fonction de la sélection de la checkbox
        private void noDate_CheckedChanged(object sender, EventArgs e)
        {
            datePicker.Enabled = !noDate.Checked;
        }
    }
}
