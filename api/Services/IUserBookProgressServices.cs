
using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public interface IUserBookProgressService
    {
        Task<double> GetProgressAsync(int userId, int bookId);
        Task<UserBookProgress> CreateOrUpdateProgressAsync(int userId, int bookId, double progress);
        Task<Dictionary<int, double>> GetProgressForBooksAsync(int userId, List<int> bookIds);
    }

    public class UserBookProgressService : IUserBookProgressService
    {
        private readonly ApplicationDBContext _context;

        public UserBookProgressService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<double> GetProgressAsync(int userId, int bookId)
        {
            var progress = await _context.UserBookProgresses.FirstOrDefaultAsync(p => p.UserId == userId && p.BookId == bookId);

            return progress?.Progress ?? 0.0;
        }

        public async Task<UserBookProgress> CreateOrUpdateProgressAsync(int userId, int bookId, double progress)
        {
            var existingProgress = await _context.UserBookProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.BookId == bookId);

            if (existingProgress != null)
            {
                existingProgress.Progress = progress;
                existingProgress.UpdatedAt = DateTime.UtcNow;
                _context.UserBookProgresses.Update(existingProgress);
            }
            else
            {
                existingProgress = new UserBookProgress
                {
                    UserId = userId,
                    BookId = bookId,
                    Progress = progress,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.UserBookProgresses.Add(existingProgress);
            }

            await _context.SaveChangesAsync();
            return existingProgress;
        }

        public async Task<Dictionary<int, double>> GetProgressForBooksAsync(int userId, List<int> bookIds)
        {
            var progresses = await _context.UserBookProgresses
                .Where(p => p.UserId == userId && bookIds.Contains(p.BookId))
                .ToDictionaryAsync(p => p.BookId, p => p.Progress);

            // Agregar progreso 0.0 para libros sin registro
            foreach (var bookId in bookIds)
            {
                if (!progresses.ContainsKey(bookId))
                {
                    progresses[bookId] = 0.0;
                }
            }

            return progresses;
        }
    }
}