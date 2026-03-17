using AutoMapper;
using HMS.Core.Contracts;
using HMS.Core.Entities.RoomEntities;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.QueryParameters;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace HMS.Services.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomService> _logger;
        private readonly IAttachementService _attachementService;

        public RoomService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoomService> logger, IAttachementService attachementService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _attachementService = attachementService;
        }

        public async Task<GenericResponse<bool>> CreateRoomAsync(CreateRoomDTO createRoomDTO)
        {
            var genericResponse = new GenericResponse<bool>();
            try
            {

                if (createRoomDTO is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalid room data";
                    return genericResponse;
                }

                var roomToBeCreated = _mapper.Map<Room>(createRoomDTO);

                await _unitOfWork.GetRepository<Room, int>()
                    .AddAsync(roomToBeCreated);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Room Create Successfully";
                    genericResponse.Data = true;
                }

                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to create room";
                    genericResponse.Data = false;

                }
                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error ocurred while creating a room");

                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "An un excpected Error";
                genericResponse.Data = false;

                return genericResponse;
            }

        }

        public async Task<GenericResponse<bool>> DeleteRoomAsync(int roomId)
        {
            var genericResponse = new GenericResponse<bool>();
            try
            {

                //Not Completed logic
                var room = await _unitOfWork.GetRepository<Room, int>()
                    .GetByIdAsync(roomId);

                if (room is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Room not found to delete";

                    return genericResponse;
                }

                room.RoomStatus = RoomStatus.NotExist; //soft delete
                _unitOfWork.GetRepository<Room, int>().Update(room);
                room.UpdatedAt = DateTime.Now;

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = $"Room with Id : {roomId} deleted successfully";
                    genericResponse.Data = true;
                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to delete room";
                    genericResponse.Data = false;
                }
                return genericResponse;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An un excpected behaviour happend when try to delete room");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to delete room";
                genericResponse.Data = false;
                return genericResponse;
            }
        }

        public async Task<GenericResponse<bool>> DeleteRoomImageAsync(int roomId, int imageId)
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var room = await _unitOfWork.GetRepository<Room, int>()
                        .GetByIdAsync(roomId, null, [Room => Room.RoomImages]);

                if (room is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "Room not found to delete this image";
                    return genericResponse;
                }

                if (room.RoomImages is null || room.RoomImages.Count == 0)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "No images found in this room";
                    return genericResponse;
                }

                var roomImage = room.RoomImages.FirstOrDefault(RI => RI.Id == imageId);

                if (roomImage is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "No images with this id found to delete";
                    return genericResponse;
                }

                _unitOfWork.GetRepository<RoomImage, int>().Delete(roomImage);

                var isDeletedFromServer = _attachementService.DeleteFile(roomImage.ImageUrl, "rooms");

                if (!isDeletedFromServer)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed To delete image from server";
                    return genericResponse;
                }

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Room Image deleted successfully";
                    genericResponse.Data = true;
                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to delete image";
                    genericResponse.Data = false;
                }
                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured when try to delete image"); genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to delete image";
                return genericResponse;
            }
        }

        public async Task<GenericResponse<IEnumerable<AdminRoomDTO>>> GetAllRoomsForAdminOrStaffAsync(RoomQueryParam? roomQueryParam)
        {
            var genericResponse = new GenericResponse<IEnumerable<AdminRoomDTO>>();
            IEnumerable<Room>? rooms = null;

            if (roomQueryParam is not null)
            {
                Enum.TryParse(roomQueryParam.roomType, out RoomType roomTypeEnum);
                Enum.TryParse(roomQueryParam.roomStatus, out RoomStatus roomStatusEnum);
                Expression<Func<Room, bool>>? filter = R =>
                (roomQueryParam.roomType == null || R.RoomType == roomTypeEnum) &&
                (roomQueryParam.roomStatus == null || R.RoomStatus == roomStatusEnum);

                Expression<Func<Room, object>>? orderBy = null;
                Expression<Func<Room, object>>? orderByDescending = null;
                if (roomQueryParam.sort is not null)
                {
                    switch (roomQueryParam.sort)
                    {
                        case "priceAsc":
                            orderBy = R => R.PricePerNight;
                            break;
                        case "priceDesc":
                            orderByDescending = R => R.PricePerNight;
                            break;
                        default:
                            orderBy = R => R.Id;
                            break;
                    }
                }
                else
                {
                    orderByDescending = R => R.CreatedAt;
                }

                rooms = await _unitOfWork.GetRepository<Room, int>()
                    .GetAllAsync(filter, orderBy, orderByDescending);
            }
            else
            {
                rooms = await _unitOfWork.GetRepository<Room, int>()
                .GetAllAsync();
            }

            if (rooms is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No Rooms available";
                return genericResponse;
            }

            var mappedRooms = _mapper.Map<IEnumerable<AdminRoomDTO>>(rooms);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Reseved rooms sunccessfully";
            genericResponse.Data = mappedRooms;

            return genericResponse;
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
                    case "priceAsc":
                        orderBy = R => R.PricePerNight;
                        break;
                    case "priceDesc":
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

        public async Task<GenericResponse<bool>> UpdateRoomAsync(int roomId, UpdateRoomDTO updateRoomDTO)
        {
            var genericResponse = new GenericResponse<bool>();
            try
            {

                var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId);

                if (room is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "Room to be updated not found";
                    return genericResponse;
                }

                _mapper.Map(updateRoomDTO, room);
                room.UpdatedAt = DateTime.Now;

                _unitOfWork.GetRepository<Room, int>().Update(room);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = $"Room with Id : {roomId} Updated Successfully";
                    genericResponse.Data = true;
                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = $"Failed To update room";
                    genericResponse.Data = false;
                }

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error ocurred while updating a room");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = $"Un excpected Error occured";
                genericResponse.Data = false;

                return genericResponse;
            }

        }

        public async Task<GenericResponse<bool>> UploadRoomImagesAsync(int roomId, List<IFormFile> files)
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var room = await _unitOfWork.GetRepository<Room, int>()
                        .GetByIdAsync(roomId);

                if (room is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "No Rooms found to upload Images for it";
                    return genericResponse;
                }

                foreach (var file in files)
                {
                    var fileName = await _attachementService.UploadFileAsync(file, "rooms");

                    if (fileName is null)
                        continue;

                    var roomImage = new RoomImage { ImageUrl = fileName, RoomId = room.Id };

                    await _unitOfWork.GetRepository<RoomImage, int>()
                        .AddAsync(roomImage);
                }
                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Image uploaded successfully";
                    genericResponse.Data = true;
                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to upload room images";
                    genericResponse.Data = false;
                }

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An un expected error occured while adding room images");
                genericResponse.Message = "Failed to upload room images";

                return genericResponse;
            }
        }

    }
}
