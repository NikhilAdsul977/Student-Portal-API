using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentAdminPortal.API.DomainModels;
using StudentAdminPortal.API.Repository;

namespace StudentAdminPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentsController(IStudentRepository studentRepository, IMapper mapper) {
            this._studentRepository = studentRepository;
            this._mapper = mapper;
        }

        [HttpGet]
        [Route("GetAllStudent")]
        public async Task<IActionResult> GetAllStudent()
        {
            var students = await _studentRepository.GetStudents();

            var domainStudentObject = new List<Student>();

            domainStudentObject = _mapper.Map<List<Student>>(students);
            //foreach (var std in students)
            //{
            //    Student s = new Student();
            //    s.StudentId = std.id;
            //    s.StudentFirstName = std.FirstName;
            //    s.StudentFirstName = std.Lastname;
            //    s.DOB = std.DateOfBirth;
            //    s.GenderId = std.GenderId;
            //    s.Mobile = std.Mobile;
            //    s.Email= std.Email;
            //    s.ProfileImageUrl= std.ProfileImageUrl;
            //    s.Address = new Address();
            //    s.Address.ZipCode = std.Address.PostalAddress;
            //    s.Address.StreetAdress = std.Address.PhysicalAddress;
            //    s.Address.AddressId = std.Address.Id;
            //    s.Gender = new Gender();
            //    s.Gender.GenderId= std.Gender.Id;
            //    s.Gender.Name = std.Gender.Description;
            //    domainStudentObject.Add(s);
            //}

            return Ok(domainStudentObject);
        }

    }
}
