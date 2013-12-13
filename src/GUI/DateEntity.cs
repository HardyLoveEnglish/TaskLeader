using System;
using System.Windows.Forms;
using TaskLeader.BO;

namespace TaskLeader.GUI
{
    public partial class DateEntity : UserControl, IValueRetrievable
    {
        // EntityControl members
        private int _entityID;
        public int entityID { get { return _entityID; } }
        public EntityValue value {
            get {
                if (noDate.Checked)
                    return new DateValue();
                else
                    return new DateValue() { value = datePicker.Value };
            }
        }

        public DateEntity()
        {
            InitializeComponent();
        }

        public DateEntity(String dbName, int entityID, EntityValue entityValue)
            :this()
        {
            this._entityID = entityID;
            String entityName = TrayIcon.dbs[dbName].entities[entityID].nom;

            this.Name = entityName; //Permet de sélectionner ce contrôle avec son nom
            this.nameLabel.Text = entityName;

            DateValue date = entityValue as DateValue;
            if (date.isSet)
                datePicker.Value = date.value;
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
