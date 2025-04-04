using Microsoft.Extensions.Options;
using MongoDB.Driver;

using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Vocabulary> Vocabulary => _database.GetCollection<Vocabulary>("Vocabulary");
        public IMongoCollection<Grammar> Grammar => _database.GetCollection<Grammar>("Grammar");
    }
}
