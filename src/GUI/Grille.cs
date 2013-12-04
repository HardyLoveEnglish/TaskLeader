﻿using System;
using System.Configuration;
using System.Data;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TaskLeader.BO;
using TaskLeader.DAL;
using TaskLeader.BLL;

namespace TaskLeader.GUI
{
    #region DataGridViewColumn de type date

    public class DataGridViewDateCell : DataGridViewTextBoxCell
    {
        public DataGridViewDateCell()
            :base()
        {
            this.ToolTipText = "Modifier la date";
        }
        
        protected override object GetFormattedValue(
            object value,
            int rowIndex,
            ref DataGridViewCellStyle cellStyle,
            System.ComponentModel.TypeConverter valueTypeConverter,
            System.ComponentModel.TypeConverter formattedValueTypeConverter, 
            DataGridViewDataErrorContexts context)
        {
            DateTime date;

            if (DateTime.TryParse(value.ToString(), out date))
            {
                // Récupération du delta en jours
                int diff = (date.Date - DateTime.Now.Date).Days;

                // Modification de la mise en forme des cellules
                if (diff < 0) // En retard
                {
                    cellStyle.ForeColor = Color.Red; // Affichage de la date en rouge
                    cellStyle.Font = new Font(cellStyle.Font, FontStyle.Bold); // en gras
                    cellStyle.SelectionForeColor = Color.DarkRed; // en darkRed sur séléction                    
                }
                else if (diff == 0) // Jour même
                {
                    cellStyle.ForeColor = Color.DarkOrange; // Affichage de la date en orange
                    cellStyle.Font = new Font(cellStyle.Font, FontStyle.Bold);
                    cellStyle.SelectionForeColor = Color.SaddleBrown; // en darkRed sur séléction 
                }
                else if (diff > 0 && diff <= Int32.Parse(ConfigurationManager.AppSettings["P1length"])) // Dans le futur "proche"
                {
                    cellStyle.ForeColor = Color.DarkGreen;
                    cellStyle.Font = new Font(cellStyle.Font, FontStyle.Bold); // en gras
                }

                // Modification du contenu des cellules
                if (diff == 0) // Aujourd'hui
                    return date.ToShortDateString() + Environment.NewLine + "Today"; // Valeur modifiée      
                else if (diff > 0)// Dans le futur
                    return date.ToShortDateString() + Environment.NewLine + "+ " + diff.ToString() + " jours"; // Valeur modifiée
                else
                    return date.ToShortDateString();
            }
            else
                return value.ToString();

        }

        protected override void OnClick(DataGridViewCellEventArgs e)
        {
            base.OnClick(e);

            DataRowView row = this.OwningRow.DataBoundItem as DataRowView;
            //grilleData.Cursor = Cursors.Default;
            new ComplexTooltip(
                new DatePickerPopup(
                    new TLaction(row["id"].ToString(),row["DB"].ToString()),
                    Int32.Parse(this.OwningColumn.Name)
                )
            ).Show();
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            base.OnMouseEnter(rowIndex);
            this.OwningColumn.DataGridView.Cursor = Cursors.Hand;
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
            this.OwningColumn.DataGridView.Cursor = Cursors.Default;
        }
    }

    public class DataGridViewDateColumn : DataGridViewTextBoxColumn
    {
        public DataGridViewDateColumn()
        {
            this.CellTemplate = new DataGridViewDateCell();
        }
    }

    #endregion

    public partial class Grille : UserControl
    {
        /// <summary>
        /// Dictionnaire filtre affiché => DataTable associé
        /// </summary>
        private Dictionary<Filtre, DataTable> data = new Dictionary<Filtre, DataTable>();
        /// <summary>
        /// Retourne le nom des tables affichant des actions de la base en paramètre
        /// </summary>
        /// <param name="db">Nom de la base</param>
        private Filtre[] getFiltersFromDB(String db)
        {
            // Source: http://stackoverflow.com/questions/2968356/linq-transform-dictionarykey-value-to-dictionaryvalue-key
            // ou http://stackoverflow.com/questions/146204/duplicate-keys-in-net-dictionaries
            return this.data
                .ToLookup(kp => kp.Key.dbName, kp => kp.Key)[db] // Key= Nom de la base, Value= Filtres correspondant à cette base
                .ToArray();
        }

        // Récupération de la DataSource de grilleData
        private DataTable mergeTable { get { return this.grilleData.DataSource as DataTable; } }

        public Grille()
        {
            InitializeComponent();

            this.grilleData.AutoGenerateColumns = false; //Les colonnes sont créées manuellement
            
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.Name = "Ref";
            col.DataPropertyName = "Ref";
            grilleData.Columns.Insert(0, col);

            // Création de la colonne des liens
            DataGridViewImageColumn linkCol = new DataGridViewImageColumn();
            linkCol.Name = "Liens";
            linkCol.DataPropertyName = "Liens";
            grilleData.Columns.Insert(1, linkCol);

            this.grilleData.DataSource = new DataTable();

        }

        # region Business

        /// <summary>
        /// Ajoute les actions retournées par le filtre dans le tableau.
        /// </summary>
        /// <param name="filtre">Filtre ajouté</param>
        public int add(Filtre filtre)
        {
            this.data.Add(filtre, filtre.getActions()); // Récupération des résultats du filtre et association au tableau
            TrayIcon.dbs[filtre.dbName].ActionEdited += new ActionEditedEventHandler(actionEdited); // Hook des éditions d'actions de la base correspondante

            // Création des DataGridViewColumn manquantes
            foreach (DBentity entity in TrayIcon.dbs[filtre.dbName].entities.Values)
                if (!grilleData.Columns.Contains(entity.id.ToString())) // Si nouvelle colonne, création d'une nouvelle DataGridViewColumn               
                    grilleData.Columns.Insert(this.grilleData.Columns.Count, entity.getDGWcol());

            this.mergeTable.Merge(this.data[filtre]);
            this.grilleData.Focus();

            return this.mergeTable.Rows.Count;
        }

        /// <summary>
        /// Supprime les actions retournées par le filtre du tableau
        /// </summary>
        /// <param name="filtre">Filtre à supprimer</param>
        public int remove(Filtre filtre)
        {
            this.data.Remove(filtre); // Suppression de la table du DataSet
            TrayIcon.dbs[filtre.dbName].ActionEdited -= new ActionEditedEventHandler(actionEdited);

            this.grilleData.DataSource = new DataTable(); // Efface toutes les données de la table merge
            while (this.grilleData.Columns.Count > 2)
                this.grilleData.Columns.RemoveAt(2); // Suppression de toutes les colonnes dont l'ID est > 2

            // Création de toutes les DataGridViewColumn
            foreach(Filtre filter in this.data.Keys)
                foreach (DBentity entity in TrayIcon.dbs[filter.dbName].entities.Values)
                    if (!grilleData.Columns.Contains(entity.id.ToString())) // Si nouvelle colonne, création d'une nouvelle DataGridViewColumn
                        grilleData.Columns.Insert(this.grilleData.Columns.Count, entity.getDGWcol());

            foreach (DataTable table in this.data.Values)
                this.mergeTable.Merge(table); // Merge des tables restants dans le dataset

            return this.mergeTable.Rows.Count;
        }

        /// <summary>
        /// Mise à jour du contenu de la table quand une action est créée/modifiée
        /// </summary>
        private void actionEdited(DB db, EditedActionEventArgs args)
        {
            // Mise à jour des DataTables liées à la base de l'action
            foreach (Filtre filtre in this.getFiltersFromDB(db.name))
                this.data[filtre] = filtre.getActions().Copy();

            // Rafraîchissement de la mergeTable
            this.mergeTable.Clear();
            foreach (DataTable table in this.data.Values)
                this.mergeTable.Merge(table);

            this.grilleData.Focus(); // Focus sur tableau pour permettre le scroll direct

            // Sélection de la bonne ligne
            this.grilleData.Rows.Cast<DataGridViewRow>()
                .Where(r => r.Cells["Ref"].Value.ToString().Equals(db + Environment.NewLine + "#" + args.actionID))
                .ToList().ForEach(r => r.Selected = true);
        }

        // Ouverture de la gui édition d'action
        private void modifAction(object sender, EventArgs e)
        {
            new ManipAction(
                new TLaction(
                    this.getDataFromRow(this.grilleData.SelectedRows[0].Index, "id"),
                    this.getDataFromRow(this.grilleData.SelectedRows[0].Index, "DB")
                )
            ).Show();
        }

        #endregion

        #region grilleData

        /// <summary>
        /// Retourne une donnée de la DataTable à partir de l'index dans la grille
        /// </summary>
        /// <param name="index">Index de la ligne dans la grille</param>
        /// <param name="col">Nom du champ</param>
        private String getDataFromRow(int index, String col)
        {
            return ((DataRowView)grilleData.Rows[index].DataBoundItem).Row[col].ToString();
        }

        // Mise en forme des cellules sous certaines conditions
        private void grilleData_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Gestion de la colonne PJ: TODO: créer une classe dérivée
            if (grilleData.Columns[e.ColumnIndex].Name.Equals("Liens"))
            {
                switch (e.Value.ToString())
                {
                    case (""):
                        e.Value = null; // Vidage la cellule
                        e.CellStyle.NullValue = null; // Aucun affichage si cellule vide
                        grilleData[e.ColumnIndex, e.RowIndex].ToolTipText = String.Empty;
                        break;
                    case ("1"):
                        // Récupération de la PJ
                        Enclosure pj = TrayIcon.dbs[this.getDataFromRow(e.RowIndex, "DB")].
                            getPJ(this.getDataFromRow(e.RowIndex, "id"))[0];
                        e.Value = pj.BigIcon; // Affichage de la bonne icône
                        grilleData[e.ColumnIndex, e.RowIndex].ToolTipText = pj.Titre; // Modification du tooltip de la cellule
                        if (pj.Type == "Links")
                            grilleData[e.ColumnIndex, e.RowIndex].ToolTipText += Environment.NewLine + ((Link)pj).url;
                        grilleData.Rows[e.RowIndex].Tag = pj; // Tag de la DataGridRow
                        break;
                    default:
                        // On diffère la récupération de liste
                        grilleData[e.ColumnIndex, e.RowIndex].ToolTipText = e.Value.ToString() + " PJ associées";
                        e.Value = TaskLeader.Properties.Resources.attach32;
                        break;
                }
            }
        }

        // Gestion des clicks sur le tableau d'actions
        private void grilleData_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && // Click droit
                e.RowIndex >= 0) // Ce n'est pas la ligne des headers
            {
                grilleData.Rows[e.RowIndex].Selected = true; // Sélection de la ligne           
                listeContext.Show(Cursor.Position); // Affichage du menu contextuel
            }

            if (e.Button == MouseButtons.Left && // Click gauche
                grilleData.Columns[e.ColumnIndex].Name.Equals("Liens") && // Colonne "Liens"
                e.RowIndex >= 0 && // Ce n'est pas la ligne des headers
                grilleData[e.ColumnIndex, e.RowIndex].Value.ToString() != "0") // Cellule non vide
            {
                if (grilleData[e.ColumnIndex, e.RowIndex].Value.ToString() == "1") // Un lien seulement
                    ((Enclosure)grilleData.Rows[e.RowIndex].Tag).open(); // Ouverture directe
                else // Plusieurs liens
                {
                    DB db = TrayIcon.dbs[this.getDataFromRow(e.RowIndex, "DB")];
                    List<Enclosure> links = db.getPJ(this.getDataFromRow(e.RowIndex, "id")); //Récupération des différents liens
                    linksContext.Items.Clear(); // Remise à zéro de la liste

                    foreach (Enclosure link in links)
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem(link.Titre, link.SmallIcon, this.linksContext_ItemClicked); // Création du lien avec le titre et l'icône
                        item.Tag = link; // Association du link
                        linksContext.Items.Add(item); // Ajout au menu
                    }

                    linksContext.Show(Cursor.Position); // Affichage du menu contextuel de liste
                }
            }
        }

        // Affichage d'un curseur doigt si mail attaché.
        private void grilleData_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool pjActivated =
                grilleData.Columns[e.ColumnIndex].Name.Equals("Liens") &&
                e.RowIndex >= 0 &&
                grilleData[e.ColumnIndex, e.RowIndex].Value.ToString() != "";

            if (pjActivated)
                grilleData.Cursor = Cursors.Hand;
        }

        private void grilleData_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            bool pjActivated =
                grilleData.Columns[e.ColumnIndex].Name.Equals("Liens") &&
                e.RowIndex >= 0 &&
                grilleData[e.ColumnIndex, e.RowIndex].Value.ToString() != "";

            if (pjActivated)
                grilleData.Cursor = Cursors.Default;
        }

        #endregion

        #region linksContext

        // Ouverture du lien
        private void linksContext_ItemClicked(object sender, EventArgs e)
        {
            ((Enclosure)((ToolStripMenuItem)sender).Tag).open();
        }

        #endregion

    }
}
