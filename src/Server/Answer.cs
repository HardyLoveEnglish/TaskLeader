using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskLeader.Server
{
    public enum AnswerTypes
    {
        entities_list = 0,
        dbs_list =1,
        filters_list =2,
        criterias_list =3
    }

    class Answer
    {
        public AnswerTypes answerType { get; set; }
        public object data { get; set; }
    }
}
