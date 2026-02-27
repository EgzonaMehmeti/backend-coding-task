using Claims.DTOs;

namespace Claims.Services.Interfaces
{
    public interface ICoverService
    {
        Task<IEnumerable<CoverDto>> GetAllAsync();
        Task<CoverDto?> GetByIdAsync(string id);
        Task<CoverDto> CreateAsync(CreateCoverDto dto);
        Task DeleteAsync(string id);
    }
}
