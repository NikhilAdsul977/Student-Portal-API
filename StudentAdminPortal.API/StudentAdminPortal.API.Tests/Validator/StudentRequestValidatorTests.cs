using StudentAdminPortal.API.DataModels;
using StudentAdminPortal.API.Repository;
using StudentAdminPortal.API.Validator;
using Moq;
using Xunit;
using AddStudentRequest = StudentAdminPortal.API.DomainModels.AddStudentRequest;
using UpdateStudentRequest = StudentAdminPortal.API.DomainModels.UpdateStudentRequest;

namespace StudentAdminPortal.API.Tests.Validator;

public class StudentRequestValidatorTests
{
    [Fact]
    public void AddStudentRequestValidator_WhenRequestIsCompleteAndGenderExists_IsValid()
    {
        var genderId = Guid.NewGuid();
        var repository = new Mock<IStudentRepository>();
        repository.Setup(repo => repo.GetGenders()).ReturnsAsync([new Gender { Id = genderId, Description = "Other" }]);

        var result = new AddStudentRequestValidator(repository.Object).Validate(CreateAddRequest(genderId));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void AddStudentRequestValidator_WhenRequiredFieldsAreMissing_IsInvalid()
    {
        var repository = new Mock<IStudentRepository>();
        repository.Setup(repo => repo.GetGenders()).ReturnsAsync(new List<Gender>());

        var result = new AddStudentRequestValidator(repository.Object).Validate(new AddStudentRequest());

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AddStudentRequest.StudentFirstName));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AddStudentRequest.StudentLastname));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AddStudentRequest.DOB));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AddStudentRequest.Email));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AddStudentRequest.GenderId));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AddStudentRequest.StreetAdress));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AddStudentRequest.ZipCode));
    }

    [Theory]
    [InlineData(999999999)]
    [InlineData(10000000000)]
    public void AddStudentRequestValidator_WhenMobileIsOutsideAllowedRange_IsInvalid(long mobile)
    {
        var genderId = Guid.NewGuid();
        var repository = new Mock<IStudentRepository>();
        repository.Setup(repo => repo.GetGenders()).ReturnsAsync([new Gender { Id = genderId, Description = "Other" }]);
        var request = CreateAddRequest(genderId);
        request.Mobile = mobile;

        var result = new AddStudentRequestValidator(repository.Object).Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AddStudentRequest.Mobile));
    }

    [Fact]
    public void AddStudentRequestValidator_WhenGenderDoesNotExist_IsInvalid()
    {
        var repository = new Mock<IStudentRepository>();
        repository.Setup(repo => repo.GetGenders()).ReturnsAsync(new List<Gender>());
        var request = CreateAddRequest(Guid.NewGuid());

        var result = new AddStudentRequestValidator(repository.Object).Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AddStudentRequest.GenderId));
    }

    [Fact]
    public void UpdateStudentRequestValidator_WhenRequestIsCompleteAndGenderExists_IsValid()
    {
        var genderId = Guid.NewGuid();
        var repository = new Mock<IStudentRepository>();
        repository.Setup(repo => repo.GetGenders()).ReturnsAsync([new Gender { Id = genderId, Description = "Other" }]);

        var result = new UpdateStudentRequestValidator(repository.Object).Validate(CreateUpdateRequest(genderId));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateStudentRequestValidator_WhenRequiredFieldsAreMissing_IsInvalid()
    {
        var repository = new Mock<IStudentRepository>();
        repository.Setup(repo => repo.GetGenders()).ReturnsAsync(new List<Gender>());

        var result = new UpdateStudentRequestValidator(repository.Object).Validate(new UpdateStudentRequest());

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateStudentRequest.StudentFirstName));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateStudentRequest.StudentLastname));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateStudentRequest.DOB));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateStudentRequest.Email));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateStudentRequest.GenderId));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateStudentRequest.StreetAdress));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateStudentRequest.ZipCode));
    }

    [Theory]
    [InlineData(999999999)]
    [InlineData(10000000000)]
    public void UpdateStudentRequestValidator_WhenMobileIsOutsideAllowedRange_IsInvalid(long mobile)
    {
        var genderId = Guid.NewGuid();
        var repository = new Mock<IStudentRepository>();
        repository.Setup(repo => repo.GetGenders()).ReturnsAsync([new Gender { Id = genderId, Description = "Other" }]);
        var request = CreateUpdateRequest(genderId);
        request.Mobile = mobile;

        var result = new UpdateStudentRequestValidator(repository.Object).Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateStudentRequest.Mobile));
    }

    [Fact]
    public void UpdateStudentRequestValidator_WhenGenderDoesNotExist_IsInvalid()
    {
        var repository = new Mock<IStudentRepository>();
        repository.Setup(repo => repo.GetGenders()).ReturnsAsync(new List<Gender>());
        var request = CreateUpdateRequest(Guid.NewGuid());

        var result = new UpdateStudentRequestValidator(repository.Object).Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateStudentRequest.GenderId));
    }

    private static AddStudentRequest CreateAddRequest(Guid genderId) => new()
    {
        StudentFirstName = "Ava",
        StudentLastname = "Patel",
        DOB = "2000-01-01",
        Email = "ava@example.com",
        Mobile = 1234567890,
        GenderId = genderId,
        StreetAdress = "1 Main Street",
        ZipCode = "12345"
    };

    private static UpdateStudentRequest CreateUpdateRequest(Guid genderId) => new()
    {
        StudentFirstName = "Ava",
        StudentLastname = "Patel",
        DOB = "2000-01-01",
        Email = "ava@example.com",
        Mobile = 1234567890,
        GenderId = genderId,
        StreetAdress = "1 Main Street",
        ZipCode = "12345"
    };
}
