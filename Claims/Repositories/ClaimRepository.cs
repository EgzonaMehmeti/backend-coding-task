using Claims.Data;
using Claims.Models;
using Claims.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Claims.Repositories
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly ClaimsDbContext _context;

        public ClaimRepository(ClaimsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Claim>> GetAllAsync() => await _context.Claims.ToListAsync();
            
        public async Task<Claim?> GetByIdAsync(string id) => await _context.Claims.FirstOrDefaultAsync(claim => claim.Id == id);

        public async Task AddAsync(Claim claim) 
        {
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var claim = await GetByIdAsync(id);
            if (claim == null) return;

            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();
        }
    }
}
