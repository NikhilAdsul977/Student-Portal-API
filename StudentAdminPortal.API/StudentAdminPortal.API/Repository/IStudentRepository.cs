using StudentAdminPortal.API.DataModels;

namespace StudentAdminPortal.API.Repository
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetStudents();
        Task<Student> GetStudentById(Guid studentId);
    }
}
