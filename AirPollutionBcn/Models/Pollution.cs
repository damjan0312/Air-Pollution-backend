using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace AirPollutionBackend.Models
{
    public class Pollution
    {
        
        //public ObjectId _id { get; set; }
        public string id { get; set; }
        public string cityId { get; set; }
        public string stateId { get; set; }
        public Current current { get; set; }

        public Pollution()
        {
            current = new Current();
        }
    }


}