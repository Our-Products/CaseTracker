using Application.Interfaces;
using Infrastructure.Data;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Lawyer entity.
    /// Encapsulates all database access logic for lawyers.
    /// </summary>
    public class LawyerRepository : ILawyerRepository
    {
        private readonly ApplicationDbContext _context;

        public LawyerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get lawyer by mobile number
        /// </summary>
        public async Task<Lawyer?> GetByMobileNumberAsync(string mobileNumber)
        {
            return await _context.Lawyers
                .SingleOrDefaultAsync(x => x.MobileNumber == mobileNumber);
        }

        /// <summary>
        /// Get lawyer by ID
        /// </summary>
        public async Task<Lawyer?> GetByIdAsync(Guid lawyerId)
        {
            return await _context.Lawyers
                .FirstOrDefaultAsync(x => x.LawyerId == lawyerId);
        }

        /// <summary>
        /// Check if mobile number already exists
        /// </summary>
        public async Task<bool> MobileNumberExistsAsync(string mobileNumber)
        {
            return await _context.Lawyers
                .AnyAsync(x => x.MobileNumber == mobileNumber);
        }

        /// <summary>
        /// Get all lawyers
        /// </summary>
        public async Task<IEnumerable<Lawyer>> GetAllAsync()
        {
            return await _context.Lawyers
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Add new lawyer
        /// </summary>
        public async Task<Lawyer> AddAsync(Lawyer lawyer)
        {
            _context.Lawyers.Add(lawyer);
            return await Task.FromResult(lawyer);
        }

        /// <summary>
        /// Update existing lawyer
        /// </summary>
        public async Task<Lawyer> UpdateAsync(Lawyer lawyer)
        {
            _context.Entry(lawyer).State = EntityState.Modified;
            return await Task.FromResult(lawyer);
        }

        /// <summary>
        /// Delete lawyer by ID
        /// </summary>
        public async Task DeleteAsync(Guid lawyerId)
        {
            var lawyer = await GetByIdAsync(lawyerId);
            if (lawyer != null)
            {
                _context.Lawyers.Remove(lawyer);
            }
        }

        /// <summary>
        /// Save all pending changes
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
