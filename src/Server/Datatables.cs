using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using TaskLeader.BO;

namespace TaskLeader.Server
{
    public struct NameValuePair
    {
        public string name;
        public string value;
    }

    [DataContract]
    public class DTanswer
    {

        #region Reference : http://datatables.net/usage/server-side
        //int	    iTotalRecords	        Total records, before filtering (i.e. the total number of records in the database)
        //int	    iTotalDisplayRecords	Total records, after filtering (i.e. the total number of records after filtering has been applied
        //                                  - not just the number of records being returned in this result set)
        //string	sEcho	                An unaltered copy of sEcho sent from the client side.
        //                                  This parameter will change with each draw (it is basically a draw count)
        //                                  - so it is important that this is implemented.
        //                                  Note that it strongly recommended for security reasons that you 'cast' this parameter to an integer
        //                                  in order to prevent Cross Site Scripting (XSS) attacks.
        //string	sColumns	            Deprecated Optional - this is a string of column names, comma separated (used in combination with sName) which will allow DataTables to reorder data on the client-side if required for display. Note that the number of column names returned must exactly match the number of columns in the table. For a more flexible JSON format, please consider using mData.
        //                                  Note that this parameter is deprecated and will be removed in v1.10. Please now use mData.
        //array	    aaData                  The data in a 2D array. Note that you can change the name of this parameter with sAjaxDataProp.
        #endregion

        [DataMember]
        public int sEcho { get; set; }
        [DataMember]
        public int iTotalRecords { get; set; }
        [DataMember]
        public int iTotalDisplayRecords { get; set; }
        [DataMember]
        public List<List<string>> aaData { get; set; }
        [DataMember]
        public string sColumns { get; set; } //DEPRECATED

    }

    [DataContract]
    public class DTrequest
    {

        /* http://datatables.net/usage/server-side
         * int		iDisplayStart       Display start point in the current data set.
         * int		iDisplayLength      Number of records that the table can display in the current draw.
         *                              It is expected that the number of records returned will be equal to this number,
         *                              unless the server has fewer records to return.
         * int		iColumns	        Number of columns being displayed (useful for getting individual column search info)
         * string	sSearch	            Global search field
         * bool	    bRegex	            True if the global filter should be treated as a regular expression for advanced filtering, false if not.
         * bool	    bSearchable_(int)	Indicator for if a column is flagged as searchable or not on the client-side
         * string	sSearch_(int)	    Individual column filter
         * bool	bRegex_(int)	    True if the individual column filter should be treated as a regular expression for advanced filtering,
                                    false if not
        bool	bSortable_(int)	    Indicator for if a column is flagged as sortable or not on the client-side
        int		iSortingCols	    Number of columns to sort on
        int		iSortCol_(int)	    Column being sorted on (you will need to decode this number for your database)
        string	sSortDir_(int)	    Direction to be sorted - "desc" or "asc".
        string	mDataProp_(int)	    The value specified by mDataProp for each column.
                                    This can be useful for ensuring that the processing of data is independent from the order of the columns.
        string	sEcho	            Information for DataTables to use for rendering.
        */

        [DataMember]
        public NameValuePair[] DTparams
        {
            get
            {
                List<NameValuePair> list = new List<NameValuePair>();
                foreach (KeyValuePair<string, string> kvp in this.param)
                {
                    list.Add(new NameValuePair() { name = kvp.Key, value = kvp.Value });
                }
                return list.ToArray();
            }
            set
            {
                this.param = new Dictionary<string, string>();
                foreach (NameValuePair nvp in value)
                {
                    this.param.Add(nvp.name, nvp.value);
                }
            }
        }
        private Dictionary<string, string> param;

        [DataMember]
        public Filtre[] filtres;

        private HashSet<String> DTrequestParams = new HashSet<String>(new String[] {
                "iDisplayStart",
                "iDisplayLength",
                "iColumns",
                "sSearch",
                "bRegex",
                "bSearchable_(int)",
                "sSearch_(int)",
                "bRegex_(int)",
                "bSortable_(int)",
                "iSortingCols",
                "iSortCol_(int)",
                "sSortDir_(int)",
                "mDataProp_(int)",
                "sEcho"
            }, StringComparer.InvariantCultureIgnoreCase); //TODO: trouver un moyen de générer les "_(int)";

        public bool paramsAreValid()
        {
            return true;
            //return DTrequestParams.IsSupersetOf(this.param.AllKeys);
        }

        public DTanswer getData()
        {

            // Reference = http://datatables.net/development/server-side/php_mysql

            //TODO: Dev only
            String[] aColumns = new String[] { "engine", "browser", "platform", "version", "grade" };

            // Paging
            String sLimit = "";
            if ((this.param["iDisplayStart"] != null) && this.param["iDisplayLength"] != "-1")
            {
                sLimit = "LIMIT " + Convert.ToInt32(this.param["iDisplayStart"]) + ", " + Convert.ToInt32(this.param["iDisplayLength"]);
            }

            // Ordering
            String sOrder = "";
            if (this.param["iSortCol_0"] != null)
            {
                sOrder = "ORDER BY  ";

                for (int i = 0; i < Convert.ToInt32(this.param["iSortingCols"]); i++)
                {
                    if (this.param["bSortable_" + Convert.ToInt32(this.param["iSortCol_" + i])] == "true")
                    {
                        sOrder += aColumns[Convert.ToInt32(this.param["iSortCol_" + i])];
                        sOrder += (this.param["sSortDir_" + i] == "asc") ? "asc" : "desc" + ", ";
                    }
                }

                sOrder = sOrder.Substring(0, sOrder.Length - 2);
                if (sOrder == "ORDER BY")
                    sOrder = "";
            }

            /* Filtering
               NOTE: this does not match the built-in DataTables filtering which does it
               word by word on any field. It's possible to do here, but concerned about efficiency
               on very large tables, and MySQL's regex functionality is very limited
			
            String sWhere = "";
            if ( (this.param["sSearch"] != null) && this.param["sSearch"] != "" ) {
                sWhere = "WHERE (";
                for ( int i=0 ; i < aColumns.Length ; i++ ) {
                    if ( (this.param["bSearchable_"+i] != null) && (this.param["bSearchable_"+i] == "true") )
                        sWhere += aColumns[i] + " LIKE '%" + this.param["sSearch"].Replace("'","''") + "%' OR ";
                }
                sWhere = sWhere.Substring(0, sWhere.Length - 3);
                sWhere += ")";
            }
			
            // Individual column filtering
            for ( int i=0 ; i < aColumns.Length ; i++ ) {
                if ( (this.param["bSearchable_"+i] != null) && (this.param["bSearchable_"+i] == "true") && (this.param["sSearch_"+i] != "" )) {
                    if (sWhere == "")
                        sWhere = "WHERE ";
                    else
                        sWhere += " AND ";
                    sWhere += aColumns[i] + " LIKE '%" + this.param["sSearch_"+i].Replace("'","''") + "%' ";
                }
            }
            */

            // Récupération de la DataTable résultat
            DataTable data = this.filtres[0].getActions();
            if (this.filtres.Length > 1)
            {
                for (int i = 1; i < this.filtres.Length; i++)
                {
                    data.Merge(this.filtres[i].getActions());
                }
            }

            // Conversion en tableau 2D
            List<List<string>> DTdata = new List<List<string>>();
            int total = data.Rows.Count;
            foreach (DataRow row in data.Rows)
            {
                DTdata.Add(new List<string>(row.ItemArray.Select(o => o.ToString())));
            }

            return new DTanswer()
            {
                sEcho = Convert.ToInt32(this.param["sEcho"]),
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = DTdata
            };
        }
    }
}
