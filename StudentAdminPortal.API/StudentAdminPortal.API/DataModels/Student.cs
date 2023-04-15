namespace StudentAdminPortal.API.DataModels
{
    public class Student
    {
        public Guid id { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public long Mobile { get; set; }
        public Guid GenderId { get; set; }
        public string? ProfileImageUrl { get; set; }

        //Navigation Property
        public Gender Gender { get; set; }
        public Address Address { get; set; }    
    }
}
