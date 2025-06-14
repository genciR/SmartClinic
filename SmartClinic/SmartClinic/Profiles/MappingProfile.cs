using AutoMapper;
using SmartClinic.Models.DTOs;
using SmartClinic.Models.Entities;

namespace SmartClinic.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<UserRegistrationDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<User, UserDto>();

            // Doctor Mappings
            CreateMap<DoctorCreateDto, Doctor>()
                .ForMember(dest => dest.DoctorId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
            CreateMap<DoctorUpdateDto, Doctor>()
                .ForMember(dest => dest.DoctorId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
            CreateMap<Doctor, DoctorDto>();

            // Appointment Mappings
            CreateMap<AppointmentCreateDto, Appointment>()
                .ForMember(dest => dest.AppointmentId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.EndTime, opt => opt.Ignore());
            CreateMap<Appointment, AppointmentDto>();
            CreateMap<AppointmentStatusUpdateDto, Appointment>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // MedicalHistory Mappings
            CreateMap<MedicalHistory, MedicalHistoryDto>();
        }
    }
}