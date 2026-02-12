using System.Collections.Generic;
using System.Threading.Tasks;
using SMS.Models;

namespace SMS.BLL
{
    public interface IStudentService
    {
        Task<Student?> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(Student student, string password);
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<(IEnumerable<Student> Items, int TotalCount)> GetPagedStudentsAsync(int pageNumber, int pageSize);
        Task<Student?> GetStudentByIdAsync(int id);
        Task<bool> IsUsernameOrEmailAvailableAsync(string username, string email);
        Task<bool> UpdateStudentAsync(Student student);
        Task<bool> DeleteStudentAsync(int id);
    }
}
