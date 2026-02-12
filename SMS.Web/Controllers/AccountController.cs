using System.Security.Claims;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SMS.BLL;
using SMS.Models;
using SMS.Web.Models;

namespace SMS.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IStudentService studentService, ILogger<AccountController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Student");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("User {Username} attempted to login.", model.Username);
                var student = await _studentService.LoginAsync(model.Username, model.Password);
                if (student != null)
                {
                    _logger.LogInformation("User {Username} successfully logged in.", model.Username);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                        new Claim(ClaimTypes.Name, student.Username),
                        new Claim(ClaimTypes.Email, student.Email),
                        new Claim("FullName", $"{student.FirstName} {student.LastName}"),
                        new Claim("StudentId", student.StudentId)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("Index", "Student");
                }
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                _logger.LogWarning("Failed login attempt for user {Username}.", model.Username);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new StudentRegistrationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(StudentRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var student = new Student
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Age = model.Age,
                    DOB = model.DOB,
                    Gender = model.Gender,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Username = model.Username,
                    Qualifications = model.Qualifications.Select(q => new Qualification
                    {
                        CourseName = q.CourseName,
                        University = q.University,
                        YearOfPassing = q.YearOfPassing,
                        Percentage = q.Percentage
                    }).ToList()
                };

                var success = await _studentService.RegisterAsync(student, model.Password);
                if (success)
                {
                    _logger.LogInformation("New student registered: {Username} ({Email})", model.Username, model.Email);
                    return RedirectToAction("Login");
                }
                _logger.LogWarning("Registration failed for {Username}. Username or Email may already exist.", model.Username);
                ModelState.AddModelError(string.Empty, "Username or Email already exists.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
