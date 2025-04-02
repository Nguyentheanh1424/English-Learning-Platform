using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EnglishLearningAPI.Models
{
    public class Grammar
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("rule")]
        public string Rule { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("example")]
        public string Example { get; set; } = string.Empty;
    }
}
