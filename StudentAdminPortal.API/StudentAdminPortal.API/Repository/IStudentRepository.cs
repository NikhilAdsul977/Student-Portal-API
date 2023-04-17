using StudentAdminPortal.API.DataModels;

namespace StudentAdminPortal.API.Repository
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetStudents();
        Task<Student> GetStudentById(Guid studentId);

        Task<List<Gender>> GetGenders();

        Task<bool> exists(Guid studentId);

        Task<Student> UpdateStudent(Guid studentId, Student request);

        Task<bool> DeleteStudent(Guid studentId);

        Task<Student> AddStudent(Student request);

        Task<bool> UpdateProfile(Guid studentId, string ProfileImageUrl);
    }
}
