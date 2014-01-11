﻿using System;
using System.Windows.Forms;
using TaskLeader.GUI;
using System.Runtime.Serialization;

namespace TaskLeader.BO
{
    // Structure listant les différentes informations liées à une entité de la base
	[DataContract]
    public class DBentity
    {
        /// <summary>
        /// ID de l'entité dans la base
        /// </summary>
		[DataMember]
        public int id { get; set; }

        /// <summary>
        /// Nom de l'entité pour IHM
        /// </summary>
		[DataMember]
        public String nom { get; set; }

        /// <summary>
        /// Type de l'entité: List, Text, Date
        /// </summary>
        public String type { get; set; }

        /// <summary>
        /// ID de l'entité parente
        /// </summary>
		[DataMember]
        public int parentID { get; set; }

        /// <summary>
        /// Retourne le widget IHM correspondant au type
        /// </summary>
        /// <param name="dbName">Nom de la DB correspondante</param>
        /// <param name="value">EntityValue à sélectionner</param>
        public UserControl getWidget(String dbName, EntityValue value, Control parentPanel)
        {
            switch (this.type)
            {
                case "List":
                    return new ListEntity(dbName, this.id, value, parentPanel);
                case "Text":
                    return new TextEntity(dbName, this.id, value);
                case "Date":
                    return new DateEntity(dbName, this.id, value);
                default:
                    return null;
            } 
        }

        /// <summary>
        /// Création de l'EntityValue correspondante
        /// </summary>
        /// <param name="value">Valeur de l'entité: Date, Texte ou ID</param>
        /// <param name="label">Dans le cas d'un type List, valeur du label</param>
        public EntityValue getEntityValue(String value ="", String label = "")
        {
            switch (this.type)
            {
                case "List":
                    return new ListValue(value, label);
                case "Text":
                    return new TextValue() { value = value };
                case "Date":
                    return new DateValue(value);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Création de la DataGridViewColum correspondante
        /// </summary>
        public DataGridViewColumn getDGWcol()
        {
            DataGridViewColumn col;

            switch (this.type)
            {
                case "List":
                    col = new DataGridViewTextBoxColumn();
                    break;
                case "Text":
                    col = new DataGridViewTextBoxColumn();
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    break;
                case "Date":
                    col = new DataGridViewDateColumn();
                    break;
                default:
                    col = new DataGridViewColumn();
                    break;
            }

            col.Name = this.id.ToString();
            col.HeaderText = this.nom;
            col.DataPropertyName = this.nom;
            return col;
        }
    }

    [DataContract]
    public abstract class EntityValue
    {
        // Méthode privée pour fabriquer des string compatible sql
        protected String sqlFactory(String original) { return "'" + original.Replace("'", "''") + "'"; }
        public abstract String sqlValue { get; }

        public abstract bool Equals(EntityValue b);

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
    [DataContract]
    public class ListValue : EntityValue
    {
        /// <summary>
        /// Id de la valeur dans la  base
        /// </summary>
        private int _id = 0;
        [DataMember]
        public int id { get { return _id; } set {if (value > 0) this._id = value; else throw new Exception("id doit être positif"); } }
        /// <summary>
        /// Précise si la ListValue a déjà été stockée en base
        /// </summary>
        public bool isNew { get { return _id < 0; } }

        /// <summary>
        /// Valeur SQL de l'entité= ID !
        /// </summary>
        public override String sqlValue {
            get {
                if (this._id < 0)
                    throw new Exception("L'ID n'est pas instancié");

                if (this._id > 0)
                    return this.id.ToString();
                else // id=0
                    return "NULL";
            }
        }

        /// <summary>
        /// String représentant la valeur
        /// </summary>
        [DataMember]
        public String label { get; set; }
        public String sqlLabel { get { return this.sqlFactory(this.label); } }

        #region Constructors

        /// <summary>
        /// Constructeur permettant l'accès aux propriétés publiques
        /// </summary>
        public ListValue() { }
        
        /// <summary>
        /// Constructeur vérifiant les entrées
        /// </summary>
        /// <param name="id">String représentant l'id</param>
        /// <param name="label">Label correspondant</param>
        public ListValue(String id, String label)
        {
            Int32.TryParse(id, out this._id); //Si échec du parse (valeur nulle par exemple), _id=0
            this.label = label;
        }

        /// <summary>
        /// Constructeur spécifiant uniquement le label
        /// </summary>
        public ListValue(String label)
        {
            this._id = -1;
            this.label = label;
        }

        #endregion

        public override String ToString()
        {
            return label;
        }

        public override bool Equals(EntityValue b)
        {
            // Return true if the fields match:
            return (this.id == ((ListValue)b).id && (this.id > 0 || this.label == ((ListValue)b).label));
        }
    }

    public class DateValue : EntityValue
    {
        private DateTime _value;
        public DateTime value {
            get { return _value; }
            set { _value = value; }
        }
        public override String sqlValue
        {
            get
            {
                if (this._value == DateTime.MinValue)
                    return "NULL";
                else
                    return "'" + this._value.ToString("yyyy-MM-dd") + "'";
            }
        }

        public bool isSet { get { return this._value != DateTime.MinValue; } }

        #region Constructeurs

        public DateValue(DateTime date)
        {
            this._value = date;
        }

        public DateValue(String valeur)
        {
            DateTime.TryParse(valeur, out this._value); // Si le TryParse échoue, dateValue = DateTime.MinValue
        }

        public DateValue()
        {
            this._value = DateTime.MinValue;
        }

        #endregion

        public override bool Equals(EntityValue b)
        {
            // Return true if the fields match:
            return (this._value == ((DateValue)b).value);
        }
    }

    public class TextValue : EntityValue
    {
        public String value { get; set; }
        public override String sqlValue {
            get {
                if(String.IsNullOrWhiteSpace(this.value))
                    return "NULL";
                else
                    return this.sqlFactory(this.value);
            }
        }

        public override bool Equals(EntityValue b)
        {
            return (this.value == ((TextValue)b).value);
        }

        public override string ToString()
        {
            return this.value;
        }
    }
}