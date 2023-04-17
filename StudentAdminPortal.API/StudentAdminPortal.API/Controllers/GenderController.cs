using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentAdminPortal.API.DomainModels;
using StudentAdminPortal.API.Repository;

namespace StudentAdminPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class GenderController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;
        public GenderController(IStudentRepository studentRepository, IMapper mapper)
        {
            this._studentRepository = studentRepository;
            this._mapper = mapper;
        }

        [HttpGet]
        [Route("GetGenders")]
        public async Task<IActionResult> GetGenders()
        {
            var students = await _studentRepository.GetGenders();

            var domainGenderObject = new List<Gender>();

            domainGenderObject = _mapper.Map<List<Gender>>(students);

            return Ok(domainGenderObject);
        }
    }
}
