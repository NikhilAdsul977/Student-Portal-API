using AutoMapper;
using StudentAdminPortal.API.DataModels;
using StudentAdminPortal.API.Profiles.AfterMaps;
using DomainModels = StudentAdminPortal.API.DomainModels;

namespace StudentAdminPortal.API.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() {
            CreateMap<DomainModels.Student, Student>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.StudentId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.StudentFirstName))
                .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.StudentLastname))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DOB))
                .ForMember(dest => dest.GenderId, opt => opt.MapFrom(src => src.GenderId)).ReverseMap();

            CreateMap<DomainModels.Gender, Gender>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.GenderId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Name)).ReverseMap();

            CreateMap<DomainModels.Address, Address>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AddressId))
                .ForMember(dest => dest.PhysicalAddress, opt => opt.MapFrom(src => src.StreetAdress))
                .ForMember(dest => dest.PostalAddress, opt => opt.MapFrom(src => src.ZipCode)).ReverseMap();

            CreateMap<DomainModels.UpdateStudentRequest, Student>()
                .AfterMap<UpdateStudentRequestAfterMap>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.StudentFirstName))
                .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.StudentLastname))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DOB))
                .ForMember(dest => dest.GenderId, opt => opt.MapFrom(src => src.GenderId)).ReverseMap();
        }
    }
}
