using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;
using AirPollutionBackend.Models;

namespace AirPollutionBackend.Services
{
    public class PollutionService
    {
        public static Pollution GetPollution(string cityId,string stateId)
        {
            var connectionString = "mongodb://localhost/?safe=true";

            var client = new MongoClient(connectionString);
            var db = client.GetDatabase("airpollutionDB");

            var collection = db.GetCollection<BsonDocument>("pollution");

            var builder = Builders<BsonDocument>.Filter;

            var filter = builder.Eq("cityId", ObjectId.Parse(cityId)) & builder.Eq("stateId", ObjectId.Parse(stateId));
            
            var documents = collection.Find(filter).ToList();

            foreach (BsonDocument doc in documents)
            {
                Pollution p = new Pollution();
                p.id = doc.AsBsonDocument["_id"].AsObjectId.ToString();
                p.cityId = doc.AsBsonDocument["cityId"].AsObjectId.ToString();
                p.stateId = doc.AsBsonDocument["stateId"].AsObjectId.ToString();
                p.current.weather.timestamp = doc.AsBsonDocument["current"].AsBsonDocument["weather"].AsBsonDocument["ts"].AsString;
                p.current.weather.temperature = doc.AsBsonDocument["current"].AsBsonDocument["weather"].AsBsonDocument["tp"].AsDouble.ToString() ;
                p.current.weather.pressure = doc.AsBsonDocument["current"].AsBsonDocument["weather"].AsBsonDocument["pr"].AsDouble.ToString();
                p.current.weather.humidity = doc.AsBsonDocument["current"].AsBsonDocument["weather"].AsBsonDocument["hu"].AsDouble.ToString();
                p.current.pollution.timestamp = doc.AsBsonDocument["current"].AsBsonDocument["pollution"].AsBsonDocument["ts"].AsString;
                p.current.pollution.aqius = doc.AsBsonDocument["current"].AsBsonDocument["pollution"].AsBsonDocument["aqius"].AsInt32;

                return p;
            }

            return null;
            
        }

        public static List<Pollution> getMostPollutedCities()
        {
            var connectionString = "mongodb://localhost/?safe=true";

            var client = new MongoClient(connectionString);
            var db = client.GetDatabase("airpollutionDB");

            var collection = db.GetCollection<BsonDocument>("pollution");

            List<Pollution> pollutions = new List<Pollution>();

            var filter = Builders<BsonDocument>.Filter.Empty;
            var sort = Builders<BsonDocument>.Sort.Ascending("aqius");

            var documents = collection.Find((new BsonDocument())).ToList();

            foreach (BsonDocument doc in documents)
            {
                Pollution p = new Pollution();
                p.id = doc.AsBsonDocument["_id"].AsString;
                p.cityId = doc.AsBsonDocument["cityId"].AsString.ToString();
                p.stateId = doc.AsBsonDocument["stateId"].AsString.ToString();
                p.current.weather.timestamp = doc.AsBsonDocument["current"].AsBsonDocument["weather"].AsBsonDocument["timestamp"].AsString;
                p.current.weather.temperature = doc.AsBsonDocument["current"].AsBsonDocument["weather"].AsBsonDocument["temperature"].AsString;
                p.current.weather.pressure = doc.AsBsonDocument["current"].AsBsonDocument["weather"].AsBsonDocument["pressure"].AsString;
                p.current.weather.humidity = doc.AsBsonDocument["current"].AsBsonDocument["weather"].AsBsonDocument["humidity"].AsString;
                p.current.pollution.timestamp = doc.AsBsonDocument["current"].AsBsonDocument["pollution"].AsBsonDocument["timestamp"].AsString;
                p.current.pollution.aqius = doc.AsBsonDocument["current"].AsBsonDocument["pollution"].AsBsonDocument["aqius"].AsInt32;

                pollutions.Add(p);
            }
            pollutions = pollutions.OrderByDescending(item => item.current.pollution.aqius).Take(5).ToList();
            
            return pollutions;

        }

        public static List<Pollution> getHistory(string cityId)
        {
            var connectionString = "mongodb://localhost/?safe=true";

            var client = new MongoClient(connectionString);
            var db = client.GetDatabase("airpollutionDB");

            var collection = db.GetCollection<BsonDocument>("pollutionHistory");

            var builder = Builders<BsonDocument>.Filter;

            var filter = builder.Eq("cityId", cityId);

            var documents = collection.Find(filter).ToList();

            List<Pollution> pollutions = new List<Pollution>();

            foreach (BsonDocument doc in documents)
            {
                Pollution p = new Pollution();
                p.id = doc.AsBsonDocument["_id"].AsString;
                p.cityId = doc.AsBsonDocument["cityId"].AsString;
                p.stateId = doc.AsBsonDocument["stateId"].AsString;
                p.current.weather.timestamp = doc.AsBsonDocument["current"].AsBsonDocument["weather"].AsBsonDocument["timestamp"].AsString;
                p.current.weather.temperature = doc.AsBsonDocument["current"].AsBsonDocument["weather"].AsBsonDocument["temperature"].AsString;
                p.current.weather.pressure = doc.AsBsonDocument["current"].AsBsonDocument["weather"].AsBsonDocument["pressure"].AsString;
                p.current.weather.humidity = doc.AsBsonDocument["current"].AsBsonDocument["weather"].AsBsonDocument["humidity"].AsString;
                p.current.pollution.timestamp = doc.AsBsonDocument["current"].AsBsonDocument["pollution"].AsBsonDocument["timestamp"].AsString;
                p.current.pollution.aqius = doc.AsBsonDocument["current"].AsBsonDocument["pollution"].AsBsonDocument["aqius"].AsInt32;

                pollutions.Add(p);
            }

            return pollutions;

        }

        public static bool deleteHistory(string Id)
        {
            var connectionString = "mongodb://localhost/?safe=true";

            var client = new MongoClient(connectionString);

            var db = client.GetDatabase("airpollutionDB");

            var collection = db.GetCollection<BsonDocument>("pollutionHistory");

            var builder = Builders<BsonDocument>.Filter;

            var filter = builder.Eq("_id", ObjectId.Parse(Id));

            collection.DeleteMany(filter);

            return true;
            
        }

        public static bool Add(Pollution newPollution)
        {
            var connectionString = "mongodb://localhost/?safe=true";

            var client = new MongoClient(connectionString);

            var db = client.GetDatabase("airpollutionDB");

            try
            {
                var collection = db.GetCollection<BsonDocument>("pollution");

                var builder = Builders<BsonDocument>.Filter;

                var filter = builder.Eq("cityId", newPollution.cityId);

                collection.DeleteMany(filter);
            

                builder = Builders<BsonDocument>.Filter;

                newPollution.id= ObjectId.GenerateNewId().ToString();

                collection.InsertOne( newPollution.ToBsonDocument());
            }
            catch (Exception ex)
            {
                return false;
            }
            
            return true;

        }

        public static bool AddToHistory(Pollution newPollution)
        {
            try
            {
                var connectionString = "mongodb://localhost/?safe=true";

                var client = new MongoClient(connectionString);

                var db = client.GetDatabase("airpollutionDB");

                var collection = db.GetCollection<BsonDocument>("pollutionHistory");

                //var builder = Builders<BsonDocument>.Filter;

                newPollution.id = ObjectId.GenerateNewId().ToString();

                collection.InsertOne(newPollution.ToBsonDocument());
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

    }
}