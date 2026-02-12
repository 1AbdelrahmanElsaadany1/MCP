using MCPv1.Models.Entities;

namespace MCPv1.Models.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync();
        Task<DepartmentDTO?> GetDepartmentByIdAsync(int id);
        Task<DepartmentDTO> AddDepartmentAsync(DepartmentDTO departmentDto);
        Task<DepartmentDTO> UpdateDepartmentAsync(DepartmentDTO departmentDto);
        Task<bool> DeleteDepartmentAsync(int id);
    }
}
