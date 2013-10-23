namespace TaskLeader.GUI
{
    partial class DateEntity
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.datePicker = new System.Windows.Forms.DateTimePicker();
            this.noDate = new System.Windows.Forms.CheckBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.nameLabel);
            this.flowLayoutPanel1.Controls.Add(this.datePicker);
            this.flowLayoutPanel1.Controls.Add(this.noDate);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(400, 31);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // datePicker
            // 
            this.datePicker.Location = new System.Drawing.Point(73, 3);
            this.datePicker.Name = "datePicker";
            this.datePicker.Size = new System.Drawing.Size(177, 20);
            this.datePicker.TabIndex = 26;
            // 
            // noDate
            // 
            this.noDate.AutoSize = true;
            this.noDate.Location = new System.Drawing.Point(256, 6);
            this.noDate.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.noDate.Name = "noDate";
            this.noDate.Size = new System.Drawing.Size(63, 17);
            this.noDate.TabIndex = 27;
            this.noDate.Text = "Aucune";
            this.noDate.UseVisualStyleBackColor = true;
            this.noDate.CheckedChanged += new System.EventHandler(this.noDate_CheckedChanged);
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(3, 6);
            this.nameLabel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(64, 13);
            this.nameLabel.TabIndex = 28;
            this.nameLabel.Text = "EntityName:";
            // 
            // DateEntity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "DateEntity";
            this.Size = new System.Drawing.Size(400, 31);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.DateTimePicker datePicker;
        private System.Windows.Forms.CheckBox noDate;
        private System.Windows.Forms.Label nameLabel;
    }
}
