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
    public partial class DateEntity : UserControl, EntityControl
    {
        // EntityControl members
        public int entityID;
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
            this.entityID = entityID;
            DBentity _entity = db.entities[entityID];

            this.Name = _entity.nom; //Permet de sélectionner ce contrôle avec son nom
            this.nameLabel.Text = _entity.nom;

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
