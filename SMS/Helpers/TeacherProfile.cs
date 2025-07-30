using AutoMapper;
using SMS.Core.DTOs.Teacher;
using SMS.Core.Models.Entities;

namespace SMS.Helpers
{
    public class TeacherProfile : Profile
    {
        public TeacherProfile()
        {
            CreateMap<Teacher, TeacherInfoDto>()
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser.Email))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId));

            CreateMap<Teacher, GetAllTeacherDto>()
                .IncludeBase<Teacher, TeacherInfoDto>();
            CreateMap<Teacher, GetTeacherByIdDto>()
                .IncludeBase<Teacher , TeacherInfoDto>();

            CreateMap<UpdateTeacherDto, Teacher>()
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForAllMembers(opt => opt.Ignore());
        }
    }
}
