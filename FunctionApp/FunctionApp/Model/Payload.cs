using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FunctionApp.Model
{

    //public class Function
    //{
    //    public string name { get; set; }
    //    public string id { get; set; }
    //    public string type { get; set; }
    //}


    [DataContract]
    public class Data
    {
        [DataMember]
        public string Url { get; set; }
    }

    [DataContract]
    public class Body
    {
        [DataMember]
        public Data Data { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        //public Function Function { get; set; }
        [DataMember]
        public Body Body { get; set; }
    }

}
