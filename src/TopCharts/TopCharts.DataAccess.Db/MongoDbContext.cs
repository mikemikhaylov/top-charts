using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using TopCharts.Domain.Model;
using TopCharts.Domain.Model.Api;

namespace TopCharts.DataAccess.Db
{
    public class MongoDbContext
    {
        static MongoDbContext()
        {
            ClassMaps.Register();
            BsonSerializer.RegisterSerializer(typeof(DateTime), new CustomDateTimeSerializer());
            ConventionPack conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
        }
        
        private const string KeyValueCollectionName = "KeyValues";
        private const string ItemsCollectionName = "Items";
        private readonly IMongoDatabase _database;

        public MongoDbContext(DbOptions config)
        {
            var client = new MongoClient(config.ConnectionString);
            _database = client.GetDatabase(config.DatabaseName);
        }
        
        public virtual IMongoCollection<KeyValue> KeyValues => _database.GetCollection<KeyValue>(KeyValueCollectionName);
        public virtual IMongoCollection<Item> Items => _database.GetCollection<Item>(ItemsCollectionName);
    }
}