namespace StudentAdminPortal.API.DomainModels
{
    public class AddStudentRequest
    {
        public string StudentFirstName { get; set; }
        public string StudentLastname { get; set; }
        public string DOB { get; set; }
        public string Email { get; set; }
        public long Mobile { get; set; }
        public Guid GenderId { get; set; }
        public string StreetAdress { get; set; }
        public string ZipCode { get; set; }
    }
}
