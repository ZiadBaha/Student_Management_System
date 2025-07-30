using AutoMapper;
using SMS.Core.DTOs.Department;
using SMS.Core.Models.Entities;

namespace SMS.Helpers
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<Department, CreateDepartmentDto>().ReverseMap();
            CreateMap<Department, UpdateDepartmentDto>().ReverseMap();

            CreateMap<Teacher, DepartmentTeacherDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser.Email));

            CreateMap<Department, DepartmentWithTeachersDto>()
                .ForMember(dest => dest.Teachers, opt => opt.MapFrom(src => src.Teachers));
        }
    }
}
