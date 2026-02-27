using Claims.DTOs;
using Claims.Models;
using Claims.Repositories;
using Claims.Repositories.Interfaces;
using Claims.Services.Interfaces;

namespace Claims.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _repository;
        private readonly IAuditService _auditService;
        private readonly ICoverRepository _coverRepository;

        public ClaimService(IClaimRepository repository, IAuditService auditService, ICoverRepository coverRepository)
        {
            _repository = repository;
            _auditService = auditService;
            _coverRepository = coverRepository;
        }

        public async Task<IEnumerable<ClaimDto>> GetAllAsync()
        {
            var claim = await _repository.GetAllAsync();
            return claim.Select(MapToDto);
        }

        public async Task<ClaimDto?> GetByIdAsync(string id) 
        {
            var claim = await _repository.GetByIdAsync(id);
            if (claim == null)
                return null;
            return MapToDto(claim);
        }

        public async Task<ClaimDto> CreateAsync(CreateClaimDto dto)
        {
            await ValidateClaim(dto);
            var claim = new Claim
            {
                Id = Guid.NewGuid().ToString(),
                CoverId = dto.CoverId,
                Created = dto.Created,
                Type = dto.Type,
                DamageCost = dto.DamageCost
            };

            await _repository.AddAsync(claim);
            await _auditService.AuditClaimAsync(claim.Id, "POST");

            return MapToDto(claim);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
            await _auditService.AuditClaimAsync(id, "DELETE");
        }

        private async Task ValidateClaim(CreateClaimDto dto)
        {
            if (dto.DamageCost > 100000)
                throw new ArgumentException("Damage cost cannot exceed 100, 000.");

            var cover = await _coverRepository.GetByIdAsync(dto.CoverId);

            if (cover == null)
                throw new ArgumentException("Related cover does not exist.");
            if(dto.Created.Date < cover.StartDate.Date ||
                dto.Created.Date > cover.EndDate.Date)
                throw new ArgumentException("Claim created date must be within the related cover period.");
        }

        private ClaimDto MapToDto(Claim claim)
        {
            return new ClaimDto
            {
                Id = claim.Id,
                CoverId = claim.CoverId,
                Created = claim.Created,
                Type = claim.Type,
                DamageCost = claim.DamageCost
            };
        }
    }
}
