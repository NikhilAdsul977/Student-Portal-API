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
    }
}
