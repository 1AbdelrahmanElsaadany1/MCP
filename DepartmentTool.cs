using MCPSharp;
using MCPv1.Models.Services;

namespace MCPv1
{
    public class DepartmentTool
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentTool(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [McpTool(name: "get_all_departments", Description = "This tool will get all departments.")]
        public async Task<string> GetAllDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return System.Text.Json.JsonSerializer.Serialize(departments);
        }

        [McpTool(name: "find_department_by_name", Description = "This tool will find a department by name.")]
        public async Task<string> FindDepartmentByName(
            [McpParameter(required: true, description: "Department name to search")] string name)
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var dept = departments.FirstOrDefault(d =>
                d.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            return System.Text.Json.JsonSerializer.Serialize(dept);
        }
    }
}
