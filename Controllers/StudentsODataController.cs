using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using MCPv1.Models.Services;

namespace MCPv1.Controllers
{
 
    public class StudentsController : ODataController
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }
    }
}