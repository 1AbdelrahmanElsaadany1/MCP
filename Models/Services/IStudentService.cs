using MCPv1.Models.Entities;

namespace MCPv1.Models.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDTO>> GetAllStudentsAsync();
        Task<StudentDTO?> GetStudentByIdAsync(int id);
        Task<StudentDTO> AddStudentAsync(StudentDTO studentDto);
        Task<StudentDTO> UpdateStudentAsync(StudentDTO studentDto);
        Task<bool> DeleteStudentAsync(int id);
        Task<StudentDTO> AddStudentToDepartmentAsync(int studentId, int departmentId);
    }
}
