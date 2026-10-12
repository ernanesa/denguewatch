using MongoDB.Bson;

namespace api.Data.Documents;

public sealed class CaseDocument
{
    public ObjectId Id { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Sex { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
