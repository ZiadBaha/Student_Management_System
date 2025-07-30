using AutoMapper;
using SMS.Core.DTOs.TeacherCourse;
using SMS.Core.Models.Entities;

namespace SMS.Helpers
{
    public class TeacherCourseProfile : Profile
    {
        public TeacherCourseProfile()
        {
            CreateMap<UpdateTeacherCoursesDto, Teacher>()
                .ForMember(dest => dest.CourseTeachers, opt => opt.Ignore());

            CreateMap<CourseTeacher, TeacherCourseDto>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.Course.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Course.Description));

            CreateMap<Teacher, GetTeacherCoursesDto>()
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser.LastName))
                .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.CourseTeachers));
        }
    }
}
