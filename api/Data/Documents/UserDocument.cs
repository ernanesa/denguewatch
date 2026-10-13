using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Data.Documents;

public class UserDocument
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}