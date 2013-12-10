using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using TaskLeader.DAL;
using TaskLeader.BO;

namespace TaskLeader.GUI
{
    public partial class AdminDefaut : Form
    {
        // Variables locales identifiant la base courante
        private String dbName;
        private DB db { get { return TrayIcon.dbs[dbName]; } }

        public AdminDefaut(String database)
        {
            InitializeComponent();
            this.dbName = database;
            this.Text += " - Base: " + dbName;
        }

        private void AdminDefaut_Load(object sender, EventArgs e)
        {        
            //Remplissage de la combo des filtres
            filterCombo.Items.AddRange(db.getFilters());
            filterCombo.Text = db.getDefaultFilterName();

            // Préparation des widgets
            this.entitiesPanel.SuspendLayout();

            Dictionary<int,EntityValue> defaultValues = this.db.getDefault();
            foreach (var kvp in this.db.entities)
            {
                if (defaultValues.ContainsKey(kvp.Key))
                    this.entitiesPanel.Controls.Add(kvp.Value.getWidget(this.dbName, defaultValues[kvp.Key], this.entitiesPanel));
                else
                    this.entitiesPanel.Controls.Add(kvp.Value.getWidget(this.dbName, kvp.Value.getEntityValue(), this.entitiesPanel));
            }

            this.entitiesPanel.ResumeLayout();
        }

        private void saveBut_Click(object sender, EventArgs e)
        {
            // Récupération de la liste des valeurs mise à jour
            Dictionary<int, EntityValue> updatedValues = new Dictionary<int, EntityValue>();

            foreach (IValueRetrievable control in this.entitiesPanel.Controls)
                updatedValues.Add(control.entityID, control.value);

            if (filterCombo.SelectedIndex > 0)
                db.insertDefaultFilter(((Filtre)filterCombo.SelectedItem).id);

            // Sauvegarde
            db.insertDefaut(updatedValues);

            // On affiche un message de statut sur la TrayIcon
            TrayIcon.afficheMessage("Bilan création/modification", "Valeurs par défaut mises à jour");

            //Fermeture de la Form
            this.Close();
        }
    }
}
