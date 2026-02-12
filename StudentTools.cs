using MCPSharp;
using MCPv1.Models.Entities;
using MCPv1.Models.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MCPv1
{
    public class StudentTools
    {
        public static IServiceProvider ServiceProvider { get; set; }


        [McpTool(
            name: "add_student",
            description: "Adds a new student to the system. Requires name, email, and departmentId."
        )]
        public static async Task<string> AddStudent(
            string name,
            string email,
            int departmentId)
        {
            try
            {
                if (ServiceProvider == null)
                    return "ERROR: ServiceProvider is null";

                using var scope = ServiceProvider.CreateScope();
                var studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();

                var student = new StudentDTO
                {
                    Name = name,
                    Email = email,
                    DepartmentId = departmentId
                };

                await studentService.AddStudentAsync(student);

                return $"✅ Successfully added student '{name}' with email '{email}' to department {departmentId}";
            }
            catch (Exception ex)
            {
                return $"❌ Error adding student: {ex.Message}";
            }
        }

        [McpTool(
            name: "get_departments",
            description: "Gets all departments from the system."
        )]
        public static async Task<string> GetDepartments()
        {
            try
            {
                if (ServiceProvider == null)
                    return "ERROR: ServiceProvider is null";

                using var scope = ServiceProvider.CreateScope();
                var departmentService = scope.ServiceProvider.GetRequiredService<IDepartmentService>();
                var departments = await departmentService.GetAllDepartmentsAsync();

                if (!departments.Any())
                    return "No departments found in the system.";

                var result = "📚 Departments:\n";
                foreach (var dept in departments)
                {
                    result += $"  • ID: {dept.Id} | Name: {dept.Name} | Code: {dept.Code}\n";
                }
                return result;
            }
            catch (Exception ex)
            {
                return $"❌ Error getting departments: {ex.Message}";
            }
        }

        [McpTool(
      name: "get_students",
      description: "Gets students. Use 'query' parameter for filtering: '$select=Name' for names only, '$filter=DepartmentId eq 1' for department, '$top=5' for limit. Leave empty for all students."
  )]
        public static async Task<string> GetStudents(string query = "")
        {
            try
            {
                if (ServiceProvider == null)
                    return "ERROR: ServiceProvider is null";

                using var scope = ServiceProvider.CreateScope();
                var studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();
                var students = await studentService.GetAllStudentsAsync();

                if (!students.Any())
                    return "No students found in the system.";

                string queryLower = (query ?? "").ToLower().Trim();

                if (queryLower.Contains("$select=name"))
                {
                    return "👥 Student Names:\n" + string.Join("\n", students.Select(s => $"  • {s.Name}"));
                }

                if (queryLower.Contains("departmentid eq"))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(queryLower, @"departmentid\s+eq\s+(\d+)");
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int deptId))
                    {
                        students = students.Where(s => s.DepartmentId == deptId).ToList();
                    }
                }

                if (queryLower.Contains("$top="))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(queryLower, @"\$top=(\d+)");
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int top))
                    {
                        students = students.Take(top).ToList();
                    }
                }

                var result = "👥 Students:\n";
                foreach (var student in students)
                {
                    result += $"  • {student.Name} ({student.Email})";
                    if (student.DepartmentId.HasValue)
                        result += $" - Dept ID: {student.DepartmentId}";
                    result += "\n";
                }
                return result;
            }
            catch (Exception ex)
            {
                return $"❌ Error getting students: {ex.Message}";
            }
        }
        [McpTool(
    name: "delete_student",
    description: "Permanently removes a student from the system using their unique ID number."
)]
        public static async Task<string> DeleteStudent(int studentId)
        {
            try
            {
                if (ServiceProvider == null)
                    return "ERROR: ServiceProvider is null";

                using var scope = ServiceProvider.CreateScope();
                var studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();

                // 1. Find student first so we can tell the user who we deleted
                var student = await studentService.GetStudentByIdAsync(studentId);

                if (student == null)
                {
                    return $"❌ Deletion failed: No student found with ID {studentId}.";
                }

                // 2. Perform the deletion
                bool isDeleted = await studentService.DeleteStudentAsync(studentId);

                if (isDeleted)
                {
                    return $"✅ Student '{student.Name}' (ID: {studentId}) has been successfully removed.";
                }

                return $"⚠️ The student record was found, but the database could not delete it.";
            }
            catch (Exception ex)
            {
                return $"❌ Error: {ex.Message}";
            }
        }


    }
}