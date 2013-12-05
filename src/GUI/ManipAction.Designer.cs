namespace TaskLeader.GUI
{
    partial class ManipAction
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

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.saveButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.entitiesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.dbsBox = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.linksLabel = new System.Windows.Forms.Label();
            this.addMailBut = new System.Windows.Forms.Button();
            this.addLinkBut = new System.Windows.Forms.Button();
            this.AddMailLabel = new System.Windows.Forms.Label();
            this.linksView = new System.Windows.Forms.ListView();
            this.pjListViewCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.linksViewMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameEncItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteEncItem = new System.Windows.Forms.ToolStripMenuItem();
            this.biblio = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel3.SuspendLayout();
            this.linksViewMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(645, 4);
            this.saveButton.Margin = new System.Windows.Forms.Padding(350, 4, 10, 3);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(81, 28);
            this.saveButton.TabIndex = 21;
            this.saveButton.Text = "Sauvegarder";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.sauveAction);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 430F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel1.Controls.Add(this.entitiesPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(746, 397);
            this.tableLayoutPanel1.TabIndex = 27;
            // 
            // entitiesPanel
            // 
            this.entitiesPanel.AutoScroll = true;
            this.entitiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entitiesPanel.Location = new System.Drawing.Point(3, 45);
            this.entitiesPanel.Name = "entitiesPanel";
            this.entitiesPanel.Size = new System.Drawing.Size(424, 349);
            this.entitiesPanel.TabIndex = 17;
            // 
            // flowLayoutPanel5
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel5, 2);
            this.flowLayoutPanel5.Controls.Add(this.pictureBox1);
            this.flowLayoutPanel5.Controls.Add(this.label7);
            this.flowLayoutPanel5.Controls.Add(this.dbsBox);
            this.flowLayoutPanel5.Controls.Add(this.saveButton);
            this.flowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(740, 36);
            this.flowLayoutPanel5.TabIndex = 21;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::TaskLeader.Properties.Resources.database_go32;
            this.pictureBox1.Location = new System.Drawing.Point(5, 2);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(5, 2, 3, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(43, 10);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Base d\'actions:";
            // 
            // dbsBox
            // 
            this.dbsBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dbsBox.FormattingEnabled = true;
            this.dbsBox.Location = new System.Drawing.Point(128, 7);
            this.dbsBox.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.dbsBox.Name = "dbsBox";
            this.dbsBox.Size = new System.Drawing.Size(164, 21);
            this.dbsBox.TabIndex = 1;
            this.dbsBox.SelectedValueChanged += new System.EventHandler(this.changeDB);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.linksLabel);
            this.flowLayoutPanel3.Controls.Add(this.addMailBut);
            this.flowLayoutPanel3.Controls.Add(this.addLinkBut);
            this.flowLayoutPanel3.Controls.Add(this.AddMailLabel);
            this.flowLayoutPanel3.Controls.Add(this.linksView);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(433, 45);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Padding = new System.Windows.Forms.Padding(0, 50, 0, 0);
            this.flowLayoutPanel3.Size = new System.Drawing.Size(310, 349);
            this.flowLayoutPanel3.TabIndex = 19;
            // 
            // linksLabel
            // 
            this.linksLabel.AutoSize = true;
            this.linksLabel.Location = new System.Drawing.Point(20, 58);
            this.linksLabel.Margin = new System.Windows.Forms.Padding(20, 8, 3, 0);
            this.linksLabel.Name = "linksLabel";
            this.linksLabel.Size = new System.Drawing.Size(35, 13);
            this.linksLabel.TabIndex = 13;
            this.linksLabel.Text = "Liens:";
            // 
            // addMailBut
            // 
            this.addMailBut.Image = global::TaskLeader.Properties.Resources.email_add;
            this.addMailBut.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addMailBut.Location = new System.Drawing.Point(61, 53);
            this.addMailBut.Name = "addMailBut";
            this.addMailBut.Size = new System.Drawing.Size(90, 25);
            this.addMailBut.TabIndex = 28;
            this.addMailBut.Text = "Ajouter mail";
            this.addMailBut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addMailBut.UseVisualStyleBackColor = true;
            this.addMailBut.Click += new System.EventHandler(this.addMailBut_Click);
            // 
            // addLinkBut
            // 
            this.addLinkBut.Image = global::TaskLeader.Properties.Resources.link_add;
            this.addLinkBut.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addLinkBut.Location = new System.Drawing.Point(157, 53);
            this.addLinkBut.Name = "addLinkBut";
            this.addLinkBut.Size = new System.Drawing.Size(115, 25);
            this.addLinkBut.TabIndex = 30;
            this.addLinkBut.Text = "Ajouter fichier/url";
            this.addLinkBut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addLinkBut.UseVisualStyleBackColor = true;
            this.addLinkBut.Click += new System.EventHandler(this.addLinkBut_Click);
            // 
            // AddMailLabel
            // 
            this.AddMailLabel.AutoSize = true;
            this.AddMailLabel.Location = new System.Drawing.Point(278, 58);
            this.AddMailLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.AddMailLabel.Name = "AddMailLabel";
            this.AddMailLabel.Size = new System.Drawing.Size(25, 13);
            this.AddMailLabel.TabIndex = 29;
            this.AddMailLabel.Text = "add";
            this.AddMailLabel.Visible = false;
            // 
            // linksView
            // 
            this.linksView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pjListViewCol});
            this.linksView.ContextMenuStrip = this.linksViewMenu;
            this.linksView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.linksView.HoverSelection = true;
            this.linksView.LabelEdit = true;
            this.linksView.Location = new System.Drawing.Point(3, 84);
            this.linksView.MultiSelect = false;
            this.linksView.Name = "linksView";
            this.linksView.Size = new System.Drawing.Size(300, 196);
            this.linksView.SmallImageList = this.biblio;
            this.linksView.TabIndex = 27;
            this.linksView.UseCompatibleStateImageBehavior = false;
            this.linksView.View = System.Windows.Forms.View.Details;
            this.linksView.Visible = false;
            this.linksView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.linksView_AfterLabelEdit);
            this.linksView.DoubleClick += new System.EventHandler(this.pj_Click);
            // 
            // pjListViewCol
            // 
            this.pjListViewCol.Text = "PJ";
            this.pjListViewCol.Width = 25;
            // 
            // linksViewMenu
            // 
            this.linksViewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameEncItem,
            this.deleteEncItem});
            this.linksViewMenu.Name = "linksViewMenu";
            this.linksViewMenu.Size = new System.Drawing.Size(168, 48);
            this.linksViewMenu.Opening += new System.ComponentModel.CancelEventHandler(this.linksViewMenu_Opening);
            // 
            // renameEncItem
            // 
            this.renameEncItem.Image = global::TaskLeader.Properties.Resources.textfield_rename;
            this.renameEncItem.Name = "renameEncItem";
            this.renameEncItem.Size = new System.Drawing.Size(167, 22);
            this.renameEncItem.Text = "Renommer le lien";
            this.renameEncItem.Click += new System.EventHandler(this.renameEncItem_Click);
            // 
            // deleteEncItem
            // 
            this.deleteEncItem.Image = global::TaskLeader.Properties.Resources.cross;
            this.deleteEncItem.Name = "deleteEncItem";
            this.deleteEncItem.Size = new System.Drawing.Size(167, 22);
            this.deleteEncItem.Text = "Supprimer le lien";
            this.deleteEncItem.Click += new System.EventHandler(this.deleteEncItem_Click);
            // 
            // biblio
            // 
            this.biblio.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.biblio.ImageSize = new System.Drawing.Size(30, 30);
            this.biblio.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ManipAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 397);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "ManipAction";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ManipAction_FormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.linksViewMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel entitiesPanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label linksLabel;
        private System.Windows.Forms.ImageList biblio;
        private System.Windows.Forms.Button addMailBut;
        private System.Windows.Forms.ListView linksView;
        private System.Windows.Forms.Label AddMailLabel;
        private System.Windows.Forms.ContextMenuStrip linksViewMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteEncItem;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox dbsBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button addLinkBut;
        private System.Windows.Forms.ToolStripMenuItem renameEncItem;
        private System.Windows.Forms.ColumnHeader pjListViewCol;


    }
}

