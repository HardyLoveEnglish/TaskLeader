using System;
using System.Windows.Forms;
using TaskLeader.GUI;

namespace TaskLeader.BO
{
    // Structure listant les différentes informations liées à une entité de la base
    public class DBentity
    {
        /// <summary>
        /// ID de l'entité dans la base
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Nom de l'entité pour IHM
        /// </summary>
        public String nom { get; set; }

        /// <summary>
        /// Type de l'entité: List, Text, Date
        /// </summary>
        public String type { get; set; }

        /// <summary>
        /// ID de l'entité parente
        /// </summary>
        public int parentID { get; set; }

        public UserControl getWidget(String dbName)
        {
            switch (this.type)
            {
                case "List":
                    return new ListEntity(dbName, this.id, value);
                case "Text":
                    return new TextEntity(dbName, this.id,value);
                case "Date":
                    return new DateEntity(dbName, this.id, ValueType);
            } 
        }    
    }

    public abstract class EntityValue
    {
        // Méthode privée pour fabriquer des string compatible sql
        protected String sqlFactory(String original) { return "'" + original.Replace("'", "''") + "'"; }
        public abstract String sqlValue { get; }

        protected abstract bool Equals(EntityValue b);

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            EntityValue comp = obj as EntityValue;
            if (comp == null)
                return false;
            else
                return Equals(comp);
        }

        public static bool operator ==(EntityValue a, EntityValue b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
                return true;

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
                return false;

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(EntityValue a, EntityValue b)
        {
            return !(a == b);
        }
    }

    /// <summary>
    /// Classe permettant de stocker des valeurs des entités de type List
    /// </summary>
    public class ListValue : EntityValue
    {
        /// <summary>
        /// Id de la valeur dans la  base
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// String représentant la valeur
        /// </summary>
        public String value { get; set; }
        public override String sqlValue { get { return this.sqlFactory(this.value); } }

        public override String ToString()
        {
            return value;
        }

        public override bool Equals(ListValue b)
        {
            // Return true if the fields match:
            return (this.id == b.id && (this.id > 0 || this.value == b.value));
        }
    }

    public class DateValue : EntityValue
    {
        private DateTime _value;
        public DateTime value { get { return _value; } }
        public String sqlValue { get { return "'" + this._value.ToString("yyyy-MM-dd") + "'"; } }

        public DateValue(String valeur)
        {
            DateTime.TryParse(valeur, out this._value); // Si le TryParse échoue, dateValue = DateTime.MinValue
        }
    }

    public class TextValue : EntityValue
    {
        public String value { get; set; }
        public String sqlValue { get { return this.sqlFactory(this.value); } }
    }
}
