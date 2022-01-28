using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace TopCharts.DataAccess.Db
{
    internal class CustomDateTimeSerializer : IBsonSerializer<DateTime>, IBsonSerializer<DateTime?>
    {
        private readonly DateTimeSerializer _dateTimeSerializer = new DateTimeSerializer(DateTimeKind.Utc);

        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return _dateTimeSerializer.Deserialize(context, args);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime? value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                Serialize(context, args, value.Value);
            }
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
        {
            //Probably there is a better way to make driver stop converting DateTime, 
            //it just needs more googling to be done 
            _dateTimeSerializer.Serialize(context, args, DateTime.SpecifyKind(value, DateTimeKind.Utc));
        }

        DateTime IBsonSerializer<DateTime>.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return _dateTimeSerializer.Deserialize(context, args);
        }

        DateTime? IBsonSerializer<DateTime?>.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            IBsonReader bsonReader = context.Reader;
            BsonType bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType == BsonType.Null)
            {
                return null;
            }
            return _dateTimeSerializer.Deserialize(context, args);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, (DateTime) value);
        }

        public Type ValueType { get; } = typeof(DateTime);
    }
}