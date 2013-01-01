using System;
using System.Collections.Generic;
using SuperWebSocket;
using SuperWebSocket.SubProtocol;

namespace TaskLeader.Server
{
    public enum ListTypes
    {
        dbEntities = 0,
        dbs = 1,
        filters = 2,
        criterias = 3
    }

    public class GetListRequest
    {
        public ListTypes type { get; set; }
        public object args { get; set; }
    }

    public class ListAnswerData
    {
        public int id { get; set; }
        public string label { get; set; }
    }
    public class EntitiesListAnswerData : ListAnswerData { public int parent = -1; }
    public class DbsListAnswerData : ListAnswerData { public bool defaut { get; set; } }

    public class GETLIST : JsonSubCommand<GetListRequest>
    {
        protected override void ExecuteJsonCommand(WebSocketSession session, GetListRequest commandInfo)
        {
            if (session.Logger.IsDebugEnabled)
                session.Logger.Debug("GETLIST request received from " + session.RemoteEndPoint);

            SendJsonMessage(session,
                String.Empty,
                getAnswer(commandInfo.type, commandInfo.args)
           );
        }

        private Answer getAnswer(ListTypes type, object data)
        {
            var answer = new Answer();
            var values = new List<ListAnswerData>();

            switch (type)
            {
                case ListTypes.dbEntities:
                    answer.answerType = AnswerTypes.entities_list;
                    values.Add(new EntitiesListAnswerData() { id = 0, label = "Contextes" });
                    values.Add(new EntitiesListAnswerData() { id = 1, label = "Sujets", parent = 0 });
                    values.Add(new EntitiesListAnswerData() { id = 2, label = "Destinataires" });
                    break;
                case ListTypes.dbs:
                    answer.answerType = AnswerTypes.dbs_list;
                    values.Add(new DbsListAnswerData() { id = 0, label = "Perso", defaut = true });
                    values.Add(new DbsListAnswerData() { id = 0, label = "Déploiement"});
                    break;
            }

            answer.data = values;
            return answer;
        }
    }
}
