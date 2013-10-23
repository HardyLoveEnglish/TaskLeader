using System;
using System.Windows.Forms;
using TaskLeader.DAL;

namespace TaskLeader.GUI
{
    public partial class TextEntity : UserControl, EntityControl
    {
        // EntityControl members
        public int entityID;
        public object value { get { return this.entityValue.Text; } }

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
        public TextEntity(DB db, int entityID, object entityValue)
            : this()
        {
            this.entityID = entityID;
            DBentity _entity = db.entities[entityID];

            this.Name = _entity.nom; //Permet de sélectionner ce contrôle avec son nom
            this.nameLabel.Text = _entity.nom;

            this.entityValue.Text = entityValue as String;
            this.entityValue.Select(this.entityValue.Text.Length, 0); // Curseur placé à la fin par défaut
        }
    }
}
