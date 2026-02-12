using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SMS.Models;

namespace SMS.DAL
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SMSDbContext _context;

        public StudentRepository(SMSDbContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Qualifications)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student?> GetByUsernameAsync(string username)
        {
            return await _context.Students
                .Include(s => s.Qualifications)
                .FirstOrDefaultAsync(s => s.Username == username);
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.Qualifications)
                .OrderByDescending(s => s.Id)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Student> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Students.CountAsync();
            var items = await _context.Students
                .Include(s => s.Qualifications)
                .OrderByDescending(s => s.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string username, string email)
        {
            return await _context.Students.AnyAsync(s => s.Username == username || s.Email == email);
        }

        public async Task<string> GetLatestStudentIdAsync()
        {
            var latestStudent = await _context.Students
                .OrderByDescending(s => s.Id)
                .Select(s => s.StudentId)
                .FirstOrDefaultAsync();
            return latestStudent ?? string.Empty;
        }
    }
}
