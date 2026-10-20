using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudentAdminPortal.API.Controllers;
using StudentAdminPortal.API.Repository;
using Xunit;
using ApiGender = StudentAdminPortal.API.DomainModels.Gender;
using DataGender = StudentAdminPortal.API.DataModels.Gender;

namespace StudentAdminPortal.API.Tests.Controllers;

public class GenderControllerTests
{
    private readonly Mock<IStudentRepository> _repository = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public async Task GetGenders_ReturnsMappedGenders()
    {
        var storedGenders = new List<DataGender> { new() { Id = Guid.NewGuid(), Description = "Other" } };
        var apiGenders = new List<ApiGender> { new() { GenderId = storedGenders[0].Id, Name = "Other" } };
        _repository.Setup(repository => repository.GetGenders()).ReturnsAsync(storedGenders);
        _mapper.Setup(mapper => mapper.Map<List<ApiGender>>(storedGenders)).Returns(apiGenders);
        var controller = new GenderController(_repository.Object, _mapper.Object);

        var result = await controller.GetGenders();

        var response = Assert.IsType<OkObjectResult>(result);
        Assert.Same(apiGenders, response.Value);
    }

    [Fact]
    public async Task GetGenders_WhenNoGendersExist_ReturnsEmptyList()
    {
        var storedGenders = new List<DataGender>();
        var apiGenders = new List<ApiGender>();
        _repository.Setup(repository => repository.GetGenders()).ReturnsAsync(storedGenders);
        _mapper.Setup(mapper => mapper.Map<List<ApiGender>>(storedGenders)).Returns(apiGenders);
        var controller = new GenderController(_repository.Object, _mapper.Object);

        var result = await controller.GetGenders();

        var response = Assert.IsType<OkObjectResult>(result);
        Assert.Same(apiGenders, response.Value);
        Assert.Empty((List<ApiGender>)response.Value!);
    }
}
