using EnglishLearningAPI.Data;
using MongoDB.Driver;

using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Services
{
    public class GrammarService(MongoDbContext dbContext)
    {
        private readonly IMongoCollection<Grammar> _grammarCollection = dbContext.Grammar;

        public async Task<List<Grammar>> GetAllRulesAsync()
        {
            return await _grammarCollection.Find(rule => true).ToListAsync();
        }

        public async Task<Grammar?> GetRuleByIdAsync(string id)
        {
            return await _grammarCollection.Find(rule => rule.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateRuleAsync(Grammar rule)
        {
            await _grammarCollection.InsertOneAsync(rule);
        }

        public async Task<bool> DeleteRuleAsync(string id)
        {
            var result = await _grammarCollection.DeleteOneAsync(rule => rule.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
