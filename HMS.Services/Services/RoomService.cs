using AutoMapper;
using HMS.Core.Contracts;
using HMS.Core.Entities.RoomEntities;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace HMS.Services.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<GenericResponse<IEnumerable<RoomDTO>>> GetAllRoomsForGuestAsync(string? roomType, string? sort)
        {
            var genericResponse = new GenericResponse<IEnumerable<RoomDTO>>();

            Enum.TryParse(roomType, out RoomType roomTypeEnum);
            Expression<Func<Room, bool>> filter = R =>
            (roomType == null || R.RoomType == roomTypeEnum) &&
            (R.RoomStatus == RoomStatus.Available || R.RoomStatus == RoomStatus.Reserved);

            Expression<Func<Room, object>>? orderBy = null;
            Expression<Func<Room, object>>? orderByDescending = null;
            if (sort is not null)
            {
                switch (sort)
                {
                    case "PriceAsc":
                        orderBy = R => R.PricePerNight;
                        break;
                    case "PriceDesc":
                        orderByDescending = R => R.PricePerNight;
                        break;
                    default:
                        orderBy = R => R.Id;
                        break;
                }
            }
            else
            {
                orderBy = R => R.Id;
            }

            var rooms = await _unitOfWork.GetRepository<Room, int>()
                .GetAllAsync(filter, orderBy, orderByDescending);

            if (rooms is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No Rooms found";
                return genericResponse;

            }

            var mappedRooms = _mapper.Map<IEnumerable<RoomDTO>>(rooms);
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Rooms Recieved Successfully";
            genericResponse.Data = mappedRooms;

            return genericResponse;
        }

        public async Task<GenericResponse<RoomDetailsDTO>> GetRoomDetailstAsync(int roomdId)
        {
            var genericResponse = new GenericResponse<RoomDetailsDTO>();

            Expression<Func<Room, bool>> filter = R =>
            (R.RoomStatus == RoomStatus.Available || R.RoomStatus == RoomStatus.Reserved);


            var room = await _unitOfWork.GetRepository<Room, int>()
                .GetByIdAsync(roomdId, filter, [R => R.RoomImages]);

            if (room is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = $"Room with Id : {roomdId} is not found";
                return genericResponse;
            }

            var mappedRoom = _mapper.Map<RoomDetailsDTO>(room);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Room Recived Successfully";
            genericResponse.Data = mappedRoom;

            return genericResponse;

        }
    }
}
