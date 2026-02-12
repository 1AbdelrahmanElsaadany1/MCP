using MCPv1.Models.Entities;
using MCPv1.Models.repositories;

namespace MCPv1.Models.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return departments.Select(MapToDTO);
        }

        public async Task<DepartmentDTO?> GetDepartmentByIdAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            return department != null ? MapToDTO(department) : null;
        }

        public async Task<DepartmentDTO> AddDepartmentAsync(DepartmentDTO departmentDto)
        {
            var department = new Department
            {
                Name = departmentDto.Name,
                Code = departmentDto.Code
            };

            var addedDepartment = await _departmentRepository.AddAsync(department);
            return MapToDTO(addedDepartment);
        }

        public async Task<DepartmentDTO> UpdateDepartmentAsync(DepartmentDTO departmentDto)
        {
            var department = new Department
            {
                Id = departmentDto.Id,
                Name = departmentDto.Name,
                Code = departmentDto.Code
            };

            var updatedDepartment = await _departmentRepository.UpdateAsync(department);
            return MapToDTO(updatedDepartment);
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            return await _departmentRepository.DeleteAsync(id);
        }

        private DepartmentDTO MapToDTO(Department department)
        {
            return new DepartmentDTO
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code
            };
        }
    }
}
