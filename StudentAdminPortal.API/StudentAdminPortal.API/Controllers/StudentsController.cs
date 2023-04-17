using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentAdminPortal.API.DomainModels;
using DataModels = StudentAdminPortal.API.DataModels;

using StudentAdminPortal.API.Repository;
using System;
using Azure.Core;

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
        [Route("GetStudentById/{studentId:guid}"), ActionName("GetStudentById")]
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


        [HttpDelete]
        [Route("DeleteStudent/{studentId:guid}")]
        public async Task<IActionResult> DeleteStudent([FromRoute] Guid studentId)
        {
            if (await _studentRepository.exists(studentId))
            {
                bool isDeleted = await _studentRepository.DeleteStudent(studentId);
                if (isDeleted)
                {
                    return Ok();
                }
            }
            return NotFound();
        }

        [HttpPost]
        [Route("AddStudent")]
        public async Task<IActionResult> AddStudent([FromBody] AddStudentRequest request)
        {
            var students = await _studentRepository.AddStudent(_mapper.Map<DataModels.Student>(request));
            return CreatedAtAction(nameof(GetStudentById), new {studentId = students.id}, _mapper.Map<Student>(students));
        }
    }
}
