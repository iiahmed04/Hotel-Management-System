using AutoMapper;
using HMS.Core.Entities.FeedbackEntities;
using HMS.Shared.DTOs.FeedbackDTOs;

namespace HMS.Services.Profiles.FeedbackProfiles
{
    internal class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            CreateMap<Feedback, GuestFeedbacksDTO>()
                .ForMember(
                    dest => dest.ServiceName,
                    opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : "General")
                );

            CreateMap<Feedback, ReturnedFeedbaskForAdminDTO>()
                .ForMember(dest => dest.GuestName, opt => opt.MapFrom(src => src.Guest.FullName))
                .ForMember(
                    dest => dest.ServiceName,
                    opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : "General")
                );

            CreateMap<ModerationLog, ReturnedModerationLogsForAdmin>()
                .ForMember(dest => dest.GuestName, opt => opt.MapFrom(src => src.Guest.FullName))
                .ForMember(dest => dest.Verdict, opt => opt.MapFrom(src => src.Verdict.ToString()));
        }
    }
}
