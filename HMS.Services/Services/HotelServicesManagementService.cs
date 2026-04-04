using AutoMapper;
using HMS.Core.Contracts;
using HMS.Core.Entities.ServiceEntities;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.ServiceDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HMS.Services.Services
{
    public class HotelServicesManagementService : IHotelServicesManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HotelServicesManagementService> _logger;

        public HotelServicesManagementService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<HotelServicesManagementService> logger
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GenericResponse<bool>> CreateHotelServiceByAdminAsync(
            CreateOrUpdateHotelServiceDTO createHotelServiceDTO
        )
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                if (createHotelServiceDTO is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalide service data.";
                    return genericResponse;
                }

                var serviceToAdd = _mapper.Map<Service>(createHotelServiceDTO);

                await _unitOfWork.GetRepository<Service, int>().AddAsync(serviceToAdd);

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to create service.";
                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Service Ceated successfully";
                genericResponse.Data = true;

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a hotel service.");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to create service.";
                return genericResponse;
            }
        }

        public async Task<GenericResponse<bool>> DeleteHotelServiceByAdminAsync(int id)
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var service = await _unitOfWork.GetRepository<Service, int>().GetByIdAsync(id);

                if (service is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = $"Service with Id : {id} not found to delete";
                    return genericResponse;
                }

                service.IsAvailable = false; // Soft delete by setting IsAvailable to false
                _unitOfWork.GetRepository<Service, int>().Update(service);
                service.UpdatedAt = DateTime.Now;

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to Delete service.";
                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Service Deleted successfully";
                genericResponse.Data = true;

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexcpected error to delete Service");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to Delete service.";
                return genericResponse;
            }
        }

        public async Task<
            GenericResponse<IEnumerable<HotelServicesDTO>>
        > GetAllHotelServicesForGuestAsync()
        {
            var genericResponse = new GenericResponse<IEnumerable<HotelServicesDTO>>();

            var services = await _unitOfWork
                .GetRepository<Service, int>()
                .GetAllAsync(x => (x.IsAvailable == true), null, null, null);

            if (services is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Services not found.";
                return genericResponse;
            }

            var mappedService = _mapper.Map<IEnumerable<HotelServicesDTO>>(services);

            if (mappedService is null || !mappedService.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Services not found.";
                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Services retrieved successfully.";
            genericResponse.Data = mappedService;

            return genericResponse;
        }

        public async Task<
            GenericResponse<IEnumerable<HotelServicesAdminDTO>>
        > GetAllServiceForAdminAsync(bool? IsAvailable)
        {
            var genericResponse = new GenericResponse<IEnumerable<HotelServicesAdminDTO>>();

            var repo = _unitOfWork.GetRepository<Service, int>();

            IEnumerable<Service> services = [];

            if (IsAvailable is null)
            {
                services = await repo.GetAllAsync();
            }

            if (IsAvailable is not null)
            {
                services = await repo.GetAllAsync(
                    x => x.IsAvailable == IsAvailable,
                    null,
                    null,
                    null
                );
            }

            if (services is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Not Services found";
                return genericResponse;
            }

            var mappedServices = _mapper.Map<IEnumerable<HotelServicesAdminDTO>>(services);

            if (mappedServices is null || !mappedServices.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Services not found.";
                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Services retrieved successfully.";
            genericResponse.Data = mappedServices;

            return genericResponse;
        }

        public async Task<GenericResponse<HotelServicesDTO>> GetHotelServiceByIdForGuestAsync(
            int serviceId
        )
        {
            var genericResponse = new GenericResponse<HotelServicesDTO>();

            var service = await _unitOfWork
                .GetRepository<Service, int>()
                .GetByIdAsync(serviceId, x => x.IsAvailable == true, null);

            if (service is null || service!.IsAvailable == false)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = $"Service with Id : {serviceId} not found.";
                return genericResponse;
            }

            var mappedService = _mapper.Map<HotelServicesDTO>(service);

            if (mappedService is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = $"Service with Id : {serviceId} not found.";
                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = $"Service with Id : {serviceId} retrieved successfully.";
            genericResponse.Data = mappedService;

            return genericResponse;
        }

        public async Task<GenericResponse<bool>> UpdateHotelServiceByAdminAsync(
            int id,
            CreateOrUpdateHotelServiceDTO updateHotelServiceDTO
        )
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var service = await _unitOfWork.GetRepository<Service, int>().GetByIdAsync(id);

                if (service is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "Service not found";
                    return genericResponse;
                }

                var mappedService = _mapper.Map(updateHotelServiceDTO, service);

                _unitOfWork.GetRepository<Service, int>().Update(mappedService);
                service.UpdatedAt = DateTime.Now;

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (!result)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed to update service";
                    return genericResponse;
                }

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Service updated successfully";
                genericResponse.Data = true;

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a hotel service.");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to update service.";
                return genericResponse;
            }
        }
    }
}
