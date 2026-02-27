using Claims.DTOs;

namespace Claims.Services.Interfaces
{
    public interface IClaimService
    {
        Task<IEnumerable<ClaimDto>> GetAllAsync();
        Task<ClaimDto?> GetByIdAsync(string id);
        Task<ClaimDto> CreateAsync(CreateClaimDto dto);
        Task DeleteAsync(string id);
    }
}
