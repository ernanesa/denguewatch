using api.Data.Documents;
using api.DTOs.Cases;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace api.Services
{
    public sealed class CasesService(IMongoDatabase database) : ICasesService
    {
        private readonly IMongoCollection<CaseDocument> _cases = database.GetCollection<CaseDocument>("cases");

        public async Task<CaseResponse> CreateAsync(CreateCaseRequest request, CancellationToken cancellationToken)
        {
            var document = new CaseDocument
            {
                DateOfBirth = request.DateOfBirth!.Value,
                Sex = request.Sex!,
                Location = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(
                        request.Longitude!.Value,
                        request.Latitude!.Value
                    )
                )
            };

            await _cases.InsertOneAsync(document, cancellationToken: cancellationToken);

            return ToResponse(document);
        }

        public async Task<IReadOnlyList<CaseResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            var documents = await _cases.Find(Builders<CaseDocument>.Filter.Empty).ToListAsync(cancellationToken);
            return [.. documents.Select(ToResponse)];
        }

        public async Task<CaseResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return null;
            var document = await _cases
                                .Find(item => item.Id == objectId)
                                .FirstOrDefaultAsync(cancellationToken);

            return document is null ? null : ToResponse(document);
        }

        public async Task<bool> UpdateAsync(string id, UpdateCaseRequest request, CancellationToken cancellationToken)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return false;

            var replacement = new CaseDocument
            {
                Id = objectId,
                DateOfBirth = request.DateOfBirth!.Value,
                Sex = request.Sex!,
                Location = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(
                        request.Longitude!.Value,
                        request.Latitude!.Value
                    )
                )
            };

            var result = await _cases.ReplaceOneAsync(item => item.Id == objectId, replacement, cancellationToken: cancellationToken);

            return result.MatchedCount == 1;
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return false;

            var result = await _cases.DeleteOneAsync(item => item.Id == objectId, cancellationToken);

            return result.DeletedCount == 1;
        }

        private static CaseResponse ToResponse(CaseDocument document) =>
            new(
                document.Id.ToString(),
                document.DateOfBirth,
                document.Sex,
                document.Location.Coordinates.Latitude,
                document.Location.Coordinates.Longitude
            );
    }
}