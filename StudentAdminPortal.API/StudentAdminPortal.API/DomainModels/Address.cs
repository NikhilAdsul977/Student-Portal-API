namespace StudentAdminPortal.API.DomainModels
{
    public class Address
    {
        public Guid AddressId { get; set; }
        public string StreetAdress { get; set; }
        public string ZipCode { get; set; }
        public Guid StudentId { get; set; }
    }
}