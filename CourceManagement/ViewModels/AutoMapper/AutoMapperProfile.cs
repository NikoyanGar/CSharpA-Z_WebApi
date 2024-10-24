using AutoMapper;
using CourceManagement.Data.Entities;

namespace CourceManagement.ViewModels.AutoMapper
{

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // course
            CreateMap<Course, CourseViewModel>();
            CreateMap<CourseViewModel, Course>();
            CreateMap<CourseRequest, Course>();

            //lesson
            CreateMap<Lesson, LessonViewModel>();
            CreateMap<LessonViewModel, Lesson>();
            CreateMap<LessonRequest, Lesson>()
            .ForMember(des => des.DateCreated, opt => opt.MapFrom(src => DateTime.Now));

            //user
            CreateMap<User, UserViewModel>();
            CreateMap<RegisterRequest, User>()
            .ForMember(des => des.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}