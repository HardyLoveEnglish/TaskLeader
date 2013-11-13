namespace TaskLeader.GUI
{
    partial class ListEntity
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.nameLabel = new System.Windows.Forms.Label();
            this.valuesList = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(87, 6);
            this.nameLabel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(64, 13);
            this.nameLabel.TabIndex = 14;
            this.nameLabel.Text = "EntityName:";
            // 
            // valuesList
            // 
            this.valuesList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.valuesList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.valuesList.FormattingEnabled = true;
            this.valuesList.Location = new System.Drawing.Point(157, 3);
            this.valuesList.Name = "valuesList";
            this.valuesList.Size = new System.Drawing.Size(240, 21);
            this.valuesList.TabIndex = 13;
            this.valuesList.TextUpdate += new System.EventHandler(this.valuesList_TextUpdate);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.valuesList);
            this.flowLayoutPanel1.Controls.Add(this.nameLabel);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(400, 30);
            this.flowLayoutPanel1.TabIndex = 15;
            // 
            // ListEntity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "ListEntity";
            this.Size = new System.Drawing.Size(400, 30);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.ComboBox valuesList;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;

    }
}
