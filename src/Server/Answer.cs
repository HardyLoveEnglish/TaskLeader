using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskLeader.Server
{
    public enum AnswerTypes
    {
        dbs_list = 0,
        entities_list = 1,        
        filters_list =2,
        actions_list =3
    }

    class Answer
    {
        public AnswerTypes answerType { get; set; }
        public object data { get; set; }
    }
}
