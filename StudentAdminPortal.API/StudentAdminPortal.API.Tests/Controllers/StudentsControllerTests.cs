using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudentAdminPortal.API.Controllers;
using StudentAdminPortal.API.Repository;
using Xunit;
using ApiStudent = StudentAdminPortal.API.DomainModels.Student;
using DataStudent = StudentAdminPortal.API.DataModels.Student;

namespace StudentAdminPortal.API.Tests.Controllers;

public class StudentsControllerTests
{
    private readonly Mock<IStudentRepository> _repository = new();
    private readonly Mock<IImageRepository> _imageRepository = new();
    private readonly Mock<IMapper> _mapper = new();

    private StudentsController CreateController() =>
        new(_repository.Object, _mapper.Object, _imageRepository.Object);

    [Fact]
    public async Task GetAllStudent_ReturnsMappedStudents()
    {
        var storedStudents = new List<DataStudent>
        {
            new() { id = Guid.NewGuid(), FirstName = "Ava", Lastname = "Patel", Email = "ava@example.com" }
        };
        var apiStudents = new List<ApiStudent>
        {
            new() { StudentId = storedStudents[0].id, StudentFirstName = "Ava", StudentLastname = "Patel", Email = "ava@example.com" }
        };
        _repository.Setup(repository => repository.GetStudents()).ReturnsAsync(storedStudents);
        _mapper.Setup(mapper => mapper.Map<List<ApiStudent>>(storedStudents)).Returns(apiStudents);

        var result = await CreateController().GetAllStudent();

        var response = Assert.IsType<OkObjectResult>(result);
        Assert.Same(apiStudents, response.Value);
    }

    [Fact]
    public async Task GetStudentById_WhenStudentDoesNotExist_ReturnsNotFound()
    {
        var studentId = Guid.NewGuid();
        _repository.Setup(repository => repository.GetStudentById(studentId))
            .ReturnsAsync((DataStudent)null!);

        var result = await CreateController().GetStudentById(studentId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddStudent_ReturnsCreatedResponseWithMappedStudent()
    {
        var request = new StudentAdminPortal.API.DomainModels.AddStudentRequest
        {
            StudentFirstName = "Ava",
            StudentLastname = "Patel",
            DOB = "2000-01-01",
            Email = "ava@example.com",
            Mobile = 1234567890,
            GenderId = Guid.NewGuid(),
            StreetAdress = "1 Main Street",
            ZipCode = "12345"
        };
        var storedStudent = new DataStudent
        {
            id = Guid.NewGuid(),
            FirstName = request.StudentFirstName,
            Lastname = request.StudentLastname,
            Email = request.Email
        };
        var apiStudent = new ApiStudent
        {
            StudentId = storedStudent.id,
            StudentFirstName = storedStudent.FirstName,
            StudentLastname = storedStudent.Lastname,
            Email = storedStudent.Email
        };
        _mapper.Setup(mapper => mapper.Map<DataStudent>(request)).Returns(storedStudent);
        _repository.Setup(repository => repository.AddStudent(storedStudent)).ReturnsAsync(storedStudent);
        _mapper.Setup(mapper => mapper.Map<ApiStudent>(storedStudent)).Returns(apiStudent);

        var result = await CreateController().AddStudent(request);

        var response = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(StudentsController.GetStudentById), response.ActionName);
        Assert.Equal(storedStudent.id, response.RouteValues!["studentId"]);
        Assert.Same(apiStudent, response.Value);
    }

    [Fact]
    public async Task UploadProfile_WithUnsupportedExtension_ReturnsBadRequest()
    {
        var image = new FormFile(new MemoryStream([1]), 0, 1, "profileImage", "profile.bmp");

        var result = await CreateController().UploadProfile(Guid.NewGuid(), image);

        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid image extension. Please upload jpeg, jpg, png, gif files.", response.Value);
        _repository.Verify(repository => repository.exists(It.IsAny<Guid>()), Times.Never);
        _imageRepository.Verify(imageRepository => imageRepository.Upload(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Never);
    }
}
