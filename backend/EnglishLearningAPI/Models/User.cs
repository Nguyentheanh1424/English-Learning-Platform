using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EnglishLearningAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        [BsonElement("role")]
        public string Role { get; set; } = "Student";

        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedDate")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}