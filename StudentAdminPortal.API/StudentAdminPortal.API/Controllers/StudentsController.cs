using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentAdminPortal.API.DomainModels;
using DataModels = StudentAdminPortal.API.DataModels;

using StudentAdminPortal.API.Repository;
using System;

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

            return Ok(domainStudentObject);
        }


        [HttpGet]
        [Route("GetStudentById/{studentId:guid}")]
        public async Task<IActionResult> GetStudentById([FromRoute]Guid studentId)
        {
            var students = await _studentRepository.GetStudentById(studentId);
            if (students == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<Student>(students));
            }
        }

        [HttpPut]
        [Route("UpdateStudent/{studentId:guid}")]
        public async Task<IActionResult> UpdateStudent([FromRoute] Guid studentId, [FromBody] UpdateStudentRequest request)
        {
            if (await _studentRepository.exists(studentId))
            {
                var students = await _studentRepository.UpdateStudent(studentId, _mapper.Map<DataModels.Student>(request));
                if (students != null)
                {
                    return Ok(_mapper.Map<Student>(students));
                }
            }
            return NotFound();
        }
    }
}
