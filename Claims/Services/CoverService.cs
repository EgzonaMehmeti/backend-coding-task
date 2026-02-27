using Claims.DTOs;
using Claims.Models;
using Claims.Repositories.Interfaces;
using Claims.Services.Interfaces;

namespace Claims.Services
{
    public class CoverService : ICoverService
    {
        private const decimal BaseDayRate = 1250m;
        private readonly ICoverRepository _repository;
        private readonly IAuditService _auditService;

        public CoverService(
            ICoverRepository repository,
            IAuditService auditService)
        {
            _repository = repository;
            _auditService = auditService;
        }

        public async Task<IEnumerable<CoverDto>> GetAllAsync()
        {
            var covers = await _repository.GetAllAsync();

            return covers.Select(MapToDto);
        }

        public async Task<CoverDto> GetByIdAsync(string id)
        {
            var cover = await _repository.GetByIdAsync(id);
            if (cover == null) return null;

            return MapToDto(cover);
        }

        public async Task<CoverDto> CreateAsync(CreateCoverDto dto)
        {
            ValidateCover(dto);

            var cover = new Cover
            {
                Id = Guid.NewGuid().ToString(),
                StartDate = dto.StartDate.Date,
                EndDate = dto.EndDate.Date,
                Type = dto.Type
            };

            cover.Premium = CalculatePremium(cover);

            await _repository.AddAsync(cover);
            await _auditService.AuditCoverAsync(cover.Id, "POST");

            return MapToDto(cover);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
            await _auditService.AuditCoverAsync(id, "DELETE");
        }

        private void ValidateCover(CreateCoverDto dto)
        {
            if (dto.StartDate.Date < DateTime.UtcNow)
                throw new ArgumentException("Start date cannot be in the past.");

            if (dto.EndDate.Date < dto.StartDate.Date)
                throw new ArgumentException("End date cannot be before start date.");

            var totalDays = (dto.EndDate - dto.StartDate).Days;

            if (totalDays > 365)
                throw new ArgumentException("Insurance period cannot exceed one year.");
        }

        private decimal CalculatePremium(Cover cover)
        {
            var totalDays = (cover.EndDate - cover.StartDate).Days;

            if (totalDays <= 0)
                throw new ArgumentException("Insurance period must be positive.");

            var basePremiumPerDay = BaseDayRate * GetTypeMultiplier(cover.Type);

            var firstPeriodDays = Math.Min(totalDays, 30);
            var secondPeriodDays = Math.Min(Math.Max(totalDays - 30, 0), 150);
            var remainingDays = Math.Max(totalDays - 180, 0);

            decimal firstPeriodPremium =
                firstPeriodDays * basePremiumPerDay;

            decimal secondPeriodPremium =
                secondPeriodDays *
                basePremiumPerDay *
                (1 - GetSecondPeriodDiscount(cover.Type));

            decimal remainingPeriodPremium =
                remainingDays *
                basePremiumPerDay *
                (1 - GetFinalPeriodDiscount(cover.Type));

            return firstPeriodPremium
                   + secondPeriodPremium
                   + remainingPeriodPremium;
        }
        private decimal GetTypeMultiplier(CoverType type)
        {
            return type switch
            {
                CoverType.Yacht => 1.10m,
                CoverType.PassengerShip => 1.20m,
                CoverType.Tanker => 1.50m,
                _ => 1.30m
            };
        }

        private decimal GetSecondPeriodDiscount(CoverType type)
        {
            return type == CoverType.Yacht
                ? 0.05m
                : 0.02m;
        }

        private decimal GetFinalPeriodDiscount(CoverType type)
        {
            return type == CoverType.Yacht
                ? 0.08m   // 5% + additional 3%
                : 0.03m;  // 2% + additional 1%
        }

        private CoverDto MapToDto(Cover cover)
        {
            return new CoverDto
            {
                Id = cover.Id,
                StartDate = cover.StartDate,
                EndDate = cover.EndDate,
                Type = cover.Type,
                Premium = cover.Premium
            };
        }
    }
}
