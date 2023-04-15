using StudentAdminPortal.API.DataModels;

namespace StudentAdminPortal.API.DomainModels
{
    public class Student
    {
        public Guid StudentId { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastname { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public long Mobile { get; set; }
        public Guid GenderId { get; set; }
        public string? ProfileImageUrl { get; set; }
        public Gender Gender { get; set; }
        public Address Address { get; set; }
    }
}
