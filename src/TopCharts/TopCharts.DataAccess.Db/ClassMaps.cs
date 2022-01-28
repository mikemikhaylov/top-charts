using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using TopCharts.Domain.Model;
using TopCharts.Domain.Model.Api;

namespace TopCharts.DataAccess.Db
{
    public class ClassMaps
    {
        public static void Register()
        {
            BsonClassMap.RegisterClassMap<KeyValue>(x =>
            {
                x.AutoMap();
                x.MapIdMember(xx => xx.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId))
                    .SetIgnoreIfDefault(true);
                x.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<Item>(x =>
            {
                x.AutoMap();
                x.MapIdMember(xx => xx.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId))
                    .SetIgnoreIfDefault(true);
                x.SetIgnoreExtraElements(true);
            });
        }
    }
}