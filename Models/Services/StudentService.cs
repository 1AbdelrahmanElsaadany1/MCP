using MCPv1.Models.Entities;
using MCPv1.Models.repositories;

namespace MCPv1.Models.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public StudentService(IStudentRepository studentRepository, IDepartmentRepository departmentRepository)
        {
            _studentRepository = studentRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<StudentDTO>> GetAllStudentsAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            return students.Select(MapToDTO);
        }

        public async Task<StudentDTO?> GetStudentByIdAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            return student != null ? MapToDTO(student) : null;
        }

        public async Task<StudentDTO> AddStudentAsync(StudentDTO studentDto)
        {
            var student = new Student
            {
                Name = studentDto.Name,
                Email = studentDto.Email,
                DepartmentId = studentDto.DepartmentId
            };

            var addedStudent = await _studentRepository.AddAsync(student);
            return MapToDTO(addedStudent);
        }

        public async Task<StudentDTO> UpdateStudentAsync(StudentDTO studentDto)
        {
            var student = new Student
            {
                Id = studentDto.Id,
                Name = studentDto.Name,
                Email = studentDto.Email,
                DepartmentId = studentDto.DepartmentId
            };

            var updatedStudent = await _studentRepository.UpdateAsync(student);
            return MapToDTO(updatedStudent);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            return await _studentRepository.DeleteAsync(id);
        }

        public async Task<StudentDTO> AddStudentToDepartmentAsync(int studentId, int departmentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                throw new Exception("Student not found");

            var department = await _departmentRepository.GetByIdAsync(departmentId);
            if (department == null)
                throw new Exception("Department not found");

            student.DepartmentId = departmentId;
            var updatedStudent = await _studentRepository.UpdateAsync(student);

            return MapToDTO(updatedStudent);
        }

        private StudentDTO MapToDTO(Student student)
        {
            return new StudentDTO
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                DepartmentId = student.DepartmentId,
                DepartmentName = student.Department?.Name
            };
        }
    }
}
