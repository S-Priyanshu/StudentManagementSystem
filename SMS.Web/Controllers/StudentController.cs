using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.BLL;
using SMS.Models;
using SMS.Web.Models;

namespace SMS.Web.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IStudentService studentService, ILogger<StudentController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 15;
            _logger.LogInformation("Fetching student list for page {Page}.", page);
            
            var (students, totalCount) = await _studentService.GetPagedStudentsAsync(page, pageSize);
            
            var viewModel = new StudentPagedViewModel
            {
                Students = students,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            _logger.LogInformation("Successfully retrieved {Count} students for page {Page}.", students.Count(), page);
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loggedInUserId != id.ToString())
            {
                _logger.LogWarning("User {LoggedInUser} tried to access edit page of student {TargetId}.", loggedInUserId, id);
                return Forbid();
            }

            _logger.LogInformation("Fetching student for edit. ID: {Id}", id);
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                _logger.LogWarning("Student with ID {Id} not found.", id);
                return NotFound();
            }

            var model = new StudentUpdateViewModel
            {
                Id = student.Id,
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Age = student.Age,
                DOB = student.DOB,
                Gender = student.Gender,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                Username = student.Username,
                Qualifications = student.Qualifications.Select(q => new QualificationViewModel
                {
                    CourseName = q.CourseName,
                    University = q.University,
                    YearOfPassing = q.YearOfPassing,
                    Percentage = q.Percentage
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentUpdateViewModel model)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loggedInUserId != model.Id.ToString())
            {
                _logger.LogWarning("User {LoggedInUser} tried to update profile of student {TargetId}.", loggedInUserId, model.Id);
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Updating student. ID: {Id}, StudentId: {StudentId}", model.Id, model.StudentId);
                var student = await _studentService.GetStudentByIdAsync(model.Id);
                if (student == null)
                {
                    _logger.LogWarning("Student with ID {Id} not found for update.", model.Id);
                    return NotFound();
                }

                // Update properties
                student.FirstName = model.FirstName;
                student.LastName = model.LastName;
                student.Age = model.Age;
                student.DOB = model.DOB;
                student.Gender = model.Gender;
                student.Email = model.Email;
                student.PhoneNumber = model.PhoneNumber;
                student.Username = model.Username;

                // Update Qualifications - simple approach: replace all
                student.Qualifications.Clear();
                if (model.Qualifications != null)
                {
                    foreach (var q in model.Qualifications)
                    {
                        student.Qualifications.Add(new SMS.Models.Qualification
                        {
                            CourseName = q.CourseName,
                            University = q.University,
                            YearOfPassing = q.YearOfPassing,
                            Percentage = q.Percentage,
                            StudentId = student.Id
                        });
                    }
                }

                var success = await _studentService.UpdateStudentAsync(student);
                if (success)
                {
                    _logger.LogInformation("Student updated successfully. ID: {Id}", model.Id);
                    return RedirectToAction("Index");
                }

                _logger.LogError("Failed to update student. ID: {Id}", model.Id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the student.");
            }
            return View(model);
        }
    }
}
