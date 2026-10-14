using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace api.Data.Documents;

public sealed class CaseDocument
{
    [BsonId]
    public ObjectId Id { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Sex { get; set; } = string.Empty;
    public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; } = default!;
}
