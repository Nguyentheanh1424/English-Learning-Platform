using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Vocabulary
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Word { get; set; }
    public string Meaning { get; set; }
    public string ExampleSentence { get; set; }
    public string Pronunciation { get; set; }
    public string PartOfSpeech { get; set; }
    public List<string> Synonyms { get; set; }
    public List<string> Antonyms { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
