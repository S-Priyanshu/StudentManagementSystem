using System.Collections.Generic;
using System.Threading.Tasks;
using SMS.Models;

namespace SMS.DAL
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByUsernameAsync(string username);
        Task<IEnumerable<Student>> GetAllAsync();
        Task<(IEnumerable<Student> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string username, string email);
        Task<string> GetLatestStudentIdAsync();
    }
}
