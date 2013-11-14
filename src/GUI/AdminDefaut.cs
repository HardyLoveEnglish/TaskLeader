using System;
using System.Configuration;
using System.Windows.Forms;
using System.Collections;
using TaskLeader.DAL;

namespace TaskLeader.GUI
{
    public partial class AdminDefaut : Form
    {
        String empty = "-- Aucun --";

        // Variables locales identifiant la base courante
        private String dbName;
        private DB db { get { return TrayIcon.dbs[dbName]; } }

        // Préparation des widgets
        private void loadWidgets()
        {
            this.entitiesPanel.SuspendLayout();
            this.entitiesPanel.Controls.Clear();

            foreach (int entityID in this.db.entities.Keys)
            {
                UserControl widget = new UserControl();
                switch (this.db.entities[entityID].type)
                {
                    case ("List"):
                        widget = new ListEntity(this.db, entityID, this._action.getValue(entityID));
                        int parentID = this.db.entities[entityID].parentID;
                        if (parentID > 0)
                            ((ListEntity)widget).addParent(this.entitiesPanel.Controls[this.db.entities[parentID].nom] as ListEntity);
                        break;
                    case ("Text"):
                        widget = new TextEntity(this.db, entityID, this._action.getValue(entityID));
                        break;
                    case ("Date"):
                        widget = new DateEntity(this.db, entityID, this._action.getValue(entityID));
                        break;
                }
                this.entitiesPanel.Controls.Add(widget);
            }

            this.entitiesPanel.ResumeLayout();
        }

        public AdminDefaut(String database)
        {
            InitializeComponent();
            this.dbName = database;
            this.Text += " - Base: " + dbName;
        }

        private void AdminDefaut_Load(object sender, EventArgs e)
        {        
            //Remplissage des combos
            ctxtListBox.Items.Add(empty);
            ctxtListBox.Items.AddRange(db.getEntitiesValues(DB.contexte));
            destListBox.Items.Add(empty);
            destListBox.Items.AddRange(db.getEntitiesValues(DB.destinataire));
            statutListBox.Items.Add(empty);
            statutListBox.Items.AddRange(db.getEntitiesValues(DB.statut));
            filterCombo.Items.Add(empty);
            filterCombo.Items.AddRange(db.getFiltersLabels());

            //Sélection des valeurs par défaut

            ctxtListBox.Text = db.getDefault(DB.contexte);
            if (ctxtListBox.Text == "")
                ctxtListBox.SelectedIndex = 0; // Sélection de la ligne "Aucun"

            this.updateSujet(sender, e);

            destListBox.Text = db.getDefault(DB.destinataire);
            if (destListBox.Text == "")
                destListBox.SelectedIndex = 0;

            statutListBox.Text = db.getDefault(DB.statut);
            if (statutListBox.Text == "")
                statutListBox.SelectedIndex = 0;

            filterCombo.Text = db.getDefault(DB.filtre);
            if (filterCombo.Text == "")
                filterCombo.SelectedIndex = 0;
        }

        private void updateSujet(object sender, EventArgs e)
        {
            // Remise à zéro de la liste
            sujetListBox.Items.Clear();
            sujetListBox.Items.Add(empty);
            sujetListBox.Enabled = true;

            if (ctxtListBox.SelectedIndex > 0) // Uniquement si contexte différent de "Aucun"
            {
                // Remplissage de la liste
                sujetListBox.Items.AddRange(db.getEntitiesValues(DB.sujet,ctxtListBox.Text));

                // Sélection du sujet par défaut
                sujetListBox.Text = db.getDefault(DB.sujet);
                if (sujetListBox.Text == "")
                    sujetListBox.SelectedIndex = 0;
            }
            else
                sujetListBox.Enabled = false;
        }

        private void saveBut_Click(object sender, EventArgs e)
        {
            // Récupération de la liste des valeurs mise à jour
            ArrayList updatedValues = new ArrayList();

            if (ctxtListBox.SelectedIndex > 0)
                updatedValues.Add(new DBvalue(DB.contexte, ctxtListBox.Text));

            if (sujetListBox.SelectedIndex > 0)
                updatedValues.Add(new DBvalue(DB.sujet, sujetListBox.Text));

            if (destListBox.SelectedIndex > 0)
                updatedValues.Add(new DBvalue(DB.destinataire, destListBox.Text));

            if (statutListBox.SelectedIndex > 0)
                updatedValues.Add(new DBvalue(DB.statut, statutListBox.Text));

            if (filterCombo.SelectedIndex > 0)
                updatedValues.Add(new DBvalue(DB.filtre, filterCombo.Text));

            // Sauvegarde
            db.insertDefaut(updatedValues.ToArray());
            // On affiche un message de statut sur la TrayIcon
            TrayIcon.afficheMessage("Bilan création/modification", "Valeurs par défaut mises à jour");

            //Fermeture de la Form
            this.Close();
        }
    }
}
