using MCPv1.Models.Entities;
using MCPv1.Models.Services;
using Microsoft.AspNetCore.Mvc;

namespace MCPv1.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IDepartmentService _departmentService;

        public StudentController(IStudentService studentService, IDepartmentService departmentService)
        {
            _studentService = studentService;
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return View(students);
        }

        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentDTO student)
        {
            if (ModelState.IsValid)
            {
                await _studentService.AddStudentAsync(student);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            return View(student);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentDTO student)
        {
            if (id != student.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _studentService.UpdateStudentAsync(student);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            return View(student);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _studentService.DeleteStudentAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AssignDepartment(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDepartment(int id, int departmentId)
        {
            try
            {
                await _studentService.AddStudentToDepartmentAsync(id, departmentId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                var student = await _studentService.GetStudentByIdAsync(id);
                ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                return View(student);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateJson([FromBody] StudentDTO student)
        {
            if (ModelState.IsValid)
            {
                await _studentService.AddStudentAsync(student);
                return Json(new { success = true, message = "Student created", student });
            }
            return Json(new { success = false, errors = ModelState });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJson()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Json(students);
        }
    }
}