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
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public StudentsController(IStudentRepository studentRepository, IMapper mapper, IImageRepository imageRepository)
        {
            this._studentRepository = studentRepository;
            this._mapper = mapper;
            _imageRepository = imageRepository;
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

        [HttpPost]
        [Route("UploadProfile/{studentId:guid}")]
        public async Task<IActionResult> UploadProfile([FromRoute] Guid studentId, IFormFile profileImage)
        {
            var validExtension = new List<string>
            {
                ".jpeg",".jpg",".png",".gif"
            };
            if (profileImage != null && profileImage.Length > 0)
            {
                var extension = Path.GetExtension(profileImage.FileName);
                if (validExtension.Contains(extension)) {
                    if (await _studentRepository.exists(studentId))
                    {
                        var fileName = Guid.NewGuid().ToString() + extension;

                        var fileImageUrl = await _imageRepository.Upload(profileImage, fileName);

                        if (await _studentRepository.UpdateProfile(studentId, fileImageUrl))
                        {
                            return Ok(fileImageUrl);
                        }

                        return StatusCode(StatusCodes.Status500InternalServerError, "Error Uploading Image");
                    }
                }
                else
                {
                    return BadRequest("Invalid image extension. Please upload jpeg, jpg, png, gif files.");
                }
            }
            return NotFound();
        }
    }
}
