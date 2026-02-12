using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SMS.DAL;
using SMS.Models;
using BCrypt.Net;

namespace SMS.BLL
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepo;

        public StudentService(IStudentRepository studentRepo)
        {
            _studentRepo = studentRepo;
        }

        public async Task<Student?> LoginAsync(string username, string password)
        {
            var student = await _studentRepo.GetByUsernameAsync(username);
            if (student != null && BCrypt.Net.BCrypt.Verify(password, student.PasswordHash))
            {
                return student;
            }
            return null;
        }

        public async Task<bool> RegisterAsync(Student student, string password)
        {
            // Check if user already exists
            if (await _studentRepo.ExistsAsync(student.Username, student.Email))
            {
                return false;
            }

            // Generate Student ID
            student.StudentId = await GenerateStudentIdAsync();

            // Hash password
            student.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            await _studentRepo.AddAsync(student);
            return true;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _studentRepo.GetAllAsync();
        }

        public async Task<(IEnumerable<Student> Items, int TotalCount)> GetPagedStudentsAsync(int pageNumber, int pageSize)
        {
            return await _studentRepo.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _studentRepo.GetByIdAsync(id);
        }

        public async Task<bool> IsUsernameOrEmailAvailableAsync(string username, string email)
        {
            return !await _studentRepo.ExistsAsync(username, email);
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            try
            {
                await _studentRepo.UpdateAsync(student);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            try
            {
                await _studentRepo.DeleteAsync(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<string> GenerateStudentIdAsync()
        {
            var latestId = await _studentRepo.GetLatestStudentIdAsync();
            int nextNumber = 1;

            if (!string.IsNullOrEmpty(latestId) && latestId.StartsWith("STU-"))
            {
                var parts = latestId.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            string year = DateTime.Now.Year.ToString();
            return $"STU-{year}-{nextNumber:D4}";
        }
    }
}
