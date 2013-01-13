using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using SuperWebSocket;
using SuperWebSocket.SubProtocol;
using TaskLeader.DAL;
using TaskLeader.GUI;
using TaskLeader.BO;

namespace TaskLeader.Server
{
    #region GETLIST request objects

    public enum ListTypes
    {
        dbs = 0,
        dbEntities = 1,
        filters = 2,
        actions = 3
    }

    public class GetListRequest
    {
        public ListTypes type { get; set; }
        public string db { get; set; }
        public List<Filtre> filters { get; set; }
    }

    #endregion

    #region GETLIST answer objects

    public class ListAnswerData { public string label { get; set; } }
    public class EntitiesListAnswerData : ListAnswerData
    {
        public int id { get; set; }
        public int parent = -1;
        public object[] values { get; set; }
    }
    public class DbsListAnswerData : ListAnswerData { public bool defaut { get; set; } }

    public class ActionsListAnswerData
    {
        public string[] cols { get; set; }
        public object[][] rows { get; set; }
    }

    #endregion

    public class GETLIST : JsonSubCommand<GetListRequest>
    {
        protected override void ExecuteJsonCommand(WebSocketSession session, GetListRequest commandInfo)
        {
            if (session.Logger.IsDebugEnabled)
                session.Logger.Debug("GETLIST[" + Enum.GetName(typeof(ListTypes), commandInfo.type) + "] request received from " + session.RemoteEndPoint);

            try { SendJsonMessage(session, String.Empty, getAnswer(commandInfo)); }
            catch (Exception e)
            {
                if (session.Logger.IsErrorEnabled)
                    session.Logger.Error(e.Message);
            }
        }

        private Answer getAnswer(GetListRequest request)
        {
            var answer = new Answer();
            var values = new List<ListAnswerData>();

            switch (request.type)
            {
                case ListTypes.dbEntities:
                    answer.answerType = AnswerTypes.entities_list;
                    for (int i = 0; i < DB.entities.Length; i++)
                        values.Add(new EntitiesListAnswerData()
                        {
                            id = i,
                            label = DB.entities[i].nom,
                            parent = DB.entities[i].parent,
                            values = TrayIcon.dbs[request.db].getTitres(DB.entities[i])
                        });
                    answer.data = values;
                    break;

                case ListTypes.dbs:
                    answer.answerType = AnswerTypes.dbs_list;
                    foreach (String dbName in TrayIcon.dbs.Keys)
                        values.Add(new DbsListAnswerData() { label = dbName });
                    answer.data = values;
                    break;

                case ListTypes.actions:

                    if (request.filters == null || request.filters.Count == 0)
                        throw new Exception("Missing filter in GETLIST[actions] request");

                    answer.answerType = AnswerTypes.actions_list;

                    DataTable data = request.filters[0].getActions();
                    if (request.filters.Count > 1)
                    {
                        for (int i = 1; i < request.filters.Count; i++)
                            data.Merge(request.filters[i].getActions());
                    }

                    var result = new ActionsListAnswerData(){
                        cols = data.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray(),
                        rows = data.Rows.Cast<DataRow>().Select(row => row.ItemArray).ToArray()
                    };
                    answer.data = result;
                    break;
            }

            return answer;
        }
    }
}
