using FluentValidation;
using StudentAdminPortal.API.DomainModels;
using StudentAdminPortal.API.Repository;

namespace StudentAdminPortal.API.Validator
{
    public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequest>
    {
        public UpdateStudentRequestValidator(IStudentRepository studentRepository)
        {
            RuleFor(x => x.StudentFirstName).NotEmpty();
            RuleFor(x => x.StudentLastname).NotEmpty();
            RuleFor(x => x.DOB).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Mobile).GreaterThan(999999999).LessThan(10000000000);
            RuleFor(x => x.GenderId).NotEmpty().Must(id =>
            {
                var gender = studentRepository.GetGenders().Result.ToList().FirstOrDefault(x => x.Id == id);

                if (gender != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }).WithMessage("Please select valid Gender");
            RuleFor(x => x.StreetAdress).NotEmpty();
            RuleFor(x => x.ZipCode).NotEmpty();

        }
    }
}
