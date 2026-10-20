using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudentAdminPortal.API.Controllers;
using StudentAdminPortal.API.DomainModels;
using StudentAdminPortal.API.Repository;
using Xunit;
using ApiStudent = StudentAdminPortal.API.DomainModels.Student;
using DataStudent = StudentAdminPortal.API.DataModels.Student;

namespace StudentAdminPortal.API.Tests.Controllers;

public class StudentsControllerEdgeCaseTests
{
    private readonly Mock<IStudentRepository> _repository = new();
    private readonly Mock<IImageRepository> _imageRepository = new();
    private readonly Mock<IMapper> _mapper = new();

    private StudentsController CreateController() =>
        new(_repository.Object, _mapper.Object, _imageRepository.Object);

    [Fact]
    public async Task GetAllStudent_WhenRepositoryIsEmpty_ReturnsEmptyMappedList()
    {
        var storedStudents = new List<DataStudent>();
        var apiStudents = new List<ApiStudent>();
        _repository.Setup(repository => repository.GetStudents()).ReturnsAsync(storedStudents);
        _mapper.Setup(mapper => mapper.Map<List<ApiStudent>>(storedStudents)).Returns(apiStudents);

        var result = await CreateController().GetAllStudent();

        var response = Assert.IsType<OkObjectResult>(result);
        Assert.Same(apiStudents, response.Value);
        Assert.Empty((List<ApiStudent>)response.Value!);
    }

    [Fact]
    public async Task GetStudentById_WhenStudentExists_ReturnsMappedStudent()
    {
        var studentId = Guid.NewGuid();
        var storedStudent = new DataStudent { id = studentId, FirstName = "Ava" };
        var apiStudent = new ApiStudent { StudentId = studentId, StudentFirstName = "Ava" };
        _repository.Setup(repository => repository.GetStudentById(studentId)).ReturnsAsync(storedStudent);
        _mapper.Setup(mapper => mapper.Map<ApiStudent>(storedStudent)).Returns(apiStudent);

        var result = await CreateController().GetStudentById(studentId);

        var response = Assert.IsType<OkObjectResult>(result);
        Assert.Same(apiStudent, response.Value);
    }

    [Fact]
    public async Task UpdateStudent_WhenStudentDoesNotExist_ReturnsNotFoundWithoutMappingRequest()
    {
        var studentId = Guid.NewGuid();
        _repository.Setup(repository => repository.exists(studentId)).ReturnsAsync(false);

        var result = await CreateController().UpdateStudent(studentId, new UpdateStudentRequest());

        Assert.IsType<NotFoundResult>(result);
        _repository.Verify(repository => repository.UpdateStudent(It.IsAny<Guid>(), It.IsAny<DataStudent>()), Times.Never);
        _mapper.Verify(mapper => mapper.Map<DataStudent>(It.IsAny<UpdateStudentRequest>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStudent_WhenRepositoryReturnsNull_ReturnsNotFound()
    {
        var studentId = Guid.NewGuid();
        var request = new UpdateStudentRequest();
        var mappedRequest = new DataStudent();
        _repository.Setup(repository => repository.exists(studentId)).ReturnsAsync(true);
        _mapper.Setup(mapper => mapper.Map<DataStudent>(request)).Returns(mappedRequest);
        _repository.Setup(repository => repository.UpdateStudent(studentId, mappedRequest)).ReturnsAsync((DataStudent)null!);

        var result = await CreateController().UpdateStudent(studentId, request);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateStudent_WhenUpdateSucceeds_ReturnsMappedStudent()
    {
        var studentId = Guid.NewGuid();
        var request = new UpdateStudentRequest();
        var mappedRequest = new DataStudent();
        var updatedStudent = new DataStudent { id = studentId, FirstName = "Ava" };
        var apiStudent = new ApiStudent { StudentId = studentId, StudentFirstName = "Ava" };
        _repository.Setup(repository => repository.exists(studentId)).ReturnsAsync(true);
        _mapper.Setup(mapper => mapper.Map<DataStudent>(request)).Returns(mappedRequest);
        _repository.Setup(repository => repository.UpdateStudent(studentId, mappedRequest)).ReturnsAsync(updatedStudent);
        _mapper.Setup(mapper => mapper.Map<ApiStudent>(updatedStudent)).Returns(apiStudent);

        var result = await CreateController().UpdateStudent(studentId, request);

        var response = Assert.IsType<OkObjectResult>(result);
        Assert.Same(apiStudent, response.Value);
    }

    [Fact]
    public async Task DeleteStudent_WhenStudentDoesNotExist_ReturnsNotFound()
    {
        var studentId = Guid.NewGuid();
        _repository.Setup(repository => repository.exists(studentId)).ReturnsAsync(false);

        var result = await CreateController().DeleteStudent(studentId);

        Assert.IsType<NotFoundResult>(result);
        _repository.Verify(repository => repository.DeleteStudent(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteStudent_WhenRepositoryCannotDelete_ReturnsNotFound()
    {
        var studentId = Guid.NewGuid();
        _repository.Setup(repository => repository.exists(studentId)).ReturnsAsync(true);
        _repository.Setup(repository => repository.DeleteStudent(studentId)).ReturnsAsync(false);

        var result = await CreateController().DeleteStudent(studentId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteStudent_WhenDeleteSucceeds_ReturnsOk()
    {
        var studentId = Guid.NewGuid();
        _repository.Setup(repository => repository.exists(studentId)).ReturnsAsync(true);
        _repository.Setup(repository => repository.DeleteStudent(studentId)).ReturnsAsync(true);

        var result = await CreateController().DeleteStudent(studentId);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UploadProfile_WhenFileIsNull_ReturnsNotFound()
    {
        var result = await CreateController().UploadProfile(Guid.NewGuid(), null!);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UploadProfile_WhenFileIsEmpty_ReturnsNotFound()
    {
        var image = CreateImage("profile.jpg", []);

        var result = await CreateController().UploadProfile(Guid.NewGuid(), image);

        Assert.IsType<NotFoundResult>(result);
    }

    [Theory]
    [InlineData("profile.jpeg")]
    [InlineData("profile.jpg")]
    [InlineData("profile.png")]
    [InlineData("profile.gif")]
    public async Task UploadProfile_WhenFileExtensionIsAllowed_StoresImageAndReturnsUrl(string fileName)
    {
        var studentId = Guid.NewGuid();
        var image = CreateImage(fileName, [1, 2, 3]);
        var expectedUrl = $"Resources/{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        string? uploadedFileName = null;
        _repository.Setup(repository => repository.exists(studentId)).ReturnsAsync(true);
        _imageRepository.Setup(repository => repository.Upload(image, It.IsAny<string>()))
            .Callback<IFormFile, string>((_, name) => uploadedFileName = name)
            .ReturnsAsync(expectedUrl);
        _repository.Setup(repository => repository.UpdateProfile(studentId, expectedUrl)).ReturnsAsync(true);

        var result = await CreateController().UploadProfile(studentId, image);

        var response = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedUrl, response.Value);
        Assert.NotNull(uploadedFileName);
        Assert.EndsWith(Path.GetExtension(fileName), uploadedFileName);
        _repository.Verify(repository => repository.UpdateProfile(studentId, expectedUrl), Times.Once);
    }

    [Fact]
    public async Task UploadProfile_WhenStudentDoesNotExist_ReturnsNotFoundWithoutUploading()
    {
        var studentId = Guid.NewGuid();
        var image = CreateImage("profile.jpg", [1]);
        _repository.Setup(repository => repository.exists(studentId)).ReturnsAsync(false);

        var result = await CreateController().UploadProfile(studentId, image);

        Assert.IsType<NotFoundResult>(result);
        _imageRepository.Verify(repository => repository.Upload(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UploadProfile_WhenProfileUpdateFails_ReturnsInternalServerError()
    {
        var studentId = Guid.NewGuid();
        var image = CreateImage("profile.jpg", [1]);
        const string imageUrl = "Resources/profile.jpg";
        _repository.Setup(repository => repository.exists(studentId)).ReturnsAsync(true);
        _imageRepository.Setup(repository => repository.Upload(image, It.IsAny<string>())).ReturnsAsync(imageUrl);
        _repository.Setup(repository => repository.UpdateProfile(studentId, imageUrl)).ReturnsAsync(false);

        var result = await CreateController().UploadProfile(studentId, image);

        var response = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
        Assert.Equal("Error Uploading Image", response.Value);
    }

    private static FormFile CreateImage(string fileName, byte[] contents)
    {
        var stream = new MemoryStream(contents);
        return new FormFile(stream, 0, stream.Length, "profileImage", fileName);
    }
}
