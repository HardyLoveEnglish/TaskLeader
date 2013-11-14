﻿using System;
using System.Windows.Forms;
using TaskLeader.DAL;
using TaskLeader.BO;

namespace TaskLeader.GUI
{
    public partial class TextEntity : UserControl, IValueRetrievable
    {
        private int _entityID;

        // EntityControl members
        public int entityID { get { return _entityID; } }
        public EntityValue value { get { return new TextValue() { value = this.entityValue.Text }; } }

        public TextEntity()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="db">Source DB</param>
        /// <param name="entityID">Related Text EntityID</param>
        /// <param name="entityValue">Value to be displayed in the text field</param>
        public TextEntity(String dbName, int entityID, EntityValue entityValue)
            : this()
        {
            this._entityID = entityID;
            String entityName = TrayIcon.dbs[dbName].entities[entityID].nom;

            this.Name = entityName; //Permet de sélectionner ce contrôle avec son nom
            this.nameLabel.Text = entityName;
            
            this.entityValue.Text = entityValue.ToString();
            this.entityValue.Select(this.entityValue.Text.Length, 0); // Curseur placé à la fin par défaut
        }
    }
}
