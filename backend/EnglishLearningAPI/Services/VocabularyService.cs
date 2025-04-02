using MongoDB.Driver;

using EnglishLearningAPI.Models;
using EnglishLearningAPI.Data;

namespace EnglishLearningAPI.Services
{
    public class VocabularyService
    {
        private readonly IMongoCollection<Vocabulary> _vocabulary;

        public VocabularyService(MongoDbContext context)
        {
            _vocabulary = context.Vocabulary;
        }

        public async Task<List<Vocabulary>> GetAllAsync() => await _vocabulary.Find(_ => true).ToListAsync();

        public async Task<Vocabulary> GetByIdAsync(string id) => await _vocabulary.Find(v => v.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Vocabulary vocab) => await _vocabulary.InsertOneAsync(vocab);

        public async Task UpdateAsync(string id, Vocabulary vocab) => 
            await _vocabulary.ReplaceOneAsync(v => v.Id == id, vocab);

        public async Task DeleteAsync(string id) => await _vocabulary.DeleteOneAsync(v => v.Id == id);
    }
}
