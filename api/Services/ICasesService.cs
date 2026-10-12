using api.DTOs.Cases;

namespace api.Services;

public interface ICasesService
{
    Task<CaseResponse> CreateAsync(
        CreateCaseRequest request,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CaseResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<CaseResponse?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken);

    Task<bool> UpdateAsync(
        string id,
        UpdateCaseRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        string id,
        CancellationToken cancellationToken);
}
