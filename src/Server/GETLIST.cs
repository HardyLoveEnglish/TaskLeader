using System;
using System.Collections.Generic;
using SuperWebSocket;
using SuperWebSocket.SubProtocol;
using TaskLeader.DAL;
using TaskLeader.GUI;

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

    public class GetListRequest {
        public ListTypes type { get; set; }
        public string db { get; set; }
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

    #endregion

    public class GETLIST : JsonSubCommand<GetListRequest>
    {
        protected override void ExecuteJsonCommand(WebSocketSession session, GetListRequest commandInfo)
        {
            if (session.Logger.IsDebugEnabled)
                session.Logger.Debug("GETLIST[" + Enum.GetName(typeof(ListTypes), commandInfo.type) + "] request received from " + session.RemoteEndPoint);

            SendJsonMessage(session, String.Empty, getAnswer(commandInfo));
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
                    break;

                case ListTypes.dbs:
                    answer.answerType = AnswerTypes.dbs_list;
                    foreach (String dbName in TrayIcon.dbs.Keys)
                        values.Add(new DbsListAnswerData() { label = dbName });
                    break;

                case ListTypes.actions:
                    break;
            }

            answer.data = values;
            return answer;
        }
    }
}
