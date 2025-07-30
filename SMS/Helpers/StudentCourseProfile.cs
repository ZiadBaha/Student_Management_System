using AutoMapper;
using SMS.Core.DTOs.Course;
using SMS.Core.DTOs.StudentCourse;
using SMS.Core.Models.Entities;

namespace SMS.Helpers
{
    public class StudentCourseProfile : Profile
    {
        public StudentCourseProfile()
        {
            CreateMap<AddCoursesToStudentDto, Student>()
                .ForMember(dest => dest.Enrollments, opt => opt.Ignore()); 

            CreateMap<UpdateStudentCoursesDto, Student>()
                .ForMember(dest => dest.Enrollments, opt => opt.Ignore()); 

            CreateMap<Enrollment, StudentCourseDto>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Course.Description))
                .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade));

        }
    }
}
