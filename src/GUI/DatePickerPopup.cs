using System;
using System.Windows.Forms;
using TaskLeader.BO;

namespace TaskLeader.GUI
{
    public partial class DatePickerPopup : UserControl, IComplexToolTipContent
    {
        #region Implémentation de l'interface IComplexToolTipContent

        public event EventHandler ClosureRequested;
        private void OnClosureRequested(object sender,EventArgs e)
        {
            if (this.ClosureRequested != null)
                this.ClosureRequested(sender,e); //Invoque le délégué
        }

        public Control control { get { return this; } }

        #endregion

        // Déclaration de l'action associée
        private TLaction v_action;
        private int _entityID;
        public String ID { get { return v_action.ID; } }

        public DatePickerPopup(TLaction action, int entityID)
        {
            InitializeComponent();
            v_action = action;
            _entityID = entityID;
            DateValue date = action.getValue(entityID) as DateValue;

            // Initialisation du composant calendar
            if (date.isSet)
                calendar.SelectionStart = date.value;
            else
                this.noDueDate.Checked = true;
        }

        // Désactivation du calendrier si nécessaire
        private void noDueDate_CheckStateChanged(object sender, System.EventArgs e)
        {
            calendar.Visible = !noDueDate.Checked;
        }

        // Sauvegarde de la nouvelle deadline
        private void validBut_Click(object sender, System.EventArgs e)
        {
            // Update de la DueDate que si c'est nécessaire
            if (noDueDate.Checked)
                v_action.setValue(_entityID, new DateValue());
            else
                v_action.setValue(_entityID, new DateValue(calendar.SelectionStart.Date));

            // On sauvegarde l'action
            v_action.save();

            // Fermeture de la fenêtre
            this.OnClosureRequested(sender, e);
        }

        private void closeBut_Click(object sender, EventArgs e)
        {
            this.OnClosureRequested(sender, e);
        }
    }
}
