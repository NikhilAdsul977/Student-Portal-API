using Microsoft.EntityFrameworkCore;
using StudentAdminPortal.API.DataModels;

namespace StudentAdminPortal.API.Repository
{
    public class SqlStudentRepository : IStudentRepository
    {
        private readonly StudentAdminContext _context;
        public SqlStudentRepository(StudentAdminContext context) {
            this._context = context;
        }
        public async Task<List<Student>> GetStudents() { 
            return await _context.Students.Include(nameof(Gender)).Include(nameof(Address)).ToListAsync();
        }

        public async Task<Student> GetStudentById(Guid studentId)
        {
            return await _context.Students.Include(nameof(Gender)).Include(nameof(Address)).Where(x => x.id == studentId).FirstOrDefaultAsync();
        }

        public async Task<List<Gender>> GetGenders()
        {
            return await _context.Genders.ToListAsync();
        }

        public async Task<bool> exists(Guid studentId)
        {
            return _context.Students.Any(x => x.id == studentId);
        }

        public async Task<Student> UpdateStudent(Guid studentId, Student request)
        {
            var student = await GetStudentById(studentId);

            if (student != null)
            {
                student.FirstName = request.FirstName;
                student.Lastname = request.Lastname;
                student.Mobile = request.Mobile;
                student.Email = request.Email;
                student.GenderId = request.GenderId;
                if (student.Address == null)
                {
                    student.Address = new Address();
                }
                student.Address.PhysicalAddress = request.Address.PhysicalAddress;
                student.Address.PostalAddress = request.Address.PostalAddress;
                int res = await _context.SaveChangesAsync();
                return student;
            }
            return null;
        }
    }
}
