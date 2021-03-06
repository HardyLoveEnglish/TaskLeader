﻿namespace TaskLeader.GUI
{
    partial class Etiquette
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
            this.valeurLabel = new System.Windows.Forms.Label();
            this.typeLabel = new System.Windows.Forms.Label();
            this.searchFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.infoBox = new System.Windows.Forms.PictureBox();
            this.closeBox = new System.Windows.Forms.PictureBox();
            this.searchFlowLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // valeurLabel
            // 
            this.valeurLabel.AutoSize = true;
            this.valeurLabel.Location = new System.Drawing.Point(38, 4);
            this.valeurLabel.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.valeurLabel.Name = "valeurLabel";
            this.valeurLabel.Size = new System.Drawing.Size(36, 13);
            this.valeurLabel.TabIndex = 4;
            this.valeurLabel.Text = "valeur";
            // 
            // typeLabel
            // 
            this.typeLabel.AutoSize = true;
            this.typeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.typeLabel.Location = new System.Drawing.Point(3, 4);
            this.typeLabel.Margin = new System.Windows.Forms.Padding(3, 4, 0, 0);
            this.typeLabel.Name = "typeLabel";
            this.typeLabel.Size = new System.Drawing.Size(35, 13);
            this.typeLabel.TabIndex = 5;
            this.typeLabel.Text = "Type";
            // 
            // searchFlowLayoutPanel
            // 
            this.searchFlowLayoutPanel.AutoSize = true;
            this.searchFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.searchFlowLayoutPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.searchFlowLayoutPanel.Controls.Add(this.typeLabel);
            this.searchFlowLayoutPanel.Controls.Add(this.valeurLabel);
            this.searchFlowLayoutPanel.Controls.Add(this.infoBox);
            this.searchFlowLayoutPanel.Controls.Add(this.closeBox);
            this.searchFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.searchFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(5, 4, 0, 4);
            this.searchFlowLayoutPanel.Name = "searchFlowLayoutPanel";
            this.searchFlowLayoutPanel.Size = new System.Drawing.Size(118, 22);
            this.searchFlowLayoutPanel.TabIndex = 6;
            // 
            // infoBox
            // 
            this.infoBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.infoBox.Image = global::TaskLeader.Properties.Resources.information;
            this.infoBox.Location = new System.Drawing.Point(77, 3);
            this.infoBox.Name = "infoBox";
            this.infoBox.Size = new System.Drawing.Size(16, 16);
            this.infoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.infoBox.TabIndex = 6;
            this.infoBox.TabStop = false;
            this.infoBox.Click += new System.EventHandler(this.infoBox_Click);
            // 
            // closeBox
            // 
            this.closeBox.BackColor = System.Drawing.Color.Transparent;
            this.closeBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.closeBox.Image = global::TaskLeader.Properties.Resources.cross;
            this.closeBox.Location = new System.Drawing.Point(99, 3);
            this.closeBox.Name = "closeBox";
            this.closeBox.Size = new System.Drawing.Size(16, 16);
            this.closeBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.closeBox.TabIndex = 7;
            this.closeBox.TabStop = false;
            this.closeBox.Click += new System.EventHandler(this.exitSearchBut_Click);
            // 
            // Etiquette
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.searchFlowLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "Etiquette";
            this.Size = new System.Drawing.Size(118, 26);
            this.searchFlowLayoutPanel.ResumeLayout(false);
            this.searchFlowLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label valeurLabel;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.FlowLayoutPanel searchFlowLayoutPanel;
        private System.Windows.Forms.PictureBox infoBox;
        private System.Windows.Forms.PictureBox closeBox;

    }
}
