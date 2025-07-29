using AutoMapper;
using SMS.Core.DTOs.Student;
using SMS.Core.Models.Entities;

namespace SMS.Helpers
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, GetAllStudentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AppUser.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser.Email));

            CreateMap<Student, GetStudentByIdDto>()
                .IncludeBase<Student, GetAllStudentDto>();

            CreateMap<UpdateStudentDto, Student>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForAllMembers(opt => opt.Ignore());
        }
    }
}
