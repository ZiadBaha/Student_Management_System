using AutoMapper;
using SMS.Core.DTOs.Course;
using SMS.Core.DTOs.Enrollment;
using SMS.Core.DTOs.Teacher;
using SMS.Core.Models.Entities;

namespace SMS.Helpers
{
    public class CourseMappingProfile : Profile
    {
        public CourseMappingProfile()
        {
            CreateMap<Course, CourseDto>();
            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>();

            CreateMap<Course, CourseDetailsDto>()
                .IncludeBase<Course, CourseDto>()
                .ForMember(dest => dest.EnrolledStudents, opt => opt.MapFrom(src =>
                    src.Enrollments.Select(e => new EnrolledStudentDto
                    {
                        StudentId = e.StudentId,
                        FirstName = e.Student.AppUser.FirstName,
                        LastName = e.Student.AppUser.LastName,
                        Grade = e.Grade
                    }).ToList()))
                .ForMember(dest => dest.Teachers, opt => opt.MapFrom(src =>
                    src.CourseTeachers.Select(ct => new TeacherInfoDto
                    {
                        TeacherId = ct.Teacher.AppUser.Id,
                        FirstName = ct.Teacher.AppUser.FirstName,
                        LastName = ct.Teacher.AppUser.LastName,
                        Email = ct.Teacher.AppUser.Email,
                        DepartmentId = ct.Teacher.DepartmentId,

                    }).ToList()));
        }
    }
}
