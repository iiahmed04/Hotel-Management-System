using AutoMapper;
using HMS.Core.Entities.BookingEntities;
using HMS.Shared.DTOs.BookingDTOs;

namespace HMS.Services.Profiles.BookingProfiles
{
    internal class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingDTO>()
                .ForMember(
                    dest => dest.GuestFullName,
                    opt => opt.MapFrom(src => src.HotelUser.FullName)
                )
                .ForMember(dest => dest.GuestEmail, opt => opt.MapFrom(src => src.HotelUser.Email));

            CreateMap<Booking, MyBookingDTO>()
                .ForMember(dest => dest.CheckInDate, opt => opt.MapFrom(src => src.CheckInDate))
                .ForMember(dest => dest.CheckOutDate, opt => opt.MapFrom(src => src.CheckOutDate));
        }
    }
}
